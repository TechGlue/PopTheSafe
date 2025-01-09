using Stateless;

namespace Safe;

public class MySafe : ISafe
{
    // SafeStateMachine fields
    private readonly StateMachine<SafeStates.State, SafeStates.Triggers> _safeStateMachine;

    private readonly StateMachine<SafeStates.State, SafeStates.Triggers>.TriggerWithParameters<string>
        _pinTrigger;

    private readonly IAdminCodeGenerator _adminCodeGenerator;

    // Safe fields and properties
    public string SafeName { get; private set; }
    private string Password { get; set; } = String.Empty;
    private string EnteredPassword { get; set; } = String.Empty;

    private string _adminPassword = String.Empty;

    public MySafe(string safeName)
    {
        SafeName = safeName;

        // SafeStateMachine initialization
        _safeStateMachine =
            new StateMachine<SafeStates.State, SafeStates.Triggers>(SafeStates.State.SafeClosedUnlocked);
        _adminCodeGenerator = new AdminCodeGenerator();

        // SafeStateMachine configuration flow 
        _safeStateMachine.Configure(SafeStates.State.SafeClosedUnlocked)
            .Permit(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeOpenUnlocked);

        _safeStateMachine.Configure(SafeStates.State.SafeOpenUnlocked)
            .PermitReentry(SafeStates.Triggers.OpenSafeDoor)
            .Permit(SafeStates.Triggers.PressResetCode, SafeStates.State.SafeInProgrammingModeOpen)
            .Permit(SafeStates.Triggers.CloseSafeDoor, SafeStates.State.SafeClosedUnlocked);

        _safeStateMachine.Configure(SafeStates.State.SafeInProgrammingModeOpen)
            .Permit(SafeStates.Triggers.CloseSafeDoor, SafeStates.State.SafeInProgrammingModeClosed);


        _pinTrigger =
            _safeStateMachine.SetTriggerParameters<string>(SafeStates.Triggers.SafeCodeEntered);

        _safeStateMachine.Configure(SafeStates.State.SafeInProgrammingModeClosed)
            .PermitReentry(SafeStates.Triggers.EnterNewPin)
            .Permit(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeInProgrammingModeOpen)
            .PermitIf(SafeStates.Triggers.PressLock, SafeStates.State.SafeLocked, () => Password != String.Empty);

        _safeStateMachine.Configure(SafeStates.State.SafeLocked)
            .PermitIf(_pinTrigger, SafeStates.State.SafeLockedPinEntered, (string password) =>
            {
                EnteredPassword = password;
                return EnteredPassword != String.Empty;
            })
            .Ignore(SafeStates.Triggers.OpenSafeDoor)
            .Ignore(SafeStates.Triggers.CloseSafeDoor);

        _safeStateMachine.Configure(SafeStates.State.SafeLockedPinEntered)
            // permit only if the door open trigger is selected and the password is correct
            .PermitIf(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeOpenUnlocked,
                () => EnteredPassword == Password || EnteredPassword == _adminPassword)
            .PermitIf(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeLocked,
                () => EnteredPassword != Password && EnteredPassword != _adminPassword)
            .Ignore(SafeStates.Triggers.PressResetCode)
            .Ignore(SafeStates.Triggers.PressLock);
    }

    public void Open()
    {
        _safeStateMachine.Fire(SafeStates.Triggers.OpenSafeDoor);
    }

    public void Close()
    {
        _safeStateMachine.Fire(SafeStates.Triggers.CloseSafeDoor);
    }

    public void PressReset()
    {
        _safeStateMachine.Fire(SafeStates.Triggers.PressResetCode);
    }

    public void PressLock()
    {
        _safeStateMachine.Fire(SafeStates.Triggers.PressLock);
    }

    public void EnterCode(string password)
    {
        switch (_safeStateMachine.State)
        {
            case SafeStates.State.SafeInProgrammingModeOpen:
                Console.WriteLine("Safe door is open. Please close door to set a password.");
                break;
            case SafeStates.State.SafeInProgrammingModeClosed:
                if (VerifyFourDigitCode(password) is false)
                {
                    Console.WriteLine(
                        "Invalid password format. Look at your fingers as you type important passwords please.");
                    break;
                }

                _adminPassword = _adminCodeGenerator.CalculateAdminCode(password);
                Password = password;

                _safeStateMachine.Fire(SafeStates.Triggers.EnterNewPin);

                break;

            case SafeStates.State.SafeLocked:
                _safeStateMachine.Fire(_pinTrigger, password);
                break;
        }
    }

    public string Describe() =>
        _safeStateMachine.State switch
        {
            SafeStates.State.SafeClosedUnlocked =>
                "The safe is closed but a deep, deep fear in you leads you to believe it is not locked",
            SafeStates.State.SafeOpenUnlocked =>
                "The safe is open and unlocked; it is the least safe safe you have ever seen",
            SafeStates.State.SafeInProgrammingModeOpen =>
                "The safe door is open, it is currently unlocked and in programming mode",
            SafeStates.State.SafeInProgrammingModeClosed =>
                "The safe door is closed, it is currently unlocked and in programming mode",
            SafeStates.State.SafeLocked =>
                "The safe is locked and all your carefully collected useless junk is secure",
            SafeStates.State.SafeLockedPinEntered =>
                "The safe is locked with a pin entered. Is it the correct pin? idk try opening the safe",
            _ => "Not sure how you got here. But we're here."
        };

    public bool VerifyFourDigitCode(string password)
    {
        string pass = password.Trim();

        int length = pass.Length;

        if (length != 4)
        {
            return false;
        }

        return true;
    }
}