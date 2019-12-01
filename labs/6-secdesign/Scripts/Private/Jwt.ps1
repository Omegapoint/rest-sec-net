class Jwt
{
    [string]$AccessToken
    [datetime]$Expires

    Jwt($TokenResponse)
    {
        $this.AccessToken = $TokenResponse.access_token
        $this.Expires = [datetime]::UtcNow.AddSeconds($TokenResponse.expires_in)
     }
}
