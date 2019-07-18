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
using travelApp.Model;
using System.Net;
using Newtonsoft.Json;

namespace travelApp
{
    [Activity(Label = "RatingStar")]
    public class RatingStarActivity : Activity
    {
        String prizeID;
        EditText editFeedback;
        String rating = "0";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RatingStar);
            var i = this.Intent;
            prizeID = i.GetStringExtra("PrizeID");
            RatingBar ratingbar = FindViewById<RatingBar>(Resource.Id.ratingBar1);
            editFeedback = FindViewById<EditText>(Resource.Id.editFeedback);
            Button btnRatingStar = FindViewById<Button>(Resource.Id.btnRatingStar);
            btnRatingStar.Click += BtnRatingStar_Click;
            ratingbar.RatingBarChange += (o, e) => {
                rating = ratingbar.Rating.ToString();                
            };
        }

        private async void BtnRatingStar_Click(object osender, EventArgs e)
        {
            if (NetworkCheck.IsInternet())
            {
                using (var client = new HttpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                            (sender, cert, chain, sslPolicyErrors) => true;
                    var postData = new RatingStarPrize
                    {
                        userAccount = ((AppValue)this.Application).account,
                        prizeID = prizeID,
                        ratingStar = rating,
                        comment = editFeedback.Text
                    };
                    // create the request content and define Json  
                    var json = JsonConvert.SerializeObject(postData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    //  send a POST request                  
                    var uri = ((AppValue)this.Application).url + "/AR_admin/UserRatingStarPrize";
                    var result = await client.PostAsync(uri, content);
                    if (result.IsSuccessStatusCode)
                    {
                        var resultString = await result.Content.ReadAsStringAsync();
                        var post = JsonConvert.DeserializeObject<RatingStarResult>(resultString);
                        if (post.result != null && post.result != "" && post.result == "0")
                        {
                            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                            AlertDialog alert = dialog.Create();
                            alert.SetTitle("訊息");
                            alert.SetMessage("感謝您寶貴的建議!");
                            alert.SetButton("OK", (c, ev) =>
                            {
                                this.StartActivity(typeof(MainActivity));
                                this.Finish();
                            });
                            alert.Show();
                        }
                        else
                        {
                            Toast.MakeText(this, "寫評論失敗!", ToastLength.Long).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, ((AppValue)this.Application).errorMessage, ToastLength.Long).Show();
                        //throw new Exception(await result.Content.ReadAsStringAsync());
                    }
                }
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
            //Toast.MakeText(this, "Your Rating: " + rating, ToastLength.Short).Show();
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