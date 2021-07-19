using NUnit.Framework;
using System;
using System.Security.Cryptography.X509Certificates;
using ApiUtilLib;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Reflection;

namespace ApiUtilLibTest
{
    [TestFixture]
    public class L2SignatureTest
    {
		// file name follow unix convention...
		static readonly string privateCertName = GetLocalPath("Certificates/ssc.alpha.example.com.p12");
		static readonly string publicCertName = GetLocalPath("Certificates/ssc.alpha.example.com.cer");

		const string baseString = "message";
		const string password = "passwordp12";

		static readonly RSACryptoServiceProvider privateKey = ApiAuthorization.GetPrivateKey(ApexUtilLib.PrivateKeyFileType.P12, privateCertName, password);

		static readonly RSACryptoServiceProvider publicKey = ApiAuthorization.PublicKeyFromCer(publicCertName);

		static string GetLocalPath(string relativeFileName)
		{
            var localPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), relativeFileName.Replace('/', Path.DirectorySeparatorChar));

			return localPath;
		}

        [Test]
        public void L2_BaseString_IsNullOrEmpty_Test()
        {
            const string testBaseString = null;

			Assert.Throws<System.ArgumentNullException>(() => testBaseString.L2Signature(privateKey));
            Assert.Throws<System.ArgumentNullException>(() => "".L2Signature(privateKey));

            Assert.Throws<System.ArgumentNullException>(() => ApiAuthorization.L2Signature(null, privateKey));
            Assert.Throws<System.ArgumentNullException>(() => ApiAuthorization.L2Signature("", privateKey));
        }

		[Test]
		public void L2_PrivateKey_IsNull_Test()
		{
			Assert.Throws<System.ArgumentNullException>(() => baseString.L2Signature(null));

            Assert.Throws<System.ArgumentNullException>(() => ApiAuthorization.L2Signature(baseString, null));
		}

        const string message = "Lorem ipsum dolor sit amet, vel nihil senserit ei. Ne quo erat feugait disputationi.";

        const string expectedSignature = "OsOqG/6hJfGmpCDkqBSZ4netNJDex1lzBYTzGjvjShSFEhJEzAD1zNHKg8Zf9Dve7o9lx3+Yrhrn68nMocgUSOvinhUNF3ttLWw36GzXG7BFJRSIbeUfY3C1vAhkjxmE8oiYoIWctT9qBOL/3GY5QD1H3DiWrb3OLUjy52dsAPmK2P5ofdo8Erd5/0mTxgX+OLMADLJUXq/Aajp1ZIF+djQipPHg0Ms1sNkSHCURxyCjRMKOHNe8DH15lKcApBBjd3XPlb+PGlFl/ffc5Q1ALnAOmsqN6hi8mW+R6Eb0QZsvoRMFSA7kQdWvkCrlWtP5ux+A2Ji/b48SWFSJurVz7yRBhJFDYlvTTCGcgLfwn3TJXa/YbCK05qy307i6X9jnfYaqSYhKC61ExTZYE2SyfagAcWVlSlq3bEovZXllKAwq8Yqyez2EqkOoSzJdj5gmJ1Pb4wN/ss7yYybRSvFShQunj/t6TiQDCJuhghXOfV5Scs/wqjDMWViqrA65YOQHROqAku81NiWFmciVHjk6bNAGsp7iE0p5XnA4z9B41ZVPsxsSXUg4tZvpUrZSpNzlGFBi/uEa1UYcrUd8APzBCvUa75RhZsfxRsCOkpyOEmqoFzg4ngCfegJzBpU5La9e0SOlRvW29p9CK7fS/FZC5YJtP1kucaBN5pX/mxaYeUQ=";

		[Test]
		public void L2_Verify_Signature_Test()
		{
            Assert.IsTrue(expectedSignature.VerifyL2Signature(publicKey, message));
		}

		[Test]
		public static void L2_Basic_Test()
		{
			var dataList = new List<string[]>();

			dataList.Add(new string[]
			{
                message,
				expectedSignature,
                "Basic Test"
			});

			// Chinese Traditional
			dataList.Add(new string[]
			{
				"道続万汁国圭絶題手事足物目族月会済。",
				"BcgiwVRV5NPf2D15NMA7PjfheHY+jYeODlODuaAahd5dU/fuGanMcFpFuKJtxuCQLOE3veZMCC7V+hb/LEaBfkvXw+7gl8WtLu+T927Xs+3517AZm9vZ3nU34FIMAQpTJ8QbciFcd5FAybDiMuCfzvVE59yTSL/JmzSH4188/K6Z1uZ29VizrC2BwtVA/SHaWN1SMUGX6u0tQN5nE4dGZ9lRKm1Jd2rsUNDmqsmUZDJTbgoZbTJjNQklRv48GunXYBt/cfi9T5bryIVilqUphTIe6GrjLXZ1NVVCcMCJaCzAesX2dWUwLCEULcM4Vqw+7SWN20k4zcori5+QkwNH/eyViHwKiYY+neIusUU4HcafIXNHlYQjj1OVEXqPn2P7TzH9y+7TXheNrQ03P6NnRBjEW/bAgoCplbhYWnlNtu+BBNLn9+6rN/ePJz265Wetb16ZjG+ZwbV72PUkGxeFoT7cGBNvcC5zK4bFZV4AOr7TqE9Nt/xm9Xi7/gM0oU7zgYm+32LJaAxG2vax9EFdi3yBKrGRBYLaMH/6KEreZV+iZgLsqK/7tWEQom843iTmeRaxA4/Xeg3MLPyyxrWtQBqu2O/lv6pEf+scnc2Mg6gyc5uRm0luxJUBkqI6i/BAHGZRN1cDkMhWywAcWs3yxxV6qptFYxl6ubLCbCXtiw0=",
                "UTF8 (Chinese Traditional) Test"
			});

			// Japanese
			dataList.Add(new string[]
			{
				"員ちぞど移点お告周ひょ球独狙チウソノ法保断フヒシハ東5広みぶめい質創ごぴ採8踊表述因仁らトつ。",
				"RtNtUoRXhNFrFPMy5aJjPTB8yI9AyvLqIKmgjmarxZhB/aOLXSJtHHJMgufOLDsUzEyDenlPuRp4ju2Dp870P19H/IxLktTqkU3DZU35tqk21TWNQDmdl/P9YjY3BNJqU4YBV3A83KRDRhJh235Hjy20dbJqZAe/oL/8GboRd0W941Oj2VfC53SmVAYWQV1aJb4qV3cvoQG2OtcBMNA+ayG+0oTB9AtGZ3CqCUPqbfbb36oc81jYQj0nElHRew7QdclfpAUQaDgCF6svduji2rdXrU+fRYaiRPtm4F1zv9JVuIjKOZRqVQeQ3Nb/X8zUMEBNeWToQPmzoHz6hAEfzYUif2IJ1KqYooV29AwOvwu1itAeUwLtqlHK3QGJYaJVrw05EyAg1IsicAQ+szP+6t6Er3GjhRSXwIcpKdxLUHVtwFoK7E1L4FqxCW+Pokm97h0/rqWREt7DJvoIofQ8rtfEfao5CTaJOQyMRUx+Ds1Kytzpzd1T7aWFvdzFxo9YLfsZ/DzIy2F7iMi9c1b8WYfStlBvfUeEEeByZj+7FrvLMo9Ys5K/UweBfTcBHdPfCmW5RTJhmfK0p+EVsntLqkCbWMoQ6JdNZoASSB7E+NPGJuk3kuVo4sPnPy9vQlHsYJWktXjwTmBp4EZzfcia6U5TSWG0Wdn4ohCYQU2Y/sg=",
                "UTF8 (Japanese) Test"
			});

			// Korean
			dataList.Add(new string[]
			{
				"대통령은 즉시 이를 공포하여야 한다, 그 자율적 활동과 발전을 보장한다.",
				"GW0UWsS/bdP22Zd8D+WCZtz4LhyHF/8QemS7xTDPzhSlN+yjPtu7O0f/GGl3s+U1Cm3gUjMIRKbSKyi441Z57MD/9Ju8swtAJkHh9K/LPf/fFfm3UMN0EU7jeoEUkFG3AM8rR24ih16HFpK8RcDHDRL5+tAoU6au/JRLAnuRnhcOjunSC91OhTZJqSGYukoarLYVFxnLFyZPviZPe+aaFW4ZUrD+Kc6K2C/htHS1S/7NJedDsD8If31+dh/wdkIbvhQRDgWBJlSAoqOqmeFSRIIXW/VeufOjXZ9fxa/pmsBDN5BB5Fb3MguxebD61c0MN4F+gnRQ/5arKQL5oIn/QAGan6Ll7s7nUGpa88sdVKRqw/TVcqmYeIFgWBUhnk2p54tvWbCXski63z4QRC+4TZ/ITPgn1sDqsD5Qf9/Ly1RPpJPODNgIYb5i6vh94gchqrF1g3EphbJ3riWCqREoBuCD+yqS2DSE7QWg1gjaHtT8kzcxkt3KpJoLPlZPKt92y03/av8a0AXpc2H7pw2mJ4i13xDsiRKavE4R7pwrfUJxSxYD2jBPZgNTo3XxaboHZgFbvyyw3xHreSo9CmM0mL94qha4jv2TqGuURooiBfizxzuHeMub1t8VIAXOiTk/iQtBPvGLtsQzFW3TeAeZtiYSGBeKOmb6O1vtetBurQk=",
                "UTF8 (Korean) Test"
			});

			// Greek
			dataList.Add(new string[]
			{
				"Λορεμ ιπσθμ δολορ σιτ αμετ, τατιον ινιμιcθσ τε ηασ, ιν εαμ μοδο ποσσιμ ινvιδθντ.",
				"G6FezmgEqrnZNxqWfIE8Rcb49L3WQRcAQxQ0xX2sibejHHiOXPXU811OIsL7hsYmyLSSoY3IXTtu271MwfR1TTiODBnIqpgZ0jwmyKK7YoHUDqRgKmVscBnwotw2ntDn1eA2BAU2yKi+UOeUbDcY8dCK/qxdoKdvQg99zjmm1P4EG0dFlmh07oa2ByH4pgioaxI0sKQdDL14qbjrKOiFtfgdv5NEd1Q3kP240p9vLOoScPsRvRZlpWGPCUa0R9wQMtXZAKB3TVs+p8hu5ZHmG9JP2Jo5FRt8EkCG6V3Fg8qlbDO5m9B49atynVBsNSQkYKpCylokJI/mcESNciliQmOwkLmqh6YeELX82PSvnErIPRSAzrqkKYed/HI5gL2Z8pCOwohSfuMeoOrba3JeD98kMQHGwhw+pxSP6lnTCxLwLREhqgSrcXfymhc2TCbA/w/1gT3MjTIDjIF1HgtT2bPpjco62iuKPyrjejb4ARGcty5mlUjbPNUCD/DB4qgghnhbtvWJFJxF7Egs/BeDk5swyyvFBrlXPd/yhCpMJRAOZ0bK3Adj1ij0tVH/kHtDzRYZnF0ZQXZBlHyP2DMvlnJQbIDrTBuojRYFb8W7CPWc/P4RQIGwRv6ZvT+LLl+uuNpvNoVFc/EB0gKII819nINmCjcmuYhsboBLkJ9XHyE=",
                "UTF8 (Greek) Test"
			});

			dataList.Add(new string[]
			{
				"GET&https://test.example.com/api/v1/rest/level2/in-in/&auth_prefix_app_id=app-id-lpX54CVNltS0ye03v2mQc0b&auth_prefix_nonce=7798278298637796436&auth_prefix_signature_method=SHA256withRSA&auth_prefix_timestamp=1502163142423&auth_prefix_version=1.0",
				"hmAlq47s1yfjVjmgdMFK4OXk1Gm380Gy+cfhWrhuD2Dux523D8ywA4dn1UVsxukGYT6HnXm83tHwFiD0WWyL1nyHfViD7HDNo95pZrHFOEgrkmXzUUJ5g8aVO1EPxbMuJSlRECeLjbHSpkfyMljSVX/8KH9wt4S50Zs36f9kMcMpYZvK8Z1C7SUleoM+yIvCJPGj5mRIbZyieFXGJ0Zt3aJBsWwZeV4BcrSL/T1GQQLigOY5M7lmkmDmc/Wz7Ky82rTImbJB8HfclNkD0l4wy49vSLXh9IbIvUpErbzipjG1MmA4t1cPMA3PoyAXWAcrHHSSBBwcpD6yOr5HOXyYAKDHz5IPZLRtAOft4BPwKygzOhDR881rfFbOT8IOOSI20uXtlTa6KTIUq9Opvc0g2gnUgai0qfm6uCOi8vmEBKj5z5mOrG+39FhXbB5FEpv6ZuFPP3ATtkIg9nSWStifbkq4fJCTSAsXRF4ragNWQRgc46B4zUwWpnRZ7s83oi808A2vQXLFExPTjW4DBUTtBt+O9Z1Gp7Z5yRHekut4vLaNqrIvRcLqmn92G9QqfkFJmQGrOH7SpAtgDhGHzaNc5C+OTmCkxhFKtoblO4ItwIh5X4i3gePDcHrBDEDTwjHPjXEFbDOL0JKrA6qlgA8evXVZXLugHAs6bO5zJhacXTU=",
                "L2 BaseString Happy Path Test"
			});

			dataList.Add(new string[]
			{
				"GET&https://test.example.com/api/v1/rest/level2/in-in/&ap=裕廊坊 心邻坊&auth_prefix_app_id=app-id-lpX54CVNltS0ye03v2mQc0b&auth_prefix_nonce=7231415196459608363&auth_prefix_signature_method=SHA256withRSA&auth_prefix_timestamp=1502164219425&auth_prefix_version=1.0&oq=c# nunit mac&q=c# nunit mac",
				"q7gaCiA1/oTYtCwrj3ZMumSwuIsF++NnjDzoexpU1OunABGrVxmQrJIcDXnVkKA1gaDZf0JXJMCfyfu9r/F2S8raQUyyxt0edjEjzCPFnF+1N6Hhj5KYMxT2lggv6mSblqi0tWUXD/tSgw0ssRfNM0+v0xo4G2kzbOlZe/JzIGUX/FtkanW5xJGS0wrZIfoAO3p1OVwghN6FZkKZ3o9gH3VGZItJKEa6agPqlaH2XVbF3636nKlGRKbcc+fNDd2/EFqqxDxS3gx21a1cRJT6SfOZ7lqbnAkxvC9QW/mw42hObUx/RjyGI1QqMdJ6aRkLHQkHZ9LT41kJKg+AKsLlo/ogTdwxyekok9jt2UgVm7EFA9lkAB2T8T8q1eDalpby6BHCSBiQn9j7FhcVw6EFgWPLzlXCZl/sDzbrqX90AR38VAntTS+2f+Uc5vKxrJ41m2zD2HIYLw0oiXEjODPfvRPtCt478CCz1Y6xnMgD4hJwhTN8pyuMvdoxwWKWcCsdZhXPZrzNm/GZljT3jdRekUgAt3fBOwbji+OVt7TUqUKpYMadExrhiTRKV+dh8EUtdnJoM056YC5Ta2w/Iuxai6dT1QRW7bZOrUfOljD3Miq6Xyx3nnUW2sVQbeJkxFBJVbFAsxtpMRICcTg63VRrcGmhqxTX0QyqsxFqSrbj+eQ=",
                "L2 BaseString with UTF8 Parameters Test"
			});

			// excute test
			foreach (var item in dataList)
			{
				L2Test(item[0], privateKey, item[1], item[2]);
			}
		}

		public static void L2Test(string baseString, RSACryptoServiceProvider privateKey, string expectedSignature, string message)
		{
			var signature = baseString.L2Signature(privateKey);

            Assert.AreEqual(expectedSignature, signature, message);
		}
	}
}
