using AikoOS.Behavior.Context;
using AikoOS.Core.Models;

namespace AikoOS.App.Avatar;

public interface IAvatarStateMapper
{
    AikoAvatarState Map(
        CharacterRuntimeState state);
}