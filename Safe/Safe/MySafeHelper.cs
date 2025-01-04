using System.Text;

namespace Safe;

public static class MySafeHelper
{
    public static string CalculateAdminCode(string password)
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

    public static (int lowerLimit, int upperLimits) CalcLimits(int x)
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

    public static bool VerifyFourDigitCode(string password)
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