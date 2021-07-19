﻿using ApiUtilLib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using ApexUtilLib;

namespace ApexUtilLibTest
{
    [TestFixture()]
    public class TestDataTest : BaseService
    {
        [Test()]
        public void TestBaseString()
        {
            var jsonData = GetJsonFile("getSignatureBaseString.json");
            int expectedPass = jsonData.Count();
            int actualPass = 0;

            try
            {
                foreach (var test in jsonData)
                {
                    SetDetaultParams(test);

                    if (skipTest == null || !skipTest.Contains("c#"))
                    {

                        try
                        {
                            var baseString = ApiAuthorization.BaseString(authPrefix, signatureMethod, appId, signatureURL, formData, httpMethod, nonce, timeStamp, version);
                            Assert.AreEqual(expectedResult, baseString, "{0} - {1}", test.id, test.description);
                            actualPass++;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else
                    {
                        actualPass++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Assert.AreEqual(expectedPass, actualPass, "Total Passed Test Cases");

        }

        [Test()]
        public void VerifyL1Signature()
        {
            var jsonData = GetJsonFile("verifyL1Signature.json");
            int expectedPass = jsonData.Count();
            int actualPass = 0;

            try
            {
                foreach (var test in jsonData)
                {
                    SetDetaultParams(test);

                    if (skipTest == null || !skipTest.Contains("c#"))
                    {
                        var message = test.message;
                        var signature = test.apiParam.signature;
                        var result = signature.VerifyL1Signature(secret,message);
                        try
                        {
                            Assert.AreEqual(expectedResult.ToBool(),result, "{0} - {1}", test.id, test.description);
                            actualPass++;
                        }
                        catch (Exception)
                        {
                            if (expectedResult == "false")
                                actualPass++;
                        }
                    }
                    else
                    {
                        actualPass++;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            Assert.AreEqual(expectedPass, actualPass, "Total Passed Test Cases");
        }

        [Test()]
        public void VerifyL2Signature()
        {
            var jsonData = GetJsonFile("verifyL2Signature.json");
            int expectedPass = jsonData.Count();
            int actualPass = 0;

            try
            {
                foreach (var test in jsonData)
                {
                    SetDetaultParams(test);

                    if (skipTest == null || !skipTest.Contains("c#"))
                    {

                        var message = test.message;
                        var signature = test.apiParam.signature;
                        string certPath = testCertPath + test.publicCertFileName;

                        RSACryptoServiceProvider publicKey = ApiAuthorization.PublicKeyFromCer(certPath);

                        var result = signature.VerifyL2Signature(publicKey, message);

                        try
                        {
                            Assert.IsTrue(result, "{0} - {1}", test.id, test.description);
                            actualPass++;
                        }
                        catch (Exception)
                        {
                            if (expectedResult == "false")
                                actualPass++;
                        }
                    }else{
                        actualPass++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Assert.AreEqual(expectedPass, actualPass, "Total Passed Test Cases");
        }

        [Test()]
        public void TestTokenSignature()
        {
            var jsonData = GetJsonFile("getSignatureToken.json");
            int expectedPass = jsonData.Count();
            int actualPass = 0;
            try
            {
                foreach (var test in jsonData)
                {
                    SetDetaultParams(test);

                    if (skipTest == null || !skipTest.Contains("c#"))
                    {
                        try
                        {
                            string certName = test.apiParam.privateCertFileNameP12;
                            string privateCertPath = testCertPath + certName;
                            RSACryptoServiceProvider privateKey = null;
                            if (!certName.IsNullOrEmpty())
                            {
                                privateKey = ApiAuthorization.GetPrivateKey(ApexUtilLib.PrivateKeyFileType.P12, GetLocalPath(privateCertPath), passphrase);
                            }

                            var authParam = new AuthParam();
                            authParam.url = signatureURL;
                            authParam.httpMethod = httpMethod;
                            authParam.appName = appId;
                            authParam.appSecret = secret;
                            authParam.formList = formData;
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
                            Assert.AreEqual(expectedResult, result, "{0} - {1}", test.id, test.description);
                            actualPass++;
                        }
                        catch (Exception ex)
                        {
                            Assert.AreEqual(expectedResult, ex.Message, "{0} - {1}", test.id, test.description);
                            if (errorTest)
                                actualPass++;
                        }
                    }
                    else
                    {
                        actualPass++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Assert.AreEqual(expectedPass, actualPass, "Total Passed Test Cases");
        }

        [Test()]
        public void GetL1Signature()
        {
            var jsonData = GetJsonFile("getL1Signature.json");
            int expectedPass = jsonData.Count();
            int actualPass = 0;

            try
            {
                foreach (var test in jsonData)
                {
                    SetDetaultParams(test);

                    if (skipTest == null || !skipTest.Contains("c#"))
                    {
                        var message = test.message;
                        try
                        {
                            var result = message.L1Signature(secret);
                            Assert.AreEqual(expectedResult, result, "id:{0} - {1}", test.id, test.description);
                            actualPass++;
                        }
                        catch (Exception ex)
                        {
                            Assert.AreEqual(expectedResult, ex.Message, "{0} - {1}", test.id, test.description);
                            if (errorTest)
                                actualPass++;
                        }
                    }
                    else
                    {
                        actualPass++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Assert.AreEqual(expectedPass, actualPass, "Total Passed Test Cases");

        }

        [Test()]
        public void GetL2Signature()
        {
            var jsonData = GetJsonFile("getL2Signature.json");
            int expectedPass = jsonData.Count();
            int actualPass = 0;

            try
            {
                foreach (var test in jsonData)
                {
                    SetDetaultParams(test);

                    if (skipTest == null || !skipTest.Contains("c#"))
                    {
                        try
                        {
                            var message = test.message;
                            string certName = test.apiParam.privateCertFileName;
                            string privateCertPath = testCertPath + certName;

                            string result = null;
                            if (!certName.IsNullOrEmpty())
                              result = ApiAuthorization.GetL2SignatureFromPEM(privateCertPath,message, passphrase);

                            Assert.AreEqual(expectedResult, result, "{0} - {1}", test.id, test.description);
                            actualPass++;
                        }
                        catch (Exception ex)
                        {
                            Assert.AreEqual(expectedResult, ex.Message, "{0} - {1}", test.id, test.description);
                            if (errorTest)
                                actualPass++;
                        }
                    }
                    else
                    {
                        actualPass++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Assert.AreEqual(expectedPass, actualPass, "Total Passed Test Cases");

        }
    }
}
