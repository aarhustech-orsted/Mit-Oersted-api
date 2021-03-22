using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mit_Oersted.Domain.CommandHandler;
using Mit_Oersted.Domain.Entities;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.Domain.Messaging;
using Mit_Oersted.Domain.Models;
using Mit_Oersted.Domain.Repository;
using Mit_Oersted.Domain.Repository.Implementations;
using Mit_Oersted.WebApi.Mappers;
using Mit_Oersted.WebApi.Models.Addresses;
using Mit_Oersted.WebApi.Models.Invoices;
using Mit_Oersted.WebApi.Models.Tokens;
using Mit_Oersted.WebApi.Models.Users;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Mit_Oersted.WebApi
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
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Configuration.GetSection("GOOGLE_APPLICATION_CREDENTIALS").Value);

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS") ?? "GOOGLE_APPLICATION_CREDENTIALS")
            });

            var webapidata = JsonSerializer.Deserialize<Webapidata>(File.ReadAllText(Configuration.GetSection("webapi").Value));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://securetoken.google.com/{webapidata.ProjectId}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{webapidata.ProjectId}",
                    ValidateAudience = true,
                    ValidAudience = $"{webapidata.ProjectId}",
                    ValidateLifetime = true
                };
            });

            services.AddScoped<DatabaseEntities>();

            services.AddScoped<IMessageBus, FakeBus>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            services.AddScoped<ICommandHandler, UserCommandHandler>();
            services.AddScoped<ICommandHandler, AddressCommandHandler>();
            services.AddScoped<ICommandHandler, InvoiceCommandHandler>();

            services.AddScoped<IMapper<UserModel, UserDto>, UserMapper>();
            services.AddScoped<IMapper<AddressModel, AddressDto>, AddressMapper>();
            services.AddScoped<IMapper<InvoiceModel, InvoiceDto>, InvoiceMapper>();

            services.AddScoped<IMapper<RefreshTokenResponseDto, TokenResponseBodyDto>, RefreshTokenMapper>();
            services.AddScoped<IMapper<SignInWithPhoneNumberResponseDto, TokenResponseBodyDto>, SignInWithPhoneNumberMapper>();

            services.AddControllers();

            services.AddCors(o => o.AddPolicy("AnyCors", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mit_Oersted", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mit_Oersted v1");
                c.RoutePrefix = string.Empty;
            });

            //app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();

            app.UseRouting();
            app.UseCors("AnyCors");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
