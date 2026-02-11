
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SqlGpt.Data;
using SqlGpt.Models;

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


            builder.Services.AddControllers();

          

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
