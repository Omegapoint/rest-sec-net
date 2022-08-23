# Deprecated

This repository is deprecated in favor of
https://github.com/Omegapoint/defence-in-depth

For a Java implementation, see
https://github.com/Omegapoint/defence-in-depth-java

What is this?
-------------

This folder holds the source code for the Omegapoint course on secure
REST API in ASP.NET Core, in C#.

There are a number of labs, organized in different folders under `labs`:

  * `1-auth`       Authentication & Authorization
  * `2-errors`     Error handling
  * `3-tdd`        Test driven development
  * `4-validation` Input data validation
  * `5-headers`    HTTP security related headers
  * `6-secdesign`  Refactor into domain application layer

Each folder has a README.md file that contains instructions for the lab.

## Run the identity provider

All of the labs require an IdP that provides the tokens we need to
secure the system we are building.

The folder `IdentityServer` contains a default implementation of an
identity provider, based on IdentityServer4.  It is created from the
"in memory" base template, and very slightly adjusted for our needs.

IdentityServer4 is a very capable identity provider, and you can read
a lot more, including samples and getting started guides here:

https://identityserver.io/

You can start the identity provider by stepping into the
`IdentityServer` folder and run the following command:

```sh
dotnet run --urls=http://localhost:5000
```

If you prefer, you can instead use the provided Docker file:

Build the docker image for identity server:

```sh
docker build -t identityserver .
```

Run the docker image:

```sh
docker run -it --rm -p 5000:5000 -t identityserver
```

You can connect a shell to the image, if you want to inspect the
contents, by running a shell from a specified entrypoint:

```sh
docker run -it --rm --entrypoint sh identityserver
```

Either way, you can verify that you have a running identity provider
by querying the discovery document:

```sh
curl http://localhost:5000/.well-known/openid-configuration
```

You can also get an access token using the client credentials flow:

```
POST http://localhost:5000/connect/token
Content-Type: application/x-www-form-urlencoded

client_id=client&client_secret=secret&scope=products.read&grant_type=client_credentials
```

## Create APIs

We have use the `dotnet new` command to create all of the samples.  We
typically use `dotnet new empty` and build out from there, but you can
use `dotnet new webapi` as well.  There are many templates to choose
from, and they tend to change between major releases of .NET, so have
a look yourself by just running the `dotnet new` command.

To be able to use the `JwtBearerDefaults` class, you need to import
the `Microsoft.AspNetCore.Authentication.JwtBearer` NuGet package:

```
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

# Run an HTTP request

There are many ways to execute an HTTP request.  For example, you can
use Postman, Powershell, curl, Insomnia etc.  These days, we tend to
prefer the [rest-client][1] Visual Studio Code extension.

[1]: https://github.com/Huachao/vscode-restclient
