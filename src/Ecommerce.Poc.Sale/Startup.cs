using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Ecommerce.Poc.Sale
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ecommerce.Poc.Sale", Version = "v1" });
            });

            services.AddDbContext<SaleDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("SaleDbContext")));

            services.AddCap(x =>
            {
                x.UseEntityFramework<SaleDbContext>();

                x.UseRabbitMQ(o =>
                {
                    o.HostName = Configuration.GetValue<string>("RabbitMQ:HostName");
                    o.UserName = Configuration.GetValue<string>("RabbitMQ:UserName");
                    o.Password = Configuration.GetValue<string>("RabbitMQ:Password");
                    o.Port = Configuration.GetValue<int>("RabbitMQ:Port");
                    o.ExchangeName = Configuration.GetValue<string>("RabbitMQ:ExchangeName");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce.Poc.Sale v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
