using System;
using System.Security.Cryptography;
using ApiUtilLib;

namespace ApiUtilLib
{
    public class AuthParam
    {
        public Uri url;
        public HttpMethod httpMethod;

        public string appName;
        public string appSecret;
        public RSACryptoServiceProvider privateKey = null;

        public FormList formList;

        public string nonce;
        public string timestamp;

        internal SignatureMethod signatureMethod;
        internal string version;

        public AuthParam nextHop;
    }
}
