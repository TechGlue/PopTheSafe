using Safe.Models;

// Create a new safe
MySafe newSafe = new MySafe(TimeProvider.System, "MySafe");

// Open
Console.WriteLine("State: " + newSafe.machine.State);

// Open the safe door
newSafe.machine.Fire(SafeStates.Triggers.OpenSafeDoor);

// Check state
Console.WriteLine(newSafe.machine.State);

// Press reset code
newSafe.machine.Fire(SafeStates.Triggers.PressResetCode);

// Check state
Console.WriteLine(newSafe.machine.State);
