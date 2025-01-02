namespace Safe.Models;

public class SafeStates{
    public enum State 
    {
        SafeClosedUnlocked,
        SafeOpenUnlocked,
        SafeInProgrammingMode,
        SafeLocked
    }

    public enum Triggers
    {
        OpenSafeDoor, 
        CloseSafeDoor, 
        PressResetCode, 
        EnterNewPin, 
        GenerateAdminCode, 
        IncorrectSafeCodeEntered, 
        CorrectSafeCodeEntered, 
    } 
};