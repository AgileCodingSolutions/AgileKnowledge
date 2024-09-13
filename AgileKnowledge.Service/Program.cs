using AgileKnowledge.Service.Domain;
using AgileKnowledge.Service.Helper;
using AgileKnowledge.Service.Mappings;
using AgileKnowledge.Service.Options;
using AgileKnowledge.Service.Service;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.GetSection(ConnectionStringsOptions.Name)
	.Get<ConnectionStringsOptions>();

builder.Configuration.GetSection(OpenAIOption.Name)
	.Get<OpenAIOption>();

builder.Configuration.GetSection(JwtOptions.Name)
	.Get<JwtOptions>();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region AuthMapper

var config = new MapperConfiguration(cfg =>
{
	cfg.AddProfile<KnowledgeMapperProfile>();
});
var mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

#endregion

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll",
		builder => builder
			.SetIsOriginAllowed(_ => true)
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials());
});



builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<JwtTokenProvider>();

builder.Services.AddDbContext<KnowledgeDbContext>(options =>
{
	options.UseNpgsql(ConnectionStringsOptions.DefaultConnection);
});

builder.Services.AddSingleton<KnowledgeMemoryService>();

builder.Services.AddHostedService<QuantizeBackgroundService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();


app.UseAuthorization();

app.MapControllers();


#region MigrationDb
await using var context = app.Services.CreateScope().ServiceProvider.GetService<KnowledgeDbContext>();
{
	// await context!.Database.MigrateAsync();

	await context.Database.ExecuteSqlInterpolatedAsync($"CREATE EXTENSION IF NOT EXISTS vector;");
}
#endregion



app.Run();
