What is this?
-------------

This folder holds the source code for the course on secure REST
API in ASP.NET Core, in C#.

This is lab 2, an excersice on how to handle errors in a
secure and scaleable manner.  The code on the tip of this branch
represents the intended end result of this excersice.

## Run the code

Open a terminal and start the products service:

```shell
dotnet run --urls=http://localhost:5001
```

If you are running the service from an IDE, like Visual Studio for
Windows, instead of from a terminal, you need to configure the port in
that IDE.  Each IDE works a little differently, but Google is probably
your friend.

Verify that you get a 500 response from the error resource:

```
GET http://localhost:5001/error
```

If you enable the developer exception page, you will get a lot of
error details, if you do not, the response will be empty.  You should
never use `UseDeveloperExceptionPage` for production use since this
_will_ leak sensitive information to a potential attacker.