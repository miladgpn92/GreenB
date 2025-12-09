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

            //More info : https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters

            #region AddSwaggerExamples
            //Add services to use Example Filters in swagger
            //If you want to use the Request and Response example filters (and have called options.ExampleFilters() above), then you MUST also call
            //This method to register all ExamplesProvider classes form the assembly
            //services.AddSwaggerExamplesFromAssemblyOf<PersonRequestExample>();

            //We call this method for by reflection with the Startup type of entry assmebly (MyApi assembly)
            var mainAssembly = Assembly.GetEntryAssembly(); // => MyApi project assembly
            var mainType = mainAssembly.GetExportedTypes()[0];

            var methodName = nameof(Swashbuckle.AspNetCore.Filters.ServiceCollectionExtensions.AddSwaggerExamplesFromAssemblyOf);
            //MethodInfo method = typeof(Swashbuckle.AspNetCore.Filters.ServiceCollectionExtensions).GetMethod(methodName);
            MethodInfo method = typeof(Swashbuckle.AspNetCore.Filters.ServiceCollectionExtensions).GetRuntimeMethods().FirstOrDefault(x => x.Name == methodName && x.IsGenericMethod);
            MethodInfo generic = method.MakeGenericMethod(mainType);
            generic.Invoke(null, new[] { services });
            #endregion

            //Add services and configuration to use swagger
            services.AddSwaggerGen(options =>
            {
                var xmlDocPath = Path.Combine(AppContext.BaseDirectory, "MyApi.xml");
                //show controller XML comments like summary
                options.IncludeXmlComments(xmlDocPath, true);

                options.EnableAnnotations();

                #region DescribeAllEnumsAsStrings
                //This method was Deprecated. 
                options.SchemaFilter<EnumSchemaFilter>();
           //   options.DescribeAllEnumsAsStrings();

                //You can specify an enum to convert to/from string, uisng :
                //[JsonConverter(typeof(StringEnumConverter))]
                //public virtual MyEnums MyEnum { get; set; }

                //Or can apply the StringEnumConverter to all enums globaly, using :
                //SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
                //OR
                //JsonConvert.DefaultSettings = () =>
                //{
                //    var settings = new JsonSerializerSettings();
                //    settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
                //    return settings;
                //};
                #endregion


                options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "API V1" });
              //  options.SwaggerDoc("v2", new OpenApiInfo { Version = "v2", Title = "API V2" });

                #region Filters
                //Enable to use [SwaggerRequestExample] & [SwaggerResponseExample]
                options.ExampleFilters();


                //Set summary of action if not already set
                options.OperationFilter<ApplySummariesOperationFilter>();

                #region Add UnAuthorized to Response
                //Add 401 response and security requirements (Lock icon) to actions that need authorization
                options.OperationFilter<UnauthorizedResponsesOperationFilter>(true, "OAuth2");
                #endregion

                #region Add Jwt Authentication
                //Add Lockout icon on top of swagger ui page to authenticate
                #region Old way
                //options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                //{
                //    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                //    Name = "Authorization",
                //    In = "header"
                //});
                //options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                //{
                //    {"Bearer", new string[] { }}
                //});
                #endregion

                //options.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "OAuth2" }
                //        },
                //        Array.Empty<string>() //new[] { "readAccess", "writeAccess" }
                //    }
                //});

                //OAuth2Scheme
                //options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
                //{
                //    //Scheme = "Bearer",
                //    //In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.OAuth2,
                //    Flows = new OpenApiOAuthFlows
                //    {
                //        Password = new OpenApiOAuthFlow
                //        {
                //            TokenUrl = new Uri("https://localhost:5001/api/v1/users/Token"),
                //            //AuthorizationUrl = new Uri("https://localhost:5001/api/v1/users/Token")
                //            //Scopes = new Dictionary<string, string>
                //            //{
                //            //    { "readAccess", "Access read operations" },
                //            //    { "writeAccess", "Access write operations" }
                //            //}
                //        }
                //    }
                //});


                //          options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //          {
                //              Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                //                Enter 'Bearer' [space] and then your token in the text input below.
                //                \r\n\r\nExample: 'Bearer 12345abcdef'",
                //              Name = "Authorization",
                //              In = ParameterLocation.Header,
                //              Type = SecuritySchemeType.ApiKey,
                //              Scheme = "Bearer"
                //          });

                //          options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                //{
                //  {
                //    new OpenApiSecurityScheme
                //    {
                //      Reference = new OpenApiReference
                //        {
                //          Type = ReferenceType.SecurityScheme,
                //          Id = "Bearer"
                //        },
                //        Scheme = "oauth2",
                //        Name = "Bearer",
                //        In = ParameterLocation.Header,

                //      },
                //      new List<string>()
                //    }
                //  });

                options.OperationFilter<AddAuthHeaderOperationFilter>();
                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Description = "<b>Token only</b> - without Bearer prefix  <b style='color:red'>❤</b>",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer"
                });


                //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    In = ParameterLocation.Header,
                //    Description = "Please insert JWT with Bearer into field",
                //    Name = "Authorization",
                //    Type = SecuritySchemeType.ApiKey
                //});
                //options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                //  {
                //    {
                //      new OpenApiSecurityScheme
                //      {
                //        Reference = new OpenApiReference
                //          {
                //            Type = ReferenceType.SecurityScheme,
                //            Id = "Bearer"
                //          },
                //          Scheme = "oauth2",
                //          Name = "Bearer",
                //          In = ParameterLocation.Header,

                //        },
                //        new List<string>()
                //      }
                //    });

                //options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                //   {
                //     new OpenApiSecurityScheme
                //     {
                //       Reference = new OpenApiReference
                //       {
                //         Type = ReferenceType.SecurityScheme,
                //         Id = "Bearer"
                //       }
                //      },
                //      new string[] { }
                //    }
                //  });



                #endregion

                #region Versioning
                // Remove version parameter from all Operations
                options.OperationFilter<RemoveVersionParameters>();

                //set version "api/v{version}/[controller]" from current swagger doc verion
                options.DocumentFilter<SetVersionInPaths>();

                //Seperate and categorize end-points by doc version
                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                    var versions = methodInfo.DeclaringType
                        .GetCustomAttributes<ApiVersionAttribute>(true)
                        .SelectMany(attr => attr.Versions);

                    return versions.Any(v => $"v{v.ToString()}" == docName);
                });
                #endregion

                #endregion
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
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
               // options.SwaggerEndpoint("/swagger/v2/swagger.json", "V2 Docs");

                #region Customizing
                //// Display
                options.DocExpansion(DocExpansion.None);
                #endregion
            });

            //ReDoc UI middleware. ReDoc UI is an alternative to swagger-ui
            app.UseReDoc(options =>
            {
                options.SpecUrl("/swagger/v1/swagger.json");
                //options.SpecUrl("/swagger/v2/swagger.json");

                #region Customizing
                //By default, the ReDoc UI will be exposed at "/api-docs"
                //options.RoutePrefix = "docs";
                //options.DocumentTitle = "My API Docs";

                options.EnableUntrustedSpec();
                options.ScrollYOffset(10);
                options.HideHostname();
                options.HideDownloadButton();
                options.ExpandResponses("200,201");
                options.RequiredPropsFirst();
                options.NoAutoAuth();
                options.PathInMiddlePanel();
                options.HideLoading();
                options.NativeScrollbars();
                options.DisableSearch();
                options.OnlyRequiredInSamples();
                options.SortPropsAlphabetically();
                #endregion
            });
        }
    }
}
