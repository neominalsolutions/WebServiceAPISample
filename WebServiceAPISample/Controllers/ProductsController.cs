using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebServiceAPISample.Models;

namespace WebServiceAPISample.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ProductController : ControllerBase
  {

    // api/Products veya api/products

    [HttpGet] // HTTP Verb
    public IActionResult Get()
    {

      var products = new List<ProductResponse>();
      products.Add(new ProductResponse(1,"Ürün-1",15,25));
      products.Add(new ProductResponse(2, "Ürün-2", 15, 25));
      products.Add(new ProductResponse(3, "Ürün-3", 45, 215));

      return Ok(products);
    }

    // api/products/1 api/products/2
    [HttpGet("{id}")] // HTTP Verb /:id
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    
    public IActionResult GetById(int id) // id args yada parametre anlamına gelir
    {

      var products = new List<ProductResponse>();
      products.Add(new ProductResponse(1, "Ürün-1", 15, 25));
      products.Add(new ProductResponse(2, "Ürün-2", 15, 25));
      products.Add(new ProductResponse(3, "Ürün-3", 45, 215));

     
      var product = products.Where(item => item.Id == id).FirstOrDefault();

      return Ok(product);
    }


    // FormData yöntemi ile form alanları üzerinden veri gönderme [FromForm]
    // Request Body Json => Raw Json [FromBody]
    // api/products/create-by-form veya api/products/create-by-body 
    // File varsa mecbur olarak buradan gönderim yapmalıyız
    [HttpPost("create-by-form")]
    public IActionResult CreateFromForm([FromForm] ProductCreateRequest request, IFormFile file)
    {
      return Created(); // Status Code 201
    }

    [HttpPost("create-by-body")]
    public IActionResult CreateFromBody([FromBody] ProductCreateRequest request)
    {
      return Created("/api/products/create-by-body", new ProductResponse(1,request.Name,request.Price,request.Stock));
    }


    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    // Not: Client'dan kaynaklanan hatalardan biriside request nesnesinin yanlış gönderimi
    public IActionResult Update(int id, [FromBody] ProductUpdateRequest request)
    {

      if(request.Stock == 0)
      {
        throw new Exception("Sql Exception, Stock is required");
      }

      var products = new List<ProductResponse>();
      products.Add(new ProductResponse(1, "Ürün-1", 15, 25));
      products.Add(new ProductResponse(2, "Ürün-2", 15, 25));
      products.Add(new ProductResponse(3, "Ürün-3", 45, 215));

      if(products.Any(x=> x.Id == id)) // resource var mı kontrolü
      {
        return NoContent(); // 204 de alabiliriz. 404 de alabilir.
      }

      return NotFound(); // 404 kaynak bulunamadı

      // Not: Update işlemlerinde dönüş tipi API standartlarında 204 olarak tanımlanır.
    }


    [HttpDelete]
    public IActionResult Delete(int id)
    {
      return NoContent(); // 204 dönmesi
    }


    [HttpGet("/sendHeaders")]
    public IActionResult SendHeaders([FromHeader] string language, [FromHeader] string apiVersion)
    {
      return Ok("Header request");
    }

  }
}
