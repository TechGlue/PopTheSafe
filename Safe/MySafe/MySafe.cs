using Stateless;

namespace Safe;

public class MySafe : ISafe
{
    // SafeStateMachine fields
    private readonly StateMachine<SafeStates.State, SafeStates.Triggers> _safeStateMachine;

    private StateMachine<SafeStates.State, SafeStates.Triggers>.TriggerWithParameters<string, Action<SafeResponse>>
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
        ConfigurationStateMachine();
    }

    private void ConfigurationStateMachine()
    {
        _safeStateMachine.Configure(SafeStates.State.SafeClosedUnlocked)
            .Permit(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeOpenUnlocked);

        _safeStateMachine.Configure(SafeStates.State.SafeOpenUnlocked)
            .Permit(SafeStates.Triggers.PressResetCode, SafeStates.State.SafeInProgrammingModeOpen)
            .Permit(SafeStates.Triggers.CloseSafeDoor, SafeStates.State.SafeClosedUnlocked);

        _safeStateMachine.Configure(SafeStates.State.SafeInProgrammingModeOpen)
            .Permit(SafeStates.Triggers.CloseSafeDoor, SafeStates.State.SafeInProgrammingModeClosed);

        _pinTrigger =
            _safeStateMachine.SetTriggerParameters<string, Action<SafeResponse>>(SafeStates.Triggers.SafeCodeEntered);

        // this the block we are working on 
        _safeStateMachine.Configure(SafeStates.State.SafeInProgrammingModeClosed)
            .Permit(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeInProgrammingModeOpen)
            .PermitIf(
                _pinTrigger,
                SafeStates.State.SafeInProgrammingModePinEntered,
                (string password, Action<SafeResponse> safeResponse) =>
                {
                    if (string.IsNullOrEmpty(password))
                    {
                        safeResponse(SafeResponse.Fail("Passed in password is null or empty."));
                        return false;
                    }

                    // Generate admin password
                    _adminPassword = _adminCodeGenerator.CalculateAdminCode(password);


                    // Set user provided password
                    Password = password;
                    safeResponse(SafeResponse.Ok());
                    return true;
                }
            );


        _safeStateMachine.Configure(SafeStates.State.SafeInProgrammingModePinEntered)
            .Permit(SafeStates.Triggers.PressLock, SafeStates.State.SafeLocked);

        _safeStateMachine.Configure(SafeStates.State.SafeLocked)
            .PermitIf(_pinTrigger, SafeStates.State.SafeLockedPinEntered,
                (string password, Action<SafeResponse> safeResponse) =>
                {
                    if (string.IsNullOrEmpty(password))
                    {
                        
                        safeResponse(SafeResponse.Fail("Passed in password is null or empty."));
                        return false;
                    }

                    EnteredPassword = password;
                    
                    safeResponse(SafeResponse.Ok());
                    return true;
                });

        _safeStateMachine.Configure(SafeStates.State.SafeLockedPinEntered)
            // permit only if the door open trigger is selected and the password is correct
            .PermitIf(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeOpenUnlocked,
                () => EnteredPassword == Password || EnteredPassword == _adminPassword)
            .PermitIf(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeLocked,
                () => EnteredPassword != Password && EnteredPassword != _adminPassword);
    }

    public SafeResponse Open()
    {
        try
        {
            _safeStateMachine.Fire(SafeStates.Triggers.OpenSafeDoor);
            return SafeResponse.Ok();
        }
        catch (InvalidOperationException invalidOperation)
        {
            return SafeResponse.Fail(invalidOperation.Message);
        }
    }

    public SafeResponse Close()
    {
        try
        {
            _safeStateMachine.Fire(SafeStates.Triggers.CloseSafeDoor);
            return SafeResponse.Ok();
        }
        catch (InvalidOperationException invalidOperation)
        {
            return SafeResponse.Fail(invalidOperation.Message);
        }
    }

    public SafeResponse PressReset()
    {
        try
        {
            _safeStateMachine.Fire(SafeStates.Triggers.PressResetCode);
            return SafeResponse.Ok();
        }
        catch (InvalidOperationException invalidOperation)
        {
            return SafeResponse.Fail(invalidOperation.Message);
        }
    }

    public SafeResponse PressLock()
    {
        try
        {
            _safeStateMachine.Fire(SafeStates.Triggers.PressLock);
            return SafeResponse.Ok();
        }
        catch (InvalidOperationException invalidOperation)
        {
            return SafeResponse.Fail(invalidOperation.Message);
        }
    }

    public SafeResponse SetCode(string password, Action<SafeResponse> resultsHandler)
    {
        try
        {
            if (VerifyFourDigitCode(password) is false)
            {
                throw new ArgumentException("Invalid password format. Ensure it's numerical and 4-digits.");
            }

            _safeStateMachine.Fire(_pinTrigger, password, resultsHandler);

            resultsHandler(SafeResponse.Ok());
            return SafeResponse.Ok();
        }
        catch (Exception ex)
        {
            resultsHandler(SafeResponse.Fail(ex.Message));
            return SafeResponse.Fail(ex.Message);
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
            SafeStates.State.SafeInProgrammingModePinEntered =>
                "The safe is in programming mode with a pin entered. Are we going to lock the safe or keep it unlocked?",
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