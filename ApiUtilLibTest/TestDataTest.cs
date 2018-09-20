using ApiUtilLib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace ApexUtilLibTest
{
    [TestFixture()]
    public class TestDataTest
    {
        private string testDataPath = @"/Users/nsearch/OneDrive/Projects/test-suites-apex-api-security/testData/";

        private ApiUtilLib.SignatureMethod signatureMethod { get; set; }
        private ApiUtilLib.HttpMethod httpMethod { get; set; }
        private ApiList apiList { get; set; }
        private string timeStamp { get; set; }
        private string version { get; set; }
        private string nonce { get; set; }
        private string authPrefix { get; set; }
        private string testId { get; set; }
        private string appId { get; set; }
        private Uri url { get; set; }
        private string expectedResult { get; set; }
        private bool errorTest { get; set; }
        private string[] skipTest { get; set; }

        private void SetDetaultParams(TestParam paramFile)
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
                    timeStamp = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),10).ToString();
                version = paramFile.apiParam.version ?? "1.0";
                nonce = paramFile.apiParam.nonce ?? ApiUtilLib.ApiAuthorization.NewNonce();
                authPrefix = paramFile.apiParam.authPrefix;
                appId = paramFile.apiParam.appID;
                testId = paramFile.id;
                url = paramFile.apiParam.signatureURL.IsNullOrEmpty()==true? null : new System.Uri(paramFile.apiParam.signatureURL);
                expectedResult = CommonExtensions.GetCharp(paramFile.expectedResult);
                errorTest = paramFile.errorTest;
                skipTest = paramFile.skipTest;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetApiList(Dictionary<object,object> data = null)
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

                        string[] _subArry = {""};
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

        [Test()]
        public void TestBaseString()
        {
            string path = testDataPath + "getSignatureBaseString.json";

            TestDataService service = new TestDataService();
            var jsonData = service.LoadTestFile(path);
            int expectedPass = jsonData.Count();
            int actualPass = 0;

            try
            {
                foreach (var test in jsonData)
                {
                    SetDetaultParams(test);

                    if (skipTest == null || !skipTest.Contains("c#"))
                    {

                        var baseString = ApiAuthorization.BaseString(authPrefix, signatureMethod, appId, url, httpMethod, apiList, nonce, timeStamp, version);

                        try
                        {
                            Assert.AreEqual(expectedResult, baseString);
                            actualPass++;
                        }
                        catch (Exception ex)
                        {
                            ex = ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Assert.AreEqual(expectedPass, actualPass);

        }


    }
}
