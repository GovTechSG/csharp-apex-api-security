using NUnit.Framework;
using System;
using ApiUtilLib;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace ApiUtilLibTest
{
    [TestFixture]
    public class AuthorizationToken
    {

        static string GetLocalPath(string relativeFileName)
        {
            var localPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), relativeFileName.Replace('/', Path.DirectorySeparatorChar));

            return localPath;
        }

        // file name follow unix convention...
        static readonly string privateCertNameP12 = GetLocalPath("Certificates/ssc.alpha.example.com.p12");

        const string passphrase = "passwordp12";

        static RSACryptoServiceProvider privateKey = ApiAuthorization.GetPrivateKey(ApiUtilLib.PrivateKeyFileType.P12_OR_PFX, privateCertNameP12, passphrase);

        const string realm = "http://example.api.test/token";
        const string authPrefixL1 = "api_prefix_l1";
        const string authPrefixL2 = "api_prefix_l2";

        const HttpMethod httpMethod = HttpMethod.GET;
        private Uri url = new Uri("https://test.example.com:443/api/v1/rest/level1/in-in/?ap=裕廊坊%20心邻坊");

        const string appId = "app-id-lpX54CVNltS0ye03v2mQc0b";
        const string secret = "5aes9wG4mQgWJBfKMuYLtrEtNslm1enWG2XpGaMk";

        const string nonce = "-5816789581922453013";
        const string timestamp = "1502199514462";

        [Test]
        public void Test_L1_Basic_Test()
        {
            var expectedTokenL1 = "Apex_l1_eg realm=\"https://test.example.com\", apex_l1_eg_app_id=\"app-id-lpX54CVNltS0ye03v2mQc0b\", apex_l1_eg_nonce=\"-5816789581922453013\", apex_l1_eg_signature_method=\"HMACSHA256\", apex_l1_eg_timestamp=\"1502199514462\", apex_l1_eg_version=\"1.0\", apex_l1_eg_signature=\"YA+Ygjc1S/b2xgm8IaCFsEPcO4sEUH1AsUmm1TUHjmk=\"";

            var authParam = new AuthParam
            {
                url = url,
                httpMethod = httpMethod,
                appName = appId,
                appSecret = secret,
                timestamp = timestamp,
                nonce = nonce
            };
            var authorizationToken = ApiAuthorization.TokenV2(authParam).Token;

            Assert.AreEqual(expectedTokenL1, authorizationToken);
        }

        [Test]
        public void Test_L2_Basic_Test()
        {
            var expectedTokenL2 = "Apex_l2_eg realm=\"https://test.example.com\", apex_l2_eg_app_id=\"app-id-lpX54CVNltS0ye03v2mQc0b\", apex_l2_eg_nonce=\"-5816789581922453013\", apex_l2_eg_signature_method=\"SHA256withRSA\", apex_l2_eg_timestamp=\"1502199514462\", apex_l2_eg_version=\"1.0\", apex_l2_eg_signature=\"PkKDLI40VMiMUL3GVPLHo4upPGlUJ9BNno/GLhjiN5RpL/NUIM5v6Q10XrT4KAfEymtvjFKzt3R20uhOPEnHVJ7XsZLDZ81RRfYOISsLSzNv0HUX7zm0Y0sow6WQjk7gHPTJAWd/T6Mdv28UnZaM2Vf7tmyFO93tTCT1+ulz0a4akMdcqHORAQPOH84v217f0r6H6UtDHbw+Kn3TNVQheHxF2qcZ1d9U/+WYnvN5N+vgsM3pdEJylF+58QsVZRdZJpway/n+2QzzsEpe1Gpce646Lpvnifz2iFKbHDa6pe7cKiZ2un8dXzxXkPkqwbqu3D2YyPMgZwV5vXgEsXxgT4bqGgzSvG56Ml4/B+c5JW6p6xiZL2MpM0s1Tm2ayFfbqZ3DivB9X1nUQT3HJ0o9teyq86p8QTCDvGIMVZZPLV7ww+e4ZUnpd+RJqbDb+y4S53fs4e32+ceuwoyqofbehXKlNFeKL1sqgRnLEnsjHIMbfjdUCtHiJLNzvI0i4C53KNuGC9iFOu3u+2VVYWYcDBOFJ1lixqso0dLoJm/Uz+6uwqn4FlI4QB0LqFBOnFLvF0HnbP3s4jjSE2F7VYRrDZ9uZWvvbfCgqnFWoXqzEjbEB4DHZ9DkATc+PjFt7ZsJJ/xNEjzL7kFNzom5+UHSRSLp/e5n1kj6KOX0xVkQPkQ=\"";

            var authParam = new AuthParam
            {
                url = url,
                httpMethod = httpMethod,
                appName = appId,
                privateKey = privateKey,
                timestamp = timestamp,
                nonce = nonce
            };
            var authorizationToken = ApiAuthorization.TokenV2(authParam).Token;

            Assert.AreEqual(expectedTokenL2, authorizationToken);
        }

        [Test]
        public void Test_L2_Wrong_Password_Test()
        {

            Assert.Throws<System.Security.Cryptography.CryptographicException>(() =>
            {
                var myPrivateKey = ApiAuthorization.GetPrivateKey(ApiUtilLib.PrivateKeyFileType.P12_OR_PFX, privateCertNameP12, passphrase + "x");

                var authParam = new AuthParam
                {
                    url = url,
                    httpMethod = httpMethod,
                    appName = appId,
                    privateKey = myPrivateKey
                };
                ApiAuthorization.TokenV2(authParam);
            });
        }

        [Test]
        public void Test_L2_Not_Supported_Cert_Test()
        {
            var fileName = GetLocalPath("Certificates/ssc.alpha.example.com.pem");

            Assert.Throws<System.ArgumentNullException>(() =>
            {
                var myPrivateKey = ApiAuthorization.GetPrivateKey(ApiUtilLib.PrivateKeyFileType.P12_OR_PFX, fileName, passphrase);

                var authParam = new AuthParam
                {
                    url = url,
                    httpMethod = httpMethod,
                    appName = appId,
                    privateKey = myPrivateKey
                };
                ApiAuthorization.TokenV2(authParam);
            });
        }

        [Test]
        public void Test_L2_Invalid_FileName_Test()
        {
            var fileName = "Xssc.alpha.example.com.p12";

            Assert.Throws<System.IO.FileNotFoundException>(() =>
            {
                var myPrivateKey = ApiAuthorization.GetPrivateKey(ApiUtilLib.PrivateKeyFileType.P12_OR_PFX, fileName, passphrase);

                var authParam = new AuthParam
                {
                    url = url,
                    httpMethod = httpMethod,
                    appName = appId,
                    privateKey = myPrivateKey
                };
                ApiAuthorization.TokenV2(authParam);
            });
        }


    }
}
