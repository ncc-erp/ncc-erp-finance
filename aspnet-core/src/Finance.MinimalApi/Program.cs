using Castle.Windsor;
using Finance.MinimalApi;
using Finance.MinimalApi.Utils;
using FinanceManagement.Entities.NewEntities;
using FinanceManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<FinanceManagementDbContext>(options =>
{
    options.UseSqlServer(builder.Environment.GetAppConfiguration().GetConnectionString("Default"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/CreateBTransaction", (CreateBTransaction input, FinanceManagementDbContext _context) =>
{
    using (var uow = _context.Database.BeginTransaction())
    {
        var logger = new BTransactionLog()
        {
            Message = input.Message,
            IsValid = false
        };
        var messageClean = Helpers.RemoveNewLine(input.Message);
        var regexMoneyDetection = new Regex("GD: (.*?)So");
        var regexSTKDetection = new Regex("TK(.*?)So");
        var timeAt = DateTimeUtils.GetNow();
        logger.TimeAt = timeAt;

        var moneyDetection = Helpers.DetectionMoney(regexMoneyDetection, messageClean);
        var bankAccountDetection = Helpers.DetectionBankNumber(regexSTKDetection, messageClean);

        if (!moneyDetection.IsValid)
        {
            logger.ErrorMessage = moneyDetection.ErrorMessage;
            _context.Add(logger);
            _context.SaveChanges();
            return Results.Ok(logger);
        }

        if (!bankAccountDetection.IsValid)
        {
            logger.ErrorMessage = bankAccountDetection.ErrorMessage;
            _context.Add(logger);
            _context.SaveChanges();
            return Results.Ok(logger); ;
        }

        var bankAccount = _context.BankAccounts
                .Where(s => s.BankNumber == bankAccountDetection.Result)
                .Select(s => new BankAccountCrawl
                {
                    TenantId = s.TenantId,
                    CurrencyId = s.CurrencyId.Value,
                    Id = s.Id
                })
                .FirstOrDefault();
        if (bankAccount == null)
        {
            logger.ErrorMessage = "Can't bank account";
            _context.Add(logger);
            _context.SaveChanges();
            return Results.Ok(logger); ;
        }

        var bTransaction = _context.BTransactions.Add(new BTransaction
        {
            BankAccountId = bankAccount.Id,
            Money = moneyDetection.Result,
            TimeAt = timeAt,
            Note = input.Message,
            IsCrawl = true,
            TenantId = bankAccount.TenantId,
        });
        _context.SaveChanges();

        logger.BTransactionId = bTransaction.Entity.Id;
        logger.IsValid = true;
        logger.TenantId = bankAccount.TenantId;
        _context.Add(logger);
        _context.SaveChanges();
        uow.Commit();
        return Results.Ok(logger);
    }
})
.WithName("CreateBTransaction");

app.Run();

internal class CreateBTransaction
{
    public string Message { get; set; }
}