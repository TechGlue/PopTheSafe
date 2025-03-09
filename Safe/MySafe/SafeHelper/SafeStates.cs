namespace MySafe.SafeHelper;

public class SafeStates
{
    public enum State
    {
        SafeClosedUnlocked = 1,
        SafeOpenUnlocked = 2,
        SafeInProgrammingModeOpen = 3,
        SafeInProgrammingModeClosed = 4,
        SafeInProgrammingModePinEntered = 5,
        SafeLocked = 6,
        SafeLockedPinEntered = 7
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