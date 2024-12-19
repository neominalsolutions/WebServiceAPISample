namespace WebServiceAPISample.Models
{
  // POST, PUT ve PATCH verblerinde Request olarak nesne göndermek gerekir
  // GET ise Args olarak route parametesi veya querystring formatında göndeririz.
  public record ProductCreateRequest(string Name,decimal Price,int Stock);
  
}
