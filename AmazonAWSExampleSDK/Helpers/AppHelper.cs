using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmazonAWSExampleSDK
{
    public static class AppHelper
    {
        public static double ConvertBytesToMegabytes(long bytes)
        {
            return Math.Round((bytes / 1024f) / 1024f,3);
        }
    }
}