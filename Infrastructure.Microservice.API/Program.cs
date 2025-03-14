
using Infrastructure.Microservice.APP;
using Infrastructure.Microservice.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Microservice.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;

            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<GCPInfrastructureDBContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("Value"), b => b.MigrationsAssembly("Infrastructure.Microservice.API")));
            builder.Services.AddDbContext<GCPTemplatesDBContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("Value"), b => b.MigrationsAssembly("Infrastructure.Microservice.API")));

            builder.Services.AddScoped<IGCPInfrastructureRepository, GCPInfrastructureRepository>();
            builder.Services.AddScoped<IGCPInfrastructureServices, GCPInfrastructureServices>();
            builder.Services.AddScoped<IGCPTemplatesServices, GCPTemplatesServices>();
            builder.Services.AddScoped<IGCPTemplatesRepository, GCPTemplatesRepository>();


            builder.Services.AddCors(options =>
            {

                options.AddPolicy("nuevaPolitica", app =>
                {

                    app.AllowAnyOrigin();
                    app.AllowAnyHeader();
                    app.AllowAnyMethod();
                });



            });

            var app = builder.Build();

            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("nuevaPolitica");

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}