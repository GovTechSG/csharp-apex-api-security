# APEX API C# Security Utility 
[![Build Status](https://travis-ci.org/GovTechSG/csharp-apex-api-security.svg?branch=master)](https://travis-ci.org/GovTechSG/csharp-apex-api-security)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)
[![MIT License](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/GovTechSG/csharp-apex-api-security/blob/master/LICENSE)

A C# helper utility that construct and sign HTTP Authorization header scheme for API authentication and verification

## Table of Contents (version 2.0 - beta (2021-08-08 v2.0.1))
- [Getting Started](#getting-started)
    * [Prerequisites](#prerequisites)
    * [Query String and FormData Class](#using-the-queryData-and-formData-class)
        + [Generate QueryString](#generate-querystring)
        + [Generate FormData](#generate-formdata)
    * [Constructing L1 Authorization Header](#how-to-generate-l1-authorization-header)
    * [Supported Private Key File Type](#supported-private-key-file-type)
    * [Constructing L2 Authorization Header](#how-to-generate-l2-authorization-header)
    * [Cross Zone API from Internet to Intranet](*how-to-generate-l21-authorization-header)
    * [Cross Zone API from Intranet to Internet](*how-to-generate-l12-authorization-header)
- [Release](#release)
- [Contributing](#contributing)
- [License](#license)
- [References](#references)

## Getting Started

### Prerequisites
+ .NET Framework 4.6.1
+ Visual Studio 2019 Community
+ NUnit Framework 3.13+

Make sure that all unit test cases are passed before using the library.

#### Installing NUnit **(Important : Windows Only)**

For windows users , NUnitTestAdapter have to be installed before you can run the test cases succcessfully.

1.  From Tools menu, use Library Package Manager and select Manage NuGet packages for solution.

2.  In the left panel, select Online

3.  Locate (search for) NUnit 3.0 Test Adapter in the center panel and highlight it

4.  Click install, and select existing project ApiSecuritySolution to add the adapter.


### Using the QueryData and FormData Class
The ApiUtilLib Library provide the utility class QueryData to construct request Query String and Form Data.

#### Generate QueryString
```
    var queryData = new QueryData();

    queryData.Add("clientId", "1256-1231-4598");
    queryData.Add("accountStatus", "active");
    queryData.Add("txnDate", "2017-09-29");

    string queryString = queryData.ToString();

    string baseUrl = string.Format("https://example.com/resource{0}", queryString);
    // https://example.com/resource?clientId=1256-1231-4598&accountStatus=active&txnDate=2017-09-29
```

#### Generate FormData
```
    var formData = new FormData();

    formData.Add("phoneNo", "+1 1234 4567 890");
    formData.Add("street", "Hellowood Street");
    formData.Add("state", "AP");

    string formData = formData.ToString();
    // phoneNo=%2B1+1234+4567+890&street=Hellowood+Street&state=AP
```

**NOTE** 

For **formData** parameter used for Signature generation, the key value parameters **do not** need to be URL encoded, 
When you use this client library method **ApiAuthorization.HttpRequest**, it will do the url-encoding during the HTTP call

### How to Generate L1 Authorization Header
```
public void L1Sample()
{
    var URL = "https://{gatewayName}.api.gov.sg/api/v1/resource";
    var APP_NAME = "{appName}";
    var APP_SECRET = "{appSecret}";

    // prepare form data
    var formData = new FormData();
    formData.Add("q", "how to validate signature in pdf");
    formData.Add("ei", "yAr8YLmwCM_Fz7sPsKmLoAU");

    var authParam = new AuthParam()
    {
        url = new Uri($"{URL}"),
        httpMethod = HttpMethod.POST,

        appName = APP_NAME,
        appSecret = APP_SECRET,

        formData = formData
    };

    // get the authorization token for L1
    var authToken = ApiAuthorization.TokenV2(authParam);

    Console.WriteLine($"\n>>> BaseString :: '{authToken.BaseString}'<<<");
    Console.WriteLine($"\n>>> Authorization Token :: '{authToken.Token}'<<<");

    // make api call with authToken.Token
}
```

### Supported Private Key File Type
1. .pem/.key - pkcs#1 base64 encoded text file
2. .pem/.key - pkcs#8 base64 encoded text file
2. .p12/.pfx - pkcs#12 key store

### How to Generate L2 Authorization Header
```
public void L2Sample()
{
    var URL = "https://{gatewayName}.api.gov.sg/api/v1/resource";
    var APP_NAME = "{appName}";
    var PRIVATE_KEY_FILE_NAME = "privateKey.key";
    var PRIVATE_KEY_PASSPHRASE = "{passphrase}";

    // get the private key from pem file (in pkcs1 format)
    var privateKey = ApiAuthorization.GetPrivateKey(PRIVATE_KEY_FILE_NAME, PRIVATE_KEY_PASSPHRASE);

    // prepare queryString
    var queryData = new QueryData();
    queryData.Add("view", "net-5.0");
    queryData.Add("system", "C# sample code");

    // get url safe querystring from ToString()
    Console.WriteLine($">>> Query String >>>{queryData.ToString()}<<<");

    // prepare form data
    var formData = new FormData();
    formData.Add("name", "peter pan");
    formData.Add("age", "12");

    var authParam = new AuthParam()
    {
        url = new Uri($"{URL}{queryData.ToString()}"),
        httpMethod = HttpMethod.POST,

        appName = APP_NAME,
        privateKey = privateKey,

        formData = formData
    };

    // get the authorization token for L1
    var authToken = ApiAuthorization.TokenV2(authParam);

    Console.WriteLine($"\n>>> BaseString :: '{authToken.BaseString}'<<<");
    Console.WriteLine($"\n>>> Authorization Token :: '{authToken.Token}'<<<");

    // make api call with authToken.Token
}
```
### How to Generate L21 Authorization Header
(for cross zone api from internet to intranet)
```
public void L21Sample()
{
    var URL_WWW = "https://{www_gatewayName}.api.gov.sg/api/v1/resource";
    var APP_NAME_WWW = "www_appName";
    var PRIVATE_KEY_FILE_NAME = "www_privateKey.key");
    var PRIVATE_KEY_PASSPHRASE = "{password}";

    var URL_WOG = "https://{wog_gatewayName}.api.gov.sg/api/v1/resource";
    var APP_NAME_WOG = "{wog_AppName}";
    var APP_SECRET_WOG = "{wog_appSecret}";

    // get the private key from pem file (in pkcs1 format)
    var privateKey = ApiAuthorization.GetPrivateKey(PRIVATE_KEY_FILE_NAME, PRIVATE_KEY_PASSPHRASE);

    // prepare queryString
    var queryData = new QueryData();
    queryData.Add("view", "net-5.0");
    queryData.Add("system", "C# sample code");

    // prepare form data
    var formData = new FormData();
    formData.Add("name", "peter pan");
    formData.Add("age", "12");

    // prepare the parameters
    var authParam = new AuthParam()
    {
        url = new Uri($"{URL_WWW}{queryData.ToString()}"),
        httpMethod = HttpMethod.POST,

        appName = APP_NAME_WWW,
        privateKey = privateKey,

        formData = formData,

        nextHop = new AuthParam()
        {
            url = new Uri($"{URL_WOG}{queryData.ToString()}"),

            appName = APP_NAME_WOG,
            appSecret = APP_SECRET_WOG
        }
    };

    // get the authorization token for L21
    var authToken = ApiAuthorization.TokenV2(authParam);

    Console.WriteLine($"\n>>>{tag}<<< BaseString :: '{authToken.BaseString}'<<<");
    Console.WriteLine($"\n>>>{tag}<<< Authorization Token :: '{authToken.Token}'<<<");

    // make api call with authToken.Token
}

```

### How to Generate L12 Authorization Header
(for cross zone api from intranet to internet)
```
public void L12Sample()
{
    var URL_WOG = "https://{wog_gatewayName}.api.gov.sg/api/v1/reslource";
    var APP_NAME_WOG = "{wog_appName}";
    var APP_SECRET_WOG = "{wog_AppSecret}";

    var URL_WWW = "https://{www_appName}.api.gov.sg/api/v1/resource";
    var APP_NAME_WWW = "{www_AppName}";
    var PRIVATE_KEY_FILE_NAME = "Certificates/www_privateKey.pkcs8");
    var PRIVATE_KEY_PASSPHRASE = "{passphrase}";

    // get the private key from pem file (in pkcs8 format)
    var privateKey = ApiAuthorization.GetPrivateKey(PRIVATE_KEY_FILE_NAME, PRIVATE_KEY_PASSPHRASE);

    // prepare queryString
    var queryData = new QueryData();
    queryData.Add("view", "net-5.0");
    queryData.Add("system", "C# sample code");

    // prepare form data
    var formData = new FormData();
    formData.Add("name", "peter pan");
    formData.Add("age", "12");

    // prepare the token parameters
    var authParam = new AuthParam()
    {
        url = new Uri($"{URL_WOG}{queryData.ToString()}"),
        httpMethod = HttpMethod.POST,

        appName = APP_NAME_WOG,
        appSecret = APP_SECRET_WOG,

        formData = formData,

        nextHop = new AuthParam()
        {
            url = new Uri($"{URL_WWW}{queryData.ToString()}"),

            appName = APP_NAME_WWW,
            privateKey = privateKey,
        }
    };

    // get the authorization token
    var authToken = ApiAuthorization.TokenV2(authParam);

    Console.WriteLine($"\n>>> BaseString :: '{authToken.BaseString}'<<<");
    Console.WriteLine($"\n>>> Authorization Token :: '{authToken.Token}'<<<");

    // make api call with authToken.Token
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

