using FYP1.Authorization;
using FYP1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FYP1.Data
{
    public class SeedData
    {

        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@hotmail.com");
                await EnsureRole(serviceProvider, adminID, Constants.AdminRole);

                // allowed user can create and edit contacts that they create
                var moderatorID = await EnsureUser(serviceProvider, testUserPw, "moderator@hotmail.com");
                await EnsureRole(serviceProvider, moderatorID, Constants.ModeratorRole);

                var reviewerID = await EnsureUser(serviceProvider, testUserPw, "reviwer@hotmail.com");
                await EnsureRole(serviceProvider, reviewerID, Constants.ReviewerRole);

                var memberID = await EnsureUser(serviceProvider, testUserPw, "memberA@hotmail.com");
                await EnsureRole(serviceProvider, memberID, Constants.MemberRole);

                SeedDB(context, adminID);
/*                SeedDB(context, moderatorID);
                SeedDB(context, reviewerID);
                SeedDB(context, memberID);*/
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<Member>>();
            /*var userManager2 = serviceProvider.GetService<UserManager<FYP1.Models.Member>>();*/

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new Member
                {
                    UserName = UserName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<Member>>();

            //if (userManager == null)
            //{
            //    throw new Exception("userManager is null");
            //}

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }

        public static void SeedDB(ApplicationDbContext context, string adminID)
        {
/*            if (context.Member.Any())
            {
                return;   // DB has been seeded
            }

            context.Member.AddRange(
                new Member
                {
                    Name = "Debra Garcia",
                    Address = "1234 Main St",
                    City = "Redmond",
                    State = "WA",
                    Zip = "10999",
                    Email = "debra@example.com",
                    Status = Status.Approved,
                    OwnerID = adminID
                },*/
        }

    }

}
