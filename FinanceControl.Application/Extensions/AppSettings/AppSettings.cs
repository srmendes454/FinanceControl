using AutoMapper;
using FinanceControl.WebApi.Extensions.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using ILogger = Serilog.ILogger;

namespace FinanceControl.Extensions.AppSettings;

public class AppSettings : IAppSettings
{
    #region [ Fields ]
    public void Dispose() { }

    private readonly IContextMongoDBDatabase _mongoDb;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpContextAccessor _httpContextAccessor;
    #endregion

    #region [ Constructor ]

    public AppSettings(IMapper mapper = null,
        IConfiguration configuration = null,
        IWebHostEnvironment environment = null,
        IContextMongoDBDatabase mongoDb = null,
        IHttpContextAccessor httpContextAccessor = null)
    {
        _mapper = mapper;
        _mongoDb = mongoDb;
        _environment = environment;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _logger = Log.Logger.ForContext<AppSettings>();
    }
    #endregion

    #region [ Public Methods ]
    public ILogger GetLogger() => _logger;
    public IMapper GetMapper() => _mapper;
    public IContextMongoDBDatabase GetMongoDb() => _mongoDb;
    public IConfiguration GetConfiguration() => _configuration;
    public IHttpContextAccessor GetHttpContext() => _httpContextAccessor;
    public IWebHostEnvironment GetHostingEnvironment() => _environment;
    #endregion
}