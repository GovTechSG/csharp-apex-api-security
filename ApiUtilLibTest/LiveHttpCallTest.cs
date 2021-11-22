using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiUtilLib;
using System.Configuration;
namespace ApiUtilLibTest
{
    [TestFixture]
    public class LiveHttpCallTest
    {
        
        //[Test]
        public static void Http_Call_Test0()
        {
            // application realm
            string realm = "http://example.api.test/token";

            // authorization prefix
            string authPrefix = "Apex_l1_eg";//APEX:must be Apex_l1_eg instead of auth_l1

			// app id and app secret assign to the application
			string appId = "pa-2CPVtaU4DyN9tLZTwJtiY2Cs";
            string appSecret = "074d6b49ab1837175544f2e0850e9de3360fa1c7";
            var formData = new ApiUtilLib.ApiList();

            formData.Add("userid", "C9AB42E9-E8CD-E811-810E-00155D464800");
            formData.Add("key", "W2w6+z5dNeyXzPr08YxUK6syFOk3M62+LLk/S04yZSsEyCjWJ2Fq5iNlrbDo6PNwOV5t6Rblg4FcnxBSBrBNCtYlSVlBJVKO5fY68xXAjUo=");
            // api signing gateway name and path
            string gatewayName = "https://pa.e.api.gov.sg"; //APEX:must be https: xxx
            string apiPath = "api14021live/v1/token"; //APEX:missing main context path: api14021live
			string baseUrl = string.Format("{0}/{1}", gatewayName, apiPath);
            Console.WriteLine("\n>>>baseUrl :: '{0}'<<<", baseUrl);
            Console.WriteLine("\n>>>appId :: '{0}'<<<", appId);
            Console.WriteLine("\n>>>appSecret :: '{0}'<<<", appSecret);
            // authorization header
            var authorizationHeader = ApiAuthorization.Token(realm, authPrefix, HttpMethod.POST, new Uri(baseUrl), appId, appSecret, formData);

            Console.WriteLine("\n>>>Authorization Header :: '{0}'<<<", authorizationHeader);

            // if the target gateway name is different from signing gateway name
            //string targetGatewayName = "pa.api.gov.sg";
            string targetBaseUrl = "https://pa.api.gov.sg/api14021live/v1/token";


            // this method only for verification only
            // expecting result to be 200
            var result = ApiAuthorization.HttpRequest(new Uri(targetBaseUrl), authorizationHeader, formData, HttpMethod.POST, ignoreServerCert: true);
            Console.WriteLine(result);
            Console.ReadLine();

            Assert.True(true);
        }

        [Test]
        public static void Http_Call_Test1()
        {
            // application realm
            string realm = "http://example.api.test/token";

            // authorization prefix
            string authPrefix = "Apex_l1_eg";//APEX:must be Apex_l1_eg instead of auth_l1

            // app id and app secret assign to the application
            string appId = "mcf-wcc-daxtra-host-xtr-app-id";
            string appSecret = "23e7e70b265453a95103c756bdb2e9568519eb46";
            var formData = new ApiUtilLib.ApiList();

            formData.Add("token", "actonomy111\n");
            formData.Add("challenge", "e82160de-852cf63f-60231922");
            formData.Add("resource", "10.1.26.72");
            // api signing gateway name and path
            string gatewayName = "https://mcf.e.api.gov.sg"; //APEX:must be https: xxx
            string apiPath = "mcf-wcc-daxtra-host-xtr/daxtralicence/cvx"; //APEX:missing main context path: api14021live
            string baseUrl = string.Format("{0}/{1}", gatewayName, apiPath);
            Console.WriteLine("\n>>>baseUrl :: '{0}'<<<", baseUrl);
            Console.WriteLine("\n>>>appId :: '{0}'<<<", appId);
            Console.WriteLine("\n>>>appSecret :: '{0}'<<<", appSecret);
            // authorization header
            var authorizationHeader = ApiAuthorization.Token(realm, authPrefix, HttpMethod.POST, new Uri(baseUrl), appId, appSecret, formData);

            Console.WriteLine("\n>>>Authorization Header :: '{0}'<<<", authorizationHeader);

            // if the target gateway name is different from signing gateway name
            string targetBaseUrl = "https://mcf.api.gov.sg/mcf-wcc-daxtra-host-xtr/daxtralicence/cvx";


            // this method only for verification only
            // expecting result to be 200

            var result = ApiAuthorization.HttpRequest(new Uri(targetBaseUrl), authorizationHeader, formData, HttpMethod.POST, ignoreServerCert: true);
            Console.WriteLine(result);
            Console.ReadLine();

            Assert.True(true);
        }
    }
}
