//using System;
using System.Collections.Generic;
namespace ApexUtilLibTest
{
    public class TestParam
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public APIParam ApiParam { get; set; }

        public string PublicCertFileName { get; set; }

        public string Passphrase { get; set; }

        public string[] SkipTest { get; set; }

        public string Message { get; set; }

        public string Debug { get; set; }

        public string TestTag { get; set; }

        public bool ErrorTest { get; set; }

        public Dictionary<string, string> ExpectedResult { get; set; }

        public override string ToString()
        {
            return $"{Id} - {Description}";
        }
    }

    public class APIParam
    {
        public string Realm{ get; set; }

        public string AppID { get; set; }

        public string AuthPrefix { get; set; }

        public string Secret { get; set; }

        public string InvokeURL { get; set; }

        public string SignatureURL { get; set; }

        public string HttpMethod { get; set; }

        public string Signature { get; set; }

        public string PrivateCertFileName { get; set; }

        public string PrivateCertFileNameP12 { get; set; }

        public string Passphrase { get; set; }

        public string SignatureMethod { get; set; }

        public string Nonce { get; set; }

        public string Timestamp { get; set; }

        public string Version { get; set; }

        public Dictionary<object,object> QueryString { get; set; }

        public Dictionary<object, object> FormData { get; set; }

    }   

    //public class ExpectedResult
    //{
    //    public string golang { get; set; }
    //    public string nodejs { get; set; }
    //}
}
