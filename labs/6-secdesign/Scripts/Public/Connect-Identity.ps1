function Connect-Identity {
  <#
  
  .SYNOPSIS
  
  Log on
  
  .DESCRIPTION

  The Connect-Identity CmdLet will request an JWT token from an OAuth2
  endpoint and store it for use by other CmdLets, using Get-Token.
  
  The Connect-Identity CmdLet will store the token in the global scope,
  which means that you will be logged on for the duration of the current
  PowerShell session (or until the token expires, whichever happens
  first.)
  
  .PARAMETER ClientId
  
  The client id
    
  .PARAMETER Scopes

  The scopes that will be passed to the identity provider
    
  .PARAMETER BaseUri
  
  The base URI of the OAuth2 endpoint 
  
  .EXAMPLE

  .OUTPUTS
  
  Jwt.  Connect-Identity returns a structure that represents the JWT
  token returned from the OAuth2 endpoint.
  
  .LINK
  
  Get-Token
  about_securebydesign
  
  #>
    [CmdletBinding(ConfirmImpact="Low")]
    Param(
        [Parameter(Mandatory=$false)]
        [string] $ClientId = "cli",

        [Parameter(Mandatory=$false)]
        [string[]] $Scopes = @("products.read"),

        [Parameter(Mandatory=$false)]
        [Uri] $BaseUri = "http://localhost:5000"
    )
  
    process {
        $tokenUri = [System.Uri]::new($BaseUri, "/connect/token")
        $encodedScopes = [string]::Join("%20", $Scopes)

        # Use Device Flow
        $uri = [System.Uri]::new($BaseUri, "connect/deviceauthorization")

        $body = "client_id=$ClientId&scope=$encodedScopes"

        try {
            $authorization = Invoke-RestMethod -Uri $uri -Body $body -ContentType "application/x-www-form-urlencoded" -Method Post
        }
        catch {
            Throw $_            
        }

        $now = Get-Date
        $expires = (Get-Date).AddSeconds($authorization.expires_in)

        Write-Host "To sign in, use a web browser to open the page $($authorization.verification_uri) and enter the code $($authorization.user_code) to authenticate."

        while ($expires -gt $now) {

            $loopBody = "grant_type=urn%3Aietf%3Aparams%3Aoauth%3Agrant-type%3Adevice_code&device_code=$($authorization.device_code)&client_id=$ClientId"

            try {
                $authorized = Invoke-RestMethod -ErrorAction SilentlyContinue -Uri $tokenUri -Body $loopBody -ContentType "application/x-www-form-urlencoded" -Method Post

                $global:identityJwt = [Jwt]::new($authorized)
                return $global:identityJwt
            } catch {

            }

            Start-Sleep -Seconds $authorization.interval
        }

        Write-Error "Authorization error"
    }
}