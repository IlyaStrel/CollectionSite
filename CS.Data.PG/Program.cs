using System;
using Microsoft.Extensions.Hosting;

namespace CS.Data.PG
{
    public class Program
    {
        private static void Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            var host = Host.CreateDefaultBuilder(args).Build();
            host.Run();
        }
    }
}