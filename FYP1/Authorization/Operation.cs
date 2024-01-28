using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace FYP1.Authorization
{
    public static class Operations
    {
        public static OperationAuthorizationRequirement Create =
          new OperationAuthorizationRequirement { Name = Constants.CreateOperationName };
        public static OperationAuthorizationRequirement Read =
          new OperationAuthorizationRequirement { Name = Constants.ReadOperationName };
        public static OperationAuthorizationRequirement Update =
          new OperationAuthorizationRequirement { Name = Constants.UpdateOperationName };
        public static OperationAuthorizationRequirement Delete =
          new OperationAuthorizationRequirement { Name = Constants.DeleteOperationName };
        public static OperationAuthorizationRequirement Approve =
          new OperationAuthorizationRequirement { Name = Constants.ApproveOperationName };
        public static OperationAuthorizationRequirement Reject =
          new OperationAuthorizationRequirement { Name = Constants.RejectOperationName };
        public static OperationAuthorizationRequirement Change =
          new OperationAuthorizationRequirement { Name = Constants.ChangeOperationName };
    }

    public class Constants
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string ApproveOperationName = "Approve";
        public static readonly string RejectOperationName = "Reject";
        public static readonly string ChangeOperationName = "Change";

        public static readonly string AdminRole = "AdminRole";
        public static readonly string ModeratorRole = "ModeratorRole";
        public static readonly string ReviewerRole = "ReviewerRole";
        public static readonly string MemberRole = "MemberRole";
        public static readonly string GuestRole = "GuestRole";
    }
}
