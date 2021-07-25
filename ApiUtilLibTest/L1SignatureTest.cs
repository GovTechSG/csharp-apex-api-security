using System;
using System.Collections.Generic;
using ApiUtilLib;
using NUnit.Framework;

namespace ApiUtilLibTest
{
	[TestFixture]
	public class L1SignatureTest
    {
        const string baseString = "message";
		
        const string secret = "secret";
        const string secret2 = "5aes9wG4mQgWJBfKMuYLtrEtNslm1enWG2XpGaMk";

		const string expectedResult = "i19IcCmVwVmMVz2x4hhmqbgl1KeU0WnXBgoDYFeWNgs=";

		[Test]
		public void L1_BaseString_IsNullOrEmpty_Test()
		{
			string testBaseString = null;

			Assert.Throws<System.ArgumentNullException>(() => testBaseString.L1Signature(secret));
			Assert.Throws<System.ArgumentNullException>(() => "".L1Signature(secret));

			Assert.Throws<System.ArgumentNullException>(() => ApiAuthorization.L1Signature(null, secret));
			Assert.Throws<System.ArgumentNullException>(() => ApiAuthorization.L1Signature("", secret));
		}

		[Test]
		public void L1_Secret_IsNullOrEmpty_Test()
		{
			Assert.Throws<System.ArgumentNullException>(() => baseString.L1Signature(null));
			Assert.Throws<System.ArgumentNullException>(() => baseString.L1Signature(""));

			Assert.Throws<System.ArgumentNullException>(() => ApiAuthorization.L1Signature(baseString, null));
			Assert.Throws<System.ArgumentNullException>(() => ApiAuthorization.L1Signature(baseString, ""));
		}

		[Test]
        public void L1_Verify_Signature_Test()
        {
            Assert.IsTrue(expectedResult.VerifyL1Signature(secret, baseString));

            Assert.IsTrue(ApiAuthorization.VerifyL1Signature(expectedResult, secret, baseString));
		}
		
        [Test]
		public void L1_Verify_Signature_With_Wrong_Secret_Test()
		{
            Assert.IsFalse(expectedResult.VerifyL1Signature(secret + 'x', baseString));

			Assert.IsFalse(ApiAuthorization.VerifyL1Signature(expectedResult, secret + 'x', baseString));
		}

		[Test]
		public void L1_Verify_Signature_With_Wrong_BaseString_Test()
		{
			Assert.IsFalse(expectedResult.VerifyL1Signature(secret, baseString + 'x'));

			Assert.IsFalse(ApiAuthorization.VerifyL1Signature(expectedResult, secret, baseString + 'x'));
		}
		
        [Test]
		public void L1_BaseString_Encoding_Test()
		{
			var dataList = new List<string[]>();

			dataList.Add(new string[]
			{
				"Lorem ipsum dolor sit amet, vel nihil senserit ei. Ne quo erat feugait disputationi.",
				secret,
				"cL3lY5/rhmkxMw/dCHCa4b9Lpp/soPPACnIxtQwRQI8=",
				"Basic Test"
			});

			// Chinese Traditional
			dataList.Add(new string[]
			{
				"道続万汁国圭絶題手事足物目族月会済。",
				secret,
    			"wOHv68zuoiIjfJHW0hZcOk4lORyiAL/IGK8WSkBUnuk=",
				"UTF8 (Chinese Traditional) Test"
			});

			// Japanese
			dataList.Add(new string[]
			{
				"員ちぞど移点お告周ひょ球独狙チウソノ法保断フヒシハ東5広みぶめい質創ごぴ採8踊表述因仁らトつ。",
				secret,
                "L0ft4O8R2hxpupJVkLbgQpW0+HRw3KDgNUNf9DAEY7Y=",
				"UTF8 (Japanese) Test"
			});

			// Korean
			dataList.Add(new string[]
			{
				"대통령은 즉시 이를 공포하여야 한다, 그 자율적 활동과 발전을 보장한다.",
				secret,
    			"a6qt0t/nQ3GQFAEVTH+LMvEi0D41ZaKqC7LWJcVmHlE=",
				"UTF8 (Korean) Test"
			});

			// Greek
			dataList.Add(new string[]
			{
				"Λορεμ ιπσθμ δολορ σιτ αμετ, τατιον ινιμιcθσ τε ηασ, ιν εαμ μοδο ποσσιμ ινvιδθντ.",
				secret,
    			"WUGjbeO8Jy8Rvs5tD2biLHPR0+qtAmXeZKqX6acYL/4=",
				"UTF8 (Greek) Test"
			});

			dataList.Add(new string[]
			{
				"GET&https://test.example.com/api/v1/rest/level1/in-in/&auth_prefix_app_id=app-id-lpX54CVNltS0ye03v2mQc0b&auth_prefix_nonce=-4985265956715077053&auth_prefix_signature_method=HMACSHA256&auth_prefix_timestamp=1502159855341&auth_prefix_version=1.0&param1=def+123",
				secret2,
				"8NxfLG0pFWEq1gZEttCW4lgrb92MFaqQpeUPRDx4CAE=",
				"L1 BaseString Happy Path Test"
			});

			dataList.Add(new string[]
			{
				"GET&https://test.example.com/api/v1/rest/level1/in-in/&ap=裕廊坊 心邻坊&auth_prefix_app_id=app-id-lpX54CVNltS0ye03v2mQc0b&auth_prefix_nonce=2851111144329605674&auth_prefix_signature_method=HMACSHA256&auth_prefix_timestamp=1502163903712&auth_prefix_version=1.0",
				secret2,
				"0fE74Vf/Q7nktxeezzrYcvOeq36Pd4CJ7Ez9I00cdJk=",
				"L1 BaseString with UTF8 Parameters Test"
			});

			// excute test
			foreach (var item in dataList)
			{
				L1Test(item[0], item[1], item[2], item[3]);
			}
		}

		void L1Test(string testBaseString, string testSecret, string expectedSignature, string messageTag)
		{
			var signature = testBaseString.L1Signature(testSecret);

            Assert.AreEqual(expectedSignature, signature, messageTag);
		}
	}
}
