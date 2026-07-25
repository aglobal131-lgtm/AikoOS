namespace AikoOS.AI.Options;

public sealed class AIOptions
{
    public const string SectionName = "AI";

    public string Provider { get; set; } = "Gemini";

    public string Model { get; set; } = "gemini-2.5-flash";

    public string Endpoint { get; set; }
        = "https://generativelanguage.googleapis.com";

    public string ApiKey { get; set; } = string.Empty;
}