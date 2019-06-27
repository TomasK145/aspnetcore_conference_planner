using ConferenceDTO;
using FrontEnd.Services;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrontEnd.Pages
{
    [Authorize]
    public class MyAgendaModel : IndexModel
    {
        public MyAgendaModel(IApiClient client)
             : base(client)
        {

        }

        protected override Task<List<SessionResponse>> GetSessionsAsync()
        {
            return _apiClient.GetSessionsByAttendeeAsync(User.Identity.Name);
        }
    }
}