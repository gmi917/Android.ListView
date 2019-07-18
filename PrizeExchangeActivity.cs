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
using UrlImageViewHelper;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using travelApp.Model;

namespace travelApp
{
    [Activity(Label = "PrizeExchangeActivity")]
    public class PrizeExchangeActivity : Activity
    {
        ImageView imgExchange;
        TextView textExchangePrizeName;
        TextView textExchangePoint;
        String prizeID;
        String imgUrl;
        String prizeName;
        String point;
        Button btnOK;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PrizeExchange);
            var i = this.Intent;
            prizeID = i.GetStringExtra("PrizeID");
            imgUrl = i.GetStringExtra("imgUrl");
            prizeName = i.GetStringExtra("prizeName");
            point = i.GetStringExtra("point");
            imgExchange = FindViewById<ImageView>(Resource.Id.imgExchange);
            imgExchange.SetUrlDrawable(((AppValue)this.Application).url + imgUrl, Resource.Drawable.ic_notfound);
            FindViewById<TextView>(Resource.Id.textExchangePrizeName).Text = prizeName;
            FindViewById<TextView>(Resource.Id.textExchangePoint).Text = point+"點";
            btnOK = FindViewById<Button>(Resource.Id.btnOK);
            btnOK.Click += BtnOK_Click;
        }

        private void BtnOK_Click(object osender, EventArgs e)
        {
            if (NetworkCheck.IsInternet())
            {
                Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                AlertDialog alert = dialog.Create();
                alert.SetTitle("訊息");
                alert.SetMessage("確定兌換?");
                alert.SetButton("OK", async (c, ev) =>
                {
                    // Ok button click task 
                    using (var client = new HttpClient())
                    {
                        //ServicePointManager.ServerCertificateValidationCallback +=
                        //    (sender, cert, chain, sslPolicyErrors) => true;
                        var postData = new PrizeOrder
                        {
                            prizeId = prizeID,
                            username = ((AppValue)this.Application).account,
                            prizePoint = point
                        };
                        // create the request content and define Json  
                        var json = JsonConvert.SerializeObject(postData);                        
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        //  send a POST request                  
                        var uri = ((AppValue)this.Application).url + "/AR_admin/UserExchangePrize";                        
                        var result = await client.PostAsync(uri, content);
                        if (result.IsSuccessStatusCode)
                        {
                            var resultString = await result.Content.ReadAsStringAsync();
                            var post = JsonConvert.DeserializeObject<ExchangeResult>(resultString);
                            
                            if (post.result != null && post.result != "" && post.result == "0")
                            {                                
                                ((AppValue)this.Application).userTotalPoint = ((AppValue)this.Application).userTotalPoint - int.Parse(point);
                                Android.App.AlertDialog.Builder resultDialog = new AlertDialog.Builder(this);
                                AlertDialog resultAlert = resultDialog.Create();
                                resultAlert.SetTitle("訊息");
                                resultAlert.SetMessage("兌換成功");
                                resultAlert.SetButton("OK", (rc, rev) =>
                                {
                                    Intent ratingStarIntent = new Intent(this, typeof(RatingStarActivity));
                                    ratingStarIntent.PutExtra("PrizeID", prizeID);
                                    this.StartActivity(ratingStarIntent);
                                    this.Finish();
                                });
                                resultAlert.Show();
                            }
                            else
                            {
                                Android.App.AlertDialog.Builder failDialog = new AlertDialog.Builder(this);
                                AlertDialog failAlert = failDialog.Create();
                                failAlert.SetTitle("訊息");
                                failAlert.SetMessage("兌換失敗");
                                failAlert.SetButton("OK", (rc, rev) =>
                                {

                                });
                                failAlert.Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, ((AppValue)this.Application).errorMessage, ToastLength.Long).Show();
                            //throw new Exception(await result.Content.ReadAsStringAsync());
                        }
                    }
                });
                alert.SetButton2("cancel", (c, ev) =>
                {

                });
                alert.Show();
            }
            else
            {
                Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                AlertDialog alert = dialog.Create();
                alert.SetTitle("訊息");
                alert.SetMessage("請先開啟網路");
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