﻿using System.Net;

namespace Chirp.Core;

public interface ICheepRepository
{
    public Task<List<CheepDto>> GetCheeps(int page);
    public Task<bool> HasNextPageOfCheeps(int page);
    public Task<List<CheepDto>> GetCheepsFromAuthor(int page, string author);
    public Task<List<CheepDto>> GetCheepsUserTimeline(int page, string UserName, List<Guid> authorIds);
    public Task CreateCheep(string message, AuthorDto user);
    public Task<CheepDto> GetFirstCheepFromAuthor(Guid authorId);

}