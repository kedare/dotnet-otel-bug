using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
Console.WriteLine("pre otel");

using var tp = Sdk.CreateTracerProviderBuilder()
            .AddSource("*")
            .SetErrorStatusOnException()
            .ConfigureResource(resource => resource
                .AddService(
                    "myapp",
                    serviceVersion: "1.0.0",
                    serviceInstanceId: Environment.MachineName
                )
                .AddAttributes(new Dictionary<string, object>
                {
                    { "git.repository_url", "https://github.com/kedare/dotnet-otel-bug" },
                    { "git.commit.sha", "testing" }
                })
            )
            .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri("http://localhost:4318");
                    opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                }
            )
            .Build();

var activitySource = new ActivitySource("myapp");
using var activity = activitySource.StartActivity();

Console.WriteLine("post otel");

activity?.Stop();
tp.ForceFlush();
