using CS.Data.EFC.Base;
using CS.Data.EFC.Context;
using CS.Model.DB;
using Microsoft.Extensions.Logging;

namespace CS.Data.EFC
{
    public class ConsumerRepository : Repository<Consumer>, IConsumerRepository
    {
        readonly ILogger _logger;
        readonly SQLiteContext _context;

        public ConsumerRepository(
            SQLiteContext context, ILogger<ConsumerRepository> logger)
            : base(context, logger)
        {
            _logger = logger;
            _context = context;
        }
    }
}