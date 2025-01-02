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
    public string Password { get; set; } = "0000";

    public string AdminPassword { get; private set; } = "0000";

    public DateTime CreationTime { get; private set; } = DateTime.Now;

    public DateTime LastPasswordUpdateTime { get; private set; } = DateTime.Now;
    
    public DateTime LastAdminPasswordUpdateTime { get; private set; } = DateTime.Now;

    // open or closed states - the trigger is a char
    public StateMachine<string, char> SafeState = new StateMachine<string, char>("open");

    
    public void ChangeSafePassword(string password)
    {
        // do work to ensure the password is of 4 digits. throw exceptions
        if (VerifyFourDigitCode(password) is not true)
        {
            throw new ArgumentException("password is not valid ");
        }
        
        // update the password 
        AdminPassword = CalculateAdminCode(password);
        
        // update pass word. 
        Password = password;
        
        // finally update the times 
        LastPasswordUpdateTime = DateTime.Now;
        LastAdminPasswordUpdateTime = DateTime.Now;
    }

    public string CalculateAdminCode(string password)
    {
        int userPassword = -1; 
        
        if (Int32.TryParse(password, out userPassword) is false)
        {
            throw new ArgumentException("Invalid password. Not able to generate admin code");
        }
        
        Func<int, (int lowerLimit, int upperLimits)> calclimits = ((x) =>
        {
            int lowerLimit = x - 750;
            if (lowerLimit < 0)
            {
                lowerLimit = 0;
            }

            int upperLimit = x + 750;

            if (upperLimit > 9999)
            {
                upperLimit = 9999;
            }

            return (lowerLimit,upperLimit);
        });

        var adminPassLimits = calclimits(userPassword);

        Random rnd = new Random();

        // Probably can optimize this block
        int adminPassGenerator = rnd.Next(0, 9999);

        while (adminPassGenerator < adminPassLimits.lowerLimit && adminPassGenerator > adminPassLimits.upperLimits)
        {
            adminPassGenerator = rnd.Next(0, 9999);
        }
        
        // fill in leading 0's 
        int curPasswordSize = adminPassGenerator.ToString().Length; 
        
        StringBuilder adminPasswordBuilder = new StringBuilder();
        
        while (curPasswordSize < 4)
        {
            adminPasswordBuilder.Append(0);
            curPasswordSize++;
        }
        
        // append the password or the final portion;
        adminPasswordBuilder.Append(adminPassGenerator);

        return adminPasswordBuilder.ToString();
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