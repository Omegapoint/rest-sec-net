using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SecureByDesign.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseUrls("http://localhost:5009")
                        .UseKestrel(options => options.AddServerHeader = false)
                        .UseStartup<Startup>();
                });
    }
}
