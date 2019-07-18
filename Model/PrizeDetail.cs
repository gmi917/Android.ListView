using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace travelApp.Model
{
    class PrizeDetail
    {
        public string id { get; set; }
        public string state { get; set; }
        public string prizeDescription { get; set; }
        public string categoryName { get; set; }
        public string image { get; set; }
        public string point { get; set; }
        public string categoryID { get; set; }
        public string prizeName { get; set; }
    }
}