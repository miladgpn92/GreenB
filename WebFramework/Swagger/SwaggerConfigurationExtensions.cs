using Common.Consts;
using Common.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WebFramework.Swagger
{
    public static class SwaggerConfigurationExtensions
    {
        public class EnumSchemaFilter : ISchemaFilter
        {
            public void Apply(OpenApiSchema model, SchemaFilterContext context)
            {
                if (context.Type.IsEnum)
                {
                    model.Enum.Clear();
                    Enum.GetNames(context.Type)
                        .ToList()
                        .ForEach(n => model.Enum.Add(new OpenApiString(n)));
                }
            }
        }
   

        public class AddAuthHeaderOperationFilter : IOperationFilter
        {

            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                if (operation.Security == null)
                    operation.Security = new List<OpenApiSecurityRequirement>();


                var scheme = new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearer" } };
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [scheme] = new List<string>()
                });
            }
        }


        public static void AddSwagger(this IServiceCollection services)
        {
            Assert.NotNull(services, nameof(services));

            // اضافه‌کردن example filters به‌صورت داینامیک
            var mainAssembly = Assembly.GetEntryAssembly();
            var mainType = mainAssembly.GetExportedTypes()[0];
            var methodName = nameof(Swashbuckle.AspNetCore.Filters.ServiceCollectionExtensions.AddSwaggerExamplesFromAssemblyOf);
            MethodInfo method = typeof(Swashbuckle.AspNetCore.Filters.ServiceCollectionExtensions)
                .GetRuntimeMethods()
                .FirstOrDefault(x => x.Name == methodName && x.IsGenericMethod);
            MethodInfo generic = method.MakeGenericMethod(mainType);
            generic.Invoke(null, new[] { services });

            // پیکربندی Swagger
            services.AddSwaggerGen(options =>
            {
                var xmlDocPath = Path.Combine(AppContext.BaseDirectory, "MyApi.xml");
                options.IncludeXmlComments(xmlDocPath, true);
                options.EnableAnnotations();
                options.SchemaFilter<EnumSchemaFilter>();
                options.ExampleFilters();
                options.OperationFilter<ApplySummariesOperationFilter>();
                options.OperationFilter<AddAuthHeaderOperationFilter>();

                // 🔹 تعریف چند Swagger Doc برای هر گروه نقش
            
                options.SwaggerDoc(RoleConsts.Admin, new OpenApiInfo { Title = "Admin APIs", Version = "v1" });
                options.SwaggerDoc("Common", new OpenApiInfo { Title = "Common APIs", Version = "v1" });


                // 🔸 هر کنترلر باید [ApiExplorerSettings(GroupName = "Manager")] داشته باشه
                // تا در داکیومنت مربوطه نمایش داده بشه
                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo))
                        return false;

                    // خواندن GroupName از کنترلر یا اکشن
                    var groupName = apiDesc.GroupName ??
                                    methodInfo.DeclaringType?
                                        .GetCustomAttributes<ApiExplorerSettingsAttribute>(true)
                                        .FirstOrDefault()?.GroupName;

                    return string.Equals(groupName, docName, StringComparison.OrdinalIgnoreCase);
                });

                // 🔹 تعریف احراز هویت Bearer
                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Description = "<b>Token only</b> - without Bearer prefix",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer"
                });

                options.OperationFilter<UnauthorizedResponsesOperationFilter>(true, "bearer");

                // 🔹 اگر از Versioning استفاده نمی‌کنی این بخش را حذف کن
                // وگرنه Swagger مسیرها را نادرست می‌سازد
                // 🔸 برای ساده‌سازی، حذف شد
                // options.OperationFilter<RemoveVersionParameters>();
                // options.DocumentFilter<SetVersionInPaths>();
            });
        }


        public static void UseSwaggerAndUI(this IApplicationBuilder app)
        {
            Assert.NotNull(app, nameof(app));

            //More info : https://github.com/domaindrivendev/Swashbuckle.AspNetCore

            //Swagger middleware for generate "Open API Documentation" in swagger.json
            app.UseSwagger(options =>
            {
                //options.RouteTemplate = "api-docs/{documentName}/swagger.json";
            });

            //Swagger middleware for generate UI from swagger.json
            app.UseSwaggerUI(options =>
            {
             
                options.SwaggerEndpoint($"/swagger/{RoleConsts.Admin}/swagger.json", $"{RoleConsts.Admin}");
                options.SwaggerEndpoint($"/swagger/Common/swagger.json", $"Common");
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);

                #region Customizing
                //// Display
                options.DocExpansion(DocExpansion.None);
                #endregion
            });

         
        }
    }
}
