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

// JWT ile kimlik doðrulama ayarý
builder.Services.AddAuthentication(x =>
{
x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
{
  opt.RequireHttpsMetadata = true;
  opt.SaveToken = true; // program tarafýnda token üreten kullanýcýn oturumuna ait token bilgisine programdan da eriþim olsun.
  opt.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true, // yanlýþ keyde validate etme
    IssuerSigningKey = new SymmetricSecurityKey(key), // key deðerini sisteme tanýt
    ValidateIssuer = false, // yok
    ValidateAudience = false, // yok
    ValidateLifetime = true, // expire olursa token validation doðrulamadan geçme
    LifetimeValidator = (notbefore, expires, securityToken, validationParamaters) => // token expire oldumu algoritmasý
    {
      Console.Out.WriteLineAsync("LifetimeValidator Event");
      return expires != null && expires.Value > DateTime.UtcNow;
    }
  };

  // Kiml,ik doðrulama süreci baþarýlý yoksa baþarýsýz mýyý yönettiðimiz eventler.
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

// tüm uygulamadaki hata süreçlerinde çalýþacak olan bir ara yazýlým
app.Use(async (context, next) =>
{
	Console.Out.WriteLine(context.Request.Headers["User-Agent"]);


	try // hata durumlarýnýn denendiði kýsým
	{
		await next();
	}
	catch (Exception ex) // hatanýn sunucuda oluþtuðu an
	{
		await Console.Out.WriteLineAsync("Hata: " + ex.Message);

		await Console.Out.WriteLineAsync("Source: " + ex.Source);

		context.Response.StatusCode = 500;
		await context.Response.WriteAsJsonAsync(new { Message = ex.Message });

	}
});


app.Run();
