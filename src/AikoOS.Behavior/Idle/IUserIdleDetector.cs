namespace AikoOS.Behavior.Idle;

public interface IUserIdleDetector
{
    TimeSpan GetIdleDuration();
}