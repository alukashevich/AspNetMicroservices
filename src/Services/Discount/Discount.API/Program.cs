using Discount.API.Extensions;

namespace Discount.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host  = CreateHostBuilder(args).Build();
            host.MigrateDatabase<Program>()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}