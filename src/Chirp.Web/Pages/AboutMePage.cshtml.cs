using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class AboutMePageModel : PageModel
{
    public ICheepRepository _cheepRepository;
    public IAuthorRepository _authorRepository;
    public List<CheepDto>? Cheeps { get; set; }
    public List<Guid>? Followers { get; set; }
    public string? author { get; set; }
    public string? email { get; set; } 


    public AboutMePageModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }

    public async Task<IActionResult> OnGet()
    {  
        string? pagevalue = Request.Query["page"];
    
        author = User.Identity.Name;
        email = User.FindFirstValue("emails");
            if (pagevalue == null)
            {
                Cheeps = await _cheepRepository.GetCheepsFromAuthor(1, author);
            }
            else
            {
                Cheeps = await _cheepRepository.GetCheepsFromAuthor(Int32.Parse(pagevalue), author);
            }
        
        Followers = await _authorRepository.GetFollowedAuthors(email);
        
        return Page();
    }
}