using AikoOS.AI.Brain.Planning;
using AikoOS.Behavior.Context;

namespace AikoOS.Runtime.Planning.Factories;

public sealed class PlanningContextFactory
{
    private readonly ICharacterContext _characterContext;

    public PlanningContextFactory(
        ICharacterContext characterContext)
    {
        _characterContext = characterContext;
    }

    public PlanningContext Create(
        string? userInput)
    {
        return new PlanningContext
        {
            CurrentState =
                _characterContext.Current.StateName,

            CurrentEmotion =
                _characterContext.Current.EmotionName,

            UserInput = userInput,

            Timestamp = DateTime.UtcNow
        };
    }
}