using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ReactProject.Model
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }

        public DateTime? CreateDate { get; set; }

        public virtual ICollection<AppUserClaim> Claims { get; set; }

        public virtual ICollection<AppUserLogin> Logins { get; set; }

        public virtual ICollection<AppUserToken> Tokens { get; set; }

        public virtual ICollection<AppUserRole> UserRoles { get; set; }
    }

    public class AppuserClaimspincipalFactory : UserClaimsPrincipalFactory<AppUser>
    {
        public AppuserClaimspincipalFactory(
            UserManager<AppUser> userManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        { }

        public async override Task<ClaimsPrincipal> CreateAsync(AppUser user)
        {
            var principal = await base.CreateAsync(user);

            if (!string.IsNullOrWhiteSpace(user.DisplayName))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim("DisplayName", user.DisplayName)
                });
            }

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim("Email", user.Email)
                });
            }

            return principal;
        }
    }

    public class AppRole : IdentityRole
    {
        public virtual ICollection<AppUserRole> UserRoles { get; set; }

        public virtual ICollection<AppUserRoleClaim> RoleClaims { get; set; }
    }

    public class AppUserRole : IdentityUserRole<string>
    {
        public virtual AppUser User { get; set; }

        public virtual AppRole Role { get; set; }
    }

    public class AppUserClaim : IdentityUserClaim<string>
    {
        public virtual AppUser User { get; set; }
    }

    public class AppUserLogin : IdentityUserLogin<string>
    {
        public virtual AppUser User { get; set; }
    }

    public class AppUserRoleClaim : IdentityRoleClaim<string>
    {
        public virtual AppRole Role { get; set; }
    }

    public class AppUserToken : IdentityUserToken<string>
    {
        public virtual AppUser User { get; set; }
    }
}
