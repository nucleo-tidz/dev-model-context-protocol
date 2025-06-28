using Microsoft.AspNetCore.Http;
using ModelContextProtocol;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace infrastructure.Authorization
{
    public abstract class ToolBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected ToolBase(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        protected void IsAuthorized([CallerMemberName] string methodName = "")
        {
            var method = GetType().GetMethod(methodName);
            var attribute = method?.GetCustomAttribute<RequireRoleAttribute>();
            var user = _httpContextAccessor.HttpContext?.User;
            if (attribute != null && !attribute.IsAuthorized(user))
                throw new McpException("Access Denied",
                    new UnauthorizedAccessException($"Required role: {string.Join(", ", attribute.Roles)}")
                    , McpErrorCode.InternalError);

        }
    }
}
