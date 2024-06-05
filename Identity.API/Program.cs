using AutoMapper;
using Identity.Data;
using Identity.Domain.Authentication;
using Identity.IoC;
using Identity.Services.Authentication.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;

var allowedOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Jwt configuration starts here
var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

builder.Services.AddAuthentication(options =>
{
    //Set default Authentication Schema as Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero // remove delay of token when expire
            };
    });
//Jwt configuration ends here

DependencyResolver.RegisterServices(builder.Services);
var connectionString = builder.Configuration.GetConnectionString("identity") ?? string.Empty;
DependencyResolver.RegisterDB(builder.Services, connectionString);

builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<IdentityContext>();


var mappingConfiguration = new MapperConfiguration(mc =>
{
    mc.AddProfile(new AuthenticationMappingConfigurator(mc));

});
var mapper = mappingConfiguration.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddCors(options =>
{
    options.AddPolicy(allowedOrigins, b => { b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(allowedOrigins);

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

using (var scope = app.Services.CreateScope())
{
    using (var context = scope.ServiceProvider.GetRequiredService<IdentityContext>())
    {
        var user = context.Users.FirstOrDefault(u => u.UserName == "Administrador");
        if (user == null)
        {
            user = new User()
            {
                Email = "admin@admin.com.br",
                UserName = "Administrador",
                FullName = "Administrador",
                DisplayName = "Administrador",
                PhoneNumber = "1234567890",
                Active = true
            };
            var addResult = scope.ServiceProvider.GetRequiredService<UserManager<User>>().CreateAsync(user, "Adm@2024").Result;
        }

        user = context.Users.FirstOrDefault(u => u.UserName == "Developer");
        if (user == null)
        {
            user = new User()
            {
                Email = "Developer@Developer.com.br",
                UserName = "Developer",
                FullName = "Developer",
                DisplayName = "Developer",
                PhoneNumber = "1234567890",
                Active = true
            };
            var addResult = scope.ServiceProvider.GetRequiredService<UserManager<User>>().CreateAsync(user, "Dev@2024").Result;
        }


        user = context.Users.FirstOrDefault(u => u.UserName == "QA");
        if (user == null)
        {
            user = new User()
            {
                Email = "Qa@Qa.com.br",
                UserName = "Qa",
                FullName = "Qa",
                DisplayName = "Qa",
                PhoneNumber = "1234567890",
                Active = true
            };
            var addResult = scope.ServiceProvider.GetRequiredService<UserManager<User>>().CreateAsync(user, "Qa@2024").Result;
        }

        var roles = new List<string>() { "Administrador", "Developer", "QA" };
        roles.ForEach(role =>
        {
            var roleEntity = context.Roles.FirstOrDefault(r => r.Name == role);
            if (roleEntity == null)
            {
                context.Roles.Add(new IdentityRole()
                {
                    Name = role,
                    NormalizedName = role
                });
            }
        });
        context.SaveChanges();

        //Incluir na mão a primeira role para o primeiro user [AspNetUserRoles]
    }
}

app.Run();
