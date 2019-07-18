using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Views;
using Android.Support.V7.App;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using travelApp.Model;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace travelApp
{
    [Activity(Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        int pageindex = 1;//目前頁碼        
        int totalEmptyContent = 7;
        private List<PrizeContent> datas;
        //LayoutInflater inflater;
        //LayoutInflater inflater1;
        int visiablecount;
        int totalcount;
        int first;
        ListView listView;
        View loaderFoot;
        View noDataFoot;
        Button btnLogout;
        Button btnRegister;
        Button btnLogin;
        ImageView imgLogin;
        TextView textName;
        ProgressBar pbLoading;
        DateTime? firstTime;
        int postsCount = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            listView = FindViewById<ListView>(Resource.Id.listViewMain);
            imgLogin = FindViewById<ImageView>(Resource.Id.imgLogin);
            textName = FindViewById<TextView>(Resource.Id.textName);
            btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            btnLogout = FindViewById<Button>(Resource.Id.btnLogout);
            btnRegister = FindViewById<Button>(Resource.Id.btnRegister);
            pbLoading = FindViewById<ProgressBar>(Resource.Id.pbLoading);

            if (((AppValue)this.Application).account != "")
            {
                imgLogin.Visibility = ViewStates.Visible;
                textName.Visibility = ViewStates.Visible;
                textName.Text = ((AppValue)this.Application).account;
                btnLogin.Visibility = ViewStates.Invisible;
                btnLogout.Visibility = ViewStates.Visible;
                btnRegister.Visibility = ViewStates.Invisible;
            }
            if (NetworkCheck.IsInternet())
            {
                //check app version
                OpenAppInStore();

                getData();
                LayoutInflater inflater = LayoutInflater.FromContext(this);
                loaderFoot = inflater.Inflate(Resource.Layout.FooterLoader, null);
                listView.AddFooterView(loaderFoot);
                //隱藏底部的載入中圖示
                loaderFoot.Visibility = ViewStates.Gone;
                //listview拉倒底部
                listView.Scroll += ListView_Scroll;
                // 捲動狀態變化
                listView.ScrollStateChanged += ListView_ScrollStateChanged;
                listView.ItemClick += ListView_ItemClick;
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
            FindViewById<Button>(Resource.Id.btnLogin).Click += delegate
            {
                this.StartActivity(typeof(LoginActivity));
                this.Finish();
            };
            FindViewById<Button>(Resource.Id.btnLogout).Click += delegate
            {
                ((AppValue)this.Application).account = "";
                this.StartActivity(typeof(MainActivity));
                this.Finish();
            };
            FindViewById<Button>(Resource.Id.btnRegister).Click += delegate
            {
                this.StartActivity(typeof(RegisterActivity));
            };
        }

        private async void ListView_ItemClick(object osender, AdapterView.ItemClickEventArgs e)
        {            
            if (e.Id >= 0 && datas[e.Position].id != "")
            {
                if (NetworkCheck.IsInternet())
                {
                    using (var client = new HttpClient())
                    {
                        ServicePointManager.ServerCertificateValidationCallback +=
                            (sender, cert, chain, sslPolicyErrors) => true;
                        var uri = ((AppValue)this.Application).url + "/AR_admin/UsergetPrizeDetailbyId/" + datas[e.Position].id;
                        var response = await client.GetAsync(uri);
                        if (response.IsSuccessStatusCode)
                        {
                            string content = await response.Content.ReadAsStringAsync();
                            //handling the answer  
                            var posts = JsonConvert.DeserializeObject<List<PrizeDetail>>(content);
                            if (posts.Count > 0)
                            {
                                Intent prizeDetailIntent = new Intent(this, typeof(PrizeDetailActivity));
                                prizeDetailIntent.PutExtra("PrizeID", datas[e.Position].id);
                                this.StartActivity(prizeDetailIntent);
                                if (((AppValue)this.Application).account != "")
                                {
                                    this.Finish();
                                }
                            }
                            else
                            {
                                Toast.MakeText(this, "查無該兌換商品詳細資料", ToastLength.Short).Show();
                            }
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

        private async void ListView_ScrollStateChanged(object sender, AbsListView.ScrollStateChangedEventArgs e)
        {
            int endPage = await getEndPage();
            int totalCount=await getTotalCount();
            //判斷是否停止捲動
            if (e.ScrollState == ScrollState.Idle && (first + visiablecount) == totalcount)
            {                
                if (pageindex < endPage || postsCount< totalCount)
                {
                    if (listView.FooterViewsCount>0 && noDataFoot!=null)
                    {
                        listView.RemoveFooterView(noDataFoot);
                        LayoutInflater inflater = LayoutInflater.FromContext(this);
                        loaderFoot = inflater.Inflate(Resource.Layout.FooterLoader, null);
                        listView.AddFooterView(loaderFoot);
                    }
                    loaderFoot.Visibility = ViewStates.Visible; 
                    await Task.Delay(1000);
                    
                    if (pageindex < endPage)
                    {
                        pageindex++;
                    }                                                           
                    //取得資料
                    getData();
                    loaderFoot.Visibility = ViewStates.Gone;                    
                    //listView.RemoveFooterView(loaderFoot);                    
                }
                else
                {                                        
                    if(listView.FooterViewsCount > 0 && loaderFoot != null)
                    {
                        listView.RemoveFooterView(loaderFoot);
                    }
                    
                    
                    if (listView.FooterViewsCount >0 && noDataFoot!=null)
                    {                        
                        noDataFoot.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        LayoutInflater inflater1 = LayoutInflater.FromContext(this);
                        noDataFoot = inflater1.Inflate(Resource.Layout.FooterNoData, null);
                        listView.AddFooterView(noDataFoot);
                        noDataFoot.Visibility = ViewStates.Visible;
                    }                    
                }
            }           
        }

        private void ListView_Scroll(object sender, AbsListView.ScrollEventArgs e)
        {
            first = e.FirstVisibleItem;
            totalcount = e.TotalItemCount;            
            visiablecount = e.VisibleItemCount;
        }

        public async void getData()
        {
            pbLoading.Visibility = ViewStates.Visible;
            int PageStart = (pageindex - 1) * ((AppValue)this.Application).pagesize;
            int FooterViewsCount = 0;
            using (var client = new HttpClient())
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, cert, chain, sslPolicyErrors) => true;
                // send a GET request  
                var uri = ((AppValue)this.Application).url+"/AR_admin/UsergetPrizebyPage/" + pageindex + "/" + ((AppValue)this.Application).pagesize;
                
                try
                {
                    var response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        //handling the answer  
                        var posts = JsonConvert.DeserializeObject<List<PrizeContent>>(content);
                        //Toast.MakeText(this, "posts count=" + posts.Count, ToastLength.Long).Show();
                        if (posts.Count > 0)
                        {
                            postsCount = posts.Count;
                            datas = new List<PrizeContent>();
                            foreach (var postData in posts)
                            {
                                datas.Add(new PrizeContent { id = postData.id, prizeName = postData.prizeName, point = postData.point, image = ((AppValue)this.Application).url + postData.image });
                            }
                            if(listView.FooterViewsCount >= 1)
                            {
                                FooterViewsCount = 1;
                            }
                            //增加空白資料
                            if(posts.Count< totalEmptyContent)
                            {
                                for (int i = 0; i < totalEmptyContent - posts.Count - FooterViewsCount; i++)
                                {
                                    datas.Add(new PrizeContent { id = "", prizeName = "", point = "", image = "" });
                                }
                            }                            

                            listView.Adapter = new PrizeListAdapter(this, datas);

                            listView.DividerHeight = 0;                            
                            listView.SetSelection(PageStart); //當前頁第一項從哪頁開始
                        }else
                        {
                            var emptyText = FindViewById<TextView>(Resource.Id.emptyTextView);
                            emptyText.Visibility = ViewStates.Visible;
                            listView.EmptyView = emptyText;
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, ((AppValue)this.Application).errorMessage, ToastLength.Long).Show();
                        //throw new Exception(await response.Content.ReadAsStringAsync());
                    }
                }
                catch (System.Exception e)
                {
                    Toast.MakeText(this, ((AppValue)this.Application).errorMessage, ToastLength.Long).Show();
                    //System.Console.WriteLine(e.StackTrace);
                }

            }
            pbLoading.Visibility = ViewStates.Gone;
        }
        public async Task<int> getEndPage()
        {
            int endPage=1;
            using (var pageClient = new HttpClient())
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;
                // send a GET request  
                var uri = ((AppValue)this.Application).url + "/AR_admin/UsergetPageInfo/1/" + ((AppValue)this.Application).pagesize;
                var response = await pageClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string pageContent = await response.Content.ReadAsStringAsync();
                    //handling the answer  
                    var pageResults = JsonConvert.DeserializeObject<List<PageInfo>>(pageContent);
                    if (pageResults.Count > 0)
                    {
                        foreach (var pageData in pageResults)
                        {                            
                            endPage = int.Parse(pageData.endPageNum);
                        }
                    }
                }
                else
                {
                    Toast.MakeText(this, ((AppValue)this.Application).errorMessage, ToastLength.Long).Show();
                    //throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }            
            return endPage;
        }
        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Down)
            {
                if (!firstTime.HasValue || DateTime.Now.Second - firstTime.Value.Second > 2)
                {
                    Toast.MakeText(this, "再按一次返回鍵退出程式", ToastLength.Short).Show();
                    firstTime = DateTime.Now;                    
                }
                else
                {
                    ((AppValue)this.Application).account = "";
                    this.Finish();
                }
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }
        public async Task<int> getTotalCount()
        {
            int countData = 0;
            using (var client = new HttpClient())
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;
                // send a GET request  
                var uri = ((AppValue)this.Application).url + "/AR_admin/UsergetPrizeTotalCount";
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string pageContent = await response.Content.ReadAsStringAsync();
                    //handling the answer  
                    var totalCountResults = JsonConvert.DeserializeObject<PrizeTotalCount>(pageContent);
                    if (totalCountResults!=null&& totalCountResults.result!=""&& totalCountResults.result=="0")
                    {
                        countData = int.Parse(totalCountResults.value);                        
                    }
                }
            }
            return countData;
        }
        public async void OpenAppInStore()
        {
            bool isUsingLatestVersion = await VersionCheck.IsUsingLatestVersion();
            if (!isUsingLatestVersion)
            {
                            
                Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                Android.App.AlertDialog alert = dialog.Create();
                alert.SetTitle("訊息");
                alert.SetMessage("有最新版程式,請更新程式版本");
                alert.SetButton("OK", (c, ev) =>
                {
                    try
                    {
                        var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse($"market://details?id="+ AppInfo.PackageName));
                        intent.SetPackage("com.android.vending");
                        intent.SetFlags(ActivityFlags.NewTask);
                        Application.Context.StartActivity(intent);
                    }
                    catch (ActivityNotFoundException)
                    {
                        var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse($"https://play.google.com/store/apps/details?id="+ AppInfo.PackageName +"&hl=zh_TW"));
                        Application.Context.StartActivity(intent);
                    }

                });
                alert.SetButton2("cancel", (c, ev) =>
                {

                });
                alert.Show();                   
            }
        }
    }
}

