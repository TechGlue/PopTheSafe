// todo:
// -  Figure out the issue with the algorithm for generating the admin code 

using Safe;
using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger<MySafe> logger = factory.CreateLogger<MySafe>();
MySafe newSafe = new MySafe(TimeProvider.System, logger, "MySafe");

bool interactiveSafe = true;

while (interactiveSafe)
{
    Console.WriteLine("MySafe is in state: " + newSafe.SafeStateMachine.State);
    Console.WriteLine("Admin Code: " + newSafe.AdminPassword);

    if (newSafe.SafeIsLocked is false && newSafe.SafeInProgrammingMode is false)
    {
        Console.WriteLine(
            "1. Open MySafe Door\n2. Close MySafe Door\n3. Press Reset Code\n0. Exit");
        string? input = Console.ReadLine();

        if (input is not null)
        {
            Console.WriteLine("You entered: " + input);

            switch (input)
            {
                case "1":
                    newSafe.OpenSafeDoor();
                    break;
                case "2":
                    newSafe.CloseSafeDoor();
                    break;
                case "3":
                    newSafe.PressResetCode();
                    break;
                case "0":
                    interactiveSafe = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    else if (newSafe.SafeInProgrammingMode)
    {
        Console.WriteLine(
            "1. Open MySafe Door\n2. Close MySafe Door \n3. Enter new 4-digit safe pin\n0. Exit\n");
        string? input = Console.ReadLine();

        if (input is not null)
        {
            Console.WriteLine("You entered: " + input);

            switch (input)
            {
                case "1":
                    newSafe.OpenSafeDoor();
                    break;
                case "2":
                    newSafe.CloseSafeDoor();
                    break;
                case "3":
                    Console.WriteLine("Enter new 4-digit safe pin: ");
                    string? pin = Console.ReadLine();
                    if (pin is not null)
                    {
                        newSafe.EnterNewPin(pin);
                    }
                    break;
                case "0":
                    interactiveSafe = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
    else if (newSafe.SafeIsLocked && newSafe.SafeInProgrammingMode is false)
    {
        Console.WriteLine(
            "1. Open MySafe Door\n2. Close MySafe Door\n3. Unlock MySafe\n0. Exit");

        string? input = Console.ReadLine();

        if (input is not null)
        {
            Console.WriteLine("You entered: " + input);

            switch (input)
            {
                case "1":
                    newSafe.OpenSafeDoor();
                    break;
                case "2":
                    newSafe.CloseSafeDoor();
                    break;
                case "3":
                    Console.WriteLine("Enter 4-digit safe pin: ");
                    string? pin = Console.ReadLine();
                    if (pin != String.Empty && pin is not null)
                    {
                        newSafe.EnterSafeCode(pin);
                    }
                    break;
                case "0":
                    interactiveSafe = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}

Console.WriteLine("Exiting safe...");