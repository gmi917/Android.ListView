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
using Xamarin.Essentials;
namespace travelApp
{
    class NetworkCheck
    {
        public static bool IsInternet()
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                // Connection to internet is available
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}