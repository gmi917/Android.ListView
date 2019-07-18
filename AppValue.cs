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

namespace travelApp
{
    [Application]
    public class AppValue : Application
    {
        public String account = "";//登入帳號
        public String url = "https://211.21.173.183";
        public int userTotalPoint = 0;//總點數
        public int pagesize = 6;//每頁顯示的數量
        public String errorMessage = "系統有誤請稍後再試!";
        public AppValue(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
        }
        public override void OnCreate()
        {            
            base.OnCreate();
        }

    }
}