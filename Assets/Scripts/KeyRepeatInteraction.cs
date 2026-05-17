using UnityEngine;
using UnityEngine.InputSystem;

public class KeyRepeatInteraction : IInputInteraction
{
    public float RepeatDelay = 0.5f;
    public float RepeatInterval = 0.1f;
    public float PressPoint = 0;

    float pressPointOrDefault => PressPoint > 0 ? PressPoint : InputSystem.settings.defaultButtonPressPoint;
    float releasePointOrDefault => pressPointOrDefault * InputSystem.settings.buttonReleaseThreshold;

    double nextRepeatTime;

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
#else
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
    public static void Initialize()
    {
        InputSystem.RegisterInteraction<KeyRepeatInteraction>();
    }

    public void Process(ref InputInteractionContext context)
    {
        if (RepeatDelay <= 0 || RepeatInterval <= 0)
        {
            Debug.LogError("InitialDelayとRepeatIntervalは0より大きい値を設定してください。");
            return;
        }

        if (context.timerHasExpired)
        {
            if (context.time >= nextRepeatTime)
            {
                nextRepeatTime = context.time + RepeatInterval;

                context.PerformedAndStayPerformed();

                context.SetTimeout(RepeatInterval);
            }
            return;
        }

        switch (context.phase)
        {
            case InputActionPhase.Waiting:
                if (context.ControlIsActuated(pressPointOrDefault))
                {
                    context.Started();
                    context.PerformedAndStayPerformed();

                    nextRepeatTime = context.time + RepeatDelay;

                    context.SetTimeout(RepeatDelay);
                }
                break;
            case InputActionPhase.Performed:
                if (!context.ControlIsActuated(releasePointOrDefault))
                {
                    context.Canceled();
                }
                break;
        }
    }

    public void Reset()
    {
        nextRepeatTime = 0;
    }
}
