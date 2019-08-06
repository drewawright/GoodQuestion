using System;
using System.Collections.Generic;
using System.Linq;
using GoodQuestion.WebAPI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(GoodQuestion.WebAPI.Startup))]

namespace GoodQuestion.WebAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //CreateRolesAndUsers();
        }

        //Creates Default User Roles and an Admin User
        private void CreateRolesAndUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists("Admin"))
            {
                //Create the admin role
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);


                //Create default admin user
                var user = new ApplicationUser();
                user.UserName = "spicyAdmin";
                user.Email = "spicyAdmin@gmail.com";

                string userPWD = "Ind1@nF00d";

                var chkUser = userManager.Create(user, userPWD);

                //Add user to admin role
                if (chkUser.Succeeded)
                {
                    var resultOne = userManager.AddToRole(user.Id, "Admin");
                }
            }
        }
    }
}
