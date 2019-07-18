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
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using travelApp.Model;
using UrlImageViewHelper;

namespace travelApp
{
    [Activity(Label = "PrizeDetailActivity")]
    public class PrizeDetailActivity : Activity
    {
        String prizeID;
        Button btnExchange;
        ImageView imgPrizeLarge;
        TextView textPrizeName;
        TextView textPrizeDescription;
        TextView textPoint;
        TextView textCategoryName;       
        TextView textUserPoint;
        TextView textWarning;
        String imgUrl;
        String prizeName;
        int prizePoint=0;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrizeDetail);            
            imgPrizeLarge = FindViewById<ImageView>(Resource.Id.imgPrizeLarge);
            textUserPoint = FindViewById<TextView>(Resource.Id.textUserPoint);
            btnExchange = FindViewById<Button>(Resource.Id.btnExchange);
            textPrizeName = FindViewById<TextView>(Resource.Id.textPrizeName);
            textPrizeDescription = FindViewById<TextView>(Resource.Id.textPrizeDescription);            
            textPoint = FindViewById<TextView>(Resource.Id.textPoint);
            textCategoryName = FindViewById<TextView>(Resource.Id.textCategoryName);
            textWarning = FindViewById<TextView>(Resource.Id.textWarning);
            var i = this.Intent;
            prizeID = i.GetStringExtra("PrizeID");
            if (NetworkCheck.IsInternet())
            {
                using (var client = new HttpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                            (sender, cert, chain, sslPolicyErrors) => true;
                    var uri = ((AppValue)this.Application).url + "/AR_admin/UsergetPrizeDetailbyId/" + prizeID;
                    var response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        //handling the answer  
                        var posts = JsonConvert.DeserializeObject<List<PrizeDetail>>(content);
                        if (posts.Count > 0)
                        {
                            foreach (var postData in posts)
                            {
                                imgUrl = postData.image;
                                imgPrizeLarge.SetUrlDrawable(((AppValue)this.Application).url + postData.image, Resource.Drawable.ic_notfound);
                                prizeName = postData.prizeName;
                                textPrizeName.Text = postData.prizeName;
                                textPrizeDescription.Text = postData.prizeDescription;
                                prizePoint = int.Parse(postData.point);
                                textPoint.Text = postData.point + "點";
                                textCategoryName.Text = postData.categoryName;
                            }
                        }                        
                    }
                    else
                    {
                        Toast.MakeText(this, ((AppValue)this.Application).errorMessage, ToastLength.Long).Show();
                        //throw new Exception(await response.Content.ReadAsStringAsync());
                    }
                }
            }
            else
            {
                Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                Android.App.AlertDialog alert = dialog.Create();
                alert.SetTitle("訊息");
                alert.SetMessage("請先開啟網路");
                alert.SetButton("OK", (c, ev) =>
                {
                    // Ok button click task  
                });
                alert.Show();
            }
           
            if (((AppValue)this.Application).account != "")
            {
                textUserPoint.Visibility = ViewStates.Visible;
                textUserPoint.Text = "您的總點數是:" + Convert.ToString((((AppValue)this.Application).userTotalPoint)) + "點";
                btnExchange.Visibility = ViewStates.Visible;
            }else
            {
                textWarning.Visibility = ViewStates.Visible;
            }

            btnExchange.Click += BtnExchange_Click;
        }

        private void BtnExchange_Click(object osender, EventArgs e)
        {                        
            int userTotalPoint= ((AppValue)this.Application).userTotalPoint;            
            if (userTotalPoint>= prizePoint)
            {
                Intent prizeExchangeIntent = new Intent(this, typeof(PrizeExchangeActivity));
                prizeExchangeIntent.PutExtra("PrizeID", prizeID);
                prizeExchangeIntent.PutExtra("imgUrl", imgUrl);
                prizeExchangeIntent.PutExtra("prizeName", prizeName);
                prizeExchangeIntent.PutExtra("point", Convert.ToString(prizePoint));                
                this.StartActivity(prizeExchangeIntent);
                this.Finish();
            }else
            {
                Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                Android.App.AlertDialog alert = dialog.Create();
                alert.SetTitle("訊息");
                alert.SetMessage("您的點數不夠無法兌換該商品");
                alert.SetButton("OK", (c, ev) =>
                {
                    // Ok button click task  
                });
                alert.Show();
            }            
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Down)
            {
                ((AppValue)this.Application).account = "";
                this.Finish();

                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }
    }
}