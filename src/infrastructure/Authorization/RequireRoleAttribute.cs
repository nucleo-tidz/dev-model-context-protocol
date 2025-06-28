using System.Security.Claims;

namespace infrastructure.Authorization
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RequireRoleAttribute : Attribute
    {
        public string[] Roles { get; }
        public RequireRoleAttribute(params string[] roles)
        {
            Roles = roles;
        }
        public bool IsAuthorized(ClaimsPrincipal user)
        {
            return user != null && Roles.Any(role => user.IsInRole(role));
        }
    }
}
