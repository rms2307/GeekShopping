using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Model;
using GeekShopping.IdentityServer.Model.Context;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GeekShopping.IdentityServer.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly IdentityContext _context;
        private readonly UserManager<ApplicationUser> _user;
        private readonly RoleManager<IdentityRole> _role;

        public DbInitializer(IdentityContext context,
                UserManager<ApplicationUser> user,
                RoleManager<IdentityRole> role)
        {
            _context = context;
            _user = user;
            _role = role;
        }

        public void Initialize()
        {
            if (_role.FindByNameAsync(IdentityConfiguration.Admin).Result != null)
                return;

            CreateRoles();

            CreateUser
            (
                IdentityConfiguration.Admin,
                "robson-admin",
                "robson.admin@email.com",
                "+55 (11) 94359-3029",
                "Robson",
                "Silva",
                "&Admin123"
            );

            CreateUser
            (
                IdentityConfiguration.Client,
                "robson-client",
                "robson.client@email.com",
                "+55 (11) 94359-9514",
                "Robson",
                "Silva",
                "&Client123"
            );
        }

        private void CreateUser(string role, string userName, string email, string phoneNumber, string firstName, string lastName, string password)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
                PhoneNumber = phoneNumber,
                FirstName = firstName,
                LastName = lastName
            };

            _user.CreateAsync(user, password).GetAwaiter().GetResult();
            _user.AddToRoleAsync(user, role).GetAwaiter().GetResult();

            var claims = _user.AddClaimsAsync(user, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(JwtClaimTypes.GivenName, user.FirstName),
                new Claim(JwtClaimTypes.FamilyName, user.LastName),
                new Claim(JwtClaimTypes.Role, role)
            }).Result;
        }

        private void CreateRoles()
        {
            _role.CreateAsync(new IdentityRole(IdentityConfiguration.Admin)).GetAwaiter().GetResult();
            _role.CreateAsync(new IdentityRole(IdentityConfiguration.Client)).GetAwaiter().GetResult();
        }
    }
}
