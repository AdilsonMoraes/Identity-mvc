using AutoMapper;
using Identity.Data;
using Identity.Services.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Services
{

    public class BaseService
    {
        protected readonly IMapper Mapper;
        protected readonly IdentityContext Context;
        protected readonly ILogService LogService;
        private readonly Guid _sessionGuid;

        public BaseService(IMapper mapper, IdentityContext context, ILogService logService)
        {
            Mapper = mapper;
            Context = context;
            LogService = logService;
            _sessionGuid = Guid.NewGuid();
        }

        protected T ExecuteFaultHandledOperation<T>(Func<T> codeToExecute)
        {
            try
            {
                return codeToExecute();
            }
            catch (Exception ex)
            {
                LogService.LogException(ex, _sessionGuid);
                return default(T);
            }
        }

        protected void ExecuteFaultHandledOperation(Action codetoExecute)
        {
            try
            {
                codetoExecute();
            }
            catch (Exception ex)
            {
                LogService.LogException(ex, _sessionGuid);
            }
        }
    }
}
