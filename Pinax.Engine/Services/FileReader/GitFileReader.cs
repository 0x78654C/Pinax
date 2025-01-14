﻿using Octokit;
using Pinax.Engine.Models;
using Pinax.Engine.Models.Projects;

namespace Pinax.Engine.Services.FileReader;

public class GitFileReader : IFileReader
{
    private readonly GitHubClient _githubClient =
        new(new ProductHeaderValue("Pinax"));

    private readonly string _path;

    public GitFileReader(string token, string path)
    {
        _githubClient.Credentials = new Credentials(token);
        _path = path;
    }

    public List<Solution> GetSolutions()
    {
        return _githubClient
            .Search
            .SearchCode(new SearchCodeRequest($"user:{_path} extension:sln"))
            .Result
            .Items
            .Select(i => new FileDetails(i.Repository.HtmlUrl, i.Name))
            .Select(fd => new Solution(fd))
            .ToList();
    }

    public List<DotNetProject> GetDotNetProjects()
    {
        return _githubClient
            .Search
            .SearchCode(new SearchCodeRequest($"user:{_path} extension:csproj"))
            .Result
            .Items
            .Select(i => new FileDetails(i.Repository.HtmlUrl, i.Name))
            .Select(fd => new DotNetProject(fd))
            .ToList();
    }
}