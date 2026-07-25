namespace AikoOS.Core.Models;

public sealed class UserSettings
{
    public string AssistantName { get; set; } = "Aiko";

    public bool StartWithWindows { get; set; }

    public bool MinimizeToTray { get; set; } = true;
}