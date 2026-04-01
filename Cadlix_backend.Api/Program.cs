using System.Reflection.Metadata;
using Cadlix_backend.BusinessLogic.Services;
using Cadlix_backend.DataAccess.Repositories;
using Cadlix_backend.DataAccess.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

//connection strinf setup
Cadlix_backend.DataAccess.DbSession.ConnectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");


// Dependency injection for repositories and services
// builder.Services.AddScoped<IMovieRepository, MovieRepository>();
// builder.Services.AddScoped<IMovieService, MovieService>();

// Add controllers and Swagger services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();