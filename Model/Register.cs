﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace travelApp.Model
{
    class Register
    {
        public string userAccount { get; set; }
        public string userPWD { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string tel { get; set; }
    }
}