using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using static Nuke.Common.EnvironmentInfo;

public partial class Build
{
    /// <summary>
    /// Logs the specified information.
    /// </summary>
    /// <param name="info">The information.</param>
    static void Log(string info) { Logger.Log(info); }

    /// <summary>
    /// Logs the specified debug message.
    /// </summary>
    /// <param name="info">The information.</param>
    static void Debug(string info) { Logger.Log(info); }

    /// <summary>
    /// Logs the specified debug message.
    /// </summary>
    /// <param name="info">The information.</param>
    /// <param name="args">The arguments.</param>
    static void Debug(string info, params object[] args) { Logger.Log(info, args); }

    /// <summary>
    /// Logs the specified information message.
    /// </summary>
    /// <param name="info">The information.</param>
    static void Information(string info) { Logger.Info(info); }

    /// <summary>
    /// Logs the specified information message.
    /// </summary>
    /// <param name="info">The information.</param>
    /// <param name="args">The arguments.</param>
    static void Information(string info, params object[] args) { Logger.Info(info, args); }

    /// <summary>
    /// Logs the specified warning message.
    /// </summary>
    /// <param name="info">The information.</param>
    static void Warning(string info) { Logger.Warn(info); }

    /// <summary>
    /// Logs the specified warning message.
    /// </summary>
    /// <param name="info">The information.</param>
    /// <param name="args">The arguments.</param>
    static void Warning(string info, params object[] args) { Logger.Warn(info, args); }

    /// <summary>
    /// Logs the Azure DevOps test results command.
    /// </summary>
    /// <param name="files">The files.</param>
    /// <param name="title">The title.</param>
    /// <param name="platform">The platform.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="testType">Type of the test.</param>
    static void AzureDevOpsTestResultsCommand(IEnumerable<FileInfo> files, string title, string platform = "x64", string configuration = "Release", string testType = "VSTest")
    {
        var resultFiles = string.Join(',', files.Select(x => x.FullName.Replace('\\', Path.DirectorySeparatorChar)));
        Information($"##vso[results.publish type={testType};mergeResults=false;platform={platform}4;config={configuration};runTitle='{title}';publishRunAttachments=true;resultFiles={resultFiles};]");
    }

    /// <summary>
    /// Environments the variable.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    static string EnvironmentVariable(string key) => Variable(key);

    /// <summary>
    /// Determines whether [has environment variable] [the specified key].
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>
    ///   <c>true</c> if [has environment variable] [the specified key]; otherwise, <c>false</c>.
    /// </returns>
    static bool HasEnvironmentVariable(string key) => Variable(key) != null;
}