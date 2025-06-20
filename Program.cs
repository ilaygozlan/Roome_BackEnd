using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// CORS
app.UseCors("AllowAll");

// Swagger
if (true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// statuc files
app.UseStaticFiles(); //wwwroot

// uploadedFiles
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploadedFiles")),
    RequestPath = "/uploadedFiles"
});

// HTTPS + Controllers
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
