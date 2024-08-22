using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Common;
using WebApplication1.Util;
using WebApplication1.Service.Dtos;
using WebApplication1.Service.Api;
using WebApplication1.Service.Impl;
using static System.Runtime.InteropServices.JavaScript.JSType;
using WebApplication1.Repositories.Api;
using WebApplication1.Repositories.Impl;

//Util util = new Util();
//util.InsertFileDB("D:\\vs_studio\\vs_studio_workspace\\帳務中心－學習新手包\\帳務中心_學習新手包_題目1\\題目一_Insert_MSTMB.txt");

var builder = WebApplication.CreateBuilder(args);

// 設置 Autofac 作為 DI 容器
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// 連上db
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 在 Autofac 容器中註冊服務
builder.Host.ConfigureContainer((ContainerBuilder containerBuilder) =>
{
    containerBuilder.RegisterType<Repository>().As<IRepository>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<ErrorService>().As<IErrorService>().InstancePerLifetimeScope();

    containerBuilder.RegisterType<UnOffsetAccsum>().AsSelf().InstancePerLifetimeScope();
    containerBuilder.RegisterType<UnOffsetService>().As<IUnOffsetService>().InstancePerLifetimeScope();

    containerBuilder.RegisterType<ProfitAccsum>().AsSelf().InstancePerLifetimeScope();
    containerBuilder.RegisterType<ProfitService>().As<IProfitService>().InstancePerLifetimeScope();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();

    InMemoryCache.HCNRHData = context.HCNRHTable.ToList();
    InMemoryCache.HCNTDData = context.HCNTDTable.ToList();
    InMemoryCache.TCNUDData = context.TCNUDTable.ToList();
    InMemoryCache.MSTMBData = context.MSTMBTable.ToList();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();