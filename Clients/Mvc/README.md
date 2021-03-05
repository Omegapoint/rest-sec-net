What is this?
-------------

This folder holds the source code for a ASP.NET Core MVC client which demonstrates how to use the OIDC code flow
in order to access the API built in the course on secure REST API in ASP.NET Core, in C#.


It is based on sample from https://identitymodel.readthedocs.io/en/latest/, where we added some commonly used security features:

 - CSRF protection, using anti forgery tokens (double submit cookie pattern from https://cheatsheetseries.owasp.org/cheatsheets/Cross-Site_Request_Forgery_Prevention_Cheat_Sheet.html)

 - Host prefix cookie name (see more on https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Set-Cookie)

 - HSTS configuration, note that other security headers should be added but this is often not part of the application code and not part of this repo (see more on https://securityheaders.com/) 
 

## Run the code

1. Identity Server: Start on https://localhost:5009

2. REST API (lab 2 or later): Set options.Authority in Startup.cs to https://localhost:5009 and start the API on https://localhost:5001

3. MVC Client: Start on  https://localhost:5003

Login with "Alice" using password "alice", and then access the API from the top menu and get product 1!