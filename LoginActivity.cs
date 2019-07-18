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
using Android.Support.Design.Widget;

namespace travelApp
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        TextInputLayout accountLayout;
        EditText accountText;
        TextInputLayout passwordLayout;
        EditText passwordText;        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);
            accountLayout = FindViewById<TextInputLayout>(Resource.Id.textAccountInputLayout);
            accountText = FindViewById<EditText>(Resource.Id.textAccount);
            passwordLayout = FindViewById<TextInputLayout>(Resource.Id.textPasswordInputLayout);
            passwordText = FindViewById<EditText>(Resource.Id.textPassword);
            var btnSubmit = FindViewById<Button>(Resource.Id.btnSubmit);
            var btnCancel = FindViewById<Button>(Resource.Id.btnCancel);
            btnSubmit.Click += BtnSubmit_Click;
            btnCancel.Click += BtnCancel_Click;
            accountText.FocusChange += AccountText_FocusChange;
            passwordText.FocusChange += PasswordText_FocusChange;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.StartActivity(typeof(MainActivity));
            this.Finish();
        }

        private void PasswordText_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus && string.IsNullOrEmpty(passwordText.Text))
            {
                passwordLayout.Error = "請輸入密碼";
            }else
            {
                if (passwordText.Text.Length > 0)
                {
                    passwordLayout.Error = null;
                }
                else
                {
                    passwordLayout.Error = "請輸入密碼";
                }            
                
            }
        }

        private void AccountText_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus && string.IsNullOrEmpty(accountText.Text))
            {
                accountLayout.Error = "請輸入使用者帳號";
            }else
            {
                if (accountText.Text.Length > 0)
                {
                    accountLayout.Error = null;
                }
                else
                {
                    accountLayout.Error = "請輸入使用者帳號";
                }        
                
            }
        }

        private async void BtnSubmit_Click(object osender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(accountText.Text) && !string.IsNullOrEmpty(passwordText.Text))
                {
                    if (NetworkCheck.IsInternet())
                    {
                        using (var client = new HttpClient())
                        {
                            ServicePointManager.ServerCertificateValidationCallback +=
                                    (sender, cert, chain, sslPolicyErrors) => true;
                            var postData = new User
                            {
                                userAccount = accountText.Text,
                                userPWD = passwordText.Text
                            };
                            // create the request content and define Json  
                            var json = JsonConvert.SerializeObject(postData);
                            var content = new StringContent(json, Encoding.UTF8, "application/json");

                            //  send a POST request                  
                            var uri = ((AppValue)this.Application).url + "/AR_admin/userlogin";
                            var result = await client.PostAsync(uri, content);
                            //Toast.MakeText(this, "result.StatusCode="+ result.StatusCode, ToastLength.Long).Show();                            
                            if (result.IsSuccessStatusCode)
                            {
                                var resultString = await result.Content.ReadAsStringAsync();
                                var post = JsonConvert.DeserializeObject<LoginResult>(resultString);
                                if (post != null && post.result != null && post.result != "" && post.result == "0")
                                {
                                    ((AppValue)this.Application).account = accountText.Text;
                                    var uriPoint = ((AppValue)this.Application).url + "/AR_admin/UsergetTotalPoint/" + accountText.Text;
                                    var resultPoint = await client.GetAsync(uriPoint);
                                    if (resultPoint.IsSuccessStatusCode)
                                    {
                                        string contentPoint = await resultPoint.Content.ReadAsStringAsync();
                                        //handling the answer  
                                        var getPoint = JsonConvert.DeserializeObject<List<UserTotalPoint>>(contentPoint);
                                        if (getPoint != null && getPoint.Count > 0)
                                        {
                                            foreach (var pointData in getPoint)
                                            {
                                                ((AppValue)this.Application).userTotalPoint = int.Parse(pointData.totalPoint);
                                            }
                                            this.StartActivity(typeof(MainActivity));
                                            this.Finish();
                                        }
                                        else
                                        {
                                            Toast.MakeText(this, "取得使用者點數資料失敗!", ToastLength.Long).Show();
                                        }
                                    }
                                    else
                                    {
                                        Toast.MakeText(this, ((AppValue)this.Application).errorMessage, ToastLength.Long).Show();
                                        //Console.WriteLine("resultPoint Exception=" + resultPoint.Content.ReadAsStringAsync());
                                        //throw new Exception(await resultPoint.Content.ReadAsStringAsync());
                                    }
                                }
                                else
                                {
                                    Toast.MakeText(this, "登入失敗!", ToastLength.Long).Show();
                                }
                            }
                            else
                            {
                                Toast.MakeText(this, ((AppValue)this.Application).errorMessage, ToastLength.Long).Show();
                                //Console.WriteLine("Exception=" + result.Content.ReadAsStringAsync());
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
                }
                else
                {
                    Toast.MakeText(this, "請輸入帳號或密碼!", ToastLength.Long).Show();
                }
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, ((AppValue)this.Application).errorMessage, ToastLength.Long).Show();
                //Console.WriteLine("ex Exception=" + ex.StackTrace);
            }                        
        }
    }
}