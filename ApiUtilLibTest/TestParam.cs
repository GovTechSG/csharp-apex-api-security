using System;
using System.Collections.Generic;
namespace ApexUtilLibTest
{
    public class TestParam
    {
        public string id { get; set; }

        public string description { get; set; }

        public APIParam apiParam { get; set; }

        public string publicCertFileName { get; set; }

        public string passphrase { get; set; }

        public string[] skipTest { get; set; }

        public string message { get; set; }

        public string debug { get; set; }

        public string testTag { get; set; }

        public bool errorTest { get; set; }

        public Dictionary<string, string> expectedResult { get; set; }

        public override string ToString()
        {
            return $"{id} - {description}";
        }
    }

    public class APIParam
    {
        public string realm{ get; set; }

        public string appID { get; set; }

        public string authPrefix { get; set; }

        public string secret { get; set; }

        public string invokeURL { get; set; }

        public string signatureURL { get; set; }

        public string httpMethod { get; set; }

        public string signature { get; set; }

        public string privateCertFileName { get; set; }

        public string privateCertFileNameP12 { get; set; }

        public string passphrase { get; set; }

        public string signatureMethod { get; set; }

        public string nonce { get; set; }

        public string timestamp { get; set; }

        public string version { get; set; }

        public Dictionary<object,object> queryString { get; set; }

        public Dictionary<object, object> formData { get; set; }

    }   

    public class ExpectedResult
    {
        public string golang { get; set; }
        public string nodejs { get; set; }
    }


}
