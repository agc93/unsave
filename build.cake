#load nuget:?package=DevelopEngine.Cake&version=0.0.0-preview.0.10

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Setup<BuildData>(ctx => {
    Information("Running build setup...");
    // this repo is an experiment in a simpler form of branching/versioning so we're treating untagged master as a develop branch
    var publish = HasEnvironmentVariable("GITHUB_REF") 
        && (EnvironmentVariable("GITHUB_REF").StartsWith("refs/tags/v") || EnvironmentVariable("GITHUB_REF") == "refs/heads/main");
    return new BuildData {
        ReleaseBuild = publish,
        Projects = GetProjects(File("./src/UnSave.sln"), configuration),
        BuildVersion = packageVersion,
        Configuration = configuration,
        Frameworks = new List<string> {"net5.0"}
    };
});

Task("Default")
.IsDependentOn("Post-Build")
.IsDependentOn("NuGet")
.IsDependentOn("Publish");

Task("Publish")
.IsDependentOn("Publish-NuGet-Package");

RunTarget(target);
