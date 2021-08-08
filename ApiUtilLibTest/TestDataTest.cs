using ApiUtilLib;
using NUnit.Framework;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ApexUtilLibTest
{
    [TestFixture()]
    public class TestDataTest : BaseService
    {
        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "getSignatureBaseString.json" })]
        public void TestBaseString(TestParam testCase)
        {
            SetDetaultParams(testCase);

            if (SkipTest)
            {
                Assert.Ignore();
            }
            else
            {
                if (testCase.ErrorTest)
                {
                    Assert.Fail("No exception defined for test case...");
                }
                else
                {
                    string baseString = ApiAuthorization.BaseString(AuthPrefix, SignatureMethod, AppId, SignatureURL, FormData, HttpMethod, Nonce, TimeStamp, Version);
                    Assert.AreEqual(ExpectedResult, baseString, testCase.ToString());
                }
            }
        }

        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "verifyL1Signature.json" })]
        public void VerifyL1Signature(TestParam testCase)
        {
            SetDetaultParams(testCase);
            if (SkipTest)
            {
                Assert.Ignore();
            }
            else
            {
                if (testCase.ErrorTest)
                {
                    Assert.Fail("No exception defined for test case...");
                }
                else
                {
                    string message = testCase.Message;
                    string signature = testCase.ApiParam.Signature;
                    bool result = signature.VerifyL1Signature(Secret, message);

                    Assert.AreEqual(ExpectedResult.ToBool(), result, testCase.ToString());
                }
            }
        }

        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "verifyL2Signature.json" })]
        public void VerifyL2Signature(TestParam testCase)
        {
            SetDetaultParams(testCase);

            if (SkipTest)
            {
                Assert.Ignore();
            }
            else
            {
                if (testCase.ErrorTest)
                {
                    Assert.Fail("No exception defined for test case...");
                }
                else
                {
                    string message = testCase.Message;
                    string signature = testCase.ApiParam.Signature;
                    string certPath = testCertPath + testCase.PublicKeyFileName;

                    RSACryptoServiceProvider publicKey = ApiAuthorization.GetPublicKey(certPath, Passphrase);
                    bool result = signature.VerifyL2Signature(publicKey, message);

                    Assert.AreEqual(ExpectedResult.ToBool(), result, testCase.ToString());
                }
            }
        }

        private Exception GetTokenException<T>(AuthParam authParam) where T : Exception
        {
            return Assert.Throws<T>(() => { string result = ApiAuthorization.TokenV2(authParam).Token; });
        }

        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "getSignatureToken.json" })]
        public void GetToken(TestParam testCase)
        {
            SetDetaultParams(testCase);

            if (SkipTest)
            {
                Assert.Ignore();
            }
            else
            {
                RSACryptoServiceProvider privateKey = null;
                if (!testCase.ApiParam.PrivateKeyFileName.IsNullOrEmpty())
                {
                    privateKey = ApiAuthorization.GetPrivateKey(Path.Combine(testCertPath, testCase.ApiParam.PrivateKeyFileName), Passphrase);
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

                if (testCase.ErrorTest)
                {
                    //Assert.Fail("No exception defined for test case...");

                    Exception ex;
                    switch (testCase.Exception)
                    {
                        case "ArgumentNullException":
                            {
                                ex = GetTokenException<ArgumentNullException>(authParam);
                                break;
                            }
                        default:
                            {
                                throw new Exception($"Exception ({testCase.Exception}) not defined in json, please update the test case.");
                            }
                    };
                    ValidateErrorMessage(testCase, ex);
                }
                else
                {
                    string result = ApiAuthorization.TokenV2(authParam).Token;

                    var expectedResult = ExpectedResult;
                    if (string.IsNullOrEmpty(TimeStamp) || string.IsNullOrEmpty(Nonce))
                    {
                        if (string.IsNullOrEmpty(TimeStamp))
                        {
                            expectedResult = expectedResult.Replace("timestamp=\"%s\"", "timestamp=\"" + authParam.timestamp + "\"");
                        }

                        if (string.IsNullOrEmpty(Nonce))
                        {
                            expectedResult = expectedResult.Replace("nonce=\"%s\"", "nonce=\"" + authParam.nonce + "\"");
                        }

                        Match m = Regex.Match(result, "apex_(l[12])_([ei]g)_signature=(\\S+)", RegexOptions.IgnoreCase);
                        string signature_value = "";
                        if (m.Success)
                        {
                            signature_value = m.Groups[3].Value;
                        }

                        expectedResult = expectedResult.Replace("signature=\"%s\"", "signature=" + signature_value);
                    }

                    Assert.AreEqual(expectedResult, result, testCase.ToString());
                }
            }
        }

        private Exception GetL1SignatureException<T>(TestParam testCase) where T : Exception
        {
            return Assert.Throws<T>(() => testCase.Message.L1Signature(testCase.ApiParam.Secret));
        }

        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "getL1Signature.json" })]
        public void GetL1Signature(TestParam testCase)
        {
            SetDetaultParams(testCase);
            if (SkipTest)
            {
                Assert.Ignore();
            }
            else
            {
                if (testCase.ErrorTest)
                {
                    //Assert.Fail("No exception defined for test case...");

                    Exception ex;
                    switch(testCase.Exception)
                    {
                        case "ArgumentException":
                            {
                                ex = GetL1SignatureException<ArgumentException>(testCase);
                                break;
                            }
                        case "ArgumentNullException":
                            {
                                ex = GetL1SignatureException<ArgumentNullException>(testCase);
                                break;
                            }
                        default:
                            {
                                throw new Exception($"Exception ({testCase.Exception}) not defined in json, please update the test case.");
                            }
                    };
                    ValidateErrorMessage(testCase, ex);
                }
                else
                {
                    string message = testCase.Message;
                    string result = message.L1Signature(Secret);
                    Assert.AreEqual(ExpectedResult, result, testCase.ToString());
                }
            }
        }

        private Exception GetL2SignatureException<T>(TestParam testCase, RSACryptoServiceProvider privateKey) where T : Exception
        {
            return Assert.Throws<T>(() => testCase.Message.L2Signature(privateKey));
        }

        [Test(), TestCaseSource(nameof(GetJsonFile), new object[] { "getL2Signature.json" })]
        public void GetL2Signature(TestParam testCase)
        {
            SetDetaultParams(testCase);

            if (SkipTest)
            {
                Assert.Ignore();
            }
            else
            {
                RSACryptoServiceProvider privateKey = null;
                if (!testCase.ApiParam.PrivateKeyFileName.IsNullOrEmpty())
                {
                    privateKey = ApiAuthorization.GetPrivateKey(Path.Combine(testCertPath, testCase.ApiParam.PrivateKeyFileName), Passphrase);
                }

                if (testCase.ErrorTest)
                {
                    //Assert.Fail("No exception defined for test case...");

                    Exception ex;
                    switch (testCase.Exception)
                    {
                        case "ArgumentException":
                            {
                                ex = GetL2SignatureException<ArgumentException>(testCase, privateKey);
                                break;
                            }
                        case "ArgumentNullException":
                            {
                                ex = GetL2SignatureException<ArgumentNullException>(testCase, privateKey);
                                break;
                            }
                        default:
                            {
                                throw new Exception($"Exception ({testCase.Exception}) not defined in json, please update the test case.");
                            }
                    };
                    ValidateErrorMessage(testCase, ex);
                }
                else
                {
                    string result = testCase.Message.L2Signature(privateKey);

                    Assert.AreEqual(ExpectedResult, result, testCase.ToString());
                }
            }
        }

        private Exception GetPrivateKeyException<T>(string keyFileName, string passphrase) where T : Exception
        {
            return Assert.Throws<T>(() => ApiAuthorization.GetPrivateKey(keyFileName, passphrase));
        }

        private Exception GetPublicKeyException<T>(string keyFileName, string passphrase) where T : Exception
        {
            return Assert.Throws<T>(() => ApiAuthorization.GetPublicKey(keyFileName, passphrase));
        }

        [Test, TestCaseSource(nameof(GetJsonFile), new object[] { "verifySupportedKeyFileType.json" })]
        public void VerifySupportedKeyFileType(TestParam testCase)
        {
            SetDetaultParams(testCase);

            if (SkipTest)
            {
                Assert.Ignore();
            }
            else
            {
                if (testCase.ErrorTest)
                {
                    Exception ex = null;
                    if (testCase.ApiParam != null && testCase.ApiParam.PrivateKeyFileName != null)
                    {
                        string keyFileName = $"{testCertPath}/{testCase.ApiParam.PrivateKeyFileName}";
                        string passphrase = testCase.ApiParam.Passphrase;

                        switch (testCase.Exception)
                        {
                            case "ArgumentException":
                                {
                                    ex = GetPrivateKeyException<ArgumentException>(keyFileName, passphrase);
                                    break;
                                }
                            case "CryptographicException":
                                {
                                    ex = GetPrivateKeyException<CryptographicException>(keyFileName, passphrase);
                                    break;
                                }
                            case "FileNotFoundException":
                                {
                                    ex = GetPrivateKeyException<FileNotFoundException>(keyFileName, passphrase);
                                    break;
                                }
                            case "PemException":
                                {
                                    ex = GetPrivateKeyException<Org.BouncyCastle.OpenSsl.PemException>(keyFileName, passphrase);
                                    break;
                                }
                            case "InvalidCastException":
                                {
                                    ex = GetPrivateKeyException<InvalidCastException>(keyFileName, passphrase);
                                    break;
                                }
                            default:
                                throw new Exception($"Exception ({testCase.Exception}) not defined in json, please update the test case.");
                        }
                        ValidateErrorMessage(testCase, ex);
                    }

                    if (testCase.PublicKeyFileName != null)
                    {
                        string keyFileName = $"{testCertPath}/{testCase.PublicKeyFileName}";
                        string passphrase = testCase.Passphrase;

                        switch (testCase.Exception)
                        {
                            case "FileNotFoundException":
                                {
                                    ex = GetPublicKeyException<FileNotFoundException>(keyFileName, passphrase);
                                    break;
                                }
                            case "CryptographicException":
                                {
                                    ex = GetPublicKeyException<CryptographicException>(keyFileName, passphrase);
                                    break;
                                }
                            default:
                                throw new Exception($"Exception ({testCase.Exception}) not defined in json, please update the test case.");
                        };
                    }

                    ValidateErrorMessage(testCase, ex);
                }
                else
                {
                    RSACryptoServiceProvider privateKey = ApiAuthorization.GetPrivateKey($"{testCertPath}/{testCase.ApiParam.PrivateKeyFileName}", testCase.ApiParam.Passphrase);
                    string signature = testCase.Message.L2Signature(privateKey);

                    RSACryptoServiceProvider publicKey = ApiAuthorization.GetPublicKey($"{testCertPath}/{testCase.PublicKeyFileName}", testCase.Passphrase);
                    bool result = signature.VerifyL2Signature(publicKey, testCase.Message);

                    Assert.AreEqual(testCase.ApiParam.Signature, signature, "Signature - {0} - {1}", testCase.Id, testCase.Description);
                    Assert.IsTrue(result, "{0} - {1}", testCase.Id, testCase.Description);
                }
            }
        }
    }
}
