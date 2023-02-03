using System;
using AutoMapper;
using FinanceControl.WebApi.Extensions.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ILogger = Serilog.ILogger;

namespace FinanceControl.Extensions.AppSettings;

public interface IAppSettings : IDisposable
{
    IMapper GetMapper();
    ILogger GetLogger();
    IContextMongoDBDatabase GetMongoDb();
    IConfiguration GetConfiguration();
    IHttpContextAccessor GetHttpContext();
    Microsoft.AspNetCore.Hosting.IWebHostEnvironment GetHostingEnvironment();
}