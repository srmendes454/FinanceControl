using System;
using AutoMapper;
using FinanceControl.Infra.Context;
using FinanceControl.Infra.RequestContainer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace FinanceControl.Infra.AppSettings;

public interface IAppSettings : IDisposable
{
    IMapper GetMapper();
    ILogger GetLogger();
    IContextMongoDBDatabase GetMongoDb();
    IConfiguration GetConfiguration();
    IHttpContextAccessor GetHttpContext();
    Microsoft.AspNetCore.Hosting.IWebHostEnvironment GetHostingEnvironment();
    IRequestContainer GetRequestContainer();
}