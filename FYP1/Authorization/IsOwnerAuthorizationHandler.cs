﻿
using FYP1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using System.Threading.Tasks;

namespace FYP1.Authorization
{
    public class IsOwnerAuthorizationHandler
                : AuthorizationHandler<OperationAuthorizationRequirement, string>
    {
        UserManager<Member> _userManager;

        public IsOwnerAuthorizationHandler(UserManager<Member>
            userManager)
        {
            _userManager = userManager;
        }

        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                                   OperationAuthorizationRequirement requirement,
                                   string username)
        {
            if (context.User == null || username == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for CRUD permission, return.

            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }

            if (username == _userManager.GetUserName(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}