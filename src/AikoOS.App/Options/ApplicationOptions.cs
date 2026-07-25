namespace AikoOS.App.Options;

public sealed class ApplicationOptions
{
    public const string SectionName = "Application";

    public string Name { get; init; } = "AikoOS";

    public string Version { get; init; } = "0.1.0";

    public string Environment { get; init; } = "Development";
}