using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DotNetEnv;
using YawShop.Services.StockService;
using YawShop.Services.DiscountService;
using YawShop.Services.ProductService;
using YawShop.Services.CheckoutService;
using YawShop.Services.EmailService;
using YawShop.Services.ClientService;
using YawShop.Services.PaymentService;
using YawShop.Services.EventService;
using YawShop.Services.PaytrailService;
using YawShop.Services.GiftcardService;
using YawShop.Utilities;
using YawShop.Utilities.CheckoutCleaner;

namespace YawShop
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                WebRootPath = "Frontend/dist/",
            });

            builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

            if (builder.Environment.IsDevelopment())
            {
                // loads the local .env file to net core if in localhost development
                Env.Load();
            }
            else
            {
                builder.WebHost.UseUrls("http://0.0.0.0:5000");
            }

            builder.Services.AddDbContext<ApplicationDbContext>();

            builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
            {
                options.Password.RequireDigit = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "auth_cookie";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            builder.Services.AddAuthorization();

            builder.Services.AddControllers();

            builder.Services.Configure<SmtpSettings>(options =>
            {
                try
                {
                    options.Host = EnvVariableReader.GetVariable("SMTP_HOST");
                    options.Port = EnvVariableReader.GetVariableAsInt("SMTP_PORT");
                    options.Username = EnvVariableReader.GetVariable("SMTP_USERNAME");
                    options.Password = EnvVariableReader.GetVariable("SMTP_PASSWORD");
                    options.SenderEmail = EnvVariableReader.GetVariable("SMTP_SENDER_EMAIL");
                    options.SenderName = EnvVariableReader.GetVariable("SMTP_SENDER_NAME");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            });

            builder.Services.Configure<PaytrailSettings>(options =>
            {
                try
                {
                    options.Account = EnvVariableReader.GetVariable("PAYTRAIL_ACCOUNT");
                    options.Secret = EnvVariableReader.GetVariable("PAYTRAIL_SECRET");
                    options.RedirectSuccess = EnvVariableReader.GetVariable("PAYTRAIL_REDIRECT_SUCCESS");
                    options.RedirectCancel = EnvVariableReader.GetVariable("PAYTRAIL_REDIRECT_CANCEL");
                    options.CallbackSuccess = EnvVariableReader.GetVariable("PAYTRAIL_CALLBACK_SUCCESS");
                    options.CallbackCancel = EnvVariableReader.GetVariable("PAYTRAIL_CALLBACK_CANCEL");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            });


            builder.Services.AddHostedService<ConsumeHostedServices>();
            builder.Services.AddScoped<ICleanerTimerService, CleanerTimerService>(); // used by ConsumeHostService to clean up automatically more than 10 minutes old floating checkouts
            builder.Services.AddTransient<IEmailSender<IdentityUser>, Emailer>();
            builder.Services.AddScoped<IEmailer, Emailer>();
            builder.Services.AddScoped<IGiftcardService, GiftcardService>();
            builder.Services.AddScoped<IDiscountService, DiscountService>();
            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IPaymentService, PaytrailService>();
            builder.Services.AddScoped<ICheckoutService, CheckoutService>();
            builder.Services.AddScoped<IStockService, StockService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                policy =>
                                {
                                    policy.WithOrigins("http://localhost:5132", "http://localhost:5173", "https://dev.kauppa.klu.fi").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                                });
            });

            //Set slack notifications based on environment
            var isLocalhost = Environment.GetEnvironmentVariable("LOCALHOST");

            if (!String.Equals(isLocalhost, "true", StringComparison.InvariantCultureIgnoreCase))
            {
                builder.Logging.AddProvider(new SlackLoggerProvider());
            }

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                    await SeedDefaultUserAsync(userManager);

                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    if (context.Database.HasPendingModelChanges())
                    {
                        throw new InvalidOperationException("The database model has pending changes that need to be added to a migration.");
                    }

                }
                catch (System.Exception)
                {
                    throw;
                }

            }

            app.MapGroup("/api/v1/auth")
            .MapIdentityApi<IdentityUser>()
            .RequireAuthorization();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(MyAllowSpecificOrigins);

            app.UseStaticFiles();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Append("Content-Security-Policy",
                    "default-src 'self'; " +
                    "script-src 'self' *.tinymce.com *.tiny.cloud; " +
                    "connect-src 'self' *.tinymce.com *.tiny.cloud blob:; " +
                    "img-src 'self' *.tinymce.com *.tiny.cloud data: blob:; " +
                    "style-src 'self' 'unsafe-inline' *.tinymce.com *.tiny.cloud; " + // Adjust this to use a nonce or hash if possible
                    "font-src 'self' *.tinymce.com *.tiny.cloud https://fonts.googleapis.com https://fonts.gstatic.com data:; " +
                    "frame-ancestors 'self'; " +
                    "form-action 'self';");
                await next();
            });

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers().RequireAuthorization();

            app.MapFallbackToFile("index.html");

            app.Run();

            //Remove this debug only
            async Task SeedDefaultUserAsync(UserManager<IdentityUser> userManager)
            {
                var defaultUserEmail = EnvVariableReader.GetVariable("YAWSHOP_DEFAULT_USER");
                var defaultPassword = EnvVariableReader.GetVariable("YAWSHOP_DEFAULT_PASSWORD");

                // Check if the default user already exists
                if (await userManager.FindByEmailAsync(defaultUserEmail) == null)
                {
                    var user = new IdentityUser
                    {
                        UserName = defaultUserEmail,
                        Email = defaultUserEmail,
                        EmailConfirmed = true
                    };

                    // Create the user
                    var result = await userManager.CreateAsync(user, defaultPassword);

                }
            }
        }
    }
}
