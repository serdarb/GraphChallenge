using System;
using MongoDB.Bson;

namespace Project.Client.Web.Entities
{
    public class MyData: BaseData
    {

        public DateTime CreatedAt { get; set; }
        public int CreatedAtYear
        {
            get { return CreatedAt.Year; }
            set
            {

            }

        }
        public int CreatedAtMonth
        {
            get { return CreatedAt.Month; }
            set
            {

            }
        }

        public int CreatedAtDay
        {
            get { return CreatedAt.Day; }
            set
            {

            }
        }
        
        public string ItemType { get; set; }
        public string SaleType { get; set; }
        public int SalesCount { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Store { get; set; }
        public string Buyer { get; set; }
    }
}
