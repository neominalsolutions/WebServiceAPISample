namespace WebServiceAPISample.Models
{
  // {id:1,name:Urun1,price:10,stock:15 } json
  public record ProductResponse(int Id,string Name, decimal Price, int Stock);
 
}
