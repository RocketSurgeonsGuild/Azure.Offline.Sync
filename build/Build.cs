using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build : NukeBuild
{
    /// <summary>
    /// The main entry point for the build program.
    /// </summary>
    /// <returns></returns>
    public static int Main () => Execute<Build>(x => x.AzureDevOps);

    BuildParameters Parameters { get; set; }

    /// <inheritdoc />
    protected override void OnBuildInitialized()
    {
        Parameters = new BuildParameters(this);
    }

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = Configuration.Release;

    [Solution] readonly Solution Solution;
    [Solution("benchmarks/Benchmarks.sln")] readonly Solution BenchmarkSolution;

    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "test";
    AbsolutePath BenchmarkDirectory => RootDirectory / "benchmarks";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath PackageDirectory => ArtifactsDirectory / "packages";
    AbsolutePath BenchmarkResultsDirectory => ArtifactsDirectory / "benchmarks";
    AbsolutePath TestResultsDirectory => ArtifactsDirectory / "TestResults";
    AbsolutePath TestResultFilePath => TestResultsDirectory / $"{Parameters.Title}.trx";

    Target BuildVersion => _ => _
        .Executes(() =>
        {
            Information("Building version {0} of Rocket.Surgery.Azure.Sync ({1}) using version {2} of Nuke.",
                GitVersion.NuGetVersionV2 ?? GitVersion.NuGetVersion,
                Configuration,
                typeof(NukeBuild).Assembly.GetName().Version.ToString());
        });

    Target ShowInfo => _ => _
        .Executes(() =>
        {
            Information("Configuration: {0}", Configuration);

            Information("RepositoryName: {0}", Parameters.RepositoryName);
            Information("IsMainRepo: {0}", Parameters.IsMainRepo);
            Information("IsMasterBranch: {0}", Parameters.IsMasterBranch);
            Information("IsDevelopmentBranch: {0}", Parameters.IsDevelopmentBranch);
            Information("IsReleaseBranch: {0}", Parameters.IsReleaseBranch);
            Information("IsPullRequest: {0}", Parameters.IsPullRequest);

            Information("IsLocalBuild: {0}", Parameters.IsLocalBuild);
            Information("IsRunningOnUnix: {0}", Parameters.IsRunningOnUnix);
            Information("IsRunningOnWindows: {0}", Parameters.IsRunningOnWindows);
            Information("IsRunningOnAzureDevOps: {0}", Parameters.IsRunningOnAzureDevOps);

            Information("IsReleasable: {0}", Parameters.IsReleasable);
            Information("ShouldPublishMyGet: {0}", Parameters.ShouldPublishMyGet);
            Information("ShouldPublishNuGet: {0}", Parameters.ShouldPublishNuGet);
        });

    Target Clean => _ => _
        .DependsOn(ShowInfo)
        .Executes(() =>
        {
            DeleteDirectories(GlobDirectories(SourceDirectory, "**/bin", "**/obj"));
            DeleteDirectories(GlobDirectories(TestsDirectory, "**/bin", "**/obj"));
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(restore => restore.SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(build =>
                build
                    .SetProjectFile(Solution)
                    .SetConfiguration(Configuration)
                    .SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
                    .SetFileVersion(GitVersion.GetNormalizedFileVersion())
                    .SetInformationalVersion(GitVersion.InformationalVersion)
                    .EnableNoRestore());
        });

    Target UnitTests => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            EnsureExistingDirectory(TestResultsDirectory);

            foreach (var project in Solution.AllProjects.Where(x => x.Name.Contains("Tests")))
            {
                Information("Test: {0}", project.Name);
                DotNetTest(test =>
                    test
                        .SetProjectFile(project)
                        .SetConfiguration(Configuration)
                        .SetResultsDirectory(TestResultsDirectory)
                        .SetLogger($"trx;LogFileName={Parameters.Title}.trx")
                        .EnableNoBuild()
                        .EnableNoRestore());
            }
        });

    Target Pack => _ => _
        .DependsOn(Benchmarks)
        .Executes(() =>
        {
            DotNetPack(pack => pack.SetOutputDirectory(PackageDirectory));
        });

    Target PublishToMyGet => _ => _
        .DependsOn(Pack)
        .OnlyWhenDynamic(() => Parameters.ShouldPublishMyGet)
        .Executes(() =>
        {
            DotNetNuGetPush(args =>
                args
                    .SetApiKey(EnvironmentVariable(Parameters.MyGetApiKey))
                    .SetSource(EnvironmentVariable(Parameters.MyGetSource))
                    .SetTargetPath(string.Empty));
        });

    Target PublishToNuGet => _ => _
        .DependsOn(PublishToMyGet)
        .OnlyWhenDynamic(() => Parameters.ShouldPublishNuGet)
        .Executes(() =>
        {
            DotNetNuGetPush(args =>
                args
                    .SetApiKey(EnvironmentVariable(Parameters.NuGetApiKey))
                    .SetSource(EnvironmentVariable(Parameters.NuGetSource))
                    .SetTargetPath(string.Empty));
        });
}
