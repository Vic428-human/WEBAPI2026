using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using WEBAPI2026.Services;
using WEBAPI2026.Repositories;

namespace WEBAPI2026
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
            // 註冊 Repository
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();

            // 註冊 Service
            services.AddScoped<SalesOrderService>();
            services.AddScoped<InventoryService>();


            services.AddControllers() // 這行會把你的Controller註冊到 ASP.NET Core 的 Routing 系統，這樣 API 路由才能找到對應的 Controller 方法。
            .AddJsonOptions(options =>
            {

                // 代表不要幫我改欄位名稱，照 C# class 的屬性名稱輸出
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            // 這段是把 Swagger 加進來，讓你可以在 /swagger 頁面看到 API 文件，並且直接測試 API
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WEBAPI2026", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // 這就是啟用 Swagger UI 的地方
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WEBAPI2026 v1"));
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
