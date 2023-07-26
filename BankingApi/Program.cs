using BankingService.Business.CommandHandlers;
using BankingService.Infrastructure.DataPersistance.Contracts;
using BankingService.Infrastructure.DataPersistance.Repositories;
using BankingService.Infrastructure.Entities;

namespace BankingService
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

            builder.Services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblies(typeof(AccountCommandHandler).Assembly);
            });

            builder.Services.AddDbContext<BankDbContext>();
            builder.Services.AddTransient<IAccountRepository, AccountRepository>();
            builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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