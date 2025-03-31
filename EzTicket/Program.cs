
using AutoMapper;
using Data;
using EzTickets.Repository;
using EzTickets.Services;
using Microsoft.EntityFrameworkCore;

namespace EzTicket
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Register DataContext with Connection String
            builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CS"),
        sqlOptions => sqlOptions.MigrationsAssembly("EzTickets") // Ensure migrations are stored in 'Data' project
    ));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
