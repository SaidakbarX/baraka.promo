using baraka.promo.Areas.Identity;
using baraka.promo.BackgroundService.BackgroundModels;
using baraka.promo.Core;
using baraka.promo.Data;
using baraka.promo.Delivery;
using baraka.promo.Models;
using baraka.promo.Models.CardPaymentModels;
using baraka.promo.Models.ClickModels;
using baraka.promo.Models.MindBoxModels;
using baraka.promo.Models.PaymeModels;
using baraka.promo.Models.PushModels;
using baraka.promo.Pages.Segments;
using baraka.promo.Pages.TgPushSender;
using baraka.promo.Pages.Users;
using baraka.promo.Services;
using baraka.promo.Services.HelperServices;
using baraka.promo.Services.Loyality;
using baraka.promo.Services.PromGroup;
using baraka.promo.Services.Promo;
using BlazorDownloadFile;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Radzen;
using Resident.Monitoring.BackgroundModels;
using Resident.Monitoring.Settings;
using Serilog;
using System.Reflection;

namespace baraka.promo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //builder.Services.AddRazorComponents().AddInteractiveServerComponents();

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));



            builder.Services.AddScoped<Func<ApplicationDbContext>>(provider =>
            () => provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

            var deliveryConnection = builder.Configuration.GetConnectionString("DeliveryConnection");

            builder.Services.AddDbContext<DeliveryDbContext>(options => options.UseSqlServer(deliveryConnection),
                                ServiceLifetime.Scoped);

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;


                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;

            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager<SignInManager<IdentityUser>>();

            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddRadzenComponents();
            builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            builder.Services.AddScoped<ICurrentUser, CurrentUserService>();
            builder.Services.AddScoped<PushService>();
            builder.Services.AddScoped<PaymeService>();
            builder.Services.AddScoped<ClickService>();
            builder.Services.AddScoped<MindBoxService>();
            builder.Services.AddScoped<CardPaymentService>();
            builder.Services.AddScoped<SubscriptionService>();
            builder.Services.AddScoped<LoyalitySettingService>();


            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddTransient<UserAddModel>();
            builder.Services.AddTransient<CheckSegment>();
            builder.Services.AddTransient<SegmentUsers>();
            builder.Services.AddTransient<ToExport>();
            builder.Services.AddTransient<TgHelper>();

            builder.Services.AddSingleton<TasksToRun, TasksToRun>();
            builder.Services.AddHostedService<MainBackgroundTaskService>();
            builder.Services.AddTransient<IWriterService, WriterService>();

            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<MyCacheService>();


            //Front Services 
            builder.Services.AddScoped<MyNotificationService>();
            builder.Services.AddScoped<LoyalityCardHolderService>();
            builder.Services.AddScoped<TransactionService>();
            builder.Services.AddScoped<PromoGroupService>();
            builder.Services.AddScoped<PromoService>();

            builder.Services.AddOptions();
            builder.Services.Configure<PushSettingsModel>(builder.Configuration.GetSection("PushSettings"));
            builder.Services.Configure<ProjectSettingsModel>(builder.Configuration.GetSection("ProjectSettings"));
            builder.Services.Configure<PaymeSettingsModel>(builder.Configuration.GetSection("PaymeSettings"));
            builder.Services.Configure<ClickSettingsModel>(builder.Configuration.GetSection("ClickSettings"));
            builder.Services.Configure<MindBoxSettings>(builder.Configuration.GetSection("MindBoxSettings"));
            builder.Services.Configure<BarakaSettingsModel>(builder.Configuration.GetSection("BarakaSettings"));
            builder.Services.Configure<CardPaymentSettings>(builder.Configuration.GetSection("CardPaymentSettings"));


            builder.Services.AddBlazorDownloadFile();

            builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue; // Allow unlimited value count
                options.ValueLengthLimit = int.MaxValue; // Allow unlimited length of each value
                options.MultipartBodyLengthLimit = long.MaxValue; // Allow unlimited multipart body length
            });

            builder.Services.AddHttpClient();

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

            builder.Services.AddAuthentication();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "PROMO",
                    Version = "v1",
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                });

                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "API ключ в формате: {ключ}",
                    Name = "X-API-KEY",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKey"
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
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            },
                            Scheme = "ApiKey",
                            Name = "X-API-KEY",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    },
                });

                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);

                c.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                    {
                        return new[] { api.GroupName };
                    }

                    var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                    if (controllerActionDescriptor != null)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });
                c.DocInclusionPredicate((name, api) => true);
            });

            builder.Services.ConfigureSwaggerGen(options =>
            {
                options.CustomSchemaIds(x => x.FullName);
            });

            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("promo_cors", b =>
                {
                    b.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                    //b.WithOrigins().AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
            });

            builder.Logging.AddSerilog();
            builder.Logging.AddFile("Logs/baraka.promo.txt", LogLevel.Warning);
            
            var app = builder.Build();

            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            

            app.UseSwagger();
            //app.UseSwaggerUI();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PROMO API V1");
                c.RoutePrefix = "swagger";
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("promo_cors");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");


            using (var scope = app.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                string constring = builder.Configuration.GetConnectionString("DefaultConnection");
                logger.LogInformation($"Db conn: {builder.Configuration.GetConnectionString("DefaultConnection")}");

                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                AccountDatabaseInitializer.Seed(userManager, roleManager);
            }

            app.Run();
        }
    }
}