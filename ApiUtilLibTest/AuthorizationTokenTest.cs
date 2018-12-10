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

		static RSACryptoServiceProvider privateKey = ApiAuthorization.PrivateKeyFromP12(privateCertNameP12, passphrase);

		const string realm = "http://example.api.test/token";
		const string authPrefixL1 = "api_prefix_l1";
		const string authPrefixL2 = "api_prefix_l2";

        const HttpMethod httpMethod = HttpMethod.GET;
        private Uri url = new Uri("https://test.example.com:443/api/v1/rest/level1/in-in/?ap=裕廊坊%20心邻坊");
		
        const string appId = "app-id-lpX54CVNltS0ye03v2mQc0b";
		const string secret = "5aes9wG4mQgWJBfKMuYLtrEtNslm1enWG2XpGaMk";

        const string nonce = "-5816789581922453013";
		const string timestamp = "1502199514462";

        //[Test]
		public void Test_L1_Basic_Test()
		{
			var expectedTokenL1 = "Api_prefix_l1 realm=\"http://example.api.test/token\",api_prefix_l1_timestamp=\"1502199514462\",api_prefix_l1_nonce=\"-5816789581922453013\",api_prefix_l1_app_id=\"app-id-lpX54CVNltS0ye03v2mQc0b\",api_prefix_l1_signature_method=\"HMACSHA256\",api_prefix_l1_version=\"1.0\",api_prefix_l1_signature=\"loz2Hp2wqiK8RxWjkI6Y6Y4OzmOS/QVPevT8Z43TRM4=\"";

			var authorizationToken = ApiAuthorization.Token(
                realm
				, authPrefixL1
				, httpMethod
				, url
				, appId
				, secret
				, timestamp: timestamp
				, nonce: nonce
			);

			Assert.AreEqual(expectedTokenL1, authorizationToken);
		}

		//[Test]
		public void Test_L2_Basic_Test()
		{
			var expectedTokenL2 = "Api_prefix_l2 realm=\"http://example.api.test/token\",api_prefix_l2_timestamp=\"1502199514462\",api_prefix_l2_nonce=\"-5816789581922453013\",api_prefix_l2_app_id=\"app-id-lpX54CVNltS0ye03v2mQc0b\",api_prefix_l2_signature_method=\"SHA256withRSA\",api_prefix_l2_version=\"1.0\",api_prefix_l2_signature=\"EZuFn/n3dxJ4OA9nkdM3yvw76azvyx/HKptQoWzTNWHxMB/2FyurbbpsSb16yNU4bOzRgHlFTZZzbJeZd211M7tLfRC/YQ1Mc2aIxufG7c7H3/3IZ0WdfHIJlF+XwHOR4U5sjRhbCBwSOZzHp6V2a/nmm+CYTjW2LBHxG7aB1wNI6V1PGDp+ePVr8uoyd4MD9nJj5IqLlljtpWCBUJsa7ZZdXgwbStxAdVA3j2lk3FAH9BzaKTQV0msB50Ou/itAw95pqH4RGrWjcuUETUN82JG154SrT/+hqXlmgsgl+6vui7kyCIGnQjhH+3ZSIp/91nJKW8/1hDcNKWQzuoIS9G23rJzPIuStc1f8y/YvXjUSxNTItb4DcSGwqOs1W8+ejLofW/HDBENhhL66ZZaO0EbJmMWJDp+r7w+RtrlRa2QLsuocuAYAsc8FbhW8SBowIHt/BpuIE21SCfXhbbqYmi0WY+YjJxJ79bNsf7OzH57wQln2Ri6jUtRsCez3rP+714aSAJMLKzJPrsUsiefQDuDjl+g7Fs+Ge5eCv3EOu36qmBEAwvS8oNU8eKa0ZnuXTZrvVEyAAgqQXjv7V4tklKImHMhBv3CqWHGtmxCIqFJuJ71ss81kOJ9pc1otyMzKvSZtVyxaOFgE1hTPfsA6Y5pQayhVikeCMfX8u/uFSmM=\"";

			var authorizationToken = ApiAuthorization.Token(
				realm
				, authPrefixL2
				, httpMethod
				, url
				, appId
                , privateKey: privateKey
				, timestamp: timestamp
				, nonce: nonce
			);

            Assert.AreEqual(expectedTokenL2, authorizationToken);
		}

        [Test]
        public void Test_L2_Wrong_Password_Test()
        {

            Assert.Throws<System.Security.Cryptography.CryptographicException>(() =>
            {
                var myPrivateKey = ApiAuthorization.PrivateKeyFromP12(privateCertNameP12, passphrase + "x");

                ApiAuthorization.Token(
                    realm
                    , authPrefixL2
                    , httpMethod
                    , url
                    , appId
                    , privateKey: myPrivateKey
                );
            });
		}

        [Test]
        public void Test_L2_Not_Supported_Cert_Test()
		{
            var fileName = GetLocalPath("Certificates/ssc.alpha.example.com.pem");

            Assert.Throws<System.ArgumentNullException>(() =>
            {
                var myPrivateKey = ApiAuthorization.PrivateKeyFromP12(fileName, passphrase);

				ApiAuthorization.Token(
                realm
                , authPrefixL2
                , httpMethod
                , url
                , appId
                , privateKey: myPrivateKey
                );
            });
		}

		[Test]
		public void Test_L2_Invalid_FileName_Test()
		{
			var fileName = "Xssc.alpha.example.com.p12";

            Assert.Throws<System.IO.FileNotFoundException>(() =>
            {
				var myPrivateKey = ApiAuthorization.PrivateKeyFromP12(fileName, passphrase);

				ApiAuthorization.Token(
                realm
                , authPrefixL2
                , httpMethod
                , url
                , appId
                , privateKey: myPrivateKey
                );
            });
		}


	}
}
