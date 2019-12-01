function Get-Token {
  <#
  
  .SYNOPSIS
  
  Get the JWT token that is stored in this session
  
  .DESCRIPTION
  
  The Get-Token CmdLet will return the JWT token, or throw if
  not valid.

  .LINK
  
  Connect-Identity
  about_securebydesign
  
  #>
      [CmdletBinding(ConfirmImpact="Low")]
      Param(
          [Parameter(Mandatory=$false)]
          [switch] $AccessToken
      )
  
      process {
          if (!$global:identityJwt) {
              throw "You are not logged in. Run Connect-Identity to log in."
          }

          if ($global:identityJwt.Expires -lt [datetime]::UtcNow) {
              throw "Your session has expired. Run Connect-Identity to log in again."
          }
  
          if ($AccessToken) {
            return $global:identityJwt.AccessToken
        }

          return $global:identityJwt
      }
  }