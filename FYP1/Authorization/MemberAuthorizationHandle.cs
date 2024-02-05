using FYP1.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FYP1.Authorization
{
    public class MemberAuthorizationHandle :
    AuthorizationHandler<OperationAuthorizationRequirement, string>
    {
        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                                   OperationAuthorizationRequirement requirement,
                                   string username)
        {
            if (context.User == null || username == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for read/delete/approval/reject, return.
            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }

            // Managers can read, delete, approve or reject.
            if (context.User.IsInRole(Constants.MemberRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
