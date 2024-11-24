using FreeRedis;

namespace ControllerLock.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddFreeRedisControllerLock("106.15.239.67:6379,password=38gJschzyP7NMuH1,defaultDatabase=1");

            //builder.Services.AddMemoryControllerLock();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
