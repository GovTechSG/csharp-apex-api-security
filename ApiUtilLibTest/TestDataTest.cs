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
                SetDetaultParams(testData);

                if (!SkipTest)
                {

                    string message = testData.Message;
                    string signature = testData.ApiParam.Signature;
                    string certPath = testCertPath + testData.PublicKeyFileName;

                    //PublicKeyFileType fileType = PublicKeyFileType.CERTIFICATE;
                    //if (testData.PublicKeyFileName.ToLower().EndsWith(".key"))
                    //{
                    //    fileType = PublicKeyFileType.PUBLIC_KEY;
                    //}
                    //if (testData.PublicKeyFileName.ToLower().EndsWith(".p12") || testData.PublicKeyFileName.ToLower().EndsWith(".pfx"))
                    //{
                    //    fileType = PublicKeyFileType.P12_OR_PFX;
                    //}

                    //RSACryptoServiceProvider publicKey = ApiAuthorization.GetPublicKey(certPath, fileType, Passphrase);
                    RSACryptoServiceProvider publicKey = ApiAuthorization.GetPublicKey(certPath, GetPublicKeyFileType(certPath), Passphrase);

                    bool result = signature.VerifyL2Signature(publicKey, message);

                    Assert.AreEqual(ExpectedResult.ToBool(), result, "{0} - {1}", testData.Id, testData.Description);
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
                    string certName = testData.ApiParam.PrivateKeyFileName;
                    string privateCertPath = testCertPath + certName;
                    RSACryptoServiceProvider privateKey = null;
                    if (!certName.IsNullOrEmpty())
                    {
                        privateKey = ApiAuthorization.GetPrivateKey(GetPrivateKeyFileType(privateCertPath), GetLocalPath(privateCertPath), Passphrase);
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

                    Assert.AreEqual(expectedResult, result, "{0} - {1}", testData.Id, testData.Description);
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
                    string certName = testData.ApiParam.PrivateKeyFileName;
                    string privateCertPath = testCertPath + certName;

                    RSACryptoServiceProvider privateKey = null;

                    if (!certName.IsNullOrEmpty())
                    {
                        //if (certName.EndsWith(".pkcs8.key"))
                        //{
                        //    privateKey = ApiAuthorization.GetPrivateKey(PrivateKeyFileType.PEM_PKCS8, privateCertPath, Passphrase);
                        //}
                        //else
                        //{
                        //    privateKey = ApiAuthorization.GetPrivateKey(PrivateKeyFileType.PEM_PKCS1, privateCertPath, Passphrase);
                        //}
                        privateKey = ApiAuthorization.GetPrivateKey(GetPrivateKeyFileType(privateCertPath), privateCertPath, Passphrase);
                    }

                    string result = message.L2Signature(privateKey);

                    Assert.AreEqual(ExpectedResult, result, "{0} - {1}", testData.Id, testData.Description);
                }
                catch (Exception ex)
                {
                    // remove the file path that is machine in error message.
                    string err = ex.Message.Replace(testCertPath, "");

                    Assert.AreEqual(ExpectedResult, err, "{0} - {1}", testData.Id, testData.Description);
                }
            }
            else
            {
                Assert.Ignore();
            }
        }

        private PrivateKeyFileType GetPrivateKeyFileType(string keyFileName)
        {
            string fileExtension = Path.GetExtension(keyFileName).ToLower();

            switch (fileExtension)
            {
                case ".p12":
                case ".pfx":
                    return PrivateKeyFileType.P12_OR_PFX;
                case ".pem":
                case ".key":
                    {
                        string pemString = File.ReadAllText(keyFileName);
                        if (pemString.StartsWith("-----BEGIN RSA PRIVATE KEY-----"))
                        {
                            return PrivateKeyFileType.PEM_PKCS1;
                        }
                        else
                        {
                            return PrivateKeyFileType.PEM_PKCS8;
                        }
                    }
                default:
                    throw new CryptographicException("No supported key formats were found. Check that the file format are supported.");
            }
        }

        private PublicKeyFileType GetPublicKeyFileType(string keyFileName)
        {
            string fileExtension = Path.GetExtension(keyFileName).ToLower();

            // check if the file contain certificate?
            // by default .pem file conatain key
            if (fileExtension == ".pem")
            {
                string pemString = File.ReadAllText(keyFileName);
                if (pemString.StartsWith("-----BEGIN CERTIFICATE-----"))
                {
                    // assume .pem as .cer
                    fileExtension = ".cer";
                }
            }

            switch (fileExtension)
            {
                case ".p12":
                case ".pfx":
                    {
                        return PublicKeyFileType.P12_OR_PFX;
                    }

                case ".cer":
                    {
                        return PublicKeyFileType.CERTIFICATE;
                    }

                case ".pem":
                case ".key":
                    {
                        return PublicKeyFileType.PUBLIC_KEY;
                    }

                default:
                    throw new CryptographicException("No supported key formats were found. Check that the file format are supported.");
            }
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
                        switch (testCase.Exception)
                        {
                            case "ArgumentException":
                                {
                                    ex = Assert.Throws<ArgumentException>(() => ApiAuthorization.GetPrivateKey(GetPrivateKeyFileType($"{testCertPath}/{testCase.ApiParam.PrivateKeyFileName}"), $"{testCertPath}/{testCase.ApiParam.PrivateKeyFileName}", testCase.ApiParam.Passphrase));
                                    break;
                                }
                            case "CryptographicException":
                                {
                                    ex = Assert.Throws<CryptographicException>(() => ApiAuthorization.GetPrivateKey(GetPrivateKeyFileType($"{testCertPath}/{testCase.ApiParam.PrivateKeyFileName}"), $"{testCertPath}/{testCase.ApiParam.PrivateKeyFileName}", testCase.ApiParam.Passphrase));
                                    break;
                                }
                            case "FileNotFoundException":
                                {
                                    ex = Assert.Throws<FileNotFoundException>(() => ApiAuthorization.GetPrivateKey(GetPrivateKeyFileType($"{testCertPath}/{testCase.ApiParam.PrivateKeyFileName}"), $"{testCertPath}/{testCase.ApiParam.PrivateKeyFileName}", testCase.ApiParam.Passphrase));
                                    break;
                                }
                            case "PemException":
                                {
                                    ex = Assert.Throws<Org.BouncyCastle.OpenSsl.PemException>(() => ApiAuthorization.GetPrivateKey(GetPrivateKeyFileType($"{testCertPath}/{testCase.ApiParam.PrivateKeyFileName}"), $"{testCertPath}/{testCase.ApiParam.PrivateKeyFileName}", testCase.ApiParam.Passphrase));
                                    break;
                                }
                            case "InvalidCastException":
                                {
                                    ex = Assert.Throws<InvalidCastException>(() => ApiAuthorization.GetPrivateKey(GetPrivateKeyFileType($"{testCertPath}/{testCase.ApiParam.PrivateKeyFileName}"), $"{testCertPath}/{testCase.ApiParam.PrivateKeyFileName}", testCase.ApiParam.Passphrase));
                                    break;
                                }
                            default:
                                throw new Exception($"Exception ({testCase.Exception}) not defined in json, please update the test case.");
                        }
                        ValidateErrorMessage(testCase, ex);
                    }

                    if (testCase.PublicKeyFileName != null)
                    {
                        switch (testCase.Exception)
                        {
                            case "FileNotFoundException":
                                {
                                    ex = Assert.Throws<FileNotFoundException>(() => ApiAuthorization.GetPublicKey($"{testCertPath}/{testCase.PublicKeyFileName}", GetPublicKeyFileType($"{testCertPath}/{testCase.PublicKeyFileName}"), testCase.Passphrase));
                                    break;
                                }
                            case "CryptographicException":
                                {
                                    ex = Assert.Throws<CryptographicException>(() => ApiAuthorization.GetPublicKey($"{testCertPath}/{testCase.PublicKeyFileName}", GetPublicKeyFileType($"{testCertPath}/{testCase.PublicKeyFileName}"), testCase.Passphrase));
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
                    RSACryptoServiceProvider privateKey = ApiAuthorization.GetPrivateKey(GetPrivateKeyFileType($"{testCertPath}/{testCase.ApiParam.PrivateKeyFileName}"), $"{testCertPath}/{testCase.ApiParam.PrivateKeyFileName}", testCase.ApiParam.Passphrase);
                    string signature = testCase.Message.L2Signature(privateKey);

                    RSACryptoServiceProvider publicKey = ApiAuthorization.GetPublicKey($"{testCertPath}/{testCase.PublicKeyFileName}", GetPublicKeyFileType($"{testCertPath}/{testCase.PublicKeyFileName}"), testCase.Passphrase);
                    bool result = signature.VerifyL2Signature(publicKey, testCase.Message);

                    Assert.AreEqual(testCase.ApiParam.Signature, signature, "Signature - {0} - {1}", testCase.Id, testCase.Description);
                    Assert.IsTrue(result, "{0} - {1}", testCase.Id, testCase.Description);
                }
            }
        }

    }
}
