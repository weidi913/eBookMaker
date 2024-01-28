using FYP1.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace FYP1.Authorization
{
    public class ReviewerAuthorizationHandle :
    AuthorizationHandler<OperationAuthorizationRequirement, Member>
    {
        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                                   OperationAuthorizationRequirement requirement,
                                   Member resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for read/approval/reject, return.
            if (requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.ApproveOperationName &&
                requirement.Name != Constants.RejectOperationName)
            {
                return Task.CompletedTask;
            }

            // Managers can read, approve or reject.
            if (context.User.IsInRole(Constants.ReviewerRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
