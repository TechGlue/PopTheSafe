using Microsoft.Extensions.Time.Testing;
using Safe.Models;

namespace SafeTesting;

public class SafeTests
{
    private readonly FakeTimeProvider _fakeTimeProvider = new();
    
    [Theory]
    [InlineData("1")]
    [InlineData("01")]
    [InlineData("123")]
    [InlineData("9999")]
    public void CalculateAdminCode_FourDigitPassword_ReturnsFourDigitPassword(string password)
    {
        // Arrange 
        MySafe hotelSafe = new MySafe(_fakeTimeProvider, "MySafe");

        // Act 
        string adminCode = hotelSafe.CalculateAdminCode(password);

        // Assert
        Assert.Equal(4, adminCode.Length);
    }

    [Fact]
    public void CalculateAdminCode_FourDigitPassword_ThrowsArgumentException()
    {
        // Arrange 
        MySafe hotelSafe = new MySafe(_fakeTimeProvider, "MySafe");
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
        MySafe hotelSafe = new MySafe(_fakeTimeProvider, "MySafe");

        // Anything from 0 to 751 is banned and will fail the test
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
        MySafe hotelSafe = new MySafe(_fakeTimeProvider, "MySafe");

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
        MySafe hotelSafe = new MySafe(_fakeTimeProvider, "MySafe");

        // Act 
        bool output = hotelSafe.VerifyFourDigitCode(digits);

        // Assert
        Assert.False(output);
    }
    
    [Theory]
    [InlineData("0001")]
    [InlineData("0003")]
    [InlineData("0900")]
    public void ChangeSafePassword_GivenValidDigits_ChangesSafePassword(string digits)
    {
        // Arrange 
        MySafe hotelSafe = new MySafe(_fakeTimeProvider, "MySafe");

        // Act 
        hotelSafe.ChangeSafePassword(digits);

        // Assert
        Assert.Equal(digits, hotelSafe.Password);
    }
    
    [Fact]
    public void ChangeSafePassword_GivenValidDigits_ChangesAdminCode()
    {
        // Arrange 
        MySafe hotelSafe = new MySafe(_fakeTimeProvider, "MySafe");

        // Act 
        hotelSafe.ChangeSafePassword("0100");

        // Assert
        Assert.NotEqual("0000", hotelSafe.AdminPassword);
    }
    
    [Fact]
    public void ChangeSafePassword_GivenValidDigits_ThrowsException()
    {
        // Arrange 
        MySafe hotelSafe = new MySafe(_fakeTimeProvider, "MySafe");

        // Act 
        string myPassword = "myPassword";

        // Assert
        Assert.Throws<ArgumentException>(() => hotelSafe.ChangeSafePassword(myPassword));
    }
    
    [Fact]
    public void ChangeSafePassword_GivenValidDigits_ValidTimeUpdates()
    {
        // Arrange 
        FakeTimeProvider mockProvider = new FakeTimeProvider();
        
        // Create an Intitial time
        mockProvider.SetUtcNow(new DateTime( 2021, 1, 1, 1, 1, 1));
        
        MySafe hotelSafe = new MySafe(mockProvider, "MySafe");
        
        DateTimeOffset currentTime = mockProvider.GetUtcNow();

        // Act 
        hotelSafe.ChangeSafePassword("0001");
        
        // Update the time to showcase it's changed
        mockProvider.SetUtcNow(new DateTime( 2021, 1, 1, 12, 12, 12));
        
        hotelSafe.ChangeSafePassword("0000");

        // Assert
        Assert.NotEqual(currentTime, hotelSafe.LastPasswordUpdateTime);
        Assert.NotEqual(currentTime, hotelSafe.LastAdminPasswordUpdateTime);
        Assert.Equal(currentTime, hotelSafe.CreationTime );
        
    }
}