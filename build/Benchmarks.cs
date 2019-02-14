using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using System;
using System.Linq;
using Nuke.Common.ProjectModel;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.ControlFlow;

partial class Build
{
    Func<bool> ShouldRunBenchmarks => () => Parameters.IsReleasable;

    Target Benchmarks => _ => _
        // .OnlyWhenDynamic(ShouldRunBenchmarks)
        .DependsOn(UnitTests)
        .Executes(() =>
        {
            foreach (var project in BenchmarkSolution.AllProjects.Where(x => x.Name.Contains("Benchmarks")))
            {
                Information("Benchmarking: {0}", project.Name);
                DotNetTest(args => args
                    .SetProjectFile(project)
                    .SetResultsDirectory(BenchmarkResultsDirectory)
                    .SetConfiguration(Configuration.Release));
            }
        });
}