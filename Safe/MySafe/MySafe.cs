using Microsoft.Extensions.Logging;
using Stateless;
using static Safe.MySafeHelper;

namespace Safe;

public class MySafe : ISafe
{
    // SafeStateMachine fields
    public readonly StateMachine<SafeStates.State, SafeStates.Triggers> SafeStateMachine;

    private readonly StateMachine<SafeStates.State, SafeStates.Triggers>.TriggerWithParameters<string>
        _PinTrigger;

    private readonly ILogger<MySafe> _logger;

    private readonly TimeProvider _timeProvider;
    public string SafeName { get; private set; }
    public string Password { get; private set; } = "0000";
    public string EnteredPassword { get; private set; } = String.Empty;

    public string AdminPassword { get; private set; } = "0000";

    public DateTimeOffset CreationTime { get; private set; }
    public DateTimeOffset LastPasswordUpdateTime { get; private set; } = DateTime.Now;
    public DateTimeOffset LastAdminPasswordUpdateTime { get; private set; } = DateTime.Now;

    public MySafe(TimeProvider timeProvider, ILogger<MySafe> logger, string safeName)
    {
        _logger = logger;
        _timeProvider = timeProvider;
        SafeName = safeName;
        CreationTime = _timeProvider.GetUtcNow();

        SafeStateMachine = new StateMachine<SafeStates.State, SafeStates.Triggers>(SafeStates.State.SafeClosedUnlocked);

        // SafeStateMachine configuration flow 
        SafeStateMachine.Configure(SafeStates.State.SafeClosedUnlocked)
            .PermitReentry(SafeStates.Triggers.CloseSafeDoor)
            .Permit(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeOpenUnlocked)
            .Ignore(SafeStates.Triggers.PressResetCode); // ignore if the reset code is pressed

        SafeStateMachine.Configure(SafeStates.State.SafeOpenUnlocked)
            .PermitReentry(SafeStates.Triggers.OpenSafeDoor)
            .Permit(SafeStates.Triggers.PressResetCode, SafeStates.State.SafeInProgrammingModeOpen)
            .Permit(SafeStates.Triggers.CloseSafeDoor, SafeStates.State.SafeClosedUnlocked);

        SafeStateMachine.Configure(SafeStates.State.SafeInProgrammingModeOpen)
            .Permit(SafeStates.Triggers.CloseSafeDoor, SafeStates.State.SafeInProgrammingModeClosed)
            .PermitReentry(SafeStates.Triggers.OpenSafeDoor)
            .Ignore(SafeStates.Triggers.EnterNewPin);

        SafeStateMachine.Configure(SafeStates.State.SafeInProgrammingModeClosed)
            .Permit(SafeStates.Triggers.PressLock, SafeStates.State.SafeLocked)
            .Permit(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeInProgrammingModeOpen)
            .PermitReentry(SafeStates.Triggers.CloseSafeDoor);

        _PinTrigger =
            SafeStateMachine.SetTriggerParameters<string>(SafeStates.Triggers.SafeCodeEntered);

        SafeStateMachine.Configure(SafeStates.State.SafeLocked)
            .PermitIf(_PinTrigger, SafeStates.State.SafeLockedPinEntered, (string password) =>
            {
                EnteredPassword = password;
                return EnteredPassword != String.Empty;
            })
            .Ignore(SafeStates.Triggers.OpenSafeDoor)
            .Ignore(SafeStates.Triggers.CloseSafeDoor);

        SafeStateMachine.Configure(SafeStates.State.SafeLockedPinEntered)
            // permit only if the door open trigger is selected and the password is correct
            .PermitIf(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeOpenUnlocked,
                () => EnteredPassword == Password)
            .PermitIf(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeLocked,
                () =>
                {
                    Console.WriteLine("Invalid password. Dig deep to remember.");
                    return EnteredPassword != Password;
                })
            .Ignore(SafeStates.Triggers.EnterNewPin)
            .Ignore(SafeStates.Triggers.PressResetCode)
            .Ignore(SafeStates.Triggers.PressLock);
    }

    public void Open()
    {
        SafeStateMachine.Fire(SafeStates.Triggers.OpenSafeDoor);
    }

    public void Close()
    {
        SafeStateMachine.Fire(SafeStates.Triggers.CloseSafeDoor);
    }

    public void PressReset()
    {
        SafeStateMachine.Fire(SafeStates.Triggers.PressResetCode);
    }

    public void PressLock()
    {
        SafeStateMachine.Fire(SafeStates.Triggers.PressLock);
    }

    public void EnterCode(string password)
    {
        switch (SafeStateMachine.State)
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

                AdminPassword = CalculateAdminCode(password);
                Password = password;

                SafeStateMachine.Fire(SafeStates.Triggers.EnterNewPin);

                LastPasswordUpdateTime = _timeProvider.GetUtcNow();
                LastAdminPasswordUpdateTime = _timeProvider.GetUtcNow();
                break;

            case SafeStates.State.SafeLocked:
                // password entered but who's the imposter?
                SafeStateMachine.Fire(_PinTrigger, password);
                break;
        }
    }

    public string Describe() =>
        SafeStateMachine.State switch
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
}