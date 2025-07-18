﻿using TheFountainOfObjectsReloaded.Options;
using TheFountainOfObjectsReloaded.Fountain;

namespace TheFountainOfObjectsReloaded.GamePlay;
public class Game
{
    private Cavern _cavern;
    private GameItem _entrance = GameItem.Entrance;
    private GameItem _fountain = GameItem.Fountain;
    private readonly int _entranceRow;
    private readonly int _entranceCol;
    private readonly int _fountainRow;
    private readonly int _fountainCol;
    private int _currentRow;
    private int _currentCol;
    private int _shootingRow;
    private int _shootingCol;
    private int _perspectiveRow;
    private int _perspectiveCol;
    private int _arrowCount;
    private bool _playerWon = true;
    private bool _movedByMaelstrom = false;
    private Adjacency _adjacency;
    private int _rowIndexLimit;
    private int _colIndexLimit;
    private DateTime _playingTime;
    private TimeSpan _timeSpentPlaying;

    public Game(Cavern cavern)
    {
        _cavern = cavern;
        (_entranceRow, _entranceCol) = _cavern.GetItemIndexes(_entrance);
        (_fountainRow, _fountainCol) = _cavern.GetItemIndexes(_fountain);
        _rowIndexLimit = _cavern.GameBoard.GetLength(0);
        _colIndexLimit = _cavern.GameBoard.GetLength(1);
    }

    public void Run()
    {
        GameItem[] gameItems = new GameItem[3]
        {
            GameItem.Pit,
            GameItem.Amarok,
            GameItem.Maelstrom,
        };
        _currentRow = _entranceRow;
        _currentCol = _entranceCol;
        _arrowCount = 5;

        _playingTime = DateTime.Now;
        
        while (true)
        {
            _movedByMaelstrom = false;
            Console.WriteLine(PrintBorder());
            Console.WriteLine(AnnounceRoom(_currentRow, _currentCol));
            bool condition1 = _currentRow == _entranceRow;
            bool condition2 = _currentCol == _entranceCol;
            bool condition3 = _currentRow == _fountainRow;
            bool condition4 = _currentCol == _fountainCol;

            foreach (var item in gameItems)
            {
                if (IsSameSpotAsItem(_currentRow, _currentCol, item))
                {
                    if (item != GameItem.Maelstrom)
                    {
                        _playerWon = false;
                        Console.WriteLine(GetLosingMessage(item));
                    }
                    else if (item == GameItem.Maelstrom)
                    {
                        Console.WriteLine(GetLosingMessage(item));
                        (_currentRow, _currentCol) = MakeMoveFromMaelstrom(_currentRow, _currentCol);
                        _cavern.GameBoard[_currentRow, _currentCol] = GameItem.Empty;
                        (_perspectiveRow, _perspectiveCol) = MaelstromMakeMove(_currentRow, _currentCol);
                        _cavern.GameBoard[_perspectiveRow, _perspectiveCol] = GameItem.Maelstrom;
                        _movedByMaelstrom = true;
                        break;
                    }
                }
            }

            if (!_playerWon)
            {
                break;
            }

            if (_movedByMaelstrom)
            {
                continue;
            }

            foreach (var item in gameItems)
            {
                _adjacency = IsAdjacent(_currentRow, _currentCol, item);
                if (_adjacency != Adjacency.None)
                {
                    break;
                }
            }

            if (_adjacency != Adjacency.None)
            {
                Console.WriteLine(GetAdjacencyMessage(_adjacency));
            }

            if (condition1 || condition2 || condition3 || condition4)
            {
                Console.WriteLine(GetSpecialMessage(_currentRow, _currentCol, _cavern.IsFountainEnabled));
            }

            if (IsGameWon(_currentRow, _currentCol, _cavern.IsFountainEnabled))
            {
                _playerWon = true;
                break;
            }
            string? input = GetUserInput();
            Command command = GetCommand(input!);
            if (command == Command.Invalid)
            {
                Console.WriteLine("Invalid command! Try again");
                continue;
            }
            if (command == Command.EnableFountain)
            {
                if (CanFountainBeEnabled(_currentRow, _currentCol, _cavern.IsFountainEnabled))
                {
                    _cavern.IsFountainEnabled = true;
                }
            }
            else if (command == Command.ShootNorth || command == Command.ShootSouth
                || command == Command.ShootEast || command == Command.ShootWest)
            {
                if (_arrowCount > 0)
                {
                    (_shootingRow, _shootingCol) = EnableCommand(_currentRow, _currentCol, command);

                    if (ShootEnemy(_shootingRow, _shootingCol))
                    {
                        Console.WriteLine("Enemy killed!");
                    }
                    else
                    {
                        Console.WriteLine("There was nothing in the room to kill.");
                    }
                    _arrowCount--;
                }
                else
                {
                    Console.WriteLine("Sorry you are all out of arrows!");
                }
                
            }
            else
            {
                (_perspectiveRow, _perspectiveCol) = EnableCommand(_currentRow, _currentCol, command);
                if (IsOutOfBounds(_perspectiveRow, _perspectiveCol))
                {
                    (_perspectiveRow, _perspectiveCol) = GetProperIndexes(_perspectiveRow, _perspectiveCol);
                }
                _currentRow = _perspectiveRow;
                _currentCol = _perspectiveCol;
            }
        }

        _timeSpentPlaying = DateTime.Now - _playingTime;

        if (!_playerWon)
        {
            Console.WriteLine("\nSorry you were killed and have lost the game!");
            Console.WriteLine("Better luck next time!");
        }
        else
        {
            Console.WriteLine("\nCongratulations you are a true programmer!");
            Console.WriteLine("You have done an incredible job!");
        }

        Console.WriteLine($"The time it took you to play the game:\n" +
            $"{_timeSpentPlaying.Hours}h hours {_timeSpentPlaying.Minutes}m minutes {_timeSpentPlaying.Seconds} seconds\n");
    }

    private bool IsOutOfBounds(int row, int col)
    {
        bool condition1 = (row < 0 || row > _cavern.GameBoard.GetLength(0) - 1);
        bool condition2 = (col < 0 || col > _cavern.GameBoard.GetLength(1) - 1);

        return condition1 || condition2;
    }

    private (int row, int col) GetProperIndexes(int badRow, int badCol)
    {
        int row = (badRow % _rowIndexLimit + _rowIndexLimit) % _rowIndexLimit;
        int col = (badCol % _colIndexLimit + _colIndexLimit) % _colIndexLimit;

        return (row, col);
    }

    private (int row, int col) MakeMoveFromMaelstrom(int originalRow, int originalCol)
    {
        int row = (--originalRow);
        int col = (originalCol += 2);

        while (IsOutOfBounds(row, col))
        {
            (row, col) = GetProperIndexes(row, col);
        }
        return (row, col);
    }

    private (int row, int col) MaelstromMakeMove(int originalRow, int originalCol)
    {
        int row = (++originalRow);
        int col = (originalCol -= 2);

        while (IsOutOfBounds(row, col))
        {
            (row, col) = GetProperIndexes(row, col);
        }
        return (row, col);
    }

    private string AnnounceRoom(int row, int col)
    {
        return $"You are in the room at (Row={row}, Column={col})";
    }

    private Command GetCommand(string input)
    {
        return input.ToLower() switch
        {
            "move north" => Command.North,
            "move south" => Command.South,
            "move east" => Command.East,
            "move west" => Command.West,
            "shoot north" => Command.ShootNorth,
            "shoot south" => Command.ShootSouth,
            "shoot east" => Command.ShootEast,
            "shoot west" => Command.ShootWest,
            "enable fountain" => Command.EnableFountain,
            _ => Command.Invalid,
        };
    }

    private (int row, int col) EnableCommand(int row, int col, Command command)
    {
        return command switch
        {
            Command.North or Command.ShootNorth => (--row, col),
            Command.South or Command.ShootSouth => (++row, col),
            Command.East or Command.ShootEast => (row, ++col),
            Command.West or Command.ShootWest => (row, --col),
        };
    }


    private bool ShootEnemy(int row, int col)
    {
        int shootingRow = row;
        int shootingCol = col;

        while (IsOutOfBounds(shootingRow, shootingCol))
        {
            (shootingRow, shootingCol) = GetProperIndexes(shootingRow, shootingCol);
        }

        if (_cavern.GameBoard[shootingRow, shootingCol] != GameItem.Empty 
            && _cavern.GameBoard[shootingRow, shootingCol] != GameItem.Pit)
        {
            _cavern.GameBoard[shootingRow, shootingCol] = GameItem.Empty;
            return true;
        }
        return false;
    }

    private bool CanFountainBeEnabled(int row, int col, bool isEnabled)
    {
        return row == _fountainRow && col == _fountainCol && !isEnabled;
    }

    private string? GetUserInput()
    {
        Console.Write("What do you want to do? ");
        return Console.ReadLine();
    }

    private string PrintBorder()
    {
        return new string('-', 30);
    }

    private bool IsGameWon(int row, int col, bool isEnabled)
    {
        return (row == _entranceRow && col == _entranceCol && isEnabled);
    }

    private string GetAdjacencyMessage(Adjacency item)
    {
        return item switch
        {
            Adjacency.Pit => "You feel a draft. There is a pit in a nearby room.",
            Adjacency.Maelstrom => "You hear the growling and groaning of a maelstrom nearby.",
            Adjacency.Amarok => "You can smell the rotten stench of an amarok in a nearby room.",
            Adjacency.None => "",
        };
    }

    private Adjacency IsAdjacent(int row, int col, GameItem item)
    {
        if (_cavern.IsAdjacent(row, col, item))
        {
            return item switch
            {
                GameItem.Pit => Adjacency.Pit,
                GameItem.Maelstrom => Adjacency.Maelstrom,
                GameItem.Amarok => Adjacency.Amarok,
            };
        }
        return Adjacency.None;
    }

    private string GetSpecialMessage(int row, int col, bool isEnabled)
    {
        if (row == _entranceRow && col == _entranceCol && isEnabled)
        {
            return "The Fountain of Objects has been reactivated, and you have escaped with your life!\nYou win!\n";
        }
        else if (row == _entranceRow && col == _entranceCol && !isEnabled)
        {
            return "You see light coming from the cavern entrance.\n";
        }
        else if (row == _fountainRow && col == _fountainCol && isEnabled)
        {
            return "You hear the rushing waters from the Fountain of Objects. It has been reactivated! \n";
        }
        else if (row == _fountainRow && col == _fountainCol && !isEnabled)
        {
            return "You hear water dripping in this room. The Fountain of Objects is here!\n";
        }
        else
        {
            return string.Empty;
        }
    }

    private bool IsSameSpotAsItem(int row, int col, GameItem item)
    {
        if (IsOutOfBounds(row, col)) return false;
        return _cavern.GameBoard[row, col] == item;
    }

    private string GetLosingMessage(GameItem item)
    {
        return item switch
        {
            GameItem.Pit => "You fell into the pit!",
            GameItem.Amarok => "The amaroks have devoured you!",
            GameItem.Maelstrom => "The maelstrom is blowing you away to another room!",
        };
    }
}
