using Safe;
using Safe.Models;

// Create a new safe
MySafe newSafe = new MySafe(TimeProvider.System, "MySafe");

// Open
Console.WriteLine("State: " + newSafe.SafeStateMachine.State);

// Open the safe door
newSafe.OpenSafeDoor();

// Check state
Console.WriteLine(newSafe.SafeStateMachine.State);

// Press reset code
newSafe.PressResetCode();

// Check state
Console.WriteLine(newSafe.SafeStateMachine.State);

// Close the safe door
newSafe.CloseSafeDoor();

// Check state
Console.WriteLine(newSafe.SafeStateMachine.State);

// Enter new pin
newSafe.EnterNewPin("1234");

// Check state
Console.WriteLine(newSafe.SafeStateMachine.State);
Console.WriteLine("CurrentSafe Password: " + newSafe.Password);

// attempt to open the safe
newSafe.EnterSafeCode("3433");

// Check state
Console.WriteLine(newSafe.SafeStateMachine.State);

newSafe.EnterSafeCode("3431");

Console.WriteLine(newSafe.SafeStateMachine.State);

newSafe.EnterSafeCode("1234");

Console.WriteLine(newSafe.SafeStateMachine.State);
