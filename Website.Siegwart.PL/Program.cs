using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System.Threading.RateLimiting;
using Serilog;
using System.IO.Compression;
using System.Reflection;
using Website.Siegwart.BLL.Mappings;
using Website.Siegwart.BLL.Profiles;
using Website.Siegwart.BLL.Services.Classes;
using Website.Siegwart.BLL.Services.Interfaces;
using Website.Siegwart.DAL.Data.Configurations;
using Website.Siegwart.DAL.Data.Contexts;
using Website.Siegwart.DAL.Repositories.Classes;
using Website.Siegwart.DAL.Repositories.Interfaces;
using Website.Siegwart.PL.Data;
using Website.Siegwart.PL.Helper;
using Website.Siegwart.PL.Services;

// Resolve SameSiteMode ambiguity
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace Website.Siegwart.PL;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ========== Configure Logging (Serilog) ==========
        ConfigureLogging(builder);

        // ========== Add Services ==========
        ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

        // ========== Build Application ==========
        var app = builder.Build();

        // ========== Configure Middleware Pipeline ==========
        ConfigureMiddleware(app);

        // ========== Seed Database (async) ==========
        await SeedDatabaseAsync(app);

        await app.RunAsync();
    }

    #region Logging Configuration
    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        var environment = builder.Environment.EnvironmentName;

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Environment", environment)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "Logs/app-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                fileSizeLimitBytes: 10_000_000,
                rollOnFileSizeLimit: true,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        builder.Host.UseSerilog();

        Log.Information("========================================");
        Log.Information("✓ Application Starting");
        Log.Information("Environment: {Environment}", environment);
        Log.Information("========================================");
    }
    #endregion

    #region Services Configuration
    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // MVC & Views
        ConfigureMvc(services);

        // Session
        ConfigureSession(services);

        // Caching
        ConfigureCaching(services);

        // Compression
        ConfigureCompression(services);

        // Database
        ConfigureDatabase(services, configuration, environment);

        // Identity & Authentication (use IdentityUser)
        ConfigureIdentity(services, configuration);

        // Repositories (DAL)
        ConfigureRepositories(services);

        // Business Services (BLL / PL)
        ConfigureBusinessServices(services, configuration);

        // Localization
        ConfigureLocalization(services);

        // AutoMapper
        ConfigureAutoMapper(services, environment);

        // Security
        ConfigureSecurity(services);

        // Rate Limiting
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("contact", opt =>
            {
                opt.PermitLimit = 5;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            });
            options.RejectionStatusCode = 429;
        });

        Log.Information("✓ All services configured successfully");
    }

    private static void ConfigureMvc(IServiceCollection services)
    {
        services.AddControllersWithViews(options =>
        {
            options.CacheProfiles.Add("Default30", new CacheProfile
            {
                Duration = 1800,
                Location = ResponseCacheLocation.Any,
                VaryByHeader = "Accept-Language",
                VaryByQueryKeys = new[] { "*" }
            });

            options.CacheProfiles.Add("Short5", new CacheProfile
            {
                Duration = 300,
                Location = ResponseCacheLocation.Any,
                VaryByHeader = "Accept-Language"
            });

            options.CacheProfiles.Add("Never", new CacheProfile
            {
                Duration = 0,
                Location = ResponseCacheLocation.None,
                NoStore = true
            });
        });

        Log.Information("✓ MVC configured with cache profiles");
    }

    private static void ConfigureSession(IServiceCollection services)
    {
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.Name = "Siegwart.Session";
        });

        Log.Information("✓ Session configured");
    }

    private static void ConfigureCaching(IServiceCollection services)
    {
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 1024;
            options.CompactionPercentage = 0.25;
            options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
        });

        services.AddResponseCaching(options =>
        {
            options.MaximumBodySize = 1024 * 1024 * 10;
            options.UseCaseSensitivePaths = false;
        });

        Log.Information("✓ Caching configured");
    }

    private static void ConfigureCompression(IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();

            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
            {
                "image/svg+xml",
                "application/font-woff2",
                "application/javascript",
                "text/javascript",
                "text/css",
                "text/html",
                "text/json",
                "application/json"
            });
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        Log.Information("✓ Compression configured (Brotli + Gzip)");
    }

    private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);

                    sqlOptions.CommandTimeout(15);
                    sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    sqlOptions.MaxBatchSize(100);
                });

            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        services.AddHttpContextAccessor();

        Log.Information("✓ Database configured");
    }

    private static void ConfigureIdentity(IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredUniqueChars = 4;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

            options.SignIn.RequireConfirmedEmail = configuration.GetValue<bool>("Identity:RequireConfirmedEmail", false);
            options.SignIn.RequireConfirmedPhoneNumber = configuration.GetValue<bool>("Identity:RequireConfirmedPhoneNumber", false);
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.Configure<DataProtectionTokenProviderOptions>(opt =>
        {
            opt.TokenLifespan = TimeSpan.FromHours(configuration.GetValue<double>("Identity:TokenLifespanHours", 3));
        });

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/SignOut";
            options.AccessDeniedPath = "/Account/AccessDenied";

            options.ExpireTimeSpan = TimeSpan.FromHours(configuration.GetValue<double>("Identity:CookieExpireHours", 2));
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.Name = configuration.GetValue<string>("Identity:CookieName", "Siegwart.Auth");

            options.ReturnUrlParameter = "returnUrl";
        });

        services.Configure<SecurityStampValidatorOptions>(opts =>
        {
            opts.ValidationInterval = TimeSpan.FromMinutes(configuration.GetValue<double>("Identity:SecurityStampValidationMinutes", 30));
        });

        Log.Information("✓ Identity & Authentication configured");
    }

    private static void ConfigureRepositories(IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<INewsRepository, NewsRepository>();
        services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
        services.AddScoped<IVideoMediaRepository, VideoMediaRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        Log.Information("✓ Repositories registered");
    }

    private static void ConfigureBusinessServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<ITeamMemberService, TeamMemberService>();
        services.AddScoped<IAttachmentService, AttachmentService>();
        services.AddScoped<IVideoMediaService, VideoMediaService>();

        services.AddScoped<IUserProductService, UserProductService>();
        services.AddScoped<IUserNewsService, UserNewsService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IUserTeamMemberService, UserTeamMemberService>();

        // FIX: read Smtp settings from section "Smtp" (matches appsettings.json)
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));

        // Log SMTP config (without password) to help diagnostics
        try
        {
            var smtpConfig = configuration.GetSection("Smtp").Get<SmtpSettings>();
            Log.Information("SMTP config read: Host={Host}, Port={Port}, EnableSsl={EnableSsl}, FromEmail={FromEmail}, UserName={UserName}",
                smtpConfig?.Host, smtpConfig?.Port, smtpConfig?.EnableSsl, smtpConfig?.FromEmail, smtpConfig?.UserName);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to read SMTP configuration during startup.");
        }

        // Register email sender (use the MailKit-based sender in BLL)
        services.AddTransient<IAppEmailSender, SmtpEmailSender>();

        services.AddSingleton<IAuditService, AuditService>();

        Log.Information("✓ Business services registered");
    }

    private static void ConfigureLocalization(IServiceCollection services)
    {
        services.AddSiegwartLanguage(options =>
        {
            options.DefaultLanguage = "en";
            options.SupportedLanguages = new[] { "en", "ar" };
            options.QueryStringKey = "lang";
            options.CookieName = "siegwart.lang";
        });

        services.AddSingleton<IStaticTextService, StaticTextService>();

        services.AddLocalization(options => options.ResourcesPath = "Resources");

        Log.Information("✓ Localization configured (EN/AR)");
    }

    private static void ConfigureAutoMapper(IServiceCollection services, IWebHostEnvironment environment)
    {
        var mapperConfig = new AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.AddMaps(Assembly.GetExecutingAssembly(), typeof(ProductProfile).Assembly);
        });

        if (environment.IsDevelopment())
        {
            try
            {
                mapperConfig.AssertConfigurationIsValid();
                Log.Information("✓ AutoMapper configuration validated");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "✗ AutoMapper configuration validation failed");
            }
        }

        services.AddSingleton(mapperConfig.CreateMapper());

        Log.Information("✓ AutoMapper configured");
    }

    private static void ConfigureSecurity(IServiceCollection services)
    {
        services.AddHsts(options =>
        {
            options.MaxAge = TimeSpan.FromDays(365);
            options.IncludeSubDomains = true;
            options.Preload = true;
        });

        Log.Information("✓ Security configured (HSTS)");
    }
    #endregion

    #region Middleware Configuration
    private static void ConfigureMiddleware(WebApplication app)
    {
        var environment = app.Environment;

        ConfigureRequestLogging(app, environment);
        ConfigureErrorHandling(app, environment);
        ConfigureSecurityHeaders(app, environment);
        ConfigureStaticFiles(app);

        app.UseRouting();

        app.UseResponseCompression();
        app.UseResponseCaching();

        ConfigureCacheControl(app);

        app.UseSession();

        ConfigureRequestLocalization(app);

        app.UseAuthentication();
        app.UseRateLimiter();
        app.UseAuthorization();

        ConfigureRoutes(app);

        Log.Information("✓ Middleware pipeline configured");
    }

    private static void ConfigureRequestLogging(WebApplication app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
                };
            });
        }
        else
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.GetLevel = (httpContext, elapsed, ex) =>
                    ex != null ? Serilog.Events.LogEventLevel.Error :
                    elapsed > 2000 ? Serilog.Events.LogEventLevel.Warning :
                    Serilog.Events.LogEventLevel.Information;
            });
        }
    }

    private static void ConfigureErrorHandling(WebApplication app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
            app.UseStatusCodePagesWithReExecute("/Home/NotFound");
        }
    }

    private static void ConfigureSecurityHeaders(WebApplication app, IWebHostEnvironment environment)
    {
        app.Use(async (context, next) =>
        {
            var headers = context.Response.Headers;

            headers.Add("X-Content-Type-Options", "nosniff");
            headers.Add("X-Frame-Options", "SAMEORIGIN");
            headers.Add("X-XSS-Protection", "1; mode=block");
            headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

            headers.Add("X-DNS-Prefetch-Control", "on");

            headers.Add("Content-Security-Policy",
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://unpkg.com; " +
                "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://unpkg.com; " +
                "font-src 'self' https://fonts.gstatic.com https://cdnjs.cloudflare.com; " +
                "img-src 'self' data: https://www.google.com https://maps.googleapis.com https://maps.gstatic.com; " +
                "frame-src https://www.google.com; " +
                "connect-src 'self'; " +
                "object-src 'none'; " +
                "base-uri 'self'; " +
                "form-action 'self';");

            if (!environment.IsDevelopment())
            {
                headers.Add("Link",
                    "<https://unpkg.com>; rel=preconnect; crossorigin, " +
                    "<https://cdnjs.cloudflare.com>; rel=preconnect; crossorigin");
            }

            await next();
        });
    }

    private static void ConfigureStaticFiles(WebApplication app)
    {
        app.UseHttpsRedirection();

        var staticFileOptions = new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                var headers = ctx.Context.Response.GetTypedHeaders();
                var fileExtension = Path.GetExtension(ctx.File.Name).ToLowerInvariant();

                var cacheableExtensions = new[] { ".css", ".js", ".jpg", ".jpeg", ".png", ".gif", ".svg", ".woff", ".woff2", ".ttf", ".eot", ".ico" };

                if (cacheableExtensions.Contains(fileExtension))
                {
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromDays(365)
                    };
                }
                else
                {
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromDays(7)
                    };
                }
            }
        };

        app.UseStaticFiles(staticFileOptions);
    }

    private static void ConfigureCacheControl(WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            // Skip cache-control header injection for static file requests
            var path = context.Request.Path.Value ?? "";
            var isStaticFile = path.StartsWith("/css/", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("/js/", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("/lib/", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("/Resources/", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("/images/", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("/fonts/", StringComparison.OrdinalIgnoreCase);

            if (isStaticFile)
            {
                await next();
                return;
            }

            var headers = context.Response.GetTypedHeaders();

            var isAdminOrAccount = path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("/SuperAdmin", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("/Account", StringComparison.OrdinalIgnoreCase);

            if (isAdminOrAccount)
            {
                headers.CacheControl = new CacheControlHeaderValue
                {
                    NoStore = true,
                    NoCache = true
                };
            }
            else
            {
                headers.CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(300),
                    MustRevalidate = true
                };
            }

            context.Response.Headers[HeaderNames.Vary] = new string[] { "Accept-Encoding", "Accept-Language" };

            await next();
        });
    }

    private static void ConfigureRequestLocalization(WebApplication app)
    {
        app.UseSiegwartLanguage();

        var supportedCultures = new[] { "en", "ar" };
        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture("en")
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        app.UseRequestLocalization(localizationOptions);
    }

    private static void ConfigureRoutes(WebApplication app)
    {
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        Log.Information("✓ Routes configured");
        Log.Information("  → Default route: {controller}/{action}/{id?}");
        Log.Information("  → Use [Route] attributes in controllers for custom paths");
    }
    #endregion

    #region Database Seeding
    private static async Task SeedDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            Log.Information("========================================");
            Log.Information("✓ Starting Database Seeding...");
            Log.Information("========================================");

            var configuration = services.GetRequiredService<IConfiguration>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("SeedData");

            // Call async initializer (SeedData.InitializeAsync uses IdentityUser)
            await SeedData.InitializeAsync(services, configuration, logger);

            Log.Information("========================================");
            Log.Information("✓ Database Seeding Completed");
            Log.Information("========================================");
        }
        catch (Exception ex)
        {
            Log.Error("========================================");
            Log.Error(ex, "✗ Error occurred during database seeding");
            Log.Error("========================================");

            if (app.Environment.IsDevelopment())
            {
                throw;
            }
        }
    }
    #endregion
}