﻿using Pinax.Models.Projects;
using Pinax.Services;

namespace Pinax.Models;

public class Job
{
    private readonly Enums.Source _fileSource;
    private readonly DotNetVersions _latestDotNetVersions;
    private readonly Enums.WarningLevel _warningLevel;
    private readonly List<string> _includedLocations = new();
    private readonly List<string> _excludedLocations = new();

    public List<Solution> Solutions =
        new List<Solution>();
    public List<string> Results { get; } =
        new List<string>();
    public List<string> ValidationErrors { get; } =
        new List<string>();

    public bool IsValid => ValidationErrors.None();

    public Job(Enums.Source fileSource,
        DotNetVersions latestDotNetVersions,
        Enums.WarningLevel warningLevel = Enums.WarningLevel.None)
    {
        _fileSource = fileSource;
        _latestDotNetVersions = latestDotNetVersions;
        _warningLevel = warningLevel;
    }

    public void AddIncludedLocation(string location)
    {
        if (_includedLocations.None(l => l.Matches(location)))
        {
            _includedLocations.Add(location);
        }
    }

    public void AddExcludedLocation(string location)
    {
        if (_excludedLocations.None(l => l.Matches(location)))
        {
            _excludedLocations.Add(location);
        }
    }

    public void Execute()
    {
        PopulateSolutions();

        foreach (Solution solution in Solutions)
        {
            Results.Add($"SOLUTION: {solution.Name}");

            foreach (DotNetProject project in solution.Projects)
            {
                bool isOutdated = project.IsOutdated(_warningLevel);

                string outdatedFlag = isOutdated ? "*" : "";

                Results.Add($"{outdatedFlag}\tPROJECT: {project.ShortName} [{string.Join(';', project.ProjectTypes)}]");

                foreach (Package package in project.Packages)
                {
                    Results.Add($"\t\tPACKAGE: {package.Name} {package.Version}");
                }
            }
        }
    }

    #region Private methods

    private void PopulateSolutions()
    {
        foreach (var location in _includedLocations)
        {
            switch (_fileSource)
            {
                case Enums.Source.Disk:
                    PopulateSolutionsFromDisk(location);
                    break;
                case Enums.Source.GitHub:
                    PopulateSolutionsFromGitHub(location);
                    break;
            }
        }
    }

    private void PopulateSolutionsFromDisk(string location)
    {
        Solutions =
            DiskService.GetSolutions(location, _latestDotNetVersions)
                .Where(s =>
                    _excludedLocations.None(e =>
                        s.Name.StartsWith(e, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();
    }

    private void PopulateSolutionsFromGitHub(string location)
    {
        // TODO: Read solutions from GitHub
        // GitHub searching
        var projects = GitHubService.GetProjectFiles(location, "C#");

        foreach (string project in projects)
        {
            Results.Add($"PROJECT: {project}");
        }
    }

    #endregion
}