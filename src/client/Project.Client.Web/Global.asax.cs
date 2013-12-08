using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Project.Client.Web.Entities;
using Project.Client.Web.Repositories;

namespace Project.Client.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var repoMyData = new DataRepository<MyData>();
            
            if (repoMyData.AsQueryable().Any())
            {
                var data2013 = repoMyData.AsQueryable()
                    .Where(x => x.CreatedAtYear == 2013).ToList()
                    .GroupBy(x => new { x.CreatedAtMonth, x.ItemType })
                    .Select(
                        grp => new { Month = grp.Key.CreatedAtMonth, grp.Key.ItemType, Sum = grp.Sum(x => x.SalesCount) })
                    .OrderBy(x => x.Month)
                    .ToList();

                var data2012 = repoMyData.AsQueryable()
                    .Where(x => x.CreatedAtYear == 2012).ToList()
                    .GroupBy(x => new { x.CreatedAtMonth, x.ItemType })
                    .Select(
                        grp => new { Month = grp.Key.CreatedAtMonth, grp.Key.ItemType, Sum = grp.Sum(x => x.SalesCount) })
                    .OrderBy(x => x.Month)
                    .ToList();

                var list = data2013.Where(x => x.ItemType == "Album").ToList();
                var str = new StringBuilder("[");
                for (int i = 0; i < 12; i++)
                {
                    try
                    {
                        str.AppendFormat("{0},", list[i].Sum);
                    }
                    catch (Exception)
                    {
                        str.Append("0,");
                    }
                }
                str.Append("]");
                Application["2013_AlbumData"] = str.ToString();

                list = data2013.Where(x => x.ItemType == "Track").ToList();
                str = new StringBuilder("[");
                for (int i = 0; i < 12; i++)
                {
                    try
                    {
                        str.AppendFormat("{0},", list[i].Sum);
                    }
                    catch (Exception)
                    {
                        str.Append("0,");
                    }
                }
                str.Append("]");
                Application["2013_SingleData"] = str.ToString();

                list = data2013.Where(x => x.ItemType == "AlbumTrack").ToList();
                str = new StringBuilder("[");
                for (int i = 0; i < 12; i++)
                {
                    try
                    {
                        str.AppendFormat("{0},", list[i].Sum);
                    }
                    catch (Exception)
                    {
                        str.Append("0,");
                    }
                }
                str.Append("]");
                Application["2013_StreamingData"] = str.ToString();


                list = data2012.Where(x => x.ItemType == "Album").ToList();
                str = new StringBuilder("[");
                for (int i = 0; i < 12; i++)
                {
                    try
                    {
                        str.AppendFormat("{0},", list[i].Sum);
                    }
                    catch (Exception)
                    {
                        str.Append("0,");
                    }
                }
                str.Append("]");
                Application["2012_AlbumData"] = str.ToString();

                list = data2012.Where(x => x.ItemType == "Track").ToList();
                str = new StringBuilder("[");
                for (int i = 0; i < 12; i++)
                {
                    try
                    {
                        str.AppendFormat("{0},", list[i].Sum);
                    }
                    catch (Exception)
                    {
                        str.Append("0,");
                    }
                }
                str.Append("]");
                Application["2012_SingleData"] = str.ToString();

                list = data2012.Where(x => x.ItemType == "AlbumTrack").ToList();
                str = new StringBuilder("[");
                for (int i = 0; i < 12; i++)
                {
                    try
                    {
                        str.AppendFormat("{0},", list[i].Sum);
                    }
                    catch (Exception)
                    {
                        str.Append("0,");
                    }
                }
                str.Append("]");
                Application["2012_StreamingData"] = str.ToString();
            }
        }
    }
}
