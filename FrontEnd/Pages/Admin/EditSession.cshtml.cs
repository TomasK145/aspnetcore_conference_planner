using ConferenceDTO;
using FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace FrontEnd.Pages.Admin
{
    public class EditSessionModel : PageModel
    {
        private readonly IApiClient _apiClient;

        public EditSessionModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [BindProperty] //make sure properties get bound on form posts
        public Session Session { get; set; }

        [TempData] //TempData-backed properties also flow across pages, so we can update the Index page to show the message value too, e.g. when the session is deleted
        public string Message { get; set; }
        public bool ShowMessage => !string.IsNullOrEmpty(Message);

        public async Task OnGetAsync(int id)
        {
            var session = await _apiClient.GetSessionAsync(id);
            Session = new Session
            {
                ID = session.ID,
                ConferenceID = session.ConferenceID,
                TrackId = session.TrackId,
                Title = session.Title,
                Abstract = session.Abstract,
                StartTime = session.StartTime,
                EndTime = session.EndTime
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Message = "Session updated successfully!";

            await _apiClient.PutSessionAsync(Session);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var session = await _apiClient.GetSessionAsync(id);

            if (session != null)
            {
                await _apiClient.DeleteSessionAsync(id);
            }

            Message = "Session deleted successfully!";

            return RedirectToPage("/Index");
        }
    }
}