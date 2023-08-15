using Abp.Collections.Extensions;
using Abp.Dependency;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.Ncc
{
    public class HttpHeaderPeriodResolveContributor : IPeriodResolveContributor
    {
        public const string PeriodResolveKey = "Ncc-PeriodId";
        public ILogger Logger { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpHeaderPeriodResolveContributor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            Logger = NullLogger.Instance;
        }

        public int? ResolvePeriodId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            var periodIdHeader = httpContext.Request.Headers[PeriodResolveKey];
            if (periodIdHeader == string.Empty || periodIdHeader.Count < 1)
            {
                return null;
            }

            if (periodIdHeader.Count > 1)
            {
                Logger.Warn(
                    $"HTTP request includes more than one {PeriodResolveKey} header value. First one will be used. All of them: {periodIdHeader.JoinAsString(", ")}"
                    );
            }

            return int.TryParse(periodIdHeader.First(), out var periodId) ? periodId : (int?)null;
        }
    }
}
