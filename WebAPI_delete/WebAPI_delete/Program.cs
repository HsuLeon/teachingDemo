var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDev",
        policy =>
        {
            policy.SetIsOriginAllowed(origin =>
            {
                return true;
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });

    options.AddPolicy("AllowProd",
        policy =>
        {
            policy.WithOrigins(
                "https://cloud.giraffe.com.tw",
                "http://192.168.20.51:5000",
                "http://localhost",
                "http://localhost:3000",
                "http://localhost:5000"
            ) // Replace with your front-end origin
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseCors("AllowProd");
app.UseCors("AllowDev");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
