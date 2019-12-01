function Get-Product {
    <#
    
    .SYNOPSIS
    
    Get a product
    
    .DESCRIPTION
    
    The Get-Product CmdLet will return a product.
    
    .PARAMETER BaseUri
    
    The base URI of the resource endpoint. 
    
    .PARAMETER Id
    
    The identifier of the product to get.
    
    .EXAMPLE

    Get-Product -Id abc
    Get the product with id "abc"
    
    .LINK
  
    Connect-Identity
    about_securebydesign

    #>
    
    [CmdletBinding(ConfirmImpact="None")]
    Param(
        [Parameter(Mandatory=$true)]
        [string] $Id,
        
        [Parameter(Mandatory=$false)]
        [Uri] $BaseUri = "http://localhost:5001"
  )

    process {
        [Jwt] $jwt = Get-Token

        $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
        $headers.Add("Accept", "application/json")
        $headers.Add("Authorization", "bearer $($jwt.AccessToken)")

        $uri = [System.Uri]::new($BaseUri, "products?id=$Id")

        Invoke-RestMethod -Uri $uri -Headers $headers -Method Get
    }
}