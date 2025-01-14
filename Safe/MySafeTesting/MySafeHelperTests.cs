using Safe;
using Xunit.Abstractions;

namespace SafeTesting;

public class MySafeHelperTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IAdminCodeGenerator _adminCodeGenerator = new AdminCodeGenerator();

    public MySafeHelperTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData("1")]
    [InlineData("03")]
    [InlineData("900")]
    [InlineData("102323")]
    public void VerifyFourDigits_GivenInvalidFourDigitCode_ReturnsFalse(string digits)
    {
        // Arrange

        MySafe testSafe = new MySafe(_adminCodeGenerator);

        // Act 
        bool output = testSafe.VerifyFourDigitCode(digits);

        // Assert
        Assert.False(output);
    }


    [Theory]
    [InlineData("1023")]
    [InlineData("0000")]
    [InlineData("9999")]
    public void VerifyFourDigits_GivenValidFourDigitCode_ReturnsTrue(string digits)
    {
        // Arrange
        MySafe testSafe = new MySafe(_adminCodeGenerator);

        // Act 
        bool output = testSafe.VerifyFourDigitCode(digits);

        // Assert
        Assert.True(output);
    }


    [Fact]
    public void CalculateAdminCode_FourDigitPassword_ThrowsArgumentException()
    {
        // Arrange 
        string password = "NotAPassword";
        AdminCodeGenerator adminCodeGenerator = new AdminCodeGenerator();

        // Act - Assert
        Assert.Throws<ArgumentException>(() => adminCodeGenerator.CalculateAdminCode(password));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("01")]
    [InlineData("123")]
    [InlineData("9999")]
    public void CalculateAdminCode_FourDigitPassword_ReturnsFourDigitPassword(string password)
    {
        // Arrange 
        AdminCodeGenerator adminCodeGenerator = new AdminCodeGenerator();

        // Act 
        string adminCode = adminCodeGenerator.CalculateAdminCode(password);

        // Assert
        Assert.Equal(4, adminCode.Length);
    }

    [InlineData("9999")]
    [InlineData("1250")]
    [Theory()]
    public void CalculateAdminCode_FourDigitPassword_ReturnsFourDigitPasswordInRange(string safePassword)
    {
        // Arrange 
        AdminCodeGenerator adminPassGenerator = new AdminCodeGenerator();
        int passcode = Int32.Parse(safePassword);
        var adminPassLimits = adminPassGenerator.CalcLimits(passcode);

        // Act 
        _testOutputHelper.WriteLine("Lower" + adminPassLimits.lowerLimit);
        _testOutputHelper.WriteLine("Upper" + adminPassLimits.upperLimits);

        string generatedAdminCode = adminPassGenerator.CalculateAdminCode(safePassword);
        int pin = Int32.Parse(generatedAdminCode);

        // Assert
        bool isInRange = adminPassLimits.lowerLimit <= adminPassLimits.upperLimits
            ? pin >= adminPassLimits.lowerLimit && pin <= adminPassLimits.upperLimits // Standard range
            : pin >= adminPassLimits.upperLimits && pin <= adminPassLimits.lowerLimit; // Wrapping range
        Assert.True(isInRange);
        Assert.Equal(4, generatedAdminCode.Length);
    }


    [Fact]
    public void CalcLimits_GivenNumber_ReturnsTupleWithinLimit()
    {
        // Arrange 
        AdminCodeGenerator adminCodeGenerator = new AdminCodeGenerator();
        int x = 1001;

        // Act 
        var limits = adminCodeGenerator.CalcLimits(x);

        // Assert
        Assert.IsType<(int, int)>(limits);
        Assert.Equal(1751, limits.upperLimits);
        Assert.Equal(251, limits.lowerLimit);
    }
}