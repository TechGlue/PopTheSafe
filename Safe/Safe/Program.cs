using Safe;
using Safe.Models;
using Microsoft.Extensions.Logging;

// Create a new safe
using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger<MySafe> logger = factory.CreateLogger<MySafe>();
MySafe newSafe = new MySafe(TimeProvider.System, logger, "MySafe");

// Open the safe door
newSafe.OpenSafeDoor();

// Press reset code
newSafe.PressResetCode();

// Close the safe door
newSafe.CloseSafeDoor();

// Enter new pin
newSafe.EnterNewPin("1234");

// attempt to open the safe
newSafe.EnterSafeCode("3433");

newSafe.EnterSafeCode("3431");

newSafe.EnterSafeCode("1234");