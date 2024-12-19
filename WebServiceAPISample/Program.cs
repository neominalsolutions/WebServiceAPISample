var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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