using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load Configuration
var configuration = builder.Configuration;

// ✅ Add CORS Policy (Frontend Requests)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// ✅ Add Controllers (for API Endpoints)
builder.Services.AddControllers();

// ✅ Add Swagger (for API Testing)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Build the Application
var app = builder.Build();

// ✅ Enable Swagger Only in Development Mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Ensure HTTPS Redirection (Works Only If Properly Configured)
app.UseHttpsRedirection();

// ✅ Enable CORS (Allow Frontend Access)
app.UseCors("AllowAll");

// ✅ Set Up Routing for API Controllers
app.UseAuthorization();
app.MapControllers();

// ✅ Run the Application
app.Run();
