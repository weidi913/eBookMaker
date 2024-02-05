using FYP1.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace FYP1.Authorization
{
    public class ModeratorAuthorizationHandle :
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
            if (requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.DeleteOperationName &&
                requirement.Name != Constants.ApproveOperationName &&
                requirement.Name != Constants.RejectOperationName)
            {
                return Task.CompletedTask;
            }

            // Managers can read, delete, approve or reject.
            if (context.User.IsInRole(Constants.ModeratorRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
