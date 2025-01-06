using Microsoft.Extensions.Logging;
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

    // MySafe properties + fields
    private readonly ILogger<MySafe> _logger;
    private readonly TimeProvider _timeProvider;
    public string SafeName { get; private set; }
    public string Password { get; private set; } = "0000";

    public string AdminPassword { get; private set; } = "0000";

    // modes for the client to interact with the safe
    public bool SafeInProgrammingMode { get; private set; } = false;
    public bool SafeIsLocked { get; private set; } = false;
    public DateTimeOffset CreationTime { get; private set; }
    public DateTimeOffset LastPasswordUpdateTime { get; private set; } = DateTime.Now;
    public DateTimeOffset LastAdminPasswordUpdateTime { get; private set; } = DateTime.Now;

    public MySafe(TimeProvider timeProvider, ILogger<MySafe> logger, string safeName)
    {
        _logger = logger;
        _timeProvider = timeProvider;
        SafeName = safeName;
        CreationTime = _timeProvider.GetUtcNow();

        // Default state for the safe is SafeClosedUnlocked
        // _logger.LogInformation("Creating a new safe with name: {SafeName}", SafeName);
        SafeStateMachine = new StateMachine<SafeStates.State, SafeStates.Triggers>(SafeStates.State.SafeClosedUnlocked);
        FetchSafeState();

        // SafeStateMachine configuration flow 
        SafeStateMachine.Configure(SafeStates.State.SafeClosedUnlocked)
            .PermitReentry(SafeStates.Triggers.CloseSafeDoor)
            .Permit(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeOpenUnlocked)
            .Ignore(SafeStates.Triggers.PressResetCode) // ignore if the reset code is pressed
            .OnEntry(OnSafeClosedUnlockedEntry)
            .OnExit(OnSafeClosedUnlockedExit);

        SafeStateMachine.Configure(SafeStates.State.SafeOpenUnlocked)
            .PermitReentry(SafeStates.Triggers.OpenSafeDoor)
            .Permit(SafeStates.Triggers.PressResetCode, SafeStates.State.SafeInProgrammingModeOpen)
            .Permit(SafeStates.Triggers.CloseSafeDoor, SafeStates.State.SafeClosedUnlocked)
            .OnEntry(OnSafeOpenUnlockedEntry)
            .OnExit(OnSafeOpenUnlockedExit);

        SafeStateMachine.Configure(SafeStates.State.SafeInProgrammingModeOpen)
            .Permit(SafeStates.Triggers.CloseSafeDoor, SafeStates.State.SafeInProgrammingModeClosed)
            .PermitReentry(SafeStates.Triggers.OpenSafeDoor)
            .Ignore(SafeStates.Triggers.EnterNewPin)
            .OnEntry(OnSafeOpenProgrammingModeEntry)
            .OnExit(OnSafeOpenProgrammingModeExit);

        SafeStateMachine.Configure(SafeStates.State.SafeInProgrammingModeClosed)
            .Permit(SafeStates.Triggers.EnterNewPin, SafeStates.State.SafeLocked)
            .Permit(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeInProgrammingModeOpen)
            .PermitReentry(SafeStates.Triggers.CloseSafeDoor)
            .OnEntry(OnSafeInProgrammingModeClosedEntry)
            .OnExit(OnSafeInProgrammingModeClosedExit);

        _changedNeededParameters =
            SafeStateMachine.SetTriggerParameters<string>(SafeStates.Triggers.CorrectSafeCodeEntered);

        SafeStateMachine.Configure(SafeStates.State.SafeLocked)
            .PermitIf(_changedNeededParameters, SafeStates.State.SafeClosedUnlocked,
                (string password) => password == Password || password == AdminPassword)
            .Ignore(SafeStates.Triggers.OpenSafeDoor)
            .Ignore(SafeStates.Triggers.CloseSafeDoor)
            .OnEntry(OnSafeClosedLockedEntry)
            .OnExit(OnSafeClosedLockedExitValidPassword);
    }

    private void OnSafeInProgrammingModeClosedEntry()
    {
        FetchSafeState();
        // _logger.LogInformation("MySafe entered SafeInProgrammingMode closed.");
    }

    private void OnSafeInProgrammingModeClosedExit()
    {
        DisableSafeProgrammingMode();
        FetchSafeState();
        // _logger.LogInformation("MySafe left SafeInProgrammingMode closed.");
    }

    private void OnSafeClosedLockedEntry()
    {
        EnableSafeLock();
        FetchSafeState();
        // _logger.LogInformation("MySafe entered SafeClosedLocked state.");
    }

    private void OnSafeClosedLockedExitValidPassword()
    {
        DisableSafeLock();
        FetchSafeState();
        // _logger.LogInformation("MySafe left SafeClosedLocked state.");
    }

    private void EnableSafeLock()
    {
        SafeIsLocked = true;
    }

    private void DisableSafeLock()
    {
        SafeIsLocked = false;
    }

    private void OnSafeOpenProgrammingModeEntry()
    {
        EnableSafeProgrammingMode();
        FetchSafeState();
        // _logger.LogInformation("MySafe entered SafeProgrammingMode state.");
    }

    private void OnSafeOpenProgrammingModeExit()
    {
        FetchSafeState();
        // _logger.LogInformation("MySafe left SafeProgrammingMode state.");
    }

    private void EnableSafeProgrammingMode()
    {
        SafeInProgrammingMode = true;
    }

    private void DisableSafeProgrammingMode()
    {
        SafeInProgrammingMode = false;
    }

    private void OnSafeOpenUnlockedExit()
    {
        FetchSafeState();
        // _logger.LogInformation("MySafe left SafeOpenUnlocked state.");
    }

    private void OnSafeOpenUnlockedEntry()
    {
        FetchSafeState();
        // _logger.LogInformation("MySafe entered SafeOpenUnlocked state.");
    }

    private void OnSafeClosedUnlockedExit()
    {
        FetchSafeState();
        // _logger.LogInformation("MySafe left SafeClosedUnlocked state.");
    }

    private void OnSafeClosedUnlockedEntry()
    {
        FetchSafeState();
        // _logger.LogInformation("MySafe left SafeClosedUnlocked state.");
    }

    // MySafeAPI
    public void FetchSafeState()
    {
        // _logger.LogInformation("MySafe {SafeName} is currently in {State} state", SafeName, SafeStateMachine.State);
    }

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
            // _logger.LogError("Invalid password format. Please enter a 4 digit password.");
            // throw new ArgumentException("Invalid password format. Please enter a 4 digit password.");
            return;
        }

        ChangeSafePassword(newPin);
        SafeStateMachine.Fire(SafeStates.Triggers.EnterNewPin);
    }

    public void EnterSafeCode(string safeCode)
    {
        if (VerifyFourDigitCode(safeCode) is false)
        {
            // _logger.LogError("Invalid password format. Please enter a 4 digit password.");
            // throw new ArgumentException("Invalid password format. Please enter a 4 digit password.");
            return;
        }

        try
        {
            SafeStateMachine.Fire(_changedNeededParameters, safeCode);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Invalid password. Please enter the correct password. \n {ex}");
            // _logger.LogError("Invalid password. Please enter the correct password. \n {exception}", ex);
        }
    }

    public void ChangeSafePassword(string password)
    {
        // Ensure the password is of 4 digits. throw exceptions
        if (VerifyFourDigitCode(password) is not true)
        {
            // _logger.LogError("Invalid password format. Please enter a 4 digit password.");
            // throw new ArgumentException("Invalid password format. Please enter a 4 digit password");
            return;
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