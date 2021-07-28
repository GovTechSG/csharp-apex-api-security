using ApiUtilLib;
using NUnit.Framework;
using System;
//using System.Collections.Generic;
//using System.Linq;
//using Newtonsoft.Json.Serialization;
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

            if (!SkipTest)
            {
                try
                {
                    string baseString = ApiAuthorization.BaseString(AuthPrefix, SignatureMethod, AppId, SignatureURL, FormData, HttpMethod, Nonce, TimeStamp, Version);
                    Assert.AreEqual(ExpectedResult, baseString, "{0} - {1}", testData.Id, testData.Description);
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

        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "verifyL1Signature.json" })]
        public void VerifyL1Signature(TestParam testData)
        {
            SetDetaultParams(testData);

            if (!SkipTest)
            {
                string message = testData.Message;
                string signature = testData.ApiParam.Signature;
                bool result = signature.VerifyL1Signature(Secret, message);
                try
                {
                    Assert.AreEqual(ExpectedResult.ToBool(), result, "{0} - {1}", testData.Id, testData.Description);
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

        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "verifyL2Signature.json" })]
        public void VerifyL2Signature(TestParam testData)
        {
            try
            {
                //var test = jsonData;
                SetDetaultParams(testData);

                if (!SkipTest)
                {

                    string message = testData.Message;
                    string signature = testData.ApiParam.Signature;
                    string certPath = testCertPath + testData.PublicCertFileName;

                    PublicKeyFileType fileType = PublicKeyFileType.CERTIFICATE;
                    if (testData.PublicCertFileName.ToLower().EndsWith(".key"))
                    {
                        fileType = PublicKeyFileType.PUBLIC_KEY;
                    }
                    if (testData.PublicCertFileName.ToLower().EndsWith(".p12") || testData.PublicCertFileName.ToLower().EndsWith(".pfx"))
                    {
                        fileType = PublicKeyFileType.P12_OR_PFX;
                    }

                    RSACryptoServiceProvider publicKey = ApiAuthorization.GetPublicKey(certPath, fileType, Passphrase);

                    bool result = signature.VerifyL2Signature(publicKey, message);

                    Assert.IsTrue(result, "{0} - {1}", testData.Id, testData.Description);
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

        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "getSignatureToken.json" })]
        public void TestTokenSignature(TestParam testData)
        {
            SetDetaultParams(testData);

            if (!SkipTest)
            {
                try
                {
                    string certName = testData.ApiParam.PrivateCertFileNameP12;
                    string privateCertPath = testCertPath + certName;
                    RSACryptoServiceProvider privateKey = null;
                    if (!certName.IsNullOrEmpty())
                    {
                        privateKey = ApiAuthorization.GetPrivateKey(PrivateKeyFileType.P12_OR_PFX, GetLocalPath(privateCertPath), Passphrase);
                    }

                    AuthParam authParam = new AuthParam
                    {
                        url = SignatureURL,
                        httpMethod = HttpMethod,
                        appName = AppId,
                        appSecret = Secret,
                        formData = FormData,
                        privateKey = privateKey,
                        timestamp = TimeStamp,
                        nonce = Nonce
                    };
                    string result = ApiAuthorization.TokenV2(authParam).Token;

                    if (TimeStamp.Equals("%s") || Nonce.Equals("%s"))
                    {
                        Match m = Regex.Match(result, "apex_(l[12])_([ei]g)_signature=(\\S+)", RegexOptions.IgnoreCase);
                        string signature_value = "";
                        if (m.Success)
                        {
                            signature_value = m.Groups[3].Value;
                        }

                        ExpectedResult = ExpectedResult.Replace("signature=\"%s\"", "signature=" + signature_value);
                    }

                    Assert.AreEqual(ExpectedResult, result, "{0} - {1}", testData.Id, testData.Description);
                }
                catch (Exception ex)
                {
                    Assert.AreEqual(ExpectedResult, ex.Message, "{0} - {1}", testData.Id, testData.Description);
                }
            }
            else
            {
                Assert.Ignore();
            }
        }

        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "getL1Signature.json" })]
        public void GetL1Signature(TestParam testData)
        {
            SetDetaultParams(testData);

            if (!SkipTest)
            {
                string message = testData.Message;
                try
                {
                    string result = message.L1Signature(Secret);
                    Assert.AreEqual(ExpectedResult, result, "id:{0} - {1}", testData.Id, testData.Description);
                }
                catch (Exception ex)
                {
                    Assert.AreEqual(ExpectedResult, ex.Message, "{0} - {1}", testData.Id, testData.Description);
                }
            }
            else
            {
                Assert.Ignore();
            }
        }

        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "getL2Signature.json" })]
        public void GetL2Signature(TestParam testData)
        {
            SetDetaultParams(testData);

            if (!SkipTest)
            {
                try
                {
                    string message = testData.Message;
                    string certName = testData.ApiParam.PrivateCertFileName;
                    string privateCertPath = testCertPath + certName;

                    RSACryptoServiceProvider privateKey = null;

                    if (!certName.IsNullOrEmpty())
                    {
                        privateKey = ApiAuthorization.GetPrivateKey(PrivateKeyFileType.PEM_PKCS1, privateCertPath, Passphrase);
                    }

                    string result = message.L2Signature(privateKey);

                    Assert.AreEqual(ExpectedResult, result, "{0} - {1}", testData.Id, testData.Description);
                }
                catch (Exception ex)
                {
                    // remove the file path that is machine dependent.
                    string err = ex.Message.Replace(testCertPath, "");

                    Assert.AreEqual(ExpectedResult, err, "{0} - {1}", testData.Id, testData.Description);
                }
            }
            else
            {
                Assert.Ignore();
            }
        }
    }
}
