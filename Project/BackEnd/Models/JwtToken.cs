using System;

public class JwtTokenModel
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecretKey { get; set; }
    public TimeSpan ExpiryTime { get; set; }
}
