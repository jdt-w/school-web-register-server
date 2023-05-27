using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolWebRegister.DAL;
using SchoolWebRegister.DAL.Repositories;
using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services;
using SchoolWebRegister.Services.Authentication;
using SchoolWebRegister.Services.Authentication.JWT;
using SchoolWebRegister.Services.Courses;
using SchoolWebRegister.Services.Logging;
using SchoolWebRegister.Services.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.Bind("CompanySettings", new CompanySettings());

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, options => options.MigrationsAssembly("SchoolWebRegister.DAL"));
});
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.User.AllowedUserNameCharacters =
         "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.LoginPath = "/users/login";
        options.AccessDeniedPath = "/users/access-denied";
        options.SlidingExpiration = true;
        options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero,
            RequireSignedTokens = true,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"]
        };
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole(nameof(UserRole.Administrator)));
    options.AddPolicy("AllUsers", policy =>
    {
        policy.RequireAssertion(async handler =>
        {
            HttpContext? httpContext = handler.Resource as HttpContext;
            var service = builder.Services.BuildServiceProvider().GetService<IAuthenticationService>();
            var executor = httpContext.RequestServices.GetRequiredService<IHttpContextAccessor>();
            string? accessToken = httpContext?.Request.Cookies["accessToken"];
            string? refreshToken = httpContext?.Request.Cookies["refreshToken"];

            var result = await service.Authenticate(accessToken, refreshToken);

            if (result is OkObjectResult)
            {                
                return true;
            }
            else
            {
                var response = new BaseResponse(
                    code: StatusCode.Error,
                    error: new ErrorType
                    {
                        Message = "Invalid Tokens",
                        Type = new string[] { "AUTH_NOT_ALLOWED", "INVALID_TOKENS" }
                    }
                );
                string jsonString = JsonSerializer.Serialize(response);
                byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
                service.AppendInvalidCookies();
                httpContext.Response.OnStarting(() =>
                {
                    executor.HttpContext.Response.ContentType = "application/json";
                    executor.HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                    return Task.CompletedTask;
                });
                return false;
            }
        });
    });
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IAuthenticationService, JWTAuthenticationService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddSingleton<IPasswordValidator, PasswordValidator>();
builder.Services.AddScoped<ILoggingService, LoggingService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.AddTransient<IAuthorizationHandler, JWTAuthorizationFilter>();

var app = builder.Build();

var service = builder.Services.BuildServiceProvider().GetRequiredService<IUserService>();

var provider = builder.Services.BuildServiceProvider();
//service.DeleteUser("60b34ee2-846d-4db6-8ac3-4c903d7642b3");
//SchoolWebRegister.Tests.Helpers.DatabaseSeeder.GenerateTeacher(provider);
//SchoolWebRegister.Tests.Helpers.DatabaseSeeder.GenerateStudent(provider);
//SchoolWebRegister.Tests.Helpers.DatabaseSeeder.GenerateAdmin(provider);


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{ 
    app.UseExceptionHandler("/Home/Error"); 
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(options => options.AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());
app.UseRouting();
app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always,
});

app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets();
app.MapAreaControllerRoute(
    name: "AdminArea",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "UsersArea",
    areaName: "Users",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default", 
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();