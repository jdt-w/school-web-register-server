using System.Text;
using HotChocolate;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution.Serialization;
using HotChocolate.Types.Pagination;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolWebRegister.DAL;
using SchoolWebRegister.DAL.Repositories;
using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Domain.Permissions;
using SchoolWebRegister.Services;
using SchoolWebRegister.Services.Authentication;
using SchoolWebRegister.Services.Authentication.JWT;
using SchoolWebRegister.Services.GraphQL;
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
        policy.RequireAssertion(handler =>
        {
            var service = builder.Services.BuildServiceProvider().GetService<IAuthenticationService>();
            string? accessToken = (handler.Resource as HttpContext)?.Request.Cookies["accessToken"];
            string? refreshToken = (handler.Resource as HttpContext)?.Request.Cookies["refreshToken"];
            var result = service.Authenticate(accessToken, refreshToken).Result;
            return result.StatusCode == StatusCode.Unauthorized ? false : true;
        });
    });
});

var options = new HttpResponseFormatterOptions
{
    Json = new JsonResultFormatterOptions
    {
        Indented = true,
        NullIgnoreCondition = JsonNullIgnoreCondition.All
    }
};

builder.Services.AddHttpResponseFormatter(options);
builder.Services
    .AddGraphQLServer()
    .SetPagingOptions(new PagingOptions
    {
        MaxPageSize = 100,
        IncludeTotalCount = true
    })
    .AddQueryType<UsersQueries>()
    .AddMutationType<Mutations>()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .AddType<BaseResponse>()
    .AddType<ApplicationUserType>()
    .AddAuthorizationHandler<JWTAuthorizationFilter>()
    .ModifyParserOptions(options => options.IncludeLocations = false)
    .ModifyRequestOptions(options =>
    { 
        options.IncludeExceptionDetails = false;
    });

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthenticationService, JWTAuthenticationService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

//var provider = builder.Services.BuildServiceProvider();
//SchoolWebRegister.Tests.Helpers.DatabaseSeeder.Initialize(provider);

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

app.UseRouting();
app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always,
});

app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets();
app.MapGraphQL("/graphql");
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