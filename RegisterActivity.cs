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
using Android.Support.Design.Widget;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using travelApp.Model;
using System.Threading.Tasks;

namespace travelApp
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : Activity
    {
        Button btnRegister;
        Button btnCancel;
        TextInputLayout userTelLayout;
        EditText userTelText;
        TextInputLayout userAccountLayout;
        EditText userAccountText;
        TextInputLayout userEmailLayout;
        EditText userEmailText;
        TextInputLayout userPasswordLayout;
        EditText userPasswordText;
        TextInputLayout userConfirmLayout;
        EditText userConfirmText;
        EditText userNameText;
                
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Register);
            userAccountLayout = FindViewById<TextInputLayout>(Resource.Id.textAccountInputLayout);
            userAccountText = FindViewById<EditText>(Resource.Id.textAccount);
            userPasswordLayout = FindViewById<TextInputLayout>(Resource.Id.textPasswordInputLayout);
            userPasswordText = FindViewById<EditText>(Resource.Id.textPassword);
            userConfirmLayout = FindViewById<TextInputLayout>(Resource.Id.textConfirmInputLayout);
            userConfirmText = FindViewById<EditText>(Resource.Id.textConfirm);
            userNameText = FindViewById<EditText>(Resource.Id.textUserName);
            userEmailLayout = FindViewById<TextInputLayout>(Resource.Id.textUserEmailInputLayout);
            userEmailText = FindViewById<EditText>(Resource.Id.textUserEmail);
            userTelLayout = FindViewById<TextInputLayout>(Resource.Id.textUserTelInputLayout);
            userTelText = FindViewById<EditText>(Resource.Id.textUserTel);
            btnRegister = FindViewById<Button>(Resource.Id.btnRegister);
            btnCancel = FindViewById<Button>(Resource.Id.btnCancel);
            btnRegister.Click += BtnRegister_Click;
            btnCancel.Click += BtnCancel_Click;
            userTelText.FocusChange += UserTel_FocusChange;
            userAccountText.FocusChange += UserAccount_FocusChange;
            userPasswordText.FocusChange += UserPasswordText_FocusChange;           
            userConfirmText.FocusChange += UserConfirmText_FocusChange;            
           
            userEmailLayout.EditText.FocusChange += (s, e) =>
            {
                if (!e.HasFocus && string.IsNullOrEmpty(userEmailText.Text))
                {                    
                    userEmailLayout.Error = "請輸入e-mail";
                }
                else
                {
                    if (userEmailText.Text.Length > 0)
                    {
                        if (ValidateEmail(userEmailText.Text))
                        {                            
                            userEmailLayout.Error = null;
                        }
                        else
                        {                            
                            userEmailLayout.Error = "e-mail格式有誤";
                        }
                    }
                    else
                    {                        
                        userEmailLayout.Error = "請輸入e-mail";
                    }
                    
                }
            };            
        }

        private void UserConfirmText_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus && userPasswordText.Text.Length>0)
            {
                if (userConfirmText.Text == userPasswordText.Text)
                {                    
                    userConfirmLayout.Error = null;
                }
                else
                {
                    userConfirmLayout.Error = "密碼不一致;請重新輸入";
                }
            }
            else
            {
                if(userConfirmText.Text.Length > 0)
                {
                    if (userConfirmText.Text == userPasswordText.Text)
                    {                        
                        userConfirmLayout.Error = null;
                    }
                    else
                    {
                        userConfirmLayout.Error = "密碼不一致;請重新輸入";
                    }
                }
                else
                {
                    userConfirmLayout.Error = "請再次輸入密碼";
                }
                
            }
        }

        private void UserPasswordText_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus && userPasswordText.Text.Length>0)
            {
                userPasswordLayout.Error = null;
            }
            else
            {
                if (userPasswordText.Text.Length > 0)
                {
                    userPasswordLayout.Error = null;
                }
                else
                {
                    userPasswordLayout.Error = "請輸入密碼";
                }                
            }
        }       

        private async void UserAccount_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            bool checkUser = await ValidateUserAccount(userAccountText.Text);
            if (!e.HasFocus && string.IsNullOrEmpty(userAccountText.Text))
            {
                userAccountLayout.Error = "請輸入使用者帳號";
            }
            else
            {
                if (userAccountText.Text.Length > 0)
                {
                    if (IsNumericOrLetter(userAccountText.Text))
                    {
                        if (!checkUser)
                        {                            
                            userAccountLayout.Error = null;
                        }
                        else
                        {
                            userAccountLayout.Error = "這個帳號已經有人使用;請試試其他名稱";
                        }
                    }
                    else
                    {
                        userAccountLayout.Error = "請輸入英數字組合的帳號";
                    }
                }
                else
                {
                    userAccountLayout.Error = "請輸入使用者帳號";
                }                                
            }
        }

        private void UserTel_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus && userTelText.Text.Length>0)
            {
                if (ValidateTel(userTelText.Text))
                {
                    userTelLayout.Error = null;
                }
                else
                {
                    userTelLayout.Error = "行動電話號碼格式有誤";
                }
            }else
            {
                userTelLayout.Error = null;
            }                
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Finish();
        }

        private async void BtnRegister_Click(object osender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(userAccountText.Text) && !string.IsNullOrEmpty(userPasswordText.Text) 
                && !string.IsNullOrEmpty(userConfirmText.Text) && !string.IsNullOrEmpty(userEmailText.Text))
            {
                if (NetworkCheck.IsInternet())
                {
                    using (var client = new HttpClient())
                    {
                        ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;
                        var postData = new Register
                        {
                            userAccount = userAccountText.Text,
                            userPWD = userPasswordText.Text,
                            userName = userNameText.Text,
                            email = userEmailText.Text,
                            tel = userTelText.Text
                        };
                        // create the request content and define Json  
                        var json = JsonConvert.SerializeObject(postData);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        //  send a POST request                  
                        var uri = ((AppValue)this.Application).url + "/AR_admin/addUser";
                        var result = await client.PostAsync(uri, content);
                        if (result.IsSuccessStatusCode)
                        {
                            var resultString = await result.Content.ReadAsStringAsync();
                            var post = JsonConvert.DeserializeObject<LoginResult>(resultString);
                            if (post != null && post.result != null && post.result != "" && post.result == "0")
                            {
                                ((AppValue)this.Application).account = userAccountText.Text;
                                Toast.MakeText(this, "註冊成功!", ToastLength.Long).Show();
                                this.StartActivity(typeof(MainActivity));
                                this.Finish();
                            }
                            else
                            {
                                Toast.MakeText(this, "註冊失敗!", ToastLength.Long).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, ((AppValue)this.Application).errorMessage, ToastLength.Long).Show();
                            //throw new Exception(await result.Content.ReadAsStringAsync());
                        }
                    }
                }else
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
            }
            else
            {
                Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                Android.App.AlertDialog alert = dialog.Create();
                alert.SetTitle("訊息");
                alert.SetMessage("請輸入必填欄位");
                alert.SetButton("OK", (c, ev) =>
                {
                    // Ok button click task  
                });
                alert.Show();
            }          
        }
        private bool ValidateTel(string tel)
        {
            if(tel!=null && tel != "")
            {
                bool isTel= Regex.IsMatch(tel, "^[09]{2}[0-9]{8}$");
                return isTel;
            }else
            {
                return false;
            }           
        }
        private async Task<bool> ValidateUserAccount(String userAccount)
        {
            bool isDuplicate = false;                        
            try
            {
                if (!string.IsNullOrEmpty(userAccount))
                {
                    if (NetworkCheck.IsInternet())
                    {
                        using (var client = new HttpClient())
                        {
                            ServicePointManager.ServerCertificateValidationCallback +=
                            (sender, cert, chain, sslPolicyErrors) => true;
                            // send a GET request  
                            var uri = ((AppValue)this.Application).url + "/AR_admin/checkUser/" + userAccount;
                            var response = await client.GetAsync(uri);
                            if (response.IsSuccessStatusCode)
                            {
                                string content = await response.Content.ReadAsStringAsync();
                                //handling the answer  
                                var getResult = JsonConvert.DeserializeObject<LoginResult>(content);
                                if (getResult.result != null && getResult.result != "")
                                {
                                    if (getResult.result == "1")
                                    {
                                        isDuplicate = true;
                                    }
                                    else
                                    {
                                        isDuplicate = false;
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
                }               
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, ((AppValue)this.Application).errorMessage, ToastLength.Long).Show();
            }                        
            return isDuplicate;
        }
        private bool ValidateEmail(String email)
        {
            if(email!=null && email != "")
            {
                bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                return isEmail;
            }
            else
            {
                return false;
            }            
        }
        public static bool IsNumericOrLetter(string input)
        {
            return Regex.IsMatch(input, "^[A-Za-z0-9]+$");
        }
    }
}