What is this?
-------------

This folder holds the source code for the course on secure REST API in
ASP.NET Core, in C#.

This is lab 4, an excersice on how to validate input data.

## Run the code

Open a terminal and start the products service:

```shell
dotnet run --urls=http://localhost:5001
```

If you are running the services from an IDE, like Visual Studio for
Windows, instead of from a terminal, you need to configure the ports
in that IDE.  Each IDE works a little differently, but Google is
probably your friend.

You can now first verify that you will get a 401 from the products
service:

```
GET http://localhost:5001/api/products/1
```

Get an access token from the token endpoint:

```
POST http://localhost:5000/connect/token
Content-Type: application/x-www-form-urlencoded

client_id=client&client_secret=secret&scope=products.read&grant_type=client_credentials
```

And use the returned access token on the products endpoint:

```
GET http://localhost:5001/api/products/1
Authorization: bearer <paste your access token here>
```
