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
using travelApp.Model;
using System.Net;
using UrlImageViewHelper;

namespace travelApp
{
    class PrizeListAdapter : BaseAdapter<PrizeContent>
    {
        List<PrizeContent> items;

        Activity context;
        public PrizeListAdapter(Activity context, List<PrizeContent> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }
        public override PrizeContent this[int position]
        {
            get{ return items[position]; }
        }

        public override int Count
        {
            get{ return items.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {            
            var item = items[position];            
            var view = convertView;
            if (view == null)
            {
                //使用自訂的ListView
                view = context.LayoutInflater.Inflate(Resource.Layout.PrizeList, null);
            }           
            //view.FindViewById<TextView>(Resource.Id.tvTitle).Text = item.prizeName;
            //view.FindViewById<TextView>(Resource.Id.tvPoint).Text = "兌換點數:"+item.point+"點";
            TextView tvTitle = view.FindViewById<TextView>(Resource.Id.tvTitle);
            TextView tvPoint = view.FindViewById<TextView>(Resource.Id.tvPoint);
            ImageView imgPrize = view.FindViewById<ImageView>(Resource.Id.imgPrize);
            ImageView imgSeparator = view.FindViewById<ImageView>(Resource.Id.imgSeparator);
            if (item.prizeName == "")
            {
                tvTitle.Visibility = ViewStates.Gone;
            }
            else
            {
                tvTitle.Text = item.prizeName;
            }
            if (item.point == "")
            {
                tvPoint.Visibility = ViewStates.Gone;
            }
            else
            {
                tvPoint.Text = "兌換點數:" + item.point + "點";
            }
            if (item.image == "")
            {
                imgPrize.Visibility = ViewStates.Gone;
            }
            else
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;                
                imgPrize.SetUrlDrawable(item.image, Resource.Drawable.ic_notfound);
            }
            if (item.prizeName == "")
            {
                imgSeparator.Visibility = ViewStates.Gone;
            }
  
            return view;
        }
    }
}