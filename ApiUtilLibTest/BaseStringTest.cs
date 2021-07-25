using NUnit.Framework;
using System.Linq;
using ApiUtilLib;
using ApexUtilLibTest;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace ApiUtilLibTest
{
    [TestFixture]
    public class BaseStringTest
    {

        [Test]
        public void BaseString_Basic_Test()
        {
            var url = "https://test.example.com:443/api/v1/rest/level1/in-in/?ap=裕廊坊%20心邻坊";
            var expectedBaseString = "GET&https://test.example.com/api/v1/rest/level1/in-in/&ap=裕廊坊 心邻坊&auth_prefix_app_id=app-id-lpX54CVNltS0ye03v2mQc0b&auth_prefix_nonce=1355584618267440511&auth_prefix_signature_method=HMACSHA256&auth_prefix_timestamp=1502175057654&auth_prefix_version=1.0";

            var baseString = ApiAuthorization.BaseString(
                "auth_prefix",
                SignatureMethod.HMACSHA256,
                "app-id-lpX54CVNltS0ye03v2mQc0b",
                new System.Uri(url),
                null,
                HttpMethod.GET,
                "1355584618267440511",
                "1502175057654",
                "1.0"
            );

            Assert.AreEqual(expectedBaseString, baseString);
        }

        [Test]
        public void BaseString_BugTest()
        {

                var formData = new ApiUtilLib.FormData();

                formData.Add("Action", "SendMessage");
                formData.Add("MessageBody", "[{}]");

            var url = "https://test.example.com:443/api/v1/rest/level1/in-in/?ap=裕廊坊%20心邻坊";
            var expectedBaseString = "GET&https://test.example.com/api/v1/rest/level1/in-in/&Action=SendMessage&MessageBody=[{}]&ap=裕廊坊 心邻坊&auth_prefix_app_id=app-id-lpX54CVNltS0ye03v2mQc0b&auth_prefix_nonce=1355584618267440511&auth_prefix_signature_method=HMACSHA256&auth_prefix_timestamp=1502175057654&auth_prefix_version=1.0";

            var baseString = ApiAuthorization.BaseString(
                "auth_prefix",
                SignatureMethod.HMACSHA256,
                "app-id-lpX54CVNltS0ye03v2mQc0b",
                new System.Uri(url),
                formData,
                HttpMethod.GET,
                "1355584618267440511",
                "1502175057654",
                "1.0"
            );

            Assert.AreEqual(expectedBaseString, baseString);
        }

        [Test]
        public void BaseString_FormData_Test()
        {
            var url = "https://test.example.com:443/api/v1/rest/level1/in-in/?ap=裕廊坊%20心邻坊";
            var expectedBaseString = "POST&https://test.example.com/api/v1/rest/level1/in-in/&ap=裕廊坊 心邻坊&auth_prefix_app_id=app-id-lpX54CVNltS0ye03v2mQc0b&auth_prefix_nonce=6584351262900708156&auth_prefix_signature_method=HMACSHA256&auth_prefix_timestamp=1502184161702&auth_prefix_version=1.0&param1=data1";

            var formData = new FormData();
            formData.Add("param1", "data1");

            var baseString = ApiAuthorization.BaseString(
                "auth_prefix",
                SignatureMethod.HMACSHA256,
                "app-id-lpX54CVNltS0ye03v2mQc0b",
                new System.Uri(url),
                formData,
                HttpMethod.POST,
                "6584351262900708156",
                "1502184161702",
                "1.0"
            );

            Assert.AreEqual(expectedBaseString, baseString);
        }

        [Test]
        public void BaseString_Invalid_Url_01_Test()
        {
            var url = "ftp://test.example.com:443/api/v1/rest/level1/in-in/?ap=裕廊坊%20心邻坊";

            Assert.Throws<System.NotSupportedException>(() => ApiAuthorization.BaseString(
                "auth_prefix",
                SignatureMethod.HMACSHA256,
                "app-id-lpX54CVNltS0ye03v2mQc0b",
                new System.Uri(url),
                null,
                HttpMethod.POST,
                "6584351262900708156",
                "1502184161702",
                "1.0"
            ));
        }

        [Test]
        public void BaseString_Invalid_Url_02_Test()
        {
            var url = "://test.example.com:443/api/v1/rest/level1/in-in/?ap=裕廊坊%20心邻坊";

            Assert.Throws<System.UriFormatException>(() => ApiAuthorization.BaseString(
                "auth_prefix",
                SignatureMethod.HMACSHA256,
                "app-id-lpX54CVNltS0ye03v2mQc0b",
                new System.Uri(url),
                null,
                HttpMethod.POST,
                "6584351262900708156",
                "1502184161702",
                "1.0"
            ));
        }



    }
}
