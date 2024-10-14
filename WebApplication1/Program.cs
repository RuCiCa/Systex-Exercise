using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Common;
using WebApplication1.Service.Dtos;
using WebApplication1.Service.Api;
using WebApplication1.Service.Impl;
using static System.Runtime.InteropServices.JavaScript.JSType;
using WebApplication1.Repositories.Api;
using WebApplication1.Repositories.Impl;
using WebApplication1.service.dtos;

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
    containerBuilder.RegisterType<InMemoryCache>().SingleInstance(); 
    containerBuilder.RegisterType<ErrorService>().As<IErrorService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<Util>().AsSelf().InstancePerLifetimeScope();

    containerBuilder.RegisterType<UnOffsetRepository>().As<IUnOffsetRepository>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<UnOffsetAccsum>().AsSelf().InstancePerLifetimeScope();
    containerBuilder.RegisterType<UnOffsetService>().As<IUnOffsetService>().InstancePerLifetimeScope();

    containerBuilder.RegisterType<ProfitRepository>().As<IProfitRepository>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<ProfitAccsum>().AsSelf().InstancePerLifetimeScope();
    containerBuilder.RegisterType<ProfitService>().As<IProfitService>().InstancePerLifetimeScope();

    containerBuilder.RegisterType<ProfileRepository>().As<IProfileRepository>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<ProfileSum>().AsSelf().InstancePerLifetimeScope();
    containerBuilder.RegisterType<ProfileService>().As<IProfileService>().InstancePerLifetimeScope();
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();