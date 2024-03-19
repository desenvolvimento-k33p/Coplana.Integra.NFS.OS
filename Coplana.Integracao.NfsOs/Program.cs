using AutoMapper;
using Hangfire;
using Hangfire.MemoryStorage;
using Coplana.Integracao.NfsOs.Services;
using Microsoft.Extensions.Configuration;
using Coplana.Integracao.NfsOs.Domain.Configuration;
using Coplana.Integracao.NfsOs.Core.Interfaces;
using Coplana.Integracao.NfsOs.Core.Adapters;
using Coplana.Integracao.NfsOs.Infra.Interfaces;
using Coplana.Integracao.NfsOs.Infra.Repositories;
using Coplana.Integracao.NfsOs.Services.Interfaces;
using Coplana.Integracao.NfsOs.Services.Services;
using Coplana.Integracao.NfsOs.Services.Mapper.Profiles.BusinessPartner;
//using Coplana.Integracao.NfsOs.Core.Adapters;
//using Coplana.Integracao.NfsOs.Core.Interfaces;
//using Coplana.Integracao.NfsOs.Domain.Configuration;
//using Coplana.Integracao.NfsOs.Infra.Interfaces;
//using Coplana.Integracao.NfsOs.Infra.Repositories;
//using Coplana.Integracao.NfsOs.Services.Interfaces;
//using Coplana.Integracao.NfsOs.Services.Mapper.Profiles;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var devCorsPolicy = "devCorsPolicy";
GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

builder.Host.ConfigureDefaults(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Integração Coplana - NFS OS";
    })
    .ConfigureServices(services =>
    {
        #region [ Configuration ]
        services.Configure<Configuration>(configuration.GetSection("AppConfig"));
        #endregion

        #region [ Services ]

        //services.AddScoped<IItemsService, ItemsService>();
        #endregion

        #region [ Adapters ]
        services.AddScoped<IHttpAdapter, HttpAdapter>();
        services.AddScoped<IServiceLayerAdapter, ServiceLayerAdapter>();
        services.AddScoped<IHanaAdapter, HanaAdapter>();
        #endregion

        #region [ Repositories ]
        services.AddScoped<ILoggerRepository, LoggerRepository>();
        #endregion

        #region [ Hangfire ]
        services.AddControllers();

        services.AddHangfire((provider, configuration) =>
        {
            configuration.UseMemoryStorage();
            configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
            configuration.UseSimpleAssemblyNameTypeSerializer();
            configuration.UseRecommendedSerializerSettings();
        });

        services.AddHangfireServer();
        #endregion

        #region [ Cors ]
        services.AddCors(services =>
        {
            services.AddPolicy(devCorsPolicy, builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
        });
        #endregion

        #region [ ConfigMap ]
        var config = new AutoMapper.MapperConfiguration(cfg =>
        {

            cfg.AddProfile<CreateBusinessPartnerProfile>();
        });

        IMapper mapper = config.CreateMapper();
        services.AddSingleton(mapper);
        #endregion

    });

using (var app = builder.Build())
{
    app.UseStaticFiles();


    var insertItensOS = new BackgroundJobServerOptions
    {
        ServerName = string.Format("{0}:insertitens", Environment.MachineName),
        Queues = new[] { "insertitensqueue" },
        WorkerCount = 1
    };
    var deleteItensOS = new BackgroundJobServerOptions
    {
        ServerName = string.Format("{0}:deleteitens", Environment.MachineName),
        Queues = new[] { "deleteitensqueue" },
        WorkerCount = 1
    };



    app.UseHangfireDashboard("/scheduler", new DashboardOptions
    {
        //Authorization = new[] { new HangFireAuthorization() },
        AppPath = "/"
    });


    app.UseHangfireServer(insertItensOS);
    app.UseHangfireServer(deleteItensOS);

    app.UseHangfireDashboard();




    #region [ Scheduler ]

    #region [ DEBUG ]

#if DEBUG

    var cronServiceTeste = Cron.Minutely();
    var cronServiceDebug = Cron.Never();

    RecurringJob.AddOrUpdate<InsertItensOSService>("InsertItensOSService", job => job.ProcessAsync(), cronServiceDebug);
    RecurringJob.AddOrUpdate<DeleteItensOSService>("DeleteItensOSService", job => job.ProcessAsync(), cronServiceDebug);




#endif

    #endregion

    #region [ RELEASE ]

#if DEBUG == false

    var cronService_ = Cron.MinuteInterval(59);
    //var cronService_ = Cron.Minutely();

    RecurringJob.AddOrUpdate<InsertItensOSService>("InsertItensOSService", job => job.ProcessAsync(), cronService_, null, "insertitensqueue");
    RecurringJob.AddOrUpdate<DeleteItensOSService>("DeleteItensOSService", job => job.ProcessAsync(), cronService_, null, "deleteitensqueue");
 




#endif

    #endregion

    #endregion

    await app.RunAsync();

}
