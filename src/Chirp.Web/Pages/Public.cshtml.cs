﻿using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;



namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{

    public ICheepRepository _CheepRepository;
    public IAuthorRepository _AuthorRepository;
    public List<CheepDto>? Cheeps { get; set; }

    public List<Guid>? FollowedAuthors { get; set; }

    public int PageNumber { get; set; }

    public PublicModel(ICheepRepository repository, IAuthorRepository authorRepository)
    {
        _CheepRepository = repository;
        _AuthorRepository = authorRepository;
    }

    public async Task<IActionResult> OnGet() //use of Task https://learn.microsoft.com/en-us/aspnet/core/data/ef-rp/crud?view=aspnetcore-7.0
    {
        string? pagevalue = Request.Query["page"];


        PageNumber = pagevalue == null ? 1 : Int32.Parse(pagevalue);

        if (pagevalue == null)
        {
            Cheeps = await _CheepRepository.GetCheeps(1);
        }
        else
        {
            Cheeps = await _CheepRepository.GetCheeps(Int32.Parse(pagevalue));
        }

        if (User.Identity.IsAuthenticated)
        {
            FollowedAuthors ??= new List<Guid>();

            FollowedAuthors = await _AuthorRepository.GetFollowedAuthors(User.FindFirstValue("emails"));
        }

        return Page();
    }


    [BindProperty]
    public string Text { get; set; }

    public async Task<IActionResult> OnPost()
    {

        AuthorDto author = new()
        {
            Name = User.Identity.Name,
            Email = User.FindFirstValue("emails")// from https://stackoverflow.com/questions/30701006/how-to-get-the-current-logged-in-user-id-in-asp-net-core
        };
        await _CheepRepository.CreateCheep(Text, author);

        string redirectUrl = "~/";

        return Redirect(Url.Content(redirectUrl));
    }

    //https://www.learnrazorpages.com/razor-pages/handler-methods
    public async Task<IActionResult> OnPostFollow(string followName)
    {
        string emailOfUserThatWantsToFollow = User.FindFirstValue("emails") ?? "";
        await _AuthorRepository.FollowAnAuthor(emailOfUserThatWantsToFollow, followName);

        string redirectUrl = "~/";

        return Redirect(Url.Content(redirectUrl));
    }

    public async Task<IActionResult> OnPostUnFollow(string followName)
    {
        string emailOfUserThatWantsToUnfollow = User.FindFirstValue("emails") ?? "";
        await _AuthorRepository.UnFollowAnAuthor(emailOfUserThatWantsToUnfollow, followName);

        string redirectUrl = "~/";

        return Redirect(Url.Content(redirectUrl));
    }

}
