using System.Text;
using Stateless;
using Stateless.Graph;

namespace Safe.Models;

public class MySafe
{
    public MySafe(string safeName)
    {
        SafeName = safeName;
    }

    public string SafeName { get; set; }

    // note the password can only be 4 digits don't want that being too long
    // don't need it in the constructor
    public string Password { get; private set; } = "0000";

    public string AdminPassword { get; private set; } = "0000";

    public DateTime CreationTime { get; private set; } = DateTime.Now;

    public DateTime LastPasswordUpdateTime { get; private set; } = DateTime.Now;

    public DateTime LastAdminPasswordUpdateTime { get; private set; } = DateTime.Now;

    // open or closed states - the trigger is a char
    public StateMachine<string, char> SafeState = new StateMachine<string, char>("open");

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
        LastPasswordUpdateTime = DateTime.Now;
        LastAdminPasswordUpdateTime = DateTime.Now;
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
     * it wouldn't pick 9500-0250 or 250-1000.
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