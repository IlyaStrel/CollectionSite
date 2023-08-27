using CollectionSite.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollectionSite.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsumerController : Controller
    {
        readonly IBaseDataProvider _baseDataProvider;

        public ConsumerController(
            IBaseDataProvider baseDataProvider)
        {
            _baseDataProvider = baseDataProvider;
        }

        /// <summary>
        /// Method for scanning ports and saving data to DB
        /// (saving consumer information and scanning information)
        /// </summary>
        /// <returns></returns>
        [HttpGet("scanning")]
        public async Task<ActionResult> GetScanning(
            [FromQuery] AddressInput addressIn)
        {
            try
            {
                // Adding consumer data to sqlite DB
                await _baseDataProvider.GetConsumerData(HttpContext);

                // Adding scanning hosts data to pqsql DB
                var result = await _baseDataProvider.GetScanPortsData(HttpContext, addressIn.GetAddress());

                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}