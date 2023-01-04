using Employee_Api.Applicationdbcontext;
using Employee_Api.Identity;
using Employee_Api.ServiceContract;
using Employee_Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public  void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlServer().AddDbContext<ApplicationDbContext>
                (options => options.UseSqlServer(Configuration.GetConnectionString("constr"),
                b => b.MigrationsAssembly("Employee_Api")));
            //identity framwork DI
            services.AddTransient<IRoleStore<ApplicationRole>, ApplicationRoleStore>();
            services.AddTransient<UserManager<ApplicationUser>, ApplicationUserManager>();
            services.AddTransient<SignInManager<ApplicationUser>, ApplicationSignInManager>();
            services.AddTransient<RoleManager<ApplicationRole>, ApplicationRoleManager>();
            services.AddTransient<IUserStore<ApplicationUser>, ApplicationUserStore>();
            services.AddTransient<IUserService, UserService>();
            services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<DbContext>()
            .AddUserStore<ApplicationUserStore>()
            .AddUserManager<ApplicationUserManager>()
            .AddRoleManager<ApplicationRoleManager>()
            .AddSignInManager<ApplicationSignInManager>()
            .AddRoleStore<ApplicationRoleStore>()
            .AddDefaultTokenProviders();
            services.AddScoped<ApplicationRoleStore>();
            services.AddScoped<ApplicationUserStore>();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddControllers();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Employee_Api", Version = "v1" });
            });
              //JWT Authentication
            var appSettingSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingSection);
            var appSettings = appSettingSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(X =>
            {
                X.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                X.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(X =>
            {
                X.RequireHttpsMetadata = false;
                X.SaveToken = true; X.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public  void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee_Api v1"));
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

          //  Data
          //IServiceScopeFactory serviceScopeFactory = app.ApplicationServices.
          //GetRequiredService<IServiceScopeFactory>();
          //  using (IServiceScope scope = serviceScopeFactory.CreateScope())
          //  {
          //      var roleManager = scope.ServiceProvider.GetRequiredService
          //          <RoleManager<ApplicationRole>>();
          //      var userManager = scope.ServiceProvider.GetRequiredService
          //          <UserManager<ApplicationUser>>();
          //      //   Create Admin Role
          //      if (!await roleManager.RoleExistsAsync("Admin"))
          //      {
          //          var role = new ApplicationRole();
          //          role.Name = "Admin";
          //          await roleManager.CreateAsync(role);
          //      }


          //      //Create Employee Role
          //      if (!await roleManager.RoleExistsAsync("Employee"))
          //      {
          //          var role = new ApplicationRole();
          //          role.Name = "Employee";
          //          await roleManager.CreateAsync(role);
          //      }


          //      //Create Admin User

          //      if (await userManager.FindByNameAsync("admin") == null)
          //      {
          //          var user = new ApplicationUser();
          //          user.UserName = "admin";
          //          user.Email = "admin@gmail.com";
          //          var userPassword = "admin@123";
          //          var chkuser = await userManager.CreateAsync(user, userPassword);
          //          if (chkuser.Succeeded)
          //          {
          //              await userManager.AddToRoleAsync(user, "Admin");
          //          }
          //      }

          //      //Create Employee User

          //      if (await userManager.FindByNameAsync("employee") == null)
          //      {
          //          var user = new ApplicationUser();
          //          user.UserName = "employee";
          //          user.Email = "employee@gmail.com";
          //          var userPassword = "Admin@123";
          //          var chkuser = await userManager.CreateAsync(user, userPassword);
          //          if (chkuser.Succeeded)
          //          {
          //              await userManager.AddToRoleAsync(user, "Employee");
          //          }
          //      }
          //  }

        app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
