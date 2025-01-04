using System.Text;
using Safe.Models;
using Stateless;
using static Safe.MySafeHelper;

namespace Safe;

public class MySafe
{
    // SafeStateMachine fields
    public readonly StateMachine<SafeStates.State, SafeStates.Triggers> SafeStateMachine;
    private readonly StateMachine<SafeStates.State, SafeStates.Triggers>.TriggerWithParameters<string>
        _changedNeededParameters;
    
    // Safe properties + fields
    private readonly TimeProvider _timeProvider;
    public string SafeName { get; private set; }
    public string Password { get; private set; } = "0000";
    public string AdminPassword { get; private set; } = "0000";
    public DateTimeOffset CreationTime { get; private set; }
    public DateTimeOffset LastPasswordUpdateTime { get; private set; } = DateTime.Now;
    public DateTimeOffset LastAdminPasswordUpdateTime { get; private set; } = DateTime.Now;


    public MySafe(TimeProvider timeProvider, string safeName)
    {
        _timeProvider = timeProvider;
        SafeName = safeName;
        CreationTime = _timeProvider.GetUtcNow();
        // Default state for the safe is SafeClosedUnlocked
        SafeStateMachine = new StateMachine<SafeStates.State, SafeStates.Triggers>(SafeStates.State.SafeClosedUnlocked);

        // SafeStateMachine configuration flow 
        SafeStateMachine.Configure(SafeStates.State.SafeClosedUnlocked)
            .Permit(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeOpenUnlocked)
            .OnEntry(OnSafeClosedUnlockedEntry)
            .OnExit(OnSafeClosedUnlockedExit);

        SafeStateMachine.Configure(SafeStates.State.SafeOpenUnlocked)
            .Permit(SafeStates.Triggers.PressResetCode, SafeStates.State.SafeInProgrammingModeOpen)
            .OnEntry(OnSafeOpenUnlockedEntry)
            .OnExit(OnSafeOpenUnlockedExit);

        SafeStateMachine.Configure(SafeStates.State.SafeInProgrammingModeOpen)
            .Permit(SafeStates.Triggers.CloseSafeDoor, SafeStates.State.SafeInProgrammingModeClosed)
            .OnEntry(OnSafeOpenProgrammingModeEntry)
            .OnExit(OnSafeOpenProgrammingModeExit);

        SafeStateMachine.Configure(SafeStates.State.SafeInProgrammingModeClosed)
            .Permit(SafeStates.Triggers.EnterNewPin, SafeStates.State.SafeLocked)
            .OnEntry(OnSafeClosedLockedEntry)
            .OnExit(OnSafeClosedLockedExit);

        _changedNeededParameters =
            SafeStateMachine.SetTriggerParameters<string>(SafeStates.Triggers.CorrectSafeCodeEntered);

        SafeStateMachine.Configure(SafeStates.State.SafeLocked)
            .PermitIf(_changedNeededParameters, SafeStates.State.SafeClosedUnlocked,
                (string password) => password == Password)
            .OnEntry(OnSafeClosedLockedEntry)
            .OnExit(OnSafeLockedExitValidPassword);
    }

    private void OnSafeLockedExitValidPassword()
    {
        Console.WriteLine("Password is correct. Safe has now left SafeLocked state.");
    }

    private void OnSafeClosedLockedExit()
    {
        Console.WriteLine("Safe locked.");
    }

    private void OnSafeClosedLockedEntry()
    {
        Console.WriteLine("Safe is now locked and armed.");
    }

    private void OnSafeOpenProgrammingModeEntry()
    {
        Console.WriteLine("Safe has now entered SafeProgrammingMode state.");
    }

    private void OnSafeOpenProgrammingModeExit()
    {
        Console.WriteLine("Safe has now exited SafeProgrammingMode state.");
    }

    private void OnSafeOpenUnlockedExit()
    {
        Console.WriteLine("Safe has now left SafeOpenUnlocked state.");
    }

    private void OnSafeOpenUnlockedEntry()
    {
        Console.WriteLine("Safe has now entered SafeOpenUnlocked state.");
    }

    private void OnSafeClosedUnlockedExit()
    {
        Console.WriteLine("Safe has now left SafeClosedUnlocked state.");
    }

    private void OnSafeClosedUnlockedEntry()
    {
        Console.WriteLine("Safe has now entered SafeClosedUnlocked state.");
    }

    // MySafeAPI
    public void OpenSafeDoor()
    {
        SafeStateMachine.Fire(SafeStates.Triggers.OpenSafeDoor);
    }

    public void PressResetCode()
    {
        SafeStateMachine.Fire(SafeStates.Triggers.PressResetCode);
    }

    public void CloseSafeDoor()
    {
        SafeStateMachine.Fire(SafeStates.Triggers.CloseSafeDoor);
    }

    public void EnterNewPin(string newPin)
    {
        if (VerifyFourDigitCode(newPin) is false)
        {
            throw new ArgumentException("Invalid password format. Please enter a 4 digit password.");
        }

        ChangeSafePassword(newPin);
        SafeStateMachine.Fire(SafeStates.Triggers.EnterNewPin);
    }

    public void EnterSafeCode(string safeCode)
    {
        if (VerifyFourDigitCode(safeCode) is false)
        {
            throw new ArgumentException("Invalid password format. Please enter a 4 digit password.");
        }

        try
        {
            SafeStateMachine.Fire(_changedNeededParameters, safeCode);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Invalid Password" + ex.Message);
        }
    }

    public void ChangeSafePassword(string password)
    {
        // Ensure the password is of 4 digits. throw exceptions
        if (VerifyFourDigitCode(password) is not true)
        {
            throw new ArgumentException("Invalid password format. Please enter a 4 digit password");
        }

        // update the admin password for the safe
        AdminPassword = CalculateAdminCode(password);

        // update safe pass word with the user password 
        Password = password;

        // update the times for logging purposes
        LastPasswordUpdateTime = _timeProvider.GetUtcNow();
        LastAdminPasswordUpdateTime = _timeProvider.GetUtcNow();
    }
}