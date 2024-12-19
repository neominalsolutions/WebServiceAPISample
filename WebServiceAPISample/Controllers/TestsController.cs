using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebServiceAPISample.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TestsController : ControllerBase
  {

    // api/tests/division/25/35
    // api/tests/division?sayi1=25&sayi2=34
    //[HttpGet("division/{sayi1}/{sayi2}")]
    [HttpGet]
    public async Task<IActionResult> Division([FromQuery] int sayi1, int sayi2)
    {

      // External Error

      //var client = new HttpClient();
      //var response = await client.GetStringAsync("https://www.n11.com");

      //return Ok(response);

      // Logical Error
      //int result = sayi1 / sayi2;
      //return Ok(result);



      // test-case düşünülerek yazılmış kod
      if (sayi1 == 0 || sayi2 == 0)
      {
        return BadRequest("Sayi değeri 0 olamaz");
      }
      else
      {


        int result = sayi1 / sayi2;
        return Ok(result);
      }


    }
  }
}
