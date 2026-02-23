
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SqlGpt.Data;
using SqlGpt.Models;
using SqlGpt.Services;
using SqlGpt.Services.Interfaces;
using System.Text;

namespace SqlGpt
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<SqlGptDbContext>(options => 
                options.UseSqlServer(connectionString));

            builder.Services
            .AddIdentity<AppUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<SqlGptDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IJwtService, JwtService>();

            // advam servica polzvasht HttpClienta
            builder.Services.AddHttpClient<IClaudeService, ClaudeService>(client =>
            {
                client.BaseAddress = new Uri("https://api.anthropic.com/");
            });
            
            
            builder.Services.AddControllers();

            // addvam neshta za CORS 
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("ReactPolicy", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // addvam nastroikite za JWT 

            var jwtSection = builder.Configuration.GetSection("Jwt"); // vzimam ot appsettings neshtata za JWT
            var key = jwtSection["Key"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>{

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(key))
                };
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("ReactPolicy");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
