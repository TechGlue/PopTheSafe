using System.Text;
using Stateless;
using Stateless.Graph;

namespace Safe.Models;

public class MySafe
{
    private readonly TimeProvider _timeProvider;

    public StateMachine<SafeStates.State, SafeStates.Triggers> machine;

    private readonly StateMachine<SafeStates.State, SafeStates.Triggers>.TriggerWithParameters<string>
        changedNeededParameters;

    public MySafe(TimeProvider timeProvider, string safeName)
    {
        _timeProvider = timeProvider;
        SafeName = safeName;
        CreationTime = _timeProvider.GetUtcNow();
        
        // init the machine by default the safe is open unlocked
        machine = new StateMachine<SafeStates.State, SafeStates.Triggers>(SafeStates.State.SafeClosedUnlocked);

        // State machine configurations
        // 1. 
        machine.Configure(SafeStates.State.SafeClosedUnlocked)
            .Permit(SafeStates.Triggers.OpenSafeDoor, SafeStates.State.SafeOpenUnlocked)
            .OnEntry(OnSafeClosedUnlockedEntry)
            .OnExit(OnSafeClosedUnlockedExit);

        // Revisit
        // changedNeededParameters = machine.SetTriggerParameters<string>();

        // 3.
        machine.Configure(SafeStates.State.SafeOpenUnlocked)
            .Permit(SafeStates.Triggers.PressResetCode, SafeStates.State.SafeInProgrammingMode)
            .OnEntry(OnSafeOpenUnlockedEntry)
            .OnExit(OnSafeOpenUnlockedExit);
        
        changedNeededParameters = machine.SetTriggerParameters<string>(SafeStates.Triggers.CloseSafeDoor);
    }

    private void OnSafeOpenUnlockedExit()
    {
        Console.Out.WriteLine("Safe has now left SafeOpenUnlocked state.");
    }

    private void OnSafeOpenUnlockedEntry()
    {
        Console.Out.WriteLine("Safe has now entered SafeOpenUnlocked state.");
    }

    private void OnSafeClosedUnlockedExit()
    {
        Console.Out.WriteLine("Safe has now left SafeClosedUnlocked state.");
    }

    private void OnSafeClosedUnlockedEntry()
    {
        Console.Out.WriteLine("Safe has now entered SafeClosedUnlocked state.");
    }
    
    public void OpenSafeDoor()
    {
        machine.Fire(SafeStates.Triggers.OpenSafeDoor);
    }
    
    public void PressResetCode()
    {
        machine.Fire(SafeStates.Triggers.PressResetCode);
    }
    
    public void CloseSafeDoor()
    {
        machine.Fire(SafeStates.Triggers.CloseSafeDoor);
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
            throw new ArgumentException("password is not valid ");
        }

        // update the admin password for the safe
        AdminPassword = CalculateAdminCode(password);

        // update safe pass word with the user password 
        Password = password;

        // update the times for logging purposes
        LastPasswordUpdateTime = _timeProvider.GetUtcNow();
        LastAdminPasswordUpdateTime = _timeProvider.GetUtcNow();
    }

    // Think about: Making the limits flexible to allow better testing. For a Proof of concept this is fine.   
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


    /* Criteria
     * The admin code doesn't appear to be closer than 750 to the chosen code.  So for example if I pick 1000 as the code, the admin code doesn't appear to be anywhere in the range 250-1000 or 1000-1750.  It wraps around so if I choose 0250 as the code,
     *
     * The case below is not correct it'd be 9499 - 0250
     * -- it wouldn't pick 9500-0250 or 250-1000 -- .
     *
     */
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