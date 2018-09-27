using ApiUtilLib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;
using System.Security.Cryptography;

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

                        var baseString = ApiAuthorization.BaseString(authPrefix, signatureMethod, appId, signatureURL, httpMethod, apiList, nonce, timeStamp, version);

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

            Assert.AreEqual(expectedPass, actualPass);

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
                            Assert.AreEqual(expectedResult.ToBool(),result);
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

            Assert.AreEqual(expectedPass, actualPass);
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
                            Assert.IsTrue(result);
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

            Assert.AreEqual(expectedPass, actualPass);
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
                            var message = test.message;
                            var signature = test.apiParam.signature;
                            string certName = test.apiParam.privateCertFileName;
                            string privateCertPath = testCertPath + certName;

                            RSACryptoServiceProvider privateKey = null;
                            if (!certName.IsNullOrEmpty())
                                privateKey = ApiAuthorization.PrivateKeyFromP12(privateCertPath, passphrase);
                            var result = ApiAuthorization.Token(realm, authPrefix, httpMethod, signatureURL, appId, secret, apiList, privateKey, nonce, timeStamp, version);

                            Assert.AreEqual(expectedResult, result);
                            actualPass++;
                        }
                        catch (Exception ex)
                        {
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
                throw;
            }
            Assert.AreEqual(expectedPass, actualPass);
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
                        var signature = test.apiParam.signature;
                        try
                        {
                            var result = message.L1Signature(secret);
                            Assert.AreEqual(expectedResult, result);
                            actualPass++;
                        }
                        catch (Exception)
                        {
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

            Assert.AreEqual(expectedPass, actualPass);

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
                            var signature = test.apiParam.signature;
                            string certName = test.apiParam.privateCertFileName;
                            string privateCertPath = testCertPath + certName;

                            RSACryptoServiceProvider privateKey = null;
                            if (!certName.IsNullOrEmpty())
                              privateKey = ApiAuthorization.PrivateKeyFromPEM(privateCertPath, passphrase);

                            var result = message.L2Signature(privateKey);

                            Assert.AreEqual(expectedResult, result);
                            actualPass++;
                        }
                        catch (Exception ex)
                        {
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

            Assert.AreEqual(expectedPass, actualPass);

        }
    }
}
