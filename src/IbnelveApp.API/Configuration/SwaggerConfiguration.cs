using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IbnelveApp.API.Configuration
{
    public static class SwaggerConfiguration
    {
        public static void AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "IbnelveApp API",
                    Version = "v1",
                    Description = "API para gerenciamento de equipamentos com autenticação JWT",
                    Contact = new OpenApiContact
                    {
                        Name = "IbnelveApp Team",
                        Email = "contato@ibnelveapp.com"
                    }
                });

                // Configuração para JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Autorização JWT usando o esquema Bearer. \r\n\r\n" +
                                  "Digite 'Bearer' [espaço] e então seu token na entrada de texto abaixo.\r\n\r\n" +
                                  "Exemplo: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                // Incluir comentários XML se existirem
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                // Configurações adicionais
                c.EnableAnnotations();
                c.DescribeAllParametersInCamelCase();
            });
        }

        public static void UseSwaggerWithJwt(this WebApplication app)
        {
            if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IbnelveApp API v1");
                    c.RoutePrefix = "swagger";
                    c.DocumentTitle = "IbnelveApp API Documentation";

                    // Configurações de UI
                    c.DefaultModelsExpandDepth(-1);
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                    c.EnableDeepLinking();
                    c.DisplayOperationId();
                    c.DisplayRequestDuration();

                    // Configuração para persistir autorização
                    c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
                });
            }
        }
    }
}

