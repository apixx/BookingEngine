using BookingEngine.BusinessLogic.Config;
using BookingEngine.BusinessLogic.Mapping;
using BookingEngine.BusinessLogic.Services;
using BookingEngine.BusinessLogic.Services.Interfaces;
using BookingEngine.Data;
using BookingEngine.Data.ConfigAppSettings;
using BookingEngine.Data.Configuration;
using BookingEngine.Data.Repositories;
using BookingEngine.Data.Repositories.Interfaces;
using BookingEngine.Entities.Models.Authentication;
using BookingEngine.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Options;

namespace BookingEngine
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

            // DB context
            var settings = new AppConfiguration();
            services.AddDbContextPool<DatabaseContext>(options => options.UseSqlServer(settings.sqlConnectionString));

            services.Configure<AmadeusClientOptions>(Configuration.GetSection("AmadeusClientOptions"));
            services.Configure<DatabaseOptions>(Configuration.GetSection("DatabaseOptions"));

            // The base extension method registers IHttpClientFactory
            services.AddHttpClient();
            services.AddSingleton<IAmadeusTokenService, AmadeusTokenService>();

            //services.AddHttpClient<IAmadeusApiHotelRoomsServiceProvider, AmadeusApiHotelRoomsServiceProvider>((serviceProvider, cfg) =>
            //{
            //    var amadeusClientOptions = serviceProvider.GetRequiredService<IOptions<AmadeusClientOptions>>();
            //    cfg.BaseAddress = new Uri(amadeusClientOptions.Value.Url);
            //});

            services.AddHttpClient<IAmadeusApiServiceProvider, AmadeusApiServiceProvider>((serviceProvider, cfg) =>
              {
                  var amadeusClientOptions = serviceProvider.GetRequiredService<IOptions<AmadeusClientOptions>>();
                  cfg.BaseAddress = new Uri(amadeusClientOptions.Value.Url);
              });


            // for identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();

            // for authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // jwt bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["JWT:ValidAudience"],
                    ValidIssuer = Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                };
            });

            services.AddAutoMapper(typeof(MappingProfile));

            //services.Configure<AmadeusClientOptions>(Configuration.GetSection("ApiKeys"));

            services.AddHttpClient("TravelAPI", client =>
            {
                client.BaseAddress = new Uri("https://test.api.amadeus.com");
            });


            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "AmadeusHotels.API",
                    Description = "Amadeus Hotels API Integration by Angel Donev"

                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSingleton<MyMemoryCache>();
            services.AddScoped<IHotelsSearchService, HotelsSearchService>();
            services.AddScoped<IAmadeusApiHotelRoomsServiceProvider, AmadeusApiHotelRoomsServiceProvider>();
            services.AddScoped<IProcessApiResponse, ProcessApiResponse>();
            services.AddScoped<ISearchRequestRepository, SearchRequestRepository>();
            services.AddScoped<ISearchRequestHotelRepository, SearchRequestHotelRepository>();
            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // TODO: dodadeno da se zastitime od cyclic reference koga se pravi serijalizicija vo json
            // primer bus lanes sodrzi city a toj sodrzi buslanes, pa json znae da vleze vo endless loop
            services.AddControllers().AddNewtonsoftJson(x =>
                x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // TODO: dodadeno za da update na DB avtomatski na start
            // ensure migration of context
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("../swagger/v1/swagger.json", "Amadeus Hotels API Integration");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Amadeus Hotels API Integration");
            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}