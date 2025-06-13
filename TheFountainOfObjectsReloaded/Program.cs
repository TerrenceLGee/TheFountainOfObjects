using TheFountainOfObjectsReloaded.Options;
using TheFountainOfObjectsReloaded.Fountain;
using TheFountainOfObjectsReloaded.GamePlay;

string? input = GetInput();
int choice = GetChoice(input);
GameSize size = GetGameSize(choice);

Command[] commands = new Command[]
{
    Command.North,
    Command.South,
    Command.East,
    Command.West,
    Command.ShootNorth,
    Command.ShootSouth,
    Command.ShootEast,
    Command.ShootWest,
    Command.EnableFountain,
};

if (size == GameSize.Invalid)
{
    Console.WriteLine("\nGoodbye!");
    ClearTheScreen();
    return;
}

ClearTheScreen();
DisplayCommandMenu(commands);

Cavern cavern = new Cavern(size);
Console.WriteLine();

Game game = new Game(cavern);
game.Run();

int GetChoice(string? input)
{
    int choice;

    while (!int.TryParse(input, out choice))
    {
        Console.WriteLine("\nInvalid input! ");
        ClearTheScreen();
        input = GetInput();
    }

    return choice;
}

string? GetInput()
{
    PrintMenu();
    return Console.ReadLine();
}

void PrintMenu()
{
    Console.WriteLine("\nWelcome to the Fountain of Objects Game!");
    Console.WriteLine("Please make a selection to determine the size of your world");
    Console.WriteLine($"1. {GameSize.Small}");
    Console.WriteLine($"2. {GameSize.Medium}");
    Console.WriteLine($"3. {GameSize.Large}");
    Console.WriteLine($"0. Exit");
    Console.Write("\nPlease choose one of the preceding to begin your game: ");
}

void ClearTheScreen()
{
    Console.Write("\nPress any key to continue: ");
    Console.ReadKey();
    Console.Clear();
}

GameSize GetGameSize(int choice)
{
    return choice switch
    {
        1 => GameSize.Small,
        2 => GameSize.Medium, 
        3 => GameSize.Large,
        _ => GameSize.Invalid,
    };
}

void DisplayCommandMenu(Command[] commands)
{
    Console.WriteLine("Here are the commands that you are allowed to use");
    Console.WriteLine("Please make a note of them");
    for (int i = 0; i < commands.Length; i++)
    {
        Console.WriteLine($"{i + 1}. {GetCommandOptionsString(commands[i])}");
    }
    Console.WriteLine();
    ClearTheScreen();
}

string GetCommandOptionsString(Command command)
{
    return command switch
    {
        Command.North => "Move North",
        Command.South => "Move South",
        Command.East => "Move East",
        Command.West => "Move West",
        Command.ShootNorth => "Shoot North",
        Command.ShootSouth => "Shoot South",
        Command.ShootEast => "Shoot East",
        Command.ShootWest => "Shoot West",
        Command.EnableFountain => "Enable Fountain",
        _ => "",
    };
}