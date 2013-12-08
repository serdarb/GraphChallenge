using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Project.Client.Web.Entities;
using Project.Client.Web.Repositories;

namespace Project.Client.Web.Controllers
{
    public class HomeController : Controller
    {
        readonly DataRepository<MyData> _repoMyData;
        readonly DataRepository<BrokenData> _repoBrokenData;

        public HomeController()
        {
            _repoMyData = new DataRepository<MyData>();
            _repoBrokenData = new DataRepository<BrokenData>();
        }
        
        [HttpGet]
        public ActionResult Index(int id = 2013)
        {
            ViewBag.BrokenDataCount = _repoBrokenData.AsQueryable().Count();

            if (id != 2012
                && id != 2013)
            {
                id = 2013;
            }
            ViewBag.Year = id;

            ViewBag.AlbumData = HttpContext.Application[string.Format("{0}_AlbumData", id)].ToString();
            ViewBag.SingleData = HttpContext.Application[string.Format("{0}_SingleData", id)].ToString();
            ViewBag.StreamingData = HttpContext.Application[string.Format("{0}_StreamingData", id)].ToString();

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Load()
        {

            if (_repoMyData.AsQueryable().Any())
            {
                return Redirect("/home/index");
            }

            await Task.Run(() =>
            {
                var lines = System.IO.File.ReadAllLines(ConfigurationManager.AppSettings["FilePath"]);

                var myData = new List<MyData>();
                var brokenData = new List<BrokenData>();

                object lockMe = new object();

                Parallel.ForEach(lines, delegate(string line)
                {
                    var items = line.Split(new[] { ',' }, StringSplitOptions.None);

                    try
                    {
                        int sales;
                        if (!int.TryParse(items[2], out sales)
                            || sales < 1)
                        {
                            AddToBrokenData(brokenData, line, "sales is not an int or greater than zero");
                            return;
                        }

                        DateTime soldOn;
                        if (!DateTime.TryParse(items[5], out soldOn))
                        {
                            // most of the data has no datetime value on soldon column
                            // so i used the saleperiod column value as soldon
                            if (!DateTime.TryParse(items[6], out soldOn))
                            {
                                lock (lockMe)
                                {
                                    AddToBrokenData(brokenData, line, "soldOn or salePeriod is not a datetime");
                                }
                                return;
                            }
                        }

                        var correctSaleTypes = new[] { "Download", "Streaming" };
                        var saleType = items[7];
                        if (!correctSaleTypes.Contains(saleType))
                        {
                            lock (lockMe)
                            {
                                AddToBrokenData(brokenData, line, "saleType is not valid (not Download or Streaming)");
                            }
                            return;
                        }

                        var correctItemTypes = new[] { "AlbumTrack", "Album", "Track" };
                        var itemType = items[8];
                        if (!correctItemTypes.Contains(itemType))
                        {
                            lock (lockMe)
                            {
                                AddToBrokenData(brokenData, line,
                                    "itemType is not valid (not AlbumTrack or Album or Track)");
                            }
                            return;
                        }

                        var title = items[0];
                        var artist = items[1];
                        var album = items[3];
                        var store = items[4];
                        var buyer = items[10];

                        lock (lockMe)
                        {
                            myData.Add(new MyData
                            {
                                CreatedAt = soldOn,
                                ItemType = itemType,
                                SaleType = saleType,
                                SalesCount = sales,
                                Title = title,
                                Artist = artist,
                                Album = album,
                                Store = store,
                                Buyer = buyer
                            });
                        }

                    }
                    catch (Exception ex)
                    {
                        AddToBrokenData(brokenData, line, ex.Message);
                    }
                });

                _repoMyData.AddBulk(myData);

                brokenData.RemoveAll(x => x == null);
                _repoBrokenData.AddBulk(brokenData);
            });

            return Redirect("/home/index");
        }

        private static void AddToBrokenData(ICollection<BrokenData> brokenData, string line, string message)
        {
            brokenData.Add(new BrokenData
            {
                Line = line,
                Message = message
            });
        }
    }
}