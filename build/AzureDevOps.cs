using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using GlobExpressions;
using Nuke.Common;

partial class Build
{
    Func<bool> IsRunningOnAzureDevOps => () => Parameters.IsRunningOnAzureDevOps;

    Func<bool> ShouldPublishTestResults => () => Parameters.ShouldPublishTestResults;

    Target PrintAzureDevOpsEnvironment => _ => _
        .Before(Clean)
        .DependsOn(ShowInfo)
        .OnlyWhenStatic(IsRunningOnAzureDevOps)
        .Executes(() =>
        {
            Information("AGENT_ID: {0}", EnvironmentVariable("AGENT_ID"));
            Information("AGENT_NAME: {0}", EnvironmentVariable("AGENT_NAME"));
            Information("AGENT_VERSION: {0}", EnvironmentVariable("AGENT_VERSION"));
            Information("AGENT_JOBNAME: {0}", EnvironmentVariable("AGENT_JOBNAME"));
            Information("AGENT_JOBSTATUS: {0}", EnvironmentVariable("AGENT_JOBSTATUS"));
            Information("AGENT_MACHINE_NAME: {0}", EnvironmentVariable("AGENT_MACHINE_NAME"));
            Information("\n");

            Information("BUILD_BUILDID: {0}", EnvironmentVariable("BUILD_BUILDID"));
            Information("BUILD_BUILDNUMBER: {0}", EnvironmentVariable("BUILD_BUILDNUMBER"));
            Information("BUILD_DEFINITIONNAME: {0}", EnvironmentVariable("BUILD_DEFINITIONNAME"));
            Information("BUILD_DEFINITIONVERSION: {0}", EnvironmentVariable("BUILD_DEFINITIONVERSION"));
            Information("BUILD_QUEUEDBY: {0}", EnvironmentVariable("BUILD_QUEUEDBY"));
            Information("\n");

            Information("BUILD_SOURCEBRANCHNAME: {0}", EnvironmentVariable("BUILD_SOURCEBRANCHNAME"));
            Information("BUILD_SOURCEVERSION: {0}", EnvironmentVariable("BUILD_SOURCEVERSION"));
            Information("BUILD_REPOSITORY_NAME: {0}", EnvironmentVariable("BUILD_REPOSITORY_NAME"));
            Information("BUILD_REPOSITORY_PROVIDER: {0}", EnvironmentVariable("BUILD_REPOSITORY_PROVIDER"));
        });

    Target UploadAzureDevOpsArtifacts => _ => _
        .Before(PublishToMyGet)
        .DependsOn(Pack)
        .OnlyWhenStatic(IsRunningOnAzureDevOps)
        .Executes(() =>
        {
            Information($"##vso[artifact.upload containerfolder=Packages;artifactname=Packages;]{PackageDirectory}");
        });

    Target PublishAzureDevOpsTestResults => _ => _
        .Before(PublishAzureDevOpsCodeCoverage)
        .DependsOn(UnitTests)
        .OnlyWhenStatic(ShouldPublishTestResults)
        .OnlyWhenStatic(IsRunningOnAzureDevOps)
        .Executes(() =>
        {
            IEnumerable<FileInfo> files = new DirectoryInfo(TestResultsDirectory).GlobFiles("**/*.trx");
            AzureDevOpsTestResultsCommand(files, $"{Parameters.Title} - {GitVersion.BranchName}", Configuration);
        });

    Target PublishAzureDevOpsCodeCoverage => _ => _
        .Before(PublishToMyGet)
        .DependsOn(UnitTests)
        .OnlyWhenStatic(ShouldPublishTestResults)
        .OnlyWhenDynamic(IsRunningOnAzureDevOps);

    Target AzureDevOps => _ => _
        .DependsOn(PrintAzureDevOpsEnvironment)
        .DependsOn(UploadAzureDevOpsArtifacts)
        .DependsOn(PublishAzureDevOpsTestResults)
        .DependsOn(PublishAzureDevOpsCodeCoverage)
        .DependsOn(PublishToNuGet)
        .OnlyWhenStatic(IsRunningOnAzureDevOps);
}
