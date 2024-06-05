using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.ViewModel
{
    public class LogViewModel
    {
        public string ApplicationName { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public LogViewModel(string message, string applicationName)
        {
            Message = message;
            ApplicationName = applicationName;
        }

        public LogViewModel(Exception exception, string applicationName)
        {
            Exception = exception;
            Message = exception.Message;
            ApplicationName = applicationName;
        }
    }
}
