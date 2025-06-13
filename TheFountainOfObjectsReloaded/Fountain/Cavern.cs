using System.Numerics;
using TheFountainOfObjectsReloaded.Options;

namespace TheFountainOfObjectsReloaded.Fountain;
public class Cavern
{
    private Random _random = new Random();
    public GameSize GameSize { get; init; }
    public GameItem[,] GameBoard { get; set; }
    public bool IsFountainEnabled { get; set; }

    public Cavern(GameSize gameSize)
    {
        GameSize = gameSize;
        GameBoard = InitializeGameBoard(gameSize);
        CreateEntrance();
        CreateFountain();
        CreatePits(gameSize);
        CreateMaelstroms(gameSize);
        CreateAmaroks(gameSize);
        IsFountainEnabled = false;
    }

    private GameItem[,] InitializeGameBoard(GameSize gameSize)
    {
        return new GameItem[(int)gameSize, (int)gameSize];
    }

    private bool CanPlaceItemHere(int row, int col)
    {
        return GameBoard[row, col] == GameItem.Empty;
    }

    private (int row, int col) GetValidIndexes()
    {
        int row = _random.Next(0, GameBoard.GetLength(0));
        int col = _random.Next(0, GameBoard.GetLength(1));

        while (!CanPlaceItemHere(row, col))
        {
            row = _random.Next(0, GameBoard.GetLength(0));
            col = _random.Next(0, GameBoard.GetLength(1));
        }

        return (row, col);
    }
    private bool IsValidEntrance(int row, int col)
    {
        bool condition1 = (row == 0 && col == 0);
        bool condition2 = (row == GameBoard.GetLength(0) - 1 && col == 0);
        bool condition3 = (row == 0 && col == GameBoard.GetLength(1) - 1);
        bool condition4 = (row == GameBoard.GetLength(0) && col == GameBoard.GetLength(1) - 1);
        return condition1 || condition2 || condition3 || condition4;
    }
    private void CreateEntrance()
    {
        (int entranceRow, int entranceCol) = GetValidIndexes();
        while (!IsValidEntrance(entranceRow, entranceCol))
        {
            (entranceRow, entranceCol) = GetValidIndexes();
        }
        GameBoard[entranceRow, entranceCol] = GameItem.Entrance;

    }

    private void CreateFountain()
    {
        (int fountainRow, int fountainCol) = GetValidIndexes();
        GameBoard[fountainRow, fountainCol] = GameItem.Fountain;
     }

    private void CreatePits(GameSize gameSize)
    {
        int numberOfPitsToCreate = gameSize switch
        {
            GameSize.Small => 1,
            GameSize.Medium => 2,
            GameSize.Large => 3
        };

        for (int i = 0; i < numberOfPitsToCreate; i++)
        {
            (int pitRow, int pitCol) = GetValidIndexes();
            GameBoard[pitRow, pitCol] = GameItem.Pit;
        }
    }

    private void CreateMaelstroms(GameSize gameSize)
    {
        int numberOfMaelstromsToCreate = gameSize switch
        {
            GameSize.Small => 1,
            GameSize.Medium => 2,
            GameSize.Large => 3
        };

        for (int i = 0; i < numberOfMaelstromsToCreate; i++)
        {
            (int maelstromRow, int maelstromCol) = GetValidIndexes();
            GameBoard[maelstromRow, maelstromCol] = GameItem.Maelstrom;
        }
    }

    private void CreateAmaroks(GameSize gameSize)
    {
        int numberOfAmaroksToCreate = gameSize switch
        {
            GameSize.Small => 1,
            GameSize.Medium => 2,
            GameSize.Large => 3
        };

        for (int i = 0; i < numberOfAmaroksToCreate; i++)
        {
            (int amarokRow, int amarokCol) = GetValidIndexes();
            GameBoard[amarokRow, amarokCol] = GameItem.Amarok;
        }
    }

    public void PrintGameBoard()
    {
        for (int i = 0; i < GameBoard.GetLength(0); i++)
        {
            for (int j = 0; j < GameBoard.GetLength(1); j++)
            {
                Console.Write(" " + GameBoard[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    private bool IsValidMove(int row, int col)
    {
        bool condition1 = (row >= 0 && row < GameBoard.GetLength(0));
        bool condition2 = (col >= 0 && col < GameBoard.GetLength(1));

        return condition1 && condition2;
    }

    private (int row, int col)[] GetAllIndexesOfItem(GameItem item)
    {
        int arraySize = GetCountOfItem(item);
        (int row, int col)[] indexes = new (int row, int col)[arraySize];
        int arrayIndex = 0;

        for (int i = 0; i < GameBoard.GetLength(0); i++)
        {
            for (int j = 0; j < GameBoard.GetLength(1); j++)
            {
                if (GameBoard[i, j] == item)
                {
                    indexes[arrayIndex++] = (i, j);
                }
            }
        }

        return indexes;
    }

    private bool IsCardinalAdjacency(int row, int col, GameItem item)
    {
        (int itemRow, int itemCol)[] indexes = GetAllIndexesOfItem(item);

        for (int i = 0; i < indexes.Length; i++)
        {
            (int itemRow, int itemCol) = indexes[i];
            bool condition1 = (row == itemRow) && (Math.Abs(col - itemCol) == 1);
            bool condition2 = (Math.Abs(row - itemRow) == 1) && (col == itemCol);
            if (condition1 || condition2)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsDiagonalAdjacency(int row, int col, GameItem item)
    {
        (int itemRow, int itemCol)[] indexes = GetAllIndexesOfItem(item);

        for (int i = 0; i < indexes.Length; i++)
        {
            (int itemRow, int itemCol) = indexes[i];
            bool condition1 = (Math.Abs(row - itemRow) == 1);
            bool condition2 = (Math.Abs(col - itemCol) == 1);

            if (condition1 && condition2)
            {
                return true;
            }
        }
        return false;
    }

    private int GetCountOfItem(GameItem item)
    {
        return GameBoard.Cast<object>()
            .OfType<GameItem>()
            .Count(i => i == item);
    }


    public bool IsAdjacent(int row, int col, GameItem item)
    {
        if (!IsValidMove(row, col))
        {
            return false;
        }

        return IsCardinalAdjacency(row, col, item) || IsDiagonalAdjacency(row, col, item);
    }

    public (int row, int col) GetItemIndexes(GameItem item)
    {
        int row = 0;
        int col = 0;
        for (int i = 0; i < GameBoard.GetLength(0); i++)
        {
            for (int j = 0; j < GameBoard.GetLength(1); j++)
            {
                if (GameBoard[i, j] == item)
                {
                    row = i;
                    col = j;
                    break;
                }
            }
        }

        return (row, col);
    }
}
