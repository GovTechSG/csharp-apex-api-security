using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ApexUtilLib;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Linq;

namespace ApiUtilLib
{
    public static class ApiAuthorization
    {
        // PKI - https://www.lyquidity.com/devblog/?p=70
        // hmac - https://www.jokecamp.com/blog/examples-of-creating-base64-hashes-using-hmac-sha256-in-different-languages/#csharp

        public static LoggerBase Logger
        {
            get
            {
                return LoggerManager.Logger;
            }
        }

        public static string L1Signature(this string message, string secret)
        {
            Logger.LogEnter(LoggerBase.Args(message, "***secret***"));

            if (String.IsNullOrEmpty(message))
            {
                Logger.LogError("{0} must not be null or empty.", nameof(message));

                throw new ArgumentNullException(nameof(message));
            }

			if (String.IsNullOrEmpty(secret))
			{
				Logger.LogError("{0} must not be null or empty.", nameof(secret));

				throw new ArgumentNullException(nameof(secret));
			}
			
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] secretBytes = Encoding.UTF8.GetBytes(secret);

            string base64Token = null;
            using (var hmacsha256 = new HMACSHA256(secretBytes))
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

            bool signatureValid = ApiAuthorization.L1Signature(message, secret) == signature;

            Logger.LogExit(LoggerBase.Args(signatureValid));
            return signatureValid;
        }

        public static string L2Signature(this string message, RSACryptoServiceProvider privateKey)
        {
            Logger.LogEnter(LoggerBase.Args(message, privateKey));

            if (String.IsNullOrEmpty(message))
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

        public static RSACryptoServiceProvider PrivateKeyFromP12(string certificateFileName, string password)
        {
            Logger.LogEnterExit(LoggerBase.Args(certificateFileName, "***password***"));


            var privateCert = new X509Certificate2(System.IO.File.ReadAllBytes(certificateFileName), password, X509KeyStorageFlags.Exportable);

            var OriginalPrivateKey = (RSACryptoServiceProvider)privateCert.PrivateKey;

            // Transfer the private key to overcome the following error...
            // System.Security.Cryptography.CryptographicException "Invalid algorithm specified"
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
            {
                return OriginalPrivateKey;
            }
            else
            {
                var privateKey = new RSACryptoServiceProvider();
                privateKey.ImportParameters(OriginalPrivateKey.ExportParameters(true));

                return privateKey;
            }
        }

        public static RSACryptoServiceProvider PublicKeyFromCer(string certificateFileName)
        {
            Logger.LogEnterExit(LoggerBase.Args(certificateFileName));

            var privateCert = new X509Certificate2(System.IO.File.ReadAllBytes(certificateFileName));

            return (RSACryptoServiceProvider)privateCert.PublicKey.Key;
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
            , HttpMethod httpMethod
			, ApiList formList
			, string nonce
			, string timestamp
            , string version)
		{
            try
            {
                Logger.LogEnter(LoggerBase.Args(authPrefix, signatureMethod, appId, siteUri, httpMethod, formList, nonce, timestamp));

                authPrefix = authPrefix.ToLower();

                // make sure that the url are valid
                if (siteUri.Scheme != "http" && siteUri.Scheme != "https")
                {
                    throw new System.NotSupportedException("Support http and https protocol only.");
                }

                // make sure that the port no and querystring are remove from url
                var url = string.Format("{0}://{1}{2}", siteUri.Scheme, siteUri.Host, siteUri.AbsolutePath);
                Logger.LogInformation("url:: {0}", url);

                // helper calss that handle parameters and form fields
                ApiList paramList = new ApiList();  

                // process QueryString from url by transfering it to paramList
                if (siteUri.Query.Length > 1)
                {
                    var queryString = siteUri.Query.Substring(1); // remove the ? from first character
                    Logger.LogInformation("queryString:: {0}", queryString);

                    var paramArr = queryString.Split('&');
                    foreach (string item in paramArr)
                    {
                        string key = null;
                        string val = null;
                        var itemArr = item.Split('=');
                        key = itemArr[0];
                        if(itemArr.Length>1)
                            val = itemArr[1];
                        paramList.Add(key, System.Net.WebUtility.UrlDecode(val));
                    }

                    Logger.LogInformation("paramList:: {0}", paramList);
                }

                // add the form fields to paramList
                if (formList != null && formList.Count > 0)
                {
                    paramList.AddRange(formList);
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
                data = new byte[8];
                // Fill buffer.
                rng.GetBytes(data);
            }

            Logger.LogEnterExit(LoggerBase.Args(nonce.ToString()));

            return System.Convert.ToBase64String(data);
        }

		public static String Token(
            string realm
            , string authPrefix
            , HttpMethod httpMethod
            , Uri urlPath
            , string appId
            , string secret = null
            , ApiList formList = null
            , RSACryptoServiceProvider privateKey = null
            , string nonce = null
            , string timestamp = null
            , string version = "1.0")
        {
            Logger.LogEnter(LoggerBase.Args(realm, authPrefix, httpMethod, urlPath, appId, secret, formList == null ? null : formList.ToFormData(), privateKey, nonce, timestamp, version));

            Logger.LogDebug("URL:: {0}", urlPath);

            authPrefix = authPrefix.ToLower();

            // Generate the nonce value
            nonce = nonce ?? ApiAuthorization.NewNonce().ToString();
            timestamp = timestamp ?? ApiAuthorization.NewTimestamp().ToString();

            SignatureMethod signatureMethod = SignatureMethod.HMACSHA256;
            if (secret == null)
            {
                signatureMethod = SignatureMethod.SHA256withRSA;
            }

            String baseString = BaseString(authPrefix, signatureMethod
                , appId, urlPath, httpMethod
                , formList, nonce, timestamp, version);

            String base64Token = "";
            if (secret != null)
            {
                base64Token = baseString.L1Signature(secret);
            }
            else
            {
                base64Token = baseString.L2Signature(privateKey);
            }

            var tokenList = new ApiList();

            tokenList.Add("realm", realm);
            tokenList.Add(authPrefix + "_app_id", appId);
            tokenList.Add(authPrefix + "_nonce", nonce);
            tokenList.Add(authPrefix + "_signature_method", signatureMethod.ToString());
            tokenList.Add(authPrefix + "_timestamp", timestamp);
			tokenList.Add(authPrefix + "_version", version);
			tokenList.Add(authPrefix + "_signature", base64Token);

            string authorizationToken = string.Format("{0} {1}", authPrefix.Substring(0, 1).ToUpperInvariant() + authPrefix.Substring(1), tokenList.ToString(", ", false, true));

            Logger.LogDebug("Token :: {0}", authorizationToken);

            Logger.LogExit(LoggerBase.Args(authorizationToken));
            return authorizationToken;
        }

        public static int HttpRequest(Uri url, string token = null, ApiList postData = null, HttpMethod httpMethod = HttpMethod.GET, bool ignoreServerCert = false)
        {
            Logger.LogEnter(LoggerBase.Args(url, token, postData == null ? "null" : postData.ToFormData(), httpMethod));

            int returnCode = 0;

            if (ignoreServerCert) InitiateSSLTrust();

            WebResponse response = null;
            StreamReader reader = null;
            Stream dataStream = null;

            try
            {
                // Create a request using a URL that can receive a post.   
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = httpMethod.ToString();

                if (!string.IsNullOrEmpty(token)) request.Headers.Add("Authorization", token);

                if (postData != null)
                {
                    // Set the Method property of the request to POST.  
                    request.Method = HttpMethod.POST.ToString();

                    // Create POST data and convert it to a byte array.
                    //string postData = "This is a test that posts this string to a Web server.";
                    byte[] byteArray = Encoding.UTF8.GetBytes(postData.ToFormData());

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
                Logger.LogDebug("Response Code:: {0} - {1}", ((HttpWebResponse)response).StatusDescription, (int)(((HttpWebResponse)response).StatusCode));

                returnCode = (int)(((HttpWebResponse)response).StatusCode);

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
					
                    returnCode = (int)(((HttpWebResponse)response).StatusCode);

                    Logger.LogWarning("Error Code: {0} - {1}", returnCode, (((HttpWebResponse)response).StatusDescription));

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
                if (response != null) response.Close();
                if (reader != null) reader.Close();
                if (dataStream != null) dataStream.Close();
            }

            Logger.LogExit(LoggerBase.Args(returnCode));
            return returnCode;
        }

        static bool iniSslTrust;

        public static void InitiateSSLTrust()
        {
            if (iniSslTrust) return;

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
                Console.WriteLine("{0}", ex);
            }
        }
    }
}
