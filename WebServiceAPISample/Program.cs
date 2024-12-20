using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Swagger Authorization Implementasyonu
builder.Services.AddSwaggerGen(opt =>
{

  var securityScheme = new OpenApiSecurityScheme()
  {
    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.Http,
    Scheme = "Bearer",
    BearerFormat = "JWT" // Optional
  };

  var securityRequirement = new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "bearerAuth"
            }
        },
        new string[] {}
    }
};

  opt.AddSecurityDefinition("bearerAuth", securityScheme);
  opt.AddSecurityRequirement(securityRequirement);
});

var key = Encoding.ASCII.GetBytes("qwertyuiopasdfghjklzxcvbnm123456");

// JWT ile kimlik do�rulama ayar�
builder.Services.AddAuthentication(x =>
{
x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
{
  opt.RequireHttpsMetadata = true;
  opt.SaveToken = true; // program taraf�nda token �reten kullan�c�n oturumuna ait token bilgisine programdan da eri�im olsun.
  opt.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true, // yanl�� keyde validate etme
    IssuerSigningKey = new SymmetricSecurityKey(key), // key de�erini sisteme tan�t
    ValidateIssuer = false, // yok
    ValidateAudience = false, // yok
    ValidateLifetime = true, // expire olursa token validation do�rulamadan ge�me
    LifetimeValidator = (notbefore, expires, securityToken, validationParamaters) => // token expire oldumu algoritmas�
    {
      Console.Out.WriteLineAsync("LifetimeValidator Event");
      return expires != null && expires.Value > DateTime.UtcNow;
    }
  };

  // Kiml,ik do�rulama s�reci ba�ar�l� yoksa ba�ar�s�z m�y� y�netti�imiz eventler.
  opt.Events = new JwtBearerEvents()
  {
    OnAuthenticationFailed = c =>
    {
      Console.Out.WriteLineAsync("Authentication Failed" + c.Exception.Message);
      return Task.CompletedTask;
    },
    OnTokenValidated = c =>
    {
      Console.Out.WriteLineAsync("Authentication Validated");
      return Task.CompletedTask;
    },
    OnForbidden = c =>
    {
      Console.Out.WriteAsync("Yetki Yok");
      return Task.CompletedTask;
    }
  };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();





app.MapControllers();

// t�m uygulamadaki hata s�re�lerinde �al��acak olan bir ara yaz�l�m
app.Use(async (context, next) =>
{
	Console.Out.WriteLine(context.Request.Headers["User-Agent"]);


	try // hata durumlar�n�n denendi�i k�s�m
	{
		await next();
	}
	catch (Exception ex) // hatan�n sunucuda olu�tu�u an
	{
		await Console.Out.WriteLineAsync("Hata: " + ex.Message);

		await Console.Out.WriteLineAsync("Source: " + ex.Source);

		context.Response.StatusCode = 500;
		await context.Response.WriteAsJsonAsync(new { Message = ex.Message });

	}
});


app.Run();
