using ApexUtilLib;
using ApiUtilLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ApexUtilLibTest
{
    public class BaseService
    {
        internal string testDataPath = @"/Users/nsearch/OneDrive/Projects/GovTech/testData/";
        internal string testCertPath = @"/Users/nsearch/OneDrive/Projects/GovTech/";

        internal ApiUtilLib.SignatureMethod signatureMethod { get; set; }
        internal ApiUtilLib.HttpMethod httpMethod { get; set; }
        internal ApiList apiList { get; set; }
        internal string timeStamp { get; set; }
        internal string version { get; set; }
        internal string nonce { get; set; }
        internal string authPrefix { get; set; }
        internal string testId { get; set; }
        internal string appId { get; set; }
        internal Uri signatureURL { get; set; }
        internal string expectedResult { get; set; }
        internal bool errorTest { get; set; }
        internal string[] skipTest { get; set; }
        internal string realm { get; set; }
        internal Uri invokeUrl { get; set; }
        internal string secret { get; set; }
        internal string passphrase { get; set; }

        public BaseService()
        {
        }

        internal void SetDetaultParams(TestParam paramFile)
        {
            try
            {
                signatureMethod = paramFile.apiParam.signatureMethod.ParseSignatureMethod(paramFile.apiParam.secret);
                httpMethod = paramFile.apiParam.httpMethod.ToEnum<ApiUtilLib.HttpMethod>();
                apiList = new ApiList();
                SetApiList(paramFile.apiParam.formData);
                SetApiList(paramFile.apiParam.queryString);
                timeStamp = paramFile.apiParam.timestamp;
                if (paramFile.apiParam.timestamp.IsNullOrEmpty())
                    timeStamp = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), 10).ToString();
                version = paramFile.apiParam.version ?? "1.0";
                nonce = paramFile.apiParam.nonce ?? ApiUtilLib.ApiAuthorization.NewNonce();
                authPrefix = paramFile.apiParam.authPrefix;
                appId = paramFile.apiParam.appID;
                testId = paramFile.id;
                signatureURL = paramFile.apiParam.signatureURL.IsNullOrEmpty() == true ? null : new System.Uri(paramFile.apiParam.signatureURL);
                expectedResult = CommonExtensions.GetCharp(paramFile.expectedResult);
                errorTest = paramFile.errorTest;
                skipTest = paramFile.skipTest;
                invokeUrl = paramFile.apiParam.invokeURL.IsNullOrEmpty() == true ? null : new System.Uri(paramFile.apiParam.invokeURL);
                secret = paramFile.apiParam.secret ?? null;
                realm = paramFile.apiParam.realm ?? null;
                passphrase = paramFile.apiParam.passphrase;// ?? "passwordp12";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void SetApiList(Dictionary<object, object> data = null)
        {
            try
            {
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var key = item.Key ?? "";
                        var value = item.Value ?? "";

                        if (value.ToString().StartsWith("{", StringComparison.InvariantCulture) && value.ToString().EndsWith("}", StringComparison.InvariantCulture))
                            value = "";

                        string[] _subArry = { "" };
                        string val = null;
                        if (!value.ToString().IsNullOrEmpty())
                        {
                            val = value.ToString().Trim().RemoveString(new string[] { "[", "]", "\\", "\\ ", " \\", "\"", "\\  ", "\n", " " }).Unescape();
                            _subArry = val.Split(',');
                        }

                        foreach (var subArry in _subArry)
                        {
                            var _val = subArry;
                            if (_val == "True")
                                _val = "true";
                            if (_val == "False")
                                _val = "false";

                            if (!key.ToString().IsNullOrEmpty())
                                apiList.Add(key.ToString(), _val);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal IEnumerable<TestParam> 
        GetJsonFile(string fileName)
        {
            string path = testDataPath + fileName;

            TestDataService service = new TestDataService();
            var jsonData = service.LoadTestFile(path);

            return jsonData;
        }


        public static byte[] PEM(string type, byte[] data)
        {
            string pem = Encoding.ASCII.GetString(data);
            string header = String.Format("-----BEGIN {0}-----", type);
            string footer = String.Format("-----END {0}-----", type);
            int start = pem.IndexOf(header) + header.Length;
            int end = pem.IndexOf(footer, start);
            string base64 = pem.Substring(start, (end - start));
            return Convert.FromBase64String(base64);
        }
    }
}
