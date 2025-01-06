using static Safe.MySafeHelper;

namespace SafeTesting;

public class MySafeHelperTests
{
    [Theory]
    [InlineData("1")]
    [InlineData("03")]
    [InlineData("900")]
    [InlineData("102323")]
    public void VerifyFourDigits_GivenInvalidFourDigitCode_ReturnsFalse(string digits)
    {
        // Arrange - Act 
        bool output = VerifyFourDigitCode(digits);

        // Assert
        Assert.False(output);
    }


    [Theory]
    [InlineData("1023")]
    [InlineData("0000")]
    [InlineData("9999")]
    public void VerifyFourDigits_GivenValidFourDigitCode_ReturnsTrue(string digits)
    {
        // Act 
        bool output = VerifyFourDigitCode(digits);

        // Assert
        Assert.True(output);
    }


    [Fact]
    public void CalculateAdminCode_FourDigitPassword_ThrowsArgumentException()
    {
        // Arrange 
        string password = "NotAPassword";

        // Assert
        Assert.Throws<ArgumentException>(() => CalculateAdminCode(password));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("01")]
    [InlineData("123")]
    [InlineData("9999")]
    public void CalculateAdminCode_FourDigitPassword_ReturnsFourDigitPassword(string password)
    {
        // Arrange 

        // Act 
        string adminCode = CalculateAdminCode(password);

        // Assert
        Assert.Equal(4, adminCode.Length);
    }

    [InlineData("01")]
    [InlineData("9999")]
    [InlineData("0250")]
    [InlineData("123")]
    [Theory(Skip = "Working through testing randomness")]
    public void CalculateAdminCode_FourDigitPassword_ReturnsFourDigitPasswordInRange(string safePassword)
    {
        // Arrange 

        // Anything from 0 to 751 is banned and will fail the test
        int passcode = Int32.Parse(safePassword);
        var limits = CalcLimits(passcode);

        // Act 
        string adminCode = CalculateAdminCode(safePassword);

        // Assert
        Assert.True(!(Enumerable.Range(limits.lowerLimit, passcode).Contains(Int32.Parse(adminCode)) &&
                      !Enumerable.Range(passcode, limits.upperLimits).Contains(Int32.Parse(adminCode))));
        Assert.Equal(4, adminCode.Length);
    }
}