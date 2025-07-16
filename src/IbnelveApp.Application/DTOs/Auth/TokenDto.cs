namespace IbnelveApp.Application.DTOs.Auth;

public class TokenDto
{
    public string AccessToken { get; set; }
    public DateTime ExpiresIn { get; set; }
}
