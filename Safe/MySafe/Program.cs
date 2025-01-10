using Safe;

MySafe newSafe = new MySafe("MySafe");

// choices
var actions = new Dictionary<int, Action<ISafe>>()
{
    { 1, safe => safe.Open() },
    { 2, safe => safe.Close() },
    {
        3, safe =>
        {
            Console.Write("\nEnter Safe PIN: ");
            var pin = Console.ReadLine();
            if (pin != null) safe.SetCode(pin);
        }
    },
    { 4, safe => safe.PressReset() },
    { 5, safe => safe.PressLock() },
};

while (true)
{
    Console.WriteLine("\n" + newSafe.Describe());
    Console.WriteLine("What will you do?");
    Console.WriteLine("1) Open the door");
    Console.WriteLine("2) Close the door");
    Console.WriteLine("3) Enter a pin");
    Console.WriteLine("4) Press reset");
    Console.WriteLine("5) Press lock");

    // choice then is the number 
    var action = int.TryParse(Console.ReadKey().KeyChar.ToString(), out int choice) switch
    {
        true => actions.ContainsKey(choice) switch
        {
            true => actions[choice],
            false => actions[-1],
        },
        false => actions[-1],
    };

    try
    {
        action(newSafe);
    }
    catch (InvalidOperationException)
    {
        Console.WriteLine("Unbelievable nonsense, what you've done here.\n");
    }
}