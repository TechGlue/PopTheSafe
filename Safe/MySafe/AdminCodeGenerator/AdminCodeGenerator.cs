using System.Text;

namespace Safe;

public class AdminCodeGenerator : IAdminCodeGenerator
{
    public string CalculateAdminCode(string password)
    {
        int userPassword = -1;

        if (Int32.TryParse(password, out userPassword) is false)
        {
            throw new ArgumentException("Invalid password. Not able to generate admin code");
        }

        var adminPassLimits = CalcLimits(userPassword);

        Random rnd = new Random();

        int adminPassGenerator = 0;
        bool isInRange;

        do
        {
            adminPassGenerator = rnd.Next(0, 10000);

            // Check if the generated number falls within the valid range
            isInRange = adminPassLimits.lowerLimit <= adminPassLimits.upperLimits
                ? adminPassGenerator >= adminPassLimits.lowerLimit &&
                  adminPassGenerator <= adminPassLimits.upperLimits // Standard range
                : adminPassGenerator >= adminPassLimits.upperLimits &&
                  adminPassGenerator <= adminPassLimits.lowerLimit; // Wrapping range swap the limits
        } while (!isInRange);

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
}