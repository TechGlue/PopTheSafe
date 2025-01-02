using Safe.Models;

// Create a new safe
MySafe newSafe = new MySafe(TimeProvider.System, "MySafe");

// Open
Console.WriteLine(newSafe.machine.State);
