using Safe.Models;
using Xunit.Abstractions;

namespace SafeTesting;

public class SafeTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public SafeTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData("1")]
    [InlineData("01")]
    [InlineData("123")]
    [InlineData("9999")]
    public void CalculateAdminCode_FourDigitPassword_ReturnsFourDigitPassword(string password)
    {
        // Arrange 
        MySafe hotelSafe = new MySafe("MySafe");

        // Act 
        string adminCode = hotelSafe.CalculateAdminCode(password);

        // Assert
        Assert.Equal(4, adminCode.Length);
    }

    [Fact]
    public void CalculateAdminCode_FourDigitPassword_ThrowsArgumentException()
    {
        // Arrange 
        MySafe hotelSafe = new MySafe("MySafe");
        string password = "NotAPassword";

        // Assert
        Assert.Throws<ArgumentException>(() => hotelSafe.CalculateAdminCode(password));
    }


    [InlineData("01")]
    [InlineData("9999")]
    [InlineData("0250")]
    [InlineData("123")]
    [Theory]
    public void CalculateAdminCode_FourDigitPassword_ReturnsFourDigitPasswordInRange(string safePassword)
    {
        // Arrange 
        MySafe hotelSafe = new MySafe("MySafe");

        int passcode = Int32.Parse(safePassword);
        var limits = hotelSafe.CalcLimits(passcode);

        // Act 
        string adminCode = hotelSafe.CalculateAdminCode(safePassword);

        // Assert
        Assert.True(!(Enumerable.Range(limits.lowerLimit, passcode).Contains(Int32.Parse(adminCode)) &&
                      !Enumerable.Range(passcode, limits.upperLimits).Contains(Int32.Parse(adminCode))));
        Assert.Equal(4, adminCode.Length);
    }

    [Theory]
    [InlineData("1023")]
    [InlineData("0000")]
    [InlineData("9999")]
    public void VerifyFourDigits_GivenValidFourDigitCode_ReturnsTrue(string digits)
    {
        // Arrange 
        MySafe hotelSafe = new MySafe("MySafe");

        // Act 
        bool output = hotelSafe.VerifyFourDigitCode(digits);

        // Assert
        Assert.True(output);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("03")]
    [InlineData("900")]
    [InlineData("102323")]
    public void VerifyFourDigits_GivenInvalidFourDigitCode_ReturnsFalse(string digits)
    {
        // Arrange 
        MySafe hotelSafe = new MySafe("MySafe");

        // Act 
        bool output = hotelSafe.VerifyFourDigitCode(digits);

        // Assert
        Assert.False(output);
    }
}