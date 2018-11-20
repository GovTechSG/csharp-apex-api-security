# APEX API C# Security Utility 
[![Build Status](https://travis-ci.org/GovTechSG/csharp-apex-api-security.svg?branch=master)](https://travis-ci.org/GovTechSG/csharp-apex-api-security)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)
[![MIT License](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/GovTechSG/csharp-apex-api-security/blob/master/LICENSE)

A C# helper utility that construct and sign HTTP Authorization header scheme for API authentication and verification

## Table of Contents
- [Getting Started](#getting-started)
    * [Prerequisites](#prerequisites)
    * [APIList Interface](#using-the-apilist-class)
        + [Generate QueryString](#generate-querystring)
        + [Generate FormData](#generate-formdata)
    * [Constructing HMAC256 L1 Authorization Header](#how-to-generate-hmac256-l1-authorization-header)
    * [Constructing RSA256 L2 Authorization Header](#how-to-generate-rsa256-l2-authorization-header)
- [Release](#release)
- [Contributing](#contributing)
- [License](#license)
- [References](#references)

## Getting Started

### Prerequisites
+ .NET Framework 4.6.1
+ Visual Studio IDE 2015/2017 Community+
+ NUnit Framework 3.8+ (for Windows Platform)

Make sure that all unit test cases are passed before using the library.

#### Installing NUnit **(Important : Windows Only)**

For windows users , NUnitTestAdapter have to be installed before you can run the test cases succcessfully.

1.  From Tools menu, use Library Package Manager and select Manage NuGet packages for solution.

2.  In the left panel, select Online

3.  Locate (search for) NUnit 3.0 Test Adapter in the center panel and highlight it

4.  Click install, and select existing project ApiSecuritySolution to add the adapter.


### Using the ApiList Class
The ApiUtilLib Library provide the utility class ApiList to construct request Query String and Form Data.

#### Generate QueryString
```
    var queryParam = new ApiUtilLib.ApiList();

    queryParam.Add("clientId", "1256-1231-4598");
    queryParam.Add("accountStatus", "active");
    queryParam.Add("txnDate", "2017-09-29");

    string queryString = queryParam.ToQueryString();

    string baseUrl = string.Format("https://example.com/resource/?{0}", queryString);
    // https://example.com/resource/?clientId=1256-1231-4598&accountStatus=active&txnDate=2017-09-29
```

#### Generate FormData
```
    var formData = new ApiUtilLib.ApiList();

    formData.Add("phoneNo", "+1 1234 4567 890");
    formData.Add("street", "Hellowood Street");
    formData.Add("state", "AP");

    string formData = formData.ToFormData();
    // phoneNo=%2B1+1234+4567+890&street=Hellowood+Street&state=AP
```

**NOTE** 

For **formData** parameter used for Signature generation, the key value parameters **do not** need to be URL encoded, 
When you use this client library method **ApiAuthorization.HttpRequest**, it will do the url-encoding during the HTTP call

### How to Generate HMAC256 L1 Authorization Header
```
public static void L1Sample()
{
    // application realm
    string realm = "<<your_client_host_url>>";

    // authorization prefix (i.e 'Apex_l1_eg' )
    string authPrefix = "<<authPrefix>>";

    // app id and app secret assign to the application
    string appId = "<<appId>>";
    string appSecret = "<<appSecret>>";

    // api signing Internet gateway name and path (for Intranet i.e <tenant>-pvt.i.api.gov.sg)
    string signingGateway = "<tenant>.e.api.gov.sg";
    string apiPath = "api/v1/l1/";

    // query string (optional)
    var queryParam = new ApiUtilLib.ApiList();

    queryParam.Add("clientId", "1256-1231-4598");
    queryParam.Add("accountStatus", "active");
    queryParam.Add("txnDate", "2017-09-29");

    string queryString = queryParam.ToQueryString();

    string baseUrl = string.Format("https://{0}/{1}?{2}", signingGateway, apiPath, queryString);

    // form data (optional)
    var formData = new ApiUtilLib.ApiList();

    formData.Add("phoneNo", "+1 1234 4567 890");
    formData.Add("street", "Hellowood Street");
    formData.Add("state", "AP");

    // authorization header
    var authorizationHeader = ApiAuthorization.Token(realm, authPrefix, HttpMethod.POST, new Uri(baseUrl), appId, appSecret, formData);

    Console.WriteLine("\n>>>Authorization Header :: '{0}'<<<", authorizationHeader);

    // no need append .e on the target Internet gateway name (for Intranet i.e <tenant>-pvt.api.gov.sg)
    string targetGatewayName = "<tenant>.api.gov.sg";
    string targetBaseUrl = string.Format("https://{0}/{1}?{2}", targetGatewayName, apiPath, queryString);

    // this method only for verification only
    // expecting result to be 200
    var result = ApiAuthorization.HttpRequest(new Uri(targetBaseUrl), authorizationHeader, formData, HttpMethod.POST, ignoreServerCert: true);
}
```

### How to Generate RSA256 L2 Authorization Header
```
public static void L2Sample()
{
    // application realm
    string realm = "<<your_client_host_url>>";

    // authorization prefix (i.e 'Apex_l2_eg' )
    string authPrefix = "<<authPrefix>>";

    // app id i.e 'Apex_l2_eg' assign to the application
    string appId = "<<appId>>";

    // api signing gateway name and path (for Intranet i.e <tenant>-pvt.i.api.gov.sg)
    string signingGateway = "<tenant>.e.api.gov.sg";
    string apiPath = "api/v1/l2/";

    // query string (optional)
    var queryParam = new ApiUtilLib.ApiList();

    queryParam.Add("clientId", "1256-1231-4598");
    queryParam.Add("accountStatus", "active");
    queryParam.Add("txnDate", "2017-09-29");

    string queryString = queryParam.ToQueryString();

    string baseUrl = string.Format("https://{0}/{1}?{2}", signingGateway, apiPath, queryString);

    // form data (optional)
    var formData = new ApiUtilLib.ApiList();

    formData.Add("phoneNo", "+1 1234 4567 890");
    formData.Add("street", "Hellowood Street");
    formData.Add("state", "AP");

    // private cert file and password
    string privateCertName = GetLocalPath("Certificates/alpha.example.api.com.p12");
    string password = "password";

    // get the private key from cert
    var privateKey = ApiAuthorization.PrivateKeyFromP12(privateCertName, password);
    
    // authorization header
    var authorizationHeader = ApiAuthorization.Token(realm, authPrefix, HttpMethod.POST, new Uri(baseUrl), appId, null, formData, privateKey);

    // no need append .e on the target gateway name (for Intranet i.e <tenant>-pvt.api.gov.sg)
    string targetGatewayName = "<tenant>.api.gov.sg";
    string targetBaseUrl = string.Format("https://{0}/{1}?{2}", targetGatewayName, apiPath, queryString);

    // this method only for verification only
    // expecting result to be 200
    var result = ApiAuthorization.HttpRequest(new Uri(targetBaseUrl), authorizationHeader, formData, HttpMethod.POST, ignoreServerCert: true);
}

static string GetLocalPath(string relativeFileName)
{
    var localPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), relativeFileName.Replace('/', Path.DirectorySeparatorChar));

    return localPath;
}
```
#### Sample HTTP POST Call for x-www-form-urlencoded with APEX L1 Security (for reference only)

```
[Test]
public static void Http_Call_Test()
{
    // application realm
    string realm = "http://example.api.test/token";

    // authorization prefix
    string authPrefix = "Apex_l1_eg";

    // app id and app secret assign to the application
    string appId = "tenant-1X2w7NQPzjO2azDu904XI5AE";
    string appSecret = "s0m3s3cr3t";
    var formData = new ApiUtilLib.ApiList();

    formData.Add("key1", "value1);
    formData.Add("key2", "value2");
    
    // api signing gateway name and path
    string gatewayName = "https://tenant.e.api.gov.sg";
    string apiPath = "api14021live/resource";
    string baseUrl = string.Format("{0}/{1}", gatewayName, apiPath);
    Console.WriteLine("\n>>>baseUrl :: '{0}'<<<", baseUrl);
    Console.WriteLine("\n>>>appId :: '{0}'<<<", appId);
    Console.WriteLine("\n>>>appSecret :: '{0}'<<<", appSecret);
    // authorization header
    var authorizationHeader = ApiAuthorization.Token(realm, authPrefix, HttpMethod.POST, new Uri(baseUrl), appId, appSecret, formData);

    Console.WriteLine("\n>>>Authorization Header :: '{0}'<<<", authorizationHeader);

    // if the target gateway name is different from signing gateway name
    string targetBaseUrl = "https://tenant.api.gov.sg/api14021live/resource";


    // this method only for verification only
    // expecting result to be 200

    var result = ApiAuthorization.HttpRequest(new Uri(targetBaseUrl), authorizationHeader, formData, HttpMethod.POST, ignoreServerCert: true);
    Console.WriteLine(result);
    Console.ReadLine();

    Assert.True(true);
}

```

## Release
+ See [CHANGELOG.md](CHANGELOG.md).

## Contributing
+ For more information about contributing PRs and issues, see [CONTRIBUTING.md](https://github.com/GovTechSG/csharp-apex-api-security/blob/master/.github/CONTRIBUTING.md).

## License
[MIT LICENSE ](https://github.com/GovTechSG/csharp-apex-api-security/blob/master/LICENSE)

## References
+ [Akana API Consumer Security](http://docs.akana.com/ag/cm_policies/using_api_consumer_app_sec_policy.htm)
+ [RSA and HMAC Request Signing Standard](http://tools.ietf.org/html/draft-cavage-http-signatures-05)

