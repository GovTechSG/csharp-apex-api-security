using System;
using System.Security.Cryptography;
//using ApiUtilLib;

namespace ApiUtilLib
{
    public class AuthParam
    {
        public Uri url;
        public HttpMethod httpMethod;

        public string appName;
        public string appSecret;
        public RSACryptoServiceProvider privateKey = null;

        public FormData formData;

        public string nonce;
        public string timestamp;

        internal SignatureMethod signatureMethod;
        public string version = "1.0";

        public AuthParam nextHop;
    }
}
