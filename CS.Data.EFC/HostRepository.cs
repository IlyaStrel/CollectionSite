using CS.Data.EFC.Base;
using CS.Data.EFC.Context;
using CS.Model.DB;
using Microsoft.Extensions.Logging;

namespace CS.Data.EFC
{
    public class HostRepository : Repository<Host>, IHostRepository
    {
        readonly ILogger _logger;
        readonly PGContext _context;

        public HostRepository(
            PGContext context, ILogger<HostRepository> logger)
            : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }
    }
}