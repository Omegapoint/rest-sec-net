What is this?
-------------

This folder holds the source code for the course on secure REST API in
ASP.NET Core, in C#.

This is lab 3, an excersice to use tests to verify the security and
function of a secure REST API.

## Run the code

To run the unit tests, open a terminal window and type:

```shell
cd Tests
dotnet test --filter Category=Unit
```

To run the system tests, you will need to start the product service.

```shell
cd Host
dotnet run --urls=http://localhost:5001
```

Run the system tests in a separate terminal window:

```shell
cd Tests
dotnet test --filter Category=System
```
