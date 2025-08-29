// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Running;
using Microsoft.DotNet.Cli.Commands.Workload.List;
using Microsoft.NET.Sdk.WorkloadManifestReader;
// using VsWorkloads = Microsoft.DotNet.Cli.Commands.Workload.List.VisualStudioWorkloads2;

namespace Performance;

internal class Program
{
    static void Main(string[] args)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        // Do();
        BenchmarkRunner.Run(typeof(Program).Assembly);
        stopwatch.Stop();
        Console.WriteLine($"Benchmark completed in {stopwatch.ElapsedMilliseconds} ms");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Do()
    {
        Microsoft.DotNet.Cli.Program.Main(["--info"]);
    }
}

[MemoryDiagnoser]
// [JitStatsDiagnoser]
public class InfoTests
{
    private static readonly string[] s_args = ["--info"];

    // [Benchmark]
    public int RunInfoCommand()
    {
        return Microsoft.DotNet.Cli.Program.Main(s_args);
    }
}

[MemoryDiagnoser]
// [JitStatsDiagnoser]
public class VisualStudioWorkloadTests
{
    private static SdkDirectoryWorkloadManifestProvider s_manifestProvider = new(
        @"C:\Program Files\dotnet\",
        "10.0.100-preview.7.25380.108",
        @"C:\Users\jkuhne\.dotnet",
        @"D:\repos\sdk\global.json");

    private static WorkloadResolver s_workloadResolver = WorkloadResolver.Create(
        s_manifestProvider,
        @"C:\Program Files\dotnet\",
        "10.0.100-preview.7.25380.108",
        @"C:\Users\jkuhne\.dotnet");

    private static Dictionary<string, string> s_visualStudioWorkloadIds = VisualStudioWorkloads2.GetAvailableVisualStudioWorkloads(s_workloadResolver);

    [Benchmark(Baseline = true)]
    public InstalledWorkloadsCollection GetInstalledVsWorkloads()
    {
        InstalledWorkloadsCollection vsWorkloads = new();
        VisualStudioWorkloads.GetInstalledWorkloads(s_workloadResolver, vsWorkloads, s_visualStudioWorkloadIds);
        return vsWorkloads;
    }

    [Benchmark]
    public InstalledWorkloadsCollection GetInstalledVsWorkloads2()
    {
        InstalledWorkloadsCollection vsWorkloads = new();
        VisualStudioWorkloads2.GetInstalledWorkloads(s_workloadResolver, vsWorkloads, s_visualStudioWorkloadIds);
        return vsWorkloads;
    }
}
