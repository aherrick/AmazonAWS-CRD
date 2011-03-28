using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using System.Net;

namespace AmazonAWSExampleSDK.Controllers
{
    public class HomeController : Controller
    {
        string AWSAccessKey { get; set; }
        string AWSSecretKey { get; set; }
        string AWSBucket { get; set; }
        string AWSFolder { get; set; }

        public HomeController()
        {
            // axx

            AWSAccessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
            AWSSecretKey = ConfigurationManager.AppSettings["AWSSecretKey"];

            // what bucket & folder?

            AWSBucket = "ajh";
            AWSFolder = "test/";
        }

        #region Index

        public ActionResult Index()
        {
            // get me all objects inside a given folder
            Dictionary<string, double> images = null;

            var request = new ListObjectsRequest();
            request.BucketName = AWSBucket;
            request.WithPrefix(AWSFolder);

            using (AmazonS3 client = Amazon.AWSClientFactory.CreateAmazonS3Client(AWSAccessKey, AWSSecretKey))
            {
                using (ListObjectsResponse response = client.ListObjects(request))
                {
                    images = response.S3Objects.Where(x => x.Key != AWSFolder).ToDictionary(obj => obj.Key, obj => AppHelper.ConvertBytesToMegabytes(obj.Size));
                }
            }

            return View(images);
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                using (AmazonS3 client = Amazon.AWSClientFactory.CreateAmazonS3Client(AWSAccessKey, AWSSecretKey))
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var t = (DateTime.UtcNow - new DateTime(1970, 1, 1));

                    var key = string.Format("{0}{1}-{2}", AWSFolder, (int)t.TotalSeconds, fileName);

                    var request = new PutObjectRequest();

                    request.WithBucketName(AWSBucket)
                           .WithCannedACL(S3CannedACL.PublicRead)
                           .WithKey(key).InputStream = file.InputStream;

                    using (PutObjectResponse response = client.PutObject(request)) { }
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult JSONDeleteAWSFile(string key)
        {
            var request = new DeleteObjectRequest();

            request.WithBucketName(AWSBucket)
                   .WithKey(key);

            using (AmazonS3 client = Amazon.AWSClientFactory.CreateAmazonS3Client(AWSAccessKey, AWSSecretKey))
            {
                using (DeleteObjectResponse response = client.DeleteObject(request)) { }
            }

            return Json(null);
        }

        #endregion
    }
}
