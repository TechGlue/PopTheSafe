using Safe;

// manually inject adminCodeGenerator into the created safe
IAdminCodeGenerator adminCodeGenerator = new AdminCodeGenerator();
MySafe newSafe = new MySafe("MySafe", adminCodeGenerator);

// choices
var actions = new Dictionary<int, Func<ISafe, SafeResponse>>()
{
    { 1, safe => safe.Open() },
    { 2, safe => safe.Close() },
    {
        3, safe =>
        {
            Console.Write("\nEnter Safe PIN: ");
            var pin = Console.ReadLine() ?? "";
            SafeResponse safeResponse = new SafeResponse();

            safe.SetCode(pin, result => safeResponse = result);
            return safeResponse;
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

    var result = action(newSafe);

    if (!result.isSuccessful)
    {
        Console.WriteLine($"Failed: {result.isDetail}");
    }
}