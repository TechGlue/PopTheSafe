using System.Text;
using Safe.Models;
using Stateless;

namespace Safe;

public class MySafe
{
    private readonly TimeProvider _timeProvider;

    public readonly StateMachine<SafeStates.State, SafeStates.Triggers> SafeStateMachine;

    private readonly StateMachine<SafeStates.State, SafeStates.Triggers>.TriggerWithParameters<string>
        _changedNeededParameters;

    public MySafe(TimeProvider timeProvider, string safeName)
    {
        _timeProvider = timeProvider;
        SafeName = safeName;
        CreationTime = _timeProvider.GetUtcNow();

        // initialize the machine to by default the safe is open unlocked
        SafeStateMachine = new StateMachine<SafeStates.State, SafeStates.Triggers>(SafeStates.State.SafeClosedUnlocked);

        // State machine configurations
        // 1. 
        SafeStateMachine.Configure(SafeStates.State.SafeClosedUnlocked)
            .Permit(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeOpenUnlocked)
            .OnEntry(OnSafeClosedUnlockedEntry)
            .OnExit(OnSafeClosedUnlockedExit);

        // 2.
        SafeStateMachine.Configure(SafeStates.State.SafeOpenUnlocked)
            .Permit(SafeStates.Triggers.PressResetCode, SafeStates.State.SafeInProgrammingModeOpen)
            .OnEntry(OnSafeOpenUnlockedEntry)
            .OnExit(OnSafeOpenUnlockedExit);


        // 3. 
        SafeStateMachine.Configure(SafeStates.State.SafeInProgrammingModeOpen)
            .Permit(SafeStates.Triggers.CloseSafeDoor, SafeStates.State.SafeInProgrammingModeClosed)
            .OnEntry(OnSafeOpenProgrammingModeEntry)
            .OnExit(OnSafeOpenProgrammingModeExit);

        // 4.
        SafeStateMachine.Configure(SafeStates.State.SafeInProgrammingModeClosed)
            .Permit(SafeStates.Triggers.EnterNewPin, SafeStates.State.SafeLocked)
            .OnEntry(OnSafeClosedLockedEntry)
            .OnExit(OnSafeClosedLockedExit);

        _changedNeededParameters = SafeStateMachine.SetTriggerParameters<string>(SafeStates.Triggers.CorrectSafeCodeEntered);

        SafeStateMachine.Configure(SafeStates.State.SafeLocked)
            .PermitIf(_changedNeededParameters, SafeStates.State.SafeClosedUnlocked,
                (string password) => password == Password)
            .OnEntry(OnSafeClosedLockedEntry)
            .OnExit(OnSafeLockedExitValidPassword);
    }

    // Entering and exiting logic for the states
    private void OnSafeClosedExitValidEntry()
    {
        Console.WriteLine("Safe locked");
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

    // API for the safe
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

    public string SafeName { get; set; }
    public string Password { get; private set; } = "0000";
    public string AdminPassword { get; private set; } = "0000";
    public DateTimeOffset CreationTime { get; private set; }
    public DateTimeOffset LastPasswordUpdateTime { get; private set; } = DateTime.Now;
    public DateTimeOffset LastAdminPasswordUpdateTime { get; private set; } = DateTime.Now;

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

    public string CalculateAdminCode(string password)
    {
        int userPassword = -1;

        if (Int32.TryParse(password, out userPassword) is false)
        {
            throw new ArgumentException("Invalid password. Not able to generate admin code");
        }

        var adminPassLimits = CalcLimits(userPassword);

        Random rnd = new Random();

        // Probably can optimize this block
        int adminPassGenerator = rnd.Next(0, 9999);

        while ((Enumerable.Range(adminPassLimits.lowerLimit, userPassword).Contains(adminPassGenerator) &&
                (Enumerable.Range(userPassword, adminPassLimits.upperLimits).Contains(adminPassGenerator))))
        {
            adminPassGenerator = rnd.Next(0, 9999);
        }

        // Fill in leading 0's 
        int curPasswordSize = adminPassGenerator.ToString().Length;

        StringBuilder adminPasswordBuilder = new StringBuilder();

        while (curPasswordSize < 4)
        {
            adminPasswordBuilder.Append(0);
            curPasswordSize++;
        }

        // Append the password or the final portion;
        adminPasswordBuilder.Append(adminPassGenerator);

        return adminPasswordBuilder.ToString();
    }

    public (int lowerLimit, int upperLimits) CalcLimits(int x)
    {
        // In a perfect world the lower and upper limit will be described. Keep in mind sometimes it will be greater. 
        int lowerLimit = x - 750;
        if (lowerLimit < 0)
        {
            // wrap around the difference
            lowerLimit += 9999;
        }

        int upperLimit = x + 750;

        if (upperLimit > 9999)
        {
            // wrap around the difference
            upperLimit -= 9999;
        }

        return (lowerLimit, upperLimit);
    }

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