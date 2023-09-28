﻿using Autofac;
using FirebaseAdmin;
using System.Reflection;
using ServiceStack.Redis;
using SupFAmof.API.Mapper;
using StackExchange.Redis;
using Reso.Core.Extension;
using SupFAmof.Data.Redis;
using SupFAmof.API.Helpers;
using SupFAmof.API.AppStart;
using Google.Apis.Auth.OAuth2;
using Microsoft.OpenApi.Models;
using SupFAmof.Data.Repository;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Service;
using SupFAmof.Data.MakeConnection;
using SupFAmof.Service.DTO.Request;
using Microsoft.Extensions.DependencyInjection;
using SupFAmof.Service.Service.ServiceInterface;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SupFAmof.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        [Obsolete]

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<MailPaths>(Configuration.GetSection("MailPaths"));
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder
                        //.WithOrigins(GetDomain())
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(Configuration["Endpoint:RedisEndpoint"]));
            services.AddMemoryCache();
            services.ConfigMemoryCacheAndRedisCache(Configuration["Endpoint:RedisEndpoint"]);
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SupFAmof API",
                    Version = "v1"
                });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer iJIUzI1NiIsInR5cCI6IkpXVCGlzIElzc2'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                        securitySchema,
                    new string[] { "Bearer" }
                    }
                });
                // Các cài đặt Swagger khác
                c.EnableAnnotations();
            });
            services.ConfigureAuthServices(Configuration);
            services.ConnectToConnectionString(Configuration);

            #region Firebase
            var pathToKey = Path.Combine(Directory.GetCurrentDirectory(), "Keys", "firebase.json");
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(pathToKey)
            });
            #endregion

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac, like:
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();

            builder.RegisterType<RoleService>().As<IRoleService>();
            builder.RegisterType<AccountService>().As<IAccountService>();
            builder.RegisterType<AccountBankingService>().As<IAccountBankingService>();
            builder.RegisterType<FcmTokenService>().As<IFcmTokenService>();
            builder.RegisterType<FirebaseMessagingService>().As<IFirebaseMessagingService>();
            builder.RegisterType<StaffService>().As<IStaffService>();
            builder.RegisterType<PostRegistrationService>().As<IPostRegistrationService>();
            builder.RegisterType<PostCategoryService>().As<IPostCategoryService>();
            builder.RegisterType<TrainingCertificateService>().As<ITrainingCertificateService>();
            builder.RegisterType<AccountCertificateService>().As<IAccountCertificateService>();
            builder.RegisterType<PostService>().As<IPostService>();
            builder.RegisterType<DocumentService>().As<IDocumentService>();
            builder.RegisterType<SendMailService>().As<ISendMailService>();
            builder.RegisterType<CheckInService>().As<ICheckInService>();




            //Dependency injection for redis
            builder.RegisterType<Redis>().As<Data.Redis.IRedis>().SingleInstance();

            builder.Register<IRedisClientsManager>(c =>
            new RedisManagerPool(Configuration.GetConnectionString("RedisConnectionString")));

            builder.Register(c => c.Resolve<IRedisClientsManager>().GetCacheClient());

            builder.RegisterGeneric(typeof(GenericRepository<>))
            .As(typeof(IGenericRepository<>))
            .InstancePerLifetimeScope();
        }

        public void Configure(IApplicationBuilder app)
        {
            //app.ConfigMigration<>();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseExceptionHandler("/error");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SupFAmof V2");
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);

                // Cài đặt để kích hoạt nút "Copy URL"
                //c.EnableRequestValidation();
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDeveloperExceptionPage();
            AuthConfig.Configure(app);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
