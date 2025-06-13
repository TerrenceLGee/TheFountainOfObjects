using TheFountainOfObjectsReloaded.Options;
using TheFountainOfObjectsReloaded.Fountain;
using TheFountainOfObjectsReloaded.GamePlay;

string? input = GetInput();
int choice = GetChoice(input);
GameSize size = GetGameSize(choice);

if (size == GameSize.Invalid)
{
    Console.WriteLine("\nGoodbye!");
    ClearTheScreen();
    return;
}


Cavern cavern = new Cavern(size);

Game game = new Game(cavern);
ClearTheScreen();
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
