using ApiUtilLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ApexUtilLibTest
{
    public class BaseService
    {
        // APEX 1
        //internal static string apexTestSuitePath = "https://github.com/GovTechSG/test-suites-apex-api-security/archive/master.zip";

        // for APEX2
        //internal static string apexTestSuitePath = "https://github.com/GovTechSG/test-suites-apex-api-security/zipball/master/";
        internal static string apexTestSuitePath = "https://github.com/GovTechSG/test-suites-apex-api-security/zipball/development/";

        internal static bool IsDebug = true;

        internal static bool IsDataFileDownloaded = false;
        internal static string testDataPath = GetLocalPath("temp/GovTechSG-test-suites-apex-api-security-2b397cc/testData/");
        internal static string testCertPath = GetLocalPath("temp/GovTechSG-test-suites-apex-api-security-2b397cc/");

        internal SignatureMethod SignatureMethod { get; set; }
        internal HttpMethod HttpMethod { get; set; }

        internal FormData FormData { get; set; }
        internal string TimeStamp { get; set; }
        internal string Version { get; set; }
        internal string Nonce { get; set; }
        internal string AuthPrefix { get; set; }
        internal string TestId { get; set; }
        internal string AppId { get; set; }
        internal Uri SignatureURL { get; set; }
        internal string ExpectedResult { get; set; }
        internal bool ErrorTest { get; set; }

        internal bool SkipTest { get; set; }

        internal string Realm { get; set; }
        internal Uri InvokeUrl { get; set; }
        internal string Secret { get; set; }
        internal string Passphrase { get; set; }

        public BaseService()
        {
        }

        internal static string GetLocalPath(string relativeFileName)
        {
            return Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), relativeFileName.Replace('/', Path.DirectorySeparatorChar));
        }

        internal static string DownloadFile(string sourceURL, string downloadPath)
        {
            try
            {
                long fileSize = 0;
                int bufferSize = 1024;
                bufferSize *= 1000;
                long existLen = 0;
                FileStream saveFileStream;
                saveFileStream = new FileStream(downloadPath,
                                                FileMode.Create,
                                                FileAccess.Write,
                                                FileShare.ReadWrite);

                System.Net.HttpWebRequest httpReq;
                System.Net.HttpWebResponse httpRes;
                httpReq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(sourceURL);
                httpReq.AddRange((int)existLen);
                Stream resStream;
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

                if (Directory.Exists(GetLocalPath("temp/")))
                {
                    Directory.Delete(GetLocalPath("temp/"), true);
                }
                ZipFile.ExtractToDirectory(downloadPath, GetLocalPath("temp/"));

                // determine the folder for the json files
                string path = GetLocalPath("temp/");
                DirectoryInfo dictiontory = new DirectoryInfo(path);
                DirectoryInfo[] dir = dictiontory.GetDirectories();// this get all subfolder //name in folder NetOffice.
                string dirName = dir[0].Name; //var dirName get name from array Dir;

                return dirName;
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
                if (paramFile.ApiParam != null)
                {
                    SignatureMethod = paramFile.ApiParam.SignatureMethod.ParseSignatureMethod(paramFile.ApiParam.Secret);
                    HttpMethod = paramFile.ApiParam.HttpMethod.ToEnum<HttpMethod>();

                    // queryString and formData must be saperated
                    FormData = FormData.SetupList(paramFile.ApiParam.FormData);
                    QueryData queryData = QueryData.SetupList(paramFile.ApiParam.QueryString);

                    //TimeStamp = paramFile.ApiParam.Timestamp ?? "%s";
                    TimeStamp = paramFile.ApiParam.Timestamp;

                    Version = paramFile.ApiParam.Version ?? "1.0";

                    //Nonce = paramFile.ApiParam.Nonce ?? "%s";
                    Nonce = paramFile.ApiParam.Nonce;

                    AuthPrefix = paramFile.ApiParam.AuthPrefix;
                    AppId = paramFile.ApiParam.AppID;

                    // combine queryString with URL
                    string queryString = "";
                    if (!paramFile.ApiParam.SignatureURL.IsNullOrEmpty())
                    {
                        queryString = queryData.ToString();
                        if (!queryString.IsNullOrEmpty())
                        {
                            // query start with ?, replace ? with & when url already contain queryString
                            if (paramFile.ApiParam.SignatureURL.IndexOf('?') > -1)
                            {
                                queryString = queryString.Replace("?", "&");
                            }
                        }
                    }
                    SignatureURL = paramFile.ApiParam.SignatureURL.IsNullOrEmpty() ? null : new Uri(string.Format("{0}{1}", paramFile.ApiParam.SignatureURL, queryString));

                    InvokeUrl = paramFile.ApiParam.InvokeURL.IsNullOrEmpty() ? null : new Uri(paramFile.ApiParam.InvokeURL);
                    Secret = paramFile.ApiParam.Secret ?? null;
                    Realm = paramFile.ApiParam.Realm ?? null;
                    Passphrase = paramFile.ApiParam.Passphrase ?? paramFile.Passphrase;// ?? "passwordp12";
                }

                TestId = paramFile.Id;
                ExpectedResult = CommonExtensions.GetCharp(paramFile.ExpectedResult);
                ErrorTest = paramFile.ErrorTest;

                SkipTest = paramFile.SkipTest == null ? false : paramFile.SkipTest.Contains("c#");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal static IEnumerable<TestParam> GetJsonFile(string fileName)
        {
            if (!IsDataFileDownloaded)
            {
                if (IsDebug)
                {
                    var folderName = "linkFolder";

                    // set the path to test data files
                    testDataPath = GetLocalPath($"{folderName}/testData/");
                    testCertPath = GetLocalPath($"{folderName}/");
                }
                else
                {
                    var folderName = DownloadFile(apexTestSuitePath, GetLocalPath("testSuite.zip"));

                    // set the path to test data files
                    testDataPath = GetLocalPath($"temp/{folderName}/testData/");
                    testCertPath = GetLocalPath($"temp/{folderName}/");
                }
                IsDataFileDownloaded = true;
            }

            string path = testDataPath + fileName;

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string _result = reader.ReadToEnd();

                    IEnumerable<TestParam> result = JsonConvert.DeserializeObject<IEnumerable<TestParam>>(_result);

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal static void ValidateErrorMessage(TestParam testCase, Exception ex)
        {
            // remove the file path that is machine dependent.
            //string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"/{testCertPath}/";
            string err = ex.Message.Replace(testCertPath, "");

            Assert.AreEqual(testCase.Result, err, "{0} - {1}", testCase.Id, testCase.Description);
        }
    }
}
