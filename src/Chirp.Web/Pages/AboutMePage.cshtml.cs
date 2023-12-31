using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using Azure.Identity;

namespace Chirp.Web.Pages;

public class AboutMePageModel : PageModel
{
    // Holds the functionality for the About Me page.

    // The cheep repository for fetching the user's cheeps.
    public ICheepRepository _cheepRepository;

    // The author repository for fetching the user's followers and the authors that the user follows.
    public IAuthorRepository _authorRepository;
    private readonly IConfiguration _config;

    // The list of cheeps written by the user, to be displayed on the About Me page.
    public List<CheepDto>? Cheeps { get; set; }

    // The IDs of the followed and following authors, used to fetch the names of those authors.
    public List<Guid>? FollowedID { get; set; }
    public List<Guid>? FollowersID { get; set; }

    // The names of the followwing and followed authors, to be displayed.
    public List<string>? FollowersName { get; set; }
    public List<string>? FollowedName { get; set; }

    // The name of the user
    public string? Author { get; set; }

    // The email of the user
    public string? Email { get; set; }
    public string? OID { get; set; }




    public AboutMePageModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, IConfiguration config)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _config = config;
    }

    public async Task<IActionResult> OnGet()
    {
        // Handles the request when the user navigates to the About Me page.
        // Populates the variables used by the cshtml page to render the page.
        string? pagevalue = Request.Query["page"];

        OID = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Author = User.Identity!.Name;
        Email = User.FindFirstValue("emails");
        if (pagevalue == null)
        {
            Cheeps = await _cheepRepository.GetCheepsFromAuthor(1, Author!);
        }
        else
        {
            Cheeps = await _cheepRepository.GetCheepsFromAuthor(Int32.Parse(pagevalue), Author!);
        }

        FollowedID = await _authorRepository.GetFollowedAuthors(Email);

        FollowersID = await _authorRepository.GetAuthorFollowers(Email);



        FollowedName = new List<string>();
        FollowersName = new List<string>();
        foreach (Guid id in FollowedID!)
        {

            string? user = await _authorRepository.GetAuthorNameByID(id);
            if (user != null)
            {
                FollowedName.Add(user);
            }
        }

        foreach (Guid id in FollowersID!)
        {

            string? user = await _authorRepository.GetAuthorNameByID(id);
            if (user != null)
            {
                FollowersName.Add(user);
            }
        }


        return Page();
    }

    public async Task<IActionResult> OnPostDelete(string UserOID, string UserEmail)
    {
        // The functionality to be run when the user clicks the "Forget Me!" button.
        try
        {
            var clientId = "e4f0f78c-3bcf-40d3-a75e-d6742a37053b";
            var tenantId = "ab2f43aa-cecc-43ed-a142-34012b9a7a3b";
            var clientSecret = _config["ConnectionStrings:DeleteSecret"];

            var scopes = new[] { "https://graph.microsoft.com/.default" };

            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };

            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

            // Delete User by ID
            await DeleteUserAsync(graphClient, UserOID);
            await _authorRepository.DeleteAuthor(UserEmail);

            Console.WriteLine($"User with username: {Author} deleted successfully.");
            string redirectUrl = "/MicrosoftIdentity/Account/SignOut";
            return Redirect(Url.Content(redirectUrl));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting user username: {Author}, {ex.Message}");
            string redirectUrl = "~/AboutMePage";
            return Redirect(Url.Content(redirectUrl));
        }
    }


    private async Task DeleteUserAsync(GraphServiceClient graphClient, string userId)
    {
        if (userId != null)
        {
            await graphClient.Users[userId].DeleteAsync();
        }
        else
        {
            throw new InvalidOperationException("User ID not found for the given email.");
        }
    }
}