using Safe.Models;

namespace SafeTesting;

public class SafeTests
{
    [Theory]
    [InlineData("1023")]
    [InlineData("0000")]
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