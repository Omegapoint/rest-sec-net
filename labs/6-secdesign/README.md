What is this?
-------------

This folder holds the source code for the course on secure REST API in
ASP.NET Core, in C#.

This is lab 6, a refactored solution of lab 5 that shows one way to
compose your classes to make testing and responsibilities a little
more clear, compared to the previous branches, which has each focused
on clarity of a specific point.

## Run the code

Open a terminal and start the products service:

```shell
dotnet run --urls=http://localhost:5001
```

If you are running the services from an IDE, like Visual Studio for
Windows, instead of from a terminal, you need to configure the ports
in that IDE.  Each IDE works a little differently, but Google is
probably your friend.

Get an access token from the token endpoint:

```
POST http://localhost:5000/connect/token
Content-Type: application/x-www-form-urlencoded

client_id=client&client_secret=secret&scope=products.read&grant_type=client_credentials
```

And use the returned access token on the products endpoint:

```
GET http://localhost:5001/products?id=abc
Authorization: bearer <paste your access token here>
```

## The Scripts folder

This folder also contains a very basic PowerShell module to show how
device flow works, but also as a template for how you can work with a
CLI client for management tasks.  This is a very powerful concept that
means you might not have to develop expensive UI components for some
tasks.

You will find the PowerShell module in folder Scripts.  To import the
module, use the following command:

```
Import-Module ./Scripts/SecureByDesign.psd1
```

Log on using the `Connect-Identity` CmdLet, and get a product using
`Get-Product`.
