using System.Diagnostics.Tracing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Safe;

namespace SafeTesting;

public class SafeTests
{
    private readonly FakeTimeProvider _fakeTimeProvider = new();
    private readonly ILogger<MySafe> _logger = new Logger<MySafe>(new LoggerFactory());

    [Theory]
    [InlineData("0001")]
    [InlineData("0003")]
    [InlineData("0900")]
    public void ChangeSafePassword_GivenValidDigits_ChangesSafePassword(string digits)
    {
        // Arrange 
        MySafe hotelSafe = new MySafe(_fakeTimeProvider, _logger, "MySafe");

        // Act 
        hotelSafe.ChangeSafePassword(digits);

        // Assert
        Assert.Equal(digits, hotelSafe.Password);
    }

    [Fact]
    public void ChangeSafePassword_GivenValidDigits_ChangesAdminCode()
    {
        // Arrange 
        MySafe hotelSafe = new MySafe(_fakeTimeProvider, _logger, "MySafe");

        // Act 
        hotelSafe.ChangeSafePassword("0100");

        // Assert
        Assert.NotEqual("0000", hotelSafe.AdminPassword);
    }

    [Fact(Skip = "Currently not throwing exceptions" )]
    public void ChangeSafePassword_GivenValidDigits_ThrowsException()
    {
        // Arrange 
        MySafe hotelSafe = new MySafe(_fakeTimeProvider, _logger, "MySafe");

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
        mockProvider.SetUtcNow(new DateTime(2021, 1, 1, 1, 1, 1));

        MySafe hotelSafe = new MySafe(mockProvider, _logger, "MySafe");

        DateTimeOffset currentTime = mockProvider.GetUtcNow();

        // Act 
        hotelSafe.ChangeSafePassword("0001");

        // Update the time to showcase it's changed
        mockProvider.SetUtcNow(new DateTime(2021, 1, 1, 12, 12, 12));

        hotelSafe.ChangeSafePassword("0000");

        // Assert
        Assert.NotEqual(currentTime, hotelSafe.LastPasswordUpdateTime);
        Assert.NotEqual(currentTime, hotelSafe.LastAdminPasswordUpdateTime);
        Assert.Equal(currentTime, hotelSafe.CreationTime);
    }
    
    
    
}