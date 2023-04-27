using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Api.Template.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSentry()
                .UseStartup<Startup>();
    }
}