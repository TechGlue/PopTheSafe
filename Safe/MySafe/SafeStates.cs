namespace Safe;

public class SafeStates
{
    public enum State
    {
        SafeClosedUnlocked,
        SafeOpenUnlocked,
        SafeInProgrammingModeOpen,
        SafeInProgrammingModeClosed,
        SafeInProgrammingModePinEntered,
        SafeLocked,
        SafeLockedPinEntered
    }

    public enum Triggers
    {
        OpenSafeDoor,
        CloseSafeDoor,
        PressResetCode,
        PressLock,
        SafeCodeEntered,
    }
};