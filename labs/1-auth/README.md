What is this?
-------------

This folder holds the source code for the course on secure REST
API in ASP.NET Core, in C#.

This is lab 1, an demo on how to properly secure an API endpoint.

## Run the code

Open a terminal and start the products service:

```shell
dotnet run --urls=http://localhost:5001
```

If you are running the service from an IDE, like Visual Studio for
Windows, instead of from a terminal, you need to configure the port in
that IDE.  Each IDE works a little differently, but Google is probably
your friend.

Verify that you get a 200 OK response from the products resource:

```
GET http://localhost:5001/api/products/1
```

Add authentication in the `Startup` class and verify that you now get
a 401 response from the resource:

```
GET http://localhost:5001/api/products/1
```

Get an access token from the IdP (documented in the root folder
README).  Include it in the request and verify that you get a 200 OK
response:

```
GET http://localhost:5001/api/products/1
Authorization: Bearer <access token>
```
