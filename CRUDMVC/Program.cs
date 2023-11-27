using Microsoft.EntityFrameworkCore;
using CRUDMVC.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using CRUDMVC.Models;

var builder = WebApplication.CreateBuilder(args);

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connection));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication().AddJwtBearer(options => 
    options.TokenValidationParameters = new TokenValidationParameters(){    
            ValidateIssuer = true,            
            ValidIssuer = AuthOptions.ISSUER,            
            ValidateAudience = true,           
            ValidAudience = AuthOptions.AUDIENCE,            
            ValidateLifetime = true,            
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),          
            ValidateIssuerSigningKey = true,
});
builder.Services.AddAuthorization();

var app = builder.Build();

app.Use(async (context, next) =>
{
    if(context.Request.Cookies.TryGetValue("Token", out string? token))
        context.Request.Headers.Authorization = $"Bearer {token}";
    await next.Invoke();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
