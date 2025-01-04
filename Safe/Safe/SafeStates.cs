namespace Safe.Models;

public class SafeStates{
    public enum State 
    {
        SafeClosedUnlocked,
        SafeOpenUnlocked,
        SafeInProgrammingModeOpen,
        SafeInProgrammingModeClosed,
        SafeLocked
    }

    public enum Triggers
    {
        OpenSafeDoor, 
        CloseSafeDoor, 
        PressResetCode, 
        EnterNewPin, 
        CorrectSafeCodeEntered, 
    } 
};