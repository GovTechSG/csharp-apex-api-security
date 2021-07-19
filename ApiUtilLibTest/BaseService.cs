using ApexUtilLib;
using ApiUtilLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.IO.Compression;
using System.Reflection;

namespace ApexUtilLibTest
{
    public class BaseService
    {
        // APEX 1
        //internal string apexTestSuitePath = "https://github.com/GovTechSG/test-suites-apex-api-security/archive/master.zip";

        // for APEX2
        //internal string apexTestSuitePath = "https://github.com/GovTechSG/test-suites-apex-api-security/zipball/master/";
        internal string apexTestSuitePath = "https://github.com/GovTechSG/test-suites-apex-api-security/zipball/development/";

        internal string testDataPath = GetLocalPath("temp/GovTechSG-test-suites-apex-api-security-46232ac/testData/");
        internal string testCertPath = GetLocalPath("temp/GovTechSG-test-suites-apex-api-security-46232ac/");

        internal ApiUtilLib.SignatureMethod signatureMethod { get; set; }
        internal ApiUtilLib.HttpMethod httpMethod { get; set; }
        internal ApiList apiList { get; set; }
        internal FormList formData { get; set; }
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
            downloadFile(apexTestSuitePath, GetLocalPath("testSuite.zip"));
        }



        internal static string GetLocalPath(string relativeFileName)
        {
            var localPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), relativeFileName.Replace('/', Path.DirectorySeparatorChar));
            return localPath;
        }
        internal void downloadFile(string sourceURL, string downloadPath)
        {
            try
            {
                long fileSize = 0;
                int bufferSize = 1024;
                bufferSize *= 1000;
                long existLen = 0;
                System.IO.FileStream saveFileStream;
                saveFileStream = new System.IO.FileStream(downloadPath,
                                                          System.IO.FileMode.Create,
                                                          System.IO.FileAccess.Write,
                                                          System.IO.FileShare.ReadWrite);

                System.Net.HttpWebRequest httpReq;
                System.Net.HttpWebResponse httpRes;
                httpReq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(sourceURL);
                httpReq.AddRange((int)existLen);
                System.IO.Stream resStream;
                httpRes = (System.Net.HttpWebResponse)httpReq.GetResponse();
                resStream = httpRes.GetResponseStream();

                fileSize = httpRes.ContentLength;
                int byteSize;
                byte[] downBuffer = new byte[bufferSize];

                while ((byteSize = resStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
                {
                    saveFileStream.Write(downBuffer, 0, byteSize);
                }
                saveFileStream.Close();

                if (System.IO.Directory.Exists(GetLocalPath("temp/")))
                {
                    Directory.Delete(GetLocalPath("temp/"), true);
                }
                ZipFile.ExtractToDirectory(downloadPath, GetLocalPath("temp/"));

                // determine the folder for the json files
                string path = GetLocalPath("temp/");
                DirectoryInfo dictiontory = new DirectoryInfo(path);
                DirectoryInfo[] dir = dictiontory.GetDirectories();// this get all subfolder //name in folder NetOffice.
                string dirName = dir[0].Name; //var dirName get name from array Dir;

                // set the path to test data files
                testDataPath = GetLocalPath(string.Format("temp/{0}/testData/", dirName));
                testCertPath = GetLocalPath(string.Format("temp/{0}/", dirName));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void SetDetaultParams(TestParam paramFile)
        {
            try
            {
                signatureMethod = paramFile.apiParam.signatureMethod.ParseSignatureMethod(paramFile.apiParam.secret);
                httpMethod = paramFile.apiParam.httpMethod.ToEnum<ApiUtilLib.HttpMethod>();

                // queryString and formData must be saperated
                //apiList = new ApiList();
                apiList = SetApiList(paramFile.apiParam.formData);
                formData = FormList.Convert(apiList);
                var queryData = SetApiList(paramFile.apiParam.queryString);

                timeStamp = paramFile.apiParam.timestamp ?? "%s";
                version = paramFile.apiParam.version ?? "1.0";
                nonce = paramFile.apiParam.nonce ?? "%s";
                authPrefix = paramFile.apiParam.authPrefix;
                appId = paramFile.apiParam.appID;
                testId = paramFile.id;

                // combine queryString with URL
                string queryString = "";
                if (!paramFile.apiParam.signatureURL.IsNullOrEmpty())
                {
                    queryString = queryData.ToQueryString();
                    if (!queryString.IsNullOrEmpty())
                    {
                        string joinChar = "?";
                        if (paramFile.apiParam.signatureURL.IndexOf('?') > -1) joinChar = "&";
                        queryString = joinChar + queryString;
                    }
                }
                signatureURL = paramFile.apiParam.signatureURL.IsNullOrEmpty() == true ? null : new System.Uri(string.Format("{0}{1}", paramFile.apiParam.signatureURL, queryString));
                
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

        // change return type from void to ApiList
        internal ApiList SetApiList(Dictionary<object, object> data = null)
        {
            try
            {
                var dataList = new ApiList();

                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var key = item.Key ?? "";
                        var value = item.Value ?? "";

                        String value_s = value.ToString().Trim();
                       
                        if (!key.ToString().IsNullOrEmpty())
                        {
                            string[] _queryParams = { "" };
                            string val = null;

                            if (!value_s.IsNullOrEmpty() && !(value_s.StartsWith("{", StringComparison.InvariantCulture) && value_s.EndsWith("}", StringComparison.InvariantCulture)))
                            {
                            
                                val = value_s.RemoveString(new string[] { "\\", "\\ ", " \\", "\"", "\\  ", "\n" }).Unescape();

                                if (val == "True")
                                    val = "true";
                                if (val == "False")
                                    val = "false";
                                if (val.StartsWith("[", StringComparison.InvariantCulture) && val.EndsWith("]", StringComparison.InvariantCulture))
                                {

                                    string[] _paramValues = { "" };
                                    val = val.RemoveString(new string[] { "[", "]", " " });
                                    _paramValues = val.Split(',');
                                    foreach (var paramvalue in _paramValues)
                                    {
                                        var _paramvalue = paramvalue;
                                        dataList.Add(key.ToString(), _paramvalue.Unescape());
                                    }

                                }
                                else
                                {
                                    dataList.Add(key.ToString(), val);
                                }
                            }
                            else
                            {
                                dataList.Add(key.ToString(), val);
                            }

                        }
                    }
                }

                return dataList;
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
