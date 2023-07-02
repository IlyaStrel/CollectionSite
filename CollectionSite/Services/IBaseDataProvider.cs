using CS.Model.DB;
using CS.Model.DTO;

namespace CollectionSite.Services
{
    public interface IBaseDataProvider
    {
        Task<Consumer> GetConsumerData(
            HttpContext httpContext);

        Task<List<ScanOut>> GetScanPortsData(
            HttpContext httpContext, string target);
    }
}