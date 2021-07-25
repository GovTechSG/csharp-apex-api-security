using ApiUtilLib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ApexUtilLibTest
{
    [TestFixture()]
    public class TestDataTest : BaseService
    {
        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "getSignatureBaseString.json" })]
        public void TestBaseString(TestParam testData)
        {
            SetDetaultParams(testData);

            if (!skipTest)
            {
                try
                {
                    var baseString = ApiAuthorization.BaseString(authPrefix, signatureMethod, appId, signatureURL, formData, httpMethod, nonce, timeStamp, version);
                    Assert.AreEqual(expectedResult, baseString, "{0} - {1}", testData.id, testData.description);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                Assert.Ignore();
            }
        }

        //[Test()]
        //public void TestBaseStringX()
        //{
        //    var jsonData = GetJsonFile("getSignatureBaseString.json");
        //    int expectedPass = jsonData.Count();
        //    int actualPass = 0;

        //    try
        //    {
        //        foreach (var test in jsonData)
        //        {
        //            SetDetaultParams(test);

        //            if (skipTest == null || !skipTest.Contains("c#"))
        //            {

        //                try
        //                {
        //                    var baseString = ApiAuthorization.BaseString(authPrefix, signatureMethod, appId, signatureURL, formData, httpMethod, nonce, timeStamp, version);
        //                    Assert.AreEqual(expectedResult, baseString, "{0} - {1}", test.id, test.description);
        //                    actualPass++;
        //                }
        //                catch (Exception ex)
        //                {
        //                    throw ex;
        //                }
        //            }
        //            else
        //            {
        //                actualPass++;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    Assert.AreEqual(expectedPass, actualPass, "Total Passed Test Cases");

        //}



        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "verifyL1Signature.json" })]
        public void VerifyL1Signature(TestParam testData)
        {
            SetDetaultParams(testData);

            if (!skipTest)
            {
                var message = testData.message;
                var signature = testData.apiParam.signature;
                var result = signature.VerifyL1Signature(secret, message);
                try
                {
                    Assert.AreEqual(expectedResult.ToBool(), result, "{0} - {1}", testData.id, testData.description);
                }
                catch (Exception err)
                {
                    throw err;
                }
            }
            else
            {
                Assert.Ignore();
            }
        }

        //[Test()]
        //public void VerifyL1SignatureX()
        //{
        //    var jsonData = GetJsonFile("verifyL1Signature.json");
        //    int expectedPass = jsonData.Count();
        //    int actualPass = 0;

        //    try
        //    {
        //        foreach (var test in jsonData)
        //        {
        //            SetDetaultParams(test);

        //            if (skipTest == null || !skipTest.Contains("c#"))
        //            {
        //                var message = test.message;
        //                var signature = test.apiParam.signature;
        //                var result = signature.VerifyL1Signature(secret, message);
        //                try
        //                {
        //                    Assert.AreEqual(expectedResult.ToBool(), result, "{0} - {1}", test.id, test.description);
        //                    actualPass++;
        //                }
        //                catch (Exception)
        //                {
        //                    if (expectedResult == "false")
        //                        actualPass++;
        //                }
        //            }
        //            else
        //            {
        //                actualPass++;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //    Assert.AreEqual(expectedPass, actualPass, "Total Passed Test Cases");
        //}



        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "verifyL2Signature.json" })]
        public void VerifyL2Signature(TestParam testData)
        {
            try
            {
                //var test = jsonData;
                SetDetaultParams(testData);

                if (!skipTest)
                {

                    var message = testData.message;
                    var signature = testData.apiParam.signature;
                    string certPath = testCertPath + testData.publicCertFileName;

                    var fileType = PublicKeyFileType.CERTIFICATE;
                    if (testData.publicCertFileName.ToLower().EndsWith(".key"))
                    {
                        fileType = PublicKeyFileType.PUBLIC_KEY;
                    }
                    if (testData.publicCertFileName.ToLower().EndsWith(".p12") || testData.publicCertFileName.ToLower().EndsWith(".pfx"))
                    {
                        fileType = PublicKeyFileType.P12_OR_PFX;
                    }

                    RSACryptoServiceProvider publicKey = ApiAuthorization.GetPublicKey(certPath, fileType, passphrase);

                    var result = signature.VerifyL2Signature(publicKey, message);

                    Assert.IsTrue(result, "{0} - {1}", testData.id, testData.description);
                }
                else
                {
                    Assert.Ignore();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[Test()]
        //public void VerifyL2SignatureX()
        //{
        //    var jsonData = GetJsonFile("verifyL2Signature.json");
        //    int expectedPass = jsonData.Count();
        //    int actualPass = 0;

        //    try
        //    {
        //        foreach (var test in jsonData)
        //        {
        //            SetDetaultParams(test);

        //            if (skipTest == null || !skipTest.Contains("c#"))
        //            {

        //                var message = test.message;
        //                var signature = test.apiParam.signature;
        //                string certPath = testCertPath + test.publicCertFileName;

        //                var fileType = PublicKeyFileType.CERTIFICATE;
        //                if (test.publicCertFileName.ToLower().EndsWith(".key"))
        //                {
        //                    fileType = PublicKeyFileType.PUBLIC_KEY;
        //                }
        //                if (test.publicCertFileName.ToLower().EndsWith(".p12") || test.publicCertFileName.ToLower().EndsWith(".pfx"))
        //                {
        //                    fileType = PublicKeyFileType.P12_OR_PFX;
        //                }

        //                //RSACryptoServiceProvider publicKey = ApiAuthorization.PublicKeyFromCer(certPath);
        //                RSACryptoServiceProvider publicKey = ApiAuthorization.GetPublicKey(certPath, fileType, passphrase);

        //                var result = signature.VerifyL2Signature(publicKey, message);

        //                try
        //                {
        //                    Assert.IsTrue(result, "{0} - {1}", test.id, test.description);
        //                    actualPass++;
        //                }
        //                catch (Exception)
        //                {
        //                    if (expectedResult == "false")
        //                        actualPass++;
        //                }
        //            }
        //            else
        //            {
        //                actualPass++;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    Assert.AreEqual(expectedPass, actualPass, "Total Passed Test Cases");
        //}


        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "getSignatureToken.json" })]
        public void TestTokenSignature(TestParam testData)
        {
            SetDetaultParams(testData);

            if (!skipTest)
            {
                try
                {
                    string certName = testData.apiParam.privateCertFileNameP12;
                    string privateCertPath = testCertPath + certName;
                    RSACryptoServiceProvider privateKey = null;
                    if (!certName.IsNullOrEmpty())
                    {
                        privateKey = ApiAuthorization.GetPrivateKey(ApiUtilLib.PrivateKeyFileType.P12_OR_PFX, GetLocalPath(privateCertPath), passphrase);
                    }

                    var authParam = new AuthParam();
                    authParam.url = signatureURL;
                    authParam.httpMethod = httpMethod;
                    authParam.appName = appId;
                    authParam.appSecret = secret;
                    authParam.formData = formData;
                    authParam.privateKey = privateKey;
                    authParam.timestamp = timeStamp;
                    authParam.nonce = nonce;
                    var result = ApiAuthorization.TokenV2(authParam).Token;

                    if (timeStamp.Equals("%s") || nonce.Equals("%s"))
                    {
                        Match m = Regex.Match(result, "apex_(l[12])_([ei]g)_signature=(\\S+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        String signature_value = "";
                        if (m.Success)
                            signature_value = m.Groups[3].Value;

                        expectedResult = expectedResult.Replace("signature=\"%s\"", "signature=" + signature_value);
                    }

                    Assert.AreEqual(expectedResult, result, "{0} - {1}", testData.id, testData.description);
                }
                catch (Exception ex)
                {
                    Assert.AreEqual(expectedResult, ex.Message, "{0} - {1}", testData.id, testData.description);
                }
            }
            else
            {
                Assert.Ignore();
            }
        }

        //[Test()]
        //public void TestTokenSignatureX()
        //{
        //    var jsonData = GetJsonFile("getSignatureToken.json");
        //    int expectedPass = jsonData.Count();
        //    int actualPass = 0;
        //    try
        //    {
        //        foreach (var test in jsonData)
        //        {
        //            SetDetaultParams(test);

        //            if (skipTest == null || !skipTest.Contains("c#"))
        //            {
        //                try
        //                {
        //                    string certName = test.apiParam.privateCertFileNameP12;
        //                    string privateCertPath = testCertPath + certName;
        //                    RSACryptoServiceProvider privateKey = null;
        //                    if (!certName.IsNullOrEmpty())
        //                    {
        //                        privateKey = ApiAuthorization.GetPrivateKey(ApiUtilLib.PrivateKeyFileType.P12_OR_PFX, GetLocalPath(privateCertPath), passphrase);
        //                    }

        //                    var authParam = new AuthParam();
        //                    authParam.url = signatureURL;
        //                    authParam.httpMethod = httpMethod;
        //                    authParam.appName = appId;
        //                    authParam.appSecret = secret;
        //                    authParam.formData = formData;
        //                    authParam.privateKey = privateKey;
        //                    authParam.timestamp = timeStamp;
        //                    authParam.nonce = nonce;
        //                    var result = ApiAuthorization.TokenV2(authParam).Token;

        //                    if (timeStamp.Equals("%s") || nonce.Equals("%s"))
        //                    {
        //                        Match m = Regex.Match(result, "apex_(l[12])_([ei]g)_signature=(\\S+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        //                        String signature_value = "";
        //                        if (m.Success)
        //                            signature_value = m.Groups[3].Value;

        //                        expectedResult = expectedResult.Replace("signature=\"%s\"", "signature=" + signature_value);
        //                    }
        //                    Assert.AreEqual(expectedResult, result, "{0} - {1}", test.id, test.description);
        //                    actualPass++;
        //                }
        //                catch (Exception ex)
        //                {
        //                    Assert.AreEqual(expectedResult, ex.Message, "{0} - {1}", test.id, test.description);
        //                    if (errorTest)
        //                        actualPass++;
        //                }
        //            }
        //            else
        //            {
        //                actualPass++;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    Assert.AreEqual(expectedPass, actualPass, "Total Passed Test Cases");
        //}



        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "getL1Signature.json" })]
        public void GetL1Signature(TestParam testData)
        {
            SetDetaultParams(testData);

            if (!skipTest)
            {
                var message = testData.message;
                try
                {
                    var result = message.L1Signature(secret);
                    Assert.AreEqual(expectedResult, result, "id:{0} - {1}", testData.id, testData.description);
                }
                catch (Exception ex)
                {
                    Assert.AreEqual(expectedResult, ex.Message, "{0} - {1}", testData.id, testData.description);
                }
            }
            else
            {
                Assert.Ignore();
            }
        }

        //public void GetL1SignatureX()
        //{
        //    var jsonData = GetJsonFile("getL1Signature.json");
        //    int expectedPass = jsonData.Count();
        //    int actualPass = 0;

        //    try
        //    {
        //        foreach (var test in jsonData)
        //        {
        //            SetDetaultParams(test);

        //            if (skipTest == null || !skipTest.Contains("c#"))
        //            {
        //                var message = test.message;
        //                try
        //                {
        //                    var result = message.L1Signature(secret);
        //                    Assert.AreEqual(expectedResult, result, "id:{0} - {1}", test.id, test.description);
        //                    actualPass++;
        //                }
        //                catch (Exception ex)
        //                {
        //                    Assert.AreEqual(expectedResult, ex.Message, "{0} - {1}", test.id, test.description);
        //                    if (errorTest)
        //                        actualPass++;
        //                }
        //            }
        //            else
        //            {
        //                actualPass++;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    Assert.AreEqual(expectedPass, actualPass, "Total Passed Test Cases");

        //}


        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "getL2Signature.json" })]
        public void GetL2Signature(TestParam testData)
        {
            SetDetaultParams(testData);

            if (!skipTest)
            {
                try
                {
                    var message = testData.message;
                    string certName = testData.apiParam.privateCertFileName;
                    string privateCertPath = testCertPath + certName;

                    RSACryptoServiceProvider privateKey = null;

                    if (!certName.IsNullOrEmpty())
                        privateKey = ApiAuthorization.GetPrivateKey(PrivateKeyFileType.PEM_PKCS1, privateCertPath, passphrase);

                    var result = message.L2Signature(privateKey);

                    Assert.AreEqual(expectedResult, result, "{0} - {1}", testData.id, testData.description);
                }
                catch (Exception ex)
                {
                    // remove the file path that is machine dependent.
                    var err = ex.Message.Replace(testCertPath, "");

                    Assert.AreEqual(expectedResult, err, "{0} - {1}", testData.id, testData.description);
                }
            }
            else
            {
                Assert.Ignore();
            }
        }

        //public void GetL2SignatureX()
        //{
        //    var jsonData = GetJsonFile("getL2Signature.json");
        //    int expectedPass = jsonData.Count();
        //    int actualPass = 0;

        //    try
        //    {
        //        foreach (var test in jsonData)
        //        {
        //            SetDetaultParams(test);

        //            if (skipTest == null || !skipTest.Contains("c#"))
        //            {
        //                try
        //                {
        //                    var message = test.message;
        //                    string certName = test.apiParam.privateCertFileName;
        //                    string privateCertPath = testCertPath + certName;

        //                    //string result = null;
        //                    //if (!certName.IsNullOrEmpty())
        //                    //  result = ApiAuthorization.GetL2SignatureFromPEM(privateCertPath,message, passphrase);

        //                    //Assert.AreEqual(expectedResult, result, "{0} - {1}", test.id, test.description);


        //                    RSACryptoServiceProvider privateKey = null;

        //                    if (!certName.IsNullOrEmpty())
        //                        privateKey = ApiAuthorization.GetPrivateKey(PrivateKeyFileType.PEM_PKCS1, privateCertPath, passphrase);

        //                    var result = message.L2Signature(privateKey);

        //                    Assert.AreEqual(expectedResult, result, "{0} - {1}", test.id, test.description);

        //                    actualPass++;
        //                }
        //                catch (Exception ex)
        //                {
        //                    // remove the file path that is machine dependent.
        //                    var err = ex.Message.Replace(testCertPath, "");

        //                    Assert.AreEqual(expectedResult, err, "{0} - {1}", test.id, test.description);
        //                    if (errorTest)
        //                        actualPass++;
        //                }
        //            }
        //            else
        //            {
        //                actualPass++;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    Assert.AreEqual(expectedPass, actualPass, "Total Passed Test Cases");

        //}
    }
}
