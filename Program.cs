using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HMProductInfoAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using HMProductInfoAPI.Services;
using HMProductInfoAPI.Hubs;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using HMProductInfoAPI.Extensions;
using Microsoft.AspNetCore.HttpLogging;
using AutoWrapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HMProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer") ?? throw new InvalidOperationException("Connection string 'SqlServer' not found.")));

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.Audience = builder.Configuration["AAD:ResourceId"];
    opt.Authority = $"{builder.Configuration["AAD:InstanceId"]}{builder.Configuration["AAD:TenantId"]}";

});


builder.Services.AddCors(op => op.AddPolicy(name : "AllowOrigin", builder =>
{
    //builder.WithOrigins("https://localhost:7048", "http://localhost:5048").AllowAnyHeader().AllowAnyMethod();
    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();

}));

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddTransient<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IProductCodeGenerationService, ProductCodeGenerationService>();

builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddHostedService(sp => (NotificationService)sp.GetService<INotificationService>());

builder.Services.AddSignalR();
//builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICachingService, CachingService>();

/*All Request-response objects to be logged*/
builder.Services.AddHttpLogging(op =>
{
    op.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.ResponsePropertiesAndHeaders;
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
}

app.ConfigureExceptionHendler(app.Environment);

app.UseCors("AllowOrigin");

app.UseAuthentication();

app.UseAuthorization();

app.UseApiResponseAndExceptionWrapper();

app.UseHttpsRedirection();

app.MapControllers();

//app.UseResponseCaching();

//app.Use(async (context, next) =>
//{
//    context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
//    {
//        Public = true,
//        MaxAge = TimeSpan.FromSeconds(5)
//    };
//    await next();
//});

app.MapHub<NotificationHub>("/notificationhub");

app.Run();
