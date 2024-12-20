using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebServiceAPISample.Models;

namespace WebServiceAPISample.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthsController : ControllerBase
  {
    // api/auths/token
    // token generation endpoint

    [HttpPost("token")]
    public IActionResult Login([FromBody] LoginRequest loginRequest)
    {

      // simüle edeceğiz ve claim oluşturacağız.
      if(loginRequest.Email == "test@test.com" || loginRequest.password == "12345" || loginRequest.Email == "ali@test.com" || loginRequest.password == "1234")
      {
        // Claim kullancı hesabına özgü oturum açarken kullanıcaya atanmış olan bilgiler
        // Not: Claimde password gibi hassas bilgiler tutulmaz, sadece client uygulamanın erişmesi gereken bilgilere yer verir.
        // jwt payload dinamik olarak kullanıcının veri tabanında çekilen bilgilere göre değişiyor.
        var claims = new List<Claim>();
        claims.Add(new Claim("Email", loginRequest.Email));

        // roles
        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        claims.Add(new Claim(ClaimTypes.Role, "Manager"));

        // User Endpointleri üzerindeki yetkiler
        // permissions
        claims.Add(new Claim("User", "Insert"));
        claims.Add(new Claim("User", "Update"));
        claims.Add(new Claim("User", "Approve"));
        claims.Add(new Claim("User", "Delete"));

        var identity = new ClaimsIdentity(claims);
        // bu oturum açan kimlik hesabı


        var key = Encoding.ASCII.GetBytes("qwertyuiopasdfghjklzxcvbnm123456");
        var tokenHandler = new JwtSecurityTokenHandler();
        var descriptor = new SecurityTokenDescriptor
        {
          Subject = identity, // hesap bilgileri claim olarak subject olarak dışarı çıkıyor
          Expires = DateTime.UtcNow.AddMinutes(30),
          SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
        };
        var token = tokenHandler.CreateToken(descriptor);
        var accessToken = tokenHandler.WriteToken(token);

        return Ok(accessToken);
      }

      return BadRequest("Kullanıcının oturum açma izni yok");
    }
  }
}
