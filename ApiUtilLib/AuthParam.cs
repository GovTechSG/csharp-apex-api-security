using System;
using System.Security.Cryptography;
using ApiUtilLib;

namespace ApexUtilLib
{
    public class AuthParam
    {
        public Uri url;
        public HttpMethod httpMethod;
        public string appName;
        public string appSecret;
        public RSACryptoServiceProvider privateKey = null;
        public FormList formList;
        public SignatureMethod signatureMethod;
        public string nonce;
        public string timestamp;
        public string version;

        public AuthParam nextHop;
    }
}
