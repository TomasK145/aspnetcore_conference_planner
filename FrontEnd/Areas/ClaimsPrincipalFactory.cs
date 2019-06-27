using FrontEnd.Data;
using FrontEnd.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FrontEnd.Areas
{
    public class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<User>
    {
        //Rather than looking up the user in the database each time the app needs to check if a user is an admin, we can read 
        //this information once when the user logs in, then store it as an additional claim on the user identity.
        //We also need to add an authoriation policy to the app that corresponds to this claim, 
        //that we can use to protect resources we only want admins to be able to access.
        private readonly IApiClient _apiClient;

        public ClaimsPrincipalFactory(IApiClient apiClient, UserManager<User> userManager, IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
            _apiClient = apiClient;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            if (user.IsAdmin)
            {
                identity.MakeAdmin();
            }

            var attendee = await _apiClient.GetAttendeeAsync(user.UserName);
            if (attendee != null)
            {
                identity.MakeAttendee();
            }

            return identity;
        }
    }
}
