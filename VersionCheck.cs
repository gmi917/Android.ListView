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
using Xamarin.Essentials;
using static Android.Provider.DocumentsContract;
using System.Threading.Tasks;
using Org.Jsoup;
using System.Text.RegularExpressions;
using Java.Lang;

namespace travelApp
{
    class VersionCheck
    {
        static string url = $"https://play.google.com/store/apps/details?id=" + AppInfo.PackageName + "&hl=zh_TW";
        public static async Task<bool> IsUsingLatestVersion()
        {
            bool isLatest = true;
            //目前版本名稱
            string currentVersionName = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;
            //目前版本號碼
            //int currentVersionCode = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionCode;
            string GooglePlayAppVersion = await getGooglePlayAppVersionbyJsoup();
            if (!string.IsNullOrEmpty(GooglePlayAppVersion) && !string.IsNullOrEmpty(currentVersionName))
            {
                if (Version.Parse(currentVersionName).CompareTo(Version.Parse(GooglePlayAppVersion)) < 0)
                {
                    isLatest = false;
                }
            }                      
            return isLatest;
        }

        public static async Task<string> getGooglePlayAppVersionbyJsoup()
        {            
            string version = null;
            try
            {
                version = await Task.Run(() => Jsoup.Connect(url)
                 .Timeout(10000)
                 .UserAgent("Mozilla/5.0 (Windows; U; WindowsNT 5.1; en-US; rv1.8.1.6) Gecko/20070725 Firefox/2.0.0.6")
                 .Referrer("http://www.google.com")
                 .Get()
                 .Select("div:contains(目前版本)").Last().Parent()
                 .Select("span").Last()
                 .OwnText());
            }
            catch(System.Exception e)
            {

            }                        
            return version;
        }
        public static string getGooglePlayAppVersionbyHttpMethod()
        {
            string version = null;            
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (var handler = new HttpClientHandler())
                {
                    using (var client = new HttpClient(handler))
                    {
                        using (var responseMsg = client.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result)
                        {
                            if (!responseMsg.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"Error connecting to the Play Store. Url={url}.");
                            }

                            try
                            {
                                var content = responseMsg.Content == null ? null : responseMsg.Content.ReadAsStringAsync().Result;

                                var versionMatch = Regex.Match(content, "<div[^>]*>目前版本</div><span[^>]*><div[^>]*><span[^>]*>(.*?)<").Groups[1];

                                if (versionMatch.Success)
                                {
                                    version = versionMatch.Value.Trim();
                                }
                            }
                            catch (System.Exception e)
                            {
                                Console.WriteLine($"Error parsing content from the Play Store. Url={url}.", e);
                            }
                        }
                    }
                }                
            }
            return version;
        }
    }
}