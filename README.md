What is this?
-------------

This directory holds the source code that was the result of the
presentation on Service API security at OpKoKo 17.2, Skövde.

Since the presentation, it has been expanded to contain a second
service that can create a JWT token, found in folder "TokenService".
The code that we created in the presentation, is found in folder
"ProductsService".

## Certificates

First, you will need to create a certificate that we can use to create
JWT tokens.  Using PowerShell, you can create and install one as
follows:

```
New-SelfSignedCertificate `
  -CertStoreLocation cert:\currentuser\my `
  -Provider "Microsoft Enhanced RSA and AES Cryptographic Provider" `
  -Subject "CN=My OAuth2 Identity" `
  -FriendlyName "OAuth2 token signing for OpKoKo presentation on ASP.NET MVC Core 2 API security" `
  -Type CodeSigningCert `
  -KeyExportPolicy Exportable `
  -KeyLength 4096 `
  -NotAfter (Get-Date).AddYears(1) `
  -HashAlgorithm SHA256
```

## Run the code

To run the sample, open two terminal windows.  In the first, start the
token service:

```shell
cd TokenService
dotnet run 
```

Note the host and port where the token service starts (typically
http://localhost:5000).  Update the URL in class JwksStore to point to
your token service.  In the second terminal, start the products
service.

```shell
cd ProductsService
dotnet run --urls http://localhost:5001
```

You will need to select a different port than the token service is
using, like we did above.

You can now first verify that you will get a 401 from the products
service:

```
GET http://localhost:5001/products HTTP/1.1
```

Get an access token from the token endpoint:

```
POST http://localhost:5000/token HTTP/1.1
```

And use the returned access token on the products endpoint:

```
GET http://localhost:5001/products HTTP/1.1
Authorization: bearer <paste your access token here>
```
