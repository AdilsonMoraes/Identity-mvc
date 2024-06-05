using Identity.Domain.Enums;
using Identity.Domain.ViewModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Services.Log
{
    public class LogService : ILogService
    {
        private readonly IConfiguration _configuration;

        public LogService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void LogMessage(string message, Guid sessionGuid, LogType logType, string applicationName = "LogEventViewModel")
        {
            var logModel = new LogViewModel(message, applicationName);
            var eventLog = new LogEventViewModel()
            {
                ApplicationName = logModel.ApplicationName,
                Content = logModel,
                Time = DateTime.Now,
                Type = logType,
                SessionGuid = sessionGuid
            };

            //logar
        }

        public void LogException(Exception ex, Guid sessionGuid, string applicationName = "LogEventViewModel")
        {
            var logModel = new LogViewModel(ex, applicationName);
            var eventLog = new LogEventViewModel()
            {
                ApplicationName = logModel.ApplicationName,
                Content = logModel,
                Time = DateTime.Now,
                Type = LogType.Error,
                SessionGuid = sessionGuid
            };
            //logar
        }

    }
}
