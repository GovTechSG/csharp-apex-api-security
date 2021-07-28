using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace ApiUtilLib
{
    public static class ApiAuthorization
    {
        // PKI - https://www.lyquidity.com/devblog/?p=70
        // hmac - https://www.jokecamp.com/blog/examples-of-creating-base64-hashes-using-hmac-sha256-in-different-languages/#csharp

        public static LoggerBase Logger => LoggerManager.Logger;

        public static string L1Signature(this string message, string secret)
        {
            Logger.LogEnter(LoggerBase.Args(message, "***secret***"));

            if (string.IsNullOrEmpty(message))
            {
                Logger.LogError("{0} must not be null or empty.", nameof(message));

                throw new ArgumentNullException(nameof(message));
            }

			if (string.IsNullOrEmpty(secret))
			{
				Logger.LogError("{0} must not be null or empty.", nameof(secret));

				throw new ArgumentNullException(nameof(secret));
			}
			
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] secretBytes = Encoding.UTF8.GetBytes(secret);

            string base64Token = null;
            using (HMACSHA256 hmacsha256 = new HMACSHA256(secretBytes))
            {
                byte[] messageHash = hmacsha256.ComputeHash(messageBytes);

                base64Token = Convert.ToBase64String(messageHash);

                Logger.LogDebug("L1 Signature:: {0}", base64Token);
            }

            Logger.LogExit(LoggerBase.Args(base64Token));
            return base64Token;
        }

        public static bool VerifyL1Signature(this string signature, string secret, string message)
        {
            Logger.LogEnter(LoggerBase.Args(signature, message, "***secret***"));

            bool signatureValid = L1Signature(message, secret) == signature;

            Logger.LogExit(LoggerBase.Args(signatureValid));
            return signatureValid;
        }

        public static string L2Signature(this string message, RSACryptoServiceProvider privateKey)
        {
            Logger.LogEnter(LoggerBase.Args(message, privateKey));

            if (string.IsNullOrEmpty(message))
			{
				Logger.LogError("{0} must not be null or empty.", nameof(message));

				throw new ArgumentNullException(nameof(message));
			}

            if (privateKey == null)
			{
				Logger.LogError("{0} must not be null.", nameof(privateKey));

				throw new ArgumentNullException(nameof(privateKey));
			}

			byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            string base64Token = null;
            using (SHA256Managed sha256 = new SHA256Managed())
            {
                byte[] signatureBytes = privateKey.SignData(messageBytes, sha256);

                base64Token = Convert.ToBase64String(signatureBytes);
            }

            Logger.LogExit(LoggerBase.Args(base64Token));
            return base64Token;
        }

        /// <summary>
        /// Supported Fie Types:
        ///     .PEM/.KEY - Private Key in pkcs1 format with Base64 Encoding, i.e. file start with
        ///         -----BEGIN RSA PRIVATE KEY-----
        ///     .PEM/.KEY - Private Key in pkcs8 format with Base64 Encoding, i.e. file start with
        ///         -----BEGIN PRIVATE KEY-----
        ///         -----BEGIN ENCRYPTED PRIVATE KEY-----
        ///     .P12/PFX - Certificate Store in binary format with p12 or pfx Encoding, optional passPhrase supported
        ///     Note:
        ///         P12/PFX Certificate Store must conatin only one pair of public/private keys with store and key passphare must be the same
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="keyFileName"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        public static RSACryptoServiceProvider GetPrivateKey(PrivateKeyFileType fileType, string keyFileName, string passPhrase = null)
        {
            Logger.LogEnterExit(LoggerBase.Args(keyFileName, "***password***"));

            RSACryptoServiceProvider privateKey = null;

            if (fileType == PrivateKeyFileType.P12_OR_PFX)
            {
                X509Certificate2 privateCert = new X509Certificate2(File.ReadAllBytes(keyFileName), passPhrase, X509KeyStorageFlags.Exportable);

                RSACryptoServiceProvider OriginalPrivateKey = (RSACryptoServiceProvider)privateCert.PrivateKey;

                if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    privateKey = OriginalPrivateKey;
                }
                else
                {
                    privateKey = new RSACryptoServiceProvider();
                    privateKey.ImportParameters(OriginalPrivateKey.ExportParameters(true));
                }
            }
            if (fileType == PrivateKeyFileType.PEM_PKCS1 || fileType == PrivateKeyFileType.PEM_PKCS8)
            {
                RSAParameters rsaParams;

                using (StreamReader reader = File.OpenText(keyFileName)) // file containing RSA PKCS8 private key
                {
                    if (fileType == PrivateKeyFileType.PEM_PKCS8)
                    {
                        RsaPrivateCrtKeyParameters keyPair_pkcs8;

                        keyPair_pkcs8 = (RsaPrivateCrtKeyParameters)new PemReader(reader, new PasswordFinder(passPhrase)).ReadObject();
                        rsaParams = DotNetUtilities.ToRSAParameters(keyPair_pkcs8);
                    }
                    else
                    {
                        AsymmetricCipherKeyPair keyPair_pkcs1;

                        keyPair_pkcs1 = (AsymmetricCipherKeyPair)new PemReader(reader, new PasswordFinder(passPhrase)).ReadObject();
                        rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)keyPair_pkcs1.Private);
                    }
                }

                privateKey = new RSACryptoServiceProvider();
                privateKey.ImportParameters(rsaParams);
            }

            return privateKey;
        }

        /// <summary>
        /// Supported File Types:
        ///     .PEM - Certificate in text format with Base64 Encoding
        ///     .CER - Certificate in text format with Base64 Encoding
        ///     .CER - Certificate in binary format with DER Encoding
        ///     .DER - Certificate in binary format with DER Encoding
        ///     .KEY - Public Key in text format with Base64 Encoding
        ///     .P12/PFX - Certificate Store in binary format with p12 or pfx Encoding, optional passPhrase supported
        ///     Note:
        ///         P12/PFX Certificate Store must conatin only one pair of public/private key with store and key passphare must be the same
        /// </summary>
        /// <param name="fileName">File name for the supported file type</param>
        /// <param name="fileType">supported file type</param>
        /// <param name="passPhrase">store and key must used the same passPhrase</param>
        /// <returns>Public Key as RSACryptoServiceProvider</returns>
        public static RSACryptoServiceProvider GetPublicKey(string fileName, PublicKeyFileType fileType = PublicKeyFileType.CERTIFICATE, string passPhrase = null)
        {
            Logger.LogEnterExit(LoggerBase.Args(fileName));

            RSACryptoServiceProvider key = null;

            if (fileType == PublicKeyFileType.P12_OR_PFX)
            {
                X509Certificate2 cert = new X509Certificate2(File.ReadAllBytes(fileName), passPhrase, X509KeyStorageFlags.Exportable);

                key = (RSACryptoServiceProvider)cert.PublicKey.Key;
            }

            if (fileType == PublicKeyFileType.CERTIFICATE)
            {
                X509Certificate2 cert = new X509Certificate2(File.ReadAllBytes(fileName));

                key = (RSACryptoServiceProvider)cert.PublicKey.Key;
            }

            if (fileType == PublicKeyFileType.PUBLIC_KEY)
            {
                using (StreamReader reader = File.OpenText(fileName))
                {
                    PemReader pr = new PemReader(reader);
                    AsymmetricKeyParameter publicKey = (AsymmetricKeyParameter)pr.ReadObject();

                    RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)publicKey);
                    key = new RSACryptoServiceProvider();
                    key.ImportParameters(rsaParams);
                }
            }

            return key;
        }

        public static bool VerifyL2Signature(this string signature, RSACryptoServiceProvider publicKey, string message)
        {
            Logger.LogEnter(LoggerBase.Args(signature, message, publicKey));

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] signatureBytes = Convert.FromBase64String(signature);

            bool signatureValid = false;
            using (SHA256Managed sha256 = new SHA256Managed())
            {
                byte[] messageHash = sha256.ComputeHash(messageBytes);

                signatureValid = publicKey.VerifyHash(messageHash, CryptoConfig.MapNameToOID("SHA256"), signatureBytes);
            }

            Logger.LogExit(LoggerBase.Args(signatureValid));
            return signatureValid;
        }

        public static string BaseString(
            string authPrefix
            , SignatureMethod signatureMethod
            , string appId
            , Uri siteUri
            , FormData formData
            , HttpMethod httpMethod
            , string nonce
            , string timestamp
            , string version)
        {
            try
            {
                Logger.LogEnter(LoggerBase.Args(authPrefix, signatureMethod, appId, siteUri, httpMethod, formData, nonce, timestamp));

                authPrefix = authPrefix.ToLower();

                // make sure that the url are valid
                if (siteUri.Scheme != "http" && siteUri.Scheme != "https")
                {
                    throw new NotSupportedException("Support http and https protocol only.");
                }

                // make sure that the port no and querystring are remove from url
                string url = siteUri.Port == (-1) || siteUri.Port == 80 || siteUri.Port == 443
                    ? string.Format("{0}://{1}{2}", siteUri.Scheme, siteUri.Host, siteUri.AbsolutePath)
                    : string.Format("{0}://{1}:{2}{3}", siteUri.Scheme, siteUri.Host, siteUri.Port, siteUri.AbsolutePath);
                Logger.LogInformation("url:: {0}", url);

                // helper calss that handle parameters and form fields
                ApiList paramList = new ApiList();

                // process QueryString from url by transfering it to paramList
                if (siteUri.Query.Length > 1)
                {
                    string queryString = siteUri.Query.Substring(1); // remove the ? from first character
                    Logger.LogInformation("queryString:: {0}", queryString);

                    string[] paramArr = queryString.Split('&');
                    foreach (string item in paramArr)
                    {
                        string key = null;
                        string val = null;
                        string[] itemArr = item.Split('=');
                        key = itemArr[0];
                        if (itemArr.Length > 1)
                        {
                            val = itemArr[1];
                        }

                        paramList.Add(WebUtility.UrlDecode(key), WebUtility.UrlDecode(val));
                    }

                    Logger.LogInformation("paramList:: {0}", paramList);
                }

                // add the form fields to paramList
                if (formData != null && formData.Count > 0)
                {
                    paramList.AddRange(formData.GetBaseStringList());
                }

                paramList.Add(authPrefix + "_timestamp", timestamp);
                paramList.Add(authPrefix + "_nonce", nonce);
                paramList.Add(authPrefix + "_app_id", appId);
                paramList.Add(authPrefix + "_signature_method", signatureMethod.ToString());
                paramList.Add(authPrefix + "_version", version);

                string baseString = httpMethod.ToString() + "&" + url + "&" + paramList.ToString();

                Logger.LogDebug("BaseString:: {0}", baseString);

                Logger.LogExit(LoggerBase.Args(baseString));
                return baseString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static long NewTimestamp()
        {
            DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);

            Logger.LogEnterExit(LoggerBase.Args(dto.ToUnixTimeMilliseconds().ToString()));
            return dto.ToUnixTimeMilliseconds();
        }

        public static string NewNonce()
        {
            long nonce = 0;
            byte[] data = null;
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                // Buffer storage.
                data = new byte[32];
                // Fill buffer.
                rng.GetBytes(data);
            }

            Logger.LogEnterExit(LoggerBase.Args(nonce.ToString()));
            return Convert.ToBase64String(data);
        }

        public static int HttpRequest(Uri url, string token, FormData formData, HttpMethod httpMethod = HttpMethod.GET, bool ignoreServerCert = false)
        {
            Logger.LogEnter(LoggerBase.Args(url, token, formData == null ? "null" : formData.ToString(), httpMethod));

            int returnCode = 0;

            if (ignoreServerCert)
            {
                InitiateSSLTrust();
            }

            WebResponse response = null;
            StreamReader reader = null;
            Stream dataStream = null;

            try
            {
                // Create a request using a URL that can receive a post.   
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = httpMethod.ToString();

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Add("Authorization", token);
                }

                if (formData != null)
                {
                    // Set the Method property of the request to POST.  
                    request.Method = HttpMethod.POST.ToString();

                    // Create POST data and convert it to a byte array.
                    //string postData = "This is a test that posts this string to a Web server.";
                    byte[] byteArray = Encoding.UTF8.GetBytes(formData.ToString());

                    // Set the ContentType property of the WebRequest.  
                    request.ContentType = "application/x-www-form-urlencoded";

                    // Set the ContentLength property of the WebRequest.  
                    request.ContentLength = byteArray.Length;

                    // Get the request stream.  
                    dataStream = request.GetRequestStream();

                    // Write the data to the request stream.  
                    dataStream.Write(byteArray, 0, byteArray.Length);

                    // Close the Stream object.  
                    dataStream.Close();
                }

                // Get the response.  
                response = request.GetResponse();

                // Display the status.  
                Logger.LogDebug("Response Code:: {0} - {1}", ((HttpWebResponse)response).StatusDescription, (int)((HttpWebResponse)response).StatusCode);

                returnCode = (int)((HttpWebResponse)response).StatusCode;

                // Get the stream containing content returned by the server.  
                dataStream = response.GetResponseStream();

                // Open the stream using a StreamReader for easy access.  
                reader = new StreamReader(dataStream);

                // Read the content.  
                string responseFromServer = reader.ReadToEnd();

                // Display the content.  
                Logger.LogDebug("Response Data :: {0}", responseFromServer);
            }
            catch (WebException e)
            {                
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    response = (HttpWebResponse)e.Response;
					
                    returnCode = (int)((HttpWebResponse)response).StatusCode;

                    Logger.LogWarning("Error Code: {0} - {1}", returnCode, ((HttpWebResponse)response).StatusDescription);

					// Get the stream containing content returned by the server.  
					dataStream = response.GetResponseStream();

					// Open the stream using a StreamReader for easy access.  
					reader = new StreamReader(dataStream);

					// Read the content.  
					string responseFromServer = reader.ReadToEnd();

					// Display the content.  
					Logger.LogDebug("Response Data :: {0}", responseFromServer);
				}
                else
                {
                    Logger.LogError("Error: {0}", e.Status);
                    Logger.LogError("Error: {0}", e.ToString());
                }
            }
            finally
            {
                // Clean up the streams and the response.  
                if (response != null)
                {
                    response.Close();
                }

                if (reader != null)
                {
                    reader.Close();
                }

                if (dataStream != null)
                {
                    dataStream.Close();
                }
            }

            Logger.LogExit(LoggerBase.Args(returnCode));
            return returnCode;
        }

        private static bool iniSslTrust;

        private static void InitiateSSLTrust()
        {
            if (iniSslTrust)
            {
                return;
            }

            try
            {
                //Change SSL checks so that all checks pass
                ServicePointManager.ServerCertificateValidationCallback =
                   new System.Net.Security.RemoteCertificateValidationCallback(
                        delegate
                        { return true; }
                    );

                iniSslTrust = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private class PasswordFinder : IPasswordFinder
        {
            private readonly string password;

            public PasswordFinder(string password)
            {
                this.password = password;
            }


            public char[] GetPassword()
            {
                return password.ToCharArray();
            }
        }

        private const string APEX1_DOMAIN = "api.gov.sg";

        public static AuthToken TokenV2(AuthParam authParam) {
            Logger.LogEnter(LoggerBase.Args(authParam));

            string nullValueErrMsg = "One or more required parameters are missing!";

            if (authParam.url == null)
            {
                Logger.LogError(nullValueErrMsg);
                throw new ArgumentNullException(nameof(authParam.url));
            }

            // split url into hostname and domain
            string gatewayName = authParam.url.Host.Split('.')[0];
            string originalDomain = string.Join(".", authParam.url.Host.Split('.').Skip(1).ToArray());

            // default api domain to external zone
            string apiDomain = string.Format(".{0}", originalDomain);
            string authSuffix = "eg";

            // for backward compatible with apex1
            if (authParam.url.Host.EndsWith(APEX1_DOMAIN))
            {
                apiDomain = string.Format(".e.{0}", originalDomain);
            }

            // switch to internal zone based on hostname suffix
            if (gatewayName.EndsWith("-pvt"))
            {
                authSuffix = "ig";

                // for backward compatible with apex1
                if (authParam.url.Host.EndsWith(APEX1_DOMAIN))
                {
                    apiDomain = string.Format(".i.{0}", originalDomain);
                }
            }

            // default auth level to l2, switch to l1 if appSecret provided
            string authLevel = "l2";
            if (authParam.appSecret != null)
            {
                authLevel = "l1";
            }

            // for backward compatible with apex1, update the signature url
            string signatureUrl = authParam.url.ToString().Replace(string.Format(".{0}", originalDomain), apiDomain);

            // Generate the nonce value
            authParam.nonce = authParam.nonce ?? NewNonce().ToString();
            authParam.timestamp = authParam.timestamp ?? NewTimestamp().ToString();

            authParam.signatureMethod = SignatureMethod.HMACSHA256;
            if (authParam.appSecret == null)
            {
                authParam.signatureMethod = SignatureMethod.SHA256withRSA;
            }

            string apexToken = "";
            List<string> baseStringList = new List<string>();

            if (authParam.appName != null)
            {
                string realm = string.Format("https://{0}", authParam.url.Host);
                string authPrefix = string.Format("apex_{0}_{1}", authLevel, authSuffix);
                if (authParam.version == null)
                {
                    authParam.version = "1.0";
                }

                string baseString = BaseString(
                        authPrefix,
                        authParam.signatureMethod,
                        authParam.appName,
                        new Uri(signatureUrl),
                        authParam.formData,
                        authParam.httpMethod,
                        authParam.nonce,
                        authParam.timestamp,
                        authParam.version
                        );
                baseStringList.Add(baseString);

                string base64Token = authLevel == "l1" ? baseString.L1Signature(authParam.appSecret) : baseString.L2Signature(authParam.privateKey);
                ApiList tokenList = new ApiList
                {
                    { "realm", realm },
                    { authPrefix + "_app_id", authParam.appName },
                    { authPrefix + "_nonce", authParam.nonce },
                    { authPrefix + "_signature_method", authParam.signatureMethod.ToString() },
                    { authPrefix + "_timestamp", authParam.timestamp },
                    { authPrefix + "_version", authParam.version },
                    { authPrefix + "_signature", base64Token }
                };

                apexToken = string.Format("{0} {1}", authPrefix.Substring(0, 1).ToUpperInvariant() + authPrefix.Substring(1), tokenList.ToString(", ", false, true));
            }
            string authToken = string.Format("{0}", apexToken);

            if (authParam.nextHop != null) {
                // propagate the information from root param to nextHop
                authParam.nextHop.httpMethod = authParam.httpMethod;
                //authParam.nextHop.queryString = authParam.queryString;
                authParam.nextHop.formData = authParam.formData;

                // get the apexToken for nextHop
                AuthToken nextHopResult = TokenV2(authParam.nextHop);

                // save the baseString
                baseStringList.Add(nextHopResult.BaseString);

                string netxHopToken = nextHopResult.Token;

                // combine the apexToken if required
                if (authToken == "") {
                    authToken = string.Format("{0}", netxHopToken);
                } else {
                    authToken += string.Format(", {0}", netxHopToken);
                }
            }

            Logger.LogExit(LoggerBase.Args(authToken, baseStringList));
            return new AuthToken(authToken, baseStringList);
        }
    }
}