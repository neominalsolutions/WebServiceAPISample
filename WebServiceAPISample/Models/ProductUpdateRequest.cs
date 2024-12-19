namespace WebServiceAPISample.Models
{
  // HTPP PUT tüm alnların güncel değerler ile değiştirilmesi için kullanılır
  public record ProductUpdateRequest(int Id,string Name, decimal Price,int Stock);
}
