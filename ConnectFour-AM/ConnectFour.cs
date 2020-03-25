using Pastel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConnectFourAM
{

    //Player class to hold player name and player ID
    class Player
    {
        public string name;
        public int id;
    }


    //Move class to hold a players moves in X and Y coordinates as well as what player made the move
    class Move
    {
        public int xPosition;
        public int yPosition;
        public Player player;
    }

    //GameData class to hold all moves, and undone moves from a given game in play.
    class GameData
    {
        public List<Queue<Move>> AllGames = new List<Queue<Move>>();
        public Stack<Move> allMoves = new Stack<Move>();
        public Stack<Move> undoneMoves = new Stack<Move>();
    }

    //GameMetric class to hold all average run times of each function reocrded for analytics
    class GameMetrics
    {
        public List<long> UpdateDisplayTimesAvg = new List<long>();
        public List<long> UndoMoveTimeAvg = new List<long>();
        public List<long> RedoMoveTimeAvg = new List<long>();
        public List<long> CheckWinTimeAvg = new List<long>();
        public List<long> MakeMoveAvg = new List<long>();
    }

    class ConnectFour
    {
        static void Main(string[] args)
        {
            GameData GameData = new GameData();
            GameMetrics gameMetrics = new GameMetrics();
            int boardHeight = 6;
            int boardWidth = 7;
            int gameChoice;
            int playerChoice;
            int[,] board = new int[boardHeight, boardWidth];
            bool gameOver = false;

            Console.WriteLine("Welcome to Connect 4 \n");
            Player p1 = new Player();
            p1.id = 1;
            Console.WriteLine("Player 1 Please enter your name");
            p1.name = Console.ReadLine();
            Player p2 = new Player();
            p2.id = 2;
            menu();
            Console.ReadLine();



            //Method to initiate the game board ensuring all x and y coordinates in the board array are set to zero, then prints the game board to the users screen.
            void initBoard()
            {
                for (var y = 0; y < boardHeight; y++)
                {
                    for (var x = 0; x < boardWidth; x++)
                    {
                        board[y, x] = 0;
                    }
                }

                Console.WriteLine("\n");

                for (int y = 0; y < boardHeight; y++)
                {
                    for (int x = 0; x < boardWidth; x++)
                    {
                        Console.Write(string.Format("  {0}  ", board[y, x]));
                    }
                    Console.Write(Environment.NewLine);
                }
                Console.WriteLine("  --------------------------------");
                Console.WriteLine("  1    2    3    4    5    6    7");
            }


            //Method to update the board x and y coordinates of each players move in the game the user has chosen to rewatch, before passing this data off to update the board visually for the player to watch.
            void reWatch(int game)
            {
                foreach (Move move in GameData.AllGames[game])
                {
                    if (move.player.id == 1)
                    {
                        board[move.yPosition, move.xPosition] = 1;
                    }
                    else
                    {
                        board[move.yPosition, move.xPosition] = 1;
                    }
                }
                RewatchBoard(GameData.AllGames[game]);
                initBoard();
                GameData.allMoves.Clear();
                GameData.undoneMoves.Clear();
                menu();
            }


            //Method that undoes a set of moves in a current game. 
            void undoMove()
            {
                var sw = new Stopwatch();
                sw.Start();
                if (GameData.allMoves.Count() >= 1)
                {
                    GameData.undoneMoves.Push(GameData.allMoves.Pop());
                    Move lastmove = GameData.undoneMoves.Peek();
                    board[lastmove.yPosition, lastmove.xPosition] = 0;

                    GameData.undoneMoves.Push(GameData.allMoves.Peek());
                    Move lastmove2 = GameData.allMoves.Pop();
                    board[lastmove2.yPosition, lastmove2.xPosition] = 0;
                }
                else
                {
                    Console.WriteLine(string.Format("No more moves to undo.").Pastel("#ff0000"));
                }
                sw.Stop();
                gameMetrics.UndoMoveTimeAvg.Add(sw.ElapsedTicks);
                updateBoard(GameData.allMoves);
            }


            //Method to redo any undone moves in a current game.
            void redoMove()
            {
                var sw = new Stopwatch();
                sw.Start();
                if (GameData.undoneMoves.Count() >= 1)
                {
                    Move move = GameData.undoneMoves.Pop();
                    board[move.yPosition, move.xPosition] = 1;
                    GameData.allMoves.Push(move);

                    Move move2 = GameData.undoneMoves.Pop();
                    board[move2.yPosition, move2.xPosition] = 1;
                    GameData.allMoves.Push(move2);
                }
                else
                {
                    Console.WriteLine(string.Format("No more moves to redo.").Pastel("#ff0000"));
                }
                sw.Stop();
                gameMetrics.RedoMoveTimeAvg.Add(sw.ElapsedTicks);
                updateBoard(GameData.allMoves);
            }


            //Method that allows the player to make a move in the game based on x and y coorindates, this also holds basic AI functionality for playing against a computer which randomly generates its next move.
            void makeMove(Player player)
            {
                var sw = new Stopwatch();
                sw.Start();

                Move thisMove = new Move();
                thisMove.player = player;
                if (player.name == "HAL 9000")
                {
                    Random map = new Random();
                    int decision = map.Next(1, 7);
                    thisMove.xPosition = decision - 1;
                }
                else
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("Player " + player.id + "'s move");
                    Console.WriteLine("Please enter the column of your move as a whole number(1 - 7)");
                    Console.WriteLine("\n");

                    var x = Console.ReadLine();
                    bool success = false;
                    do
                    {
                        success = int.TryParse(x, out int result);
                        if (success && result >= 0 && result <= 7)
                        {
                            thisMove.xPosition = result - 1;
                            success = true;
                        }
                        else
                        {
                            Console.WriteLine("please enter a valid move between 1 and 7");
                            x = Console.ReadLine();
                        }
                    } while (success == false);
                }

                for (int y = 0; y < boardHeight; y++)
                {
                    if (board[y, thisMove.xPosition] == 0)
                    {
                        board[y, thisMove.xPosition] = 1;
                        thisMove.yPosition = y;
                        break;
                    }
                }
                thisMove.player = player;
                GameData.allMoves.Push(thisMove);
                sw.Stop();
                gameMetrics.MakeMoveAvg.Add(sw.ElapsedTicks);
                updateBoard(GameData.allMoves);
                // if there have been more than 6 moves in the game (3 moves per player so far) only then start checking the win condition
                if (GameData.allMoves.Count() > 6)
                {
                    checkWin();
                }
            }


            //Method that is called elsewhere in the game, passing in all moves in said game, to update the command line display of the game as its played.
            void updateBoard(Stack<Move> allMovesStack)
            {
                var sw = new Stopwatch();
                sw.Start();
                for (int y = 5; y >= 0; y--)
                {
                    for (int x = 0; x < boardWidth; x++)
                    {
                        if (board[y, x] == 0)
                        {
                            Console.Write(string.Format("  {0}  ", board[y, x]));
                        }
                        else
                        {
                            foreach (var move in allMovesStack)
                            {
                                if (move.xPosition == x && move.yPosition == y)
                                {
                                    if (move.player.id == 1)
                                    {
                                        Console.Write(string.Format("  {0}  ", board[y, x]).Pastel("#a4f542"));
                                    }
                                    else
                                    {
                                        Console.Write(string.Format("  {0}  ", board[y, x]).Pastel("#ff0000"));
                                    }

                                }
                            }

                        }
                    }
                    Console.Write(Environment.NewLine);
                }
                Console.WriteLine("  --------------------------------");
                Console.WriteLine("  1    2    3    4    5    6    7");
                Console.WriteLine("\n");
                gameMetrics.UpdateDisplayTimesAvg.Add(sw.ElapsedMilliseconds);
                sw.Stop();
            }


            //Method that displays visually in the command line each move made in a rewatched game with a pause of 200 miliseconds 
            void RewatchBoard(Queue<Move> allMovesQueue)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    for (int x = 0; x < boardWidth; x++)
                    {
                        board[y, x] = 0;
                    }
                }
                Stack<Move> newStack = new Stack<Move>();

                //the queue must be reversed in order to update the game data for use in methods to display a previous game recording.
                foreach (var item in allMovesQueue.Reverse())
                {
                    newStack.Push(item);
                    board[item.yPosition, item.xPosition] = 1;
                    updateBoard(newStack);
                    System.Threading.Thread.Sleep(200);
                }
            }


            //Method to highlight which player won the game and reset all settings of the game ready for the next option the user picks (play again, rewatch, analytics etc)
            void Win(Player player)
            {
                if (player == p1)
                {
                    Console.WriteLine(string.Format(player.name + " has won the game").Pastel("#a4f542"));
                }
                else
                {
                    Console.WriteLine(string.Format(player.name + " has won the game").Pastel("#ff0000"));
                }
                Queue<Move> clonedStack = new Queue<Move>();
                clonedStack = new Queue<Move>(GameData.allMoves);
                GameData.AllGames.Add(clonedStack);
                GameData.allMoves.Clear();
                GameData.undoneMoves.Clear();
                gameChoice = 0;
                playerChoice = 0;
                menu();
            }


            //Method that displays and processes players options mid game.
            void gameOptions()
            {
                do
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("1:     To make a move enter");
                    Console.WriteLine("2:     To undo the last move enter");
                    Console.WriteLine("3:     To redo the last move enter");
                    Console.WriteLine("\n");

                    var x = Console.ReadLine();
                    bool success = int.TryParse(x, out int result);
                    if (success)
                    {
                        gameChoice = result;
                    }
                    else
                    {
                        Console.WriteLine("please enter a valid number");
                        gameOptions();
                    }

                    if (gameChoice == 1)
                    {
                        makeMove(p1);
                        makeMove(p2);
                    }
                    else if (gameChoice == 2)
                    {
                        undoMove();
                    }
                    else if (gameChoice == 3)
                    {
                        redoMove();
                    }
                } while (gameOver == false);
            }


            //Method that handles the main menu options and processes what the user chooses. 
            void menu()
            {
                Console.WriteLine("\n");
                Console.WriteLine("1:    Do you want to play against a human?");
                Console.WriteLine("2:    Do you want to play against a computer?");

                if (GameData.AllGames.Count > 0)
                {
                    Console.WriteLine("3:    Do you want to Re-watch a previous game?");
                    Console.WriteLine("4:    View game time complexity averages per function?");
                }
                Console.WriteLine("\n");

                var x = Console.ReadLine();
                bool success = int.TryParse(x, out int result);
                if (success && result >= 1 && result <= 4)
                {
                    playerChoice = result;
                }
                else
                {
                    Console.WriteLine("please enter a valid number");
                    menu();
                }

                if (playerChoice == 1)
                {
                    gameOver = false;
                    Console.WriteLine("Player 2 Please enter your name");
                    p2.name = Console.ReadLine();
                    initBoard();
                    gameOptions();
                }
                else if (playerChoice == 2)
                {
                    gameOver = false;
                    p2.name = "HAL 9000";
                    initBoard();
                    gameOptions();
                }
                else if (playerChoice == 3)
                {
                    Console.WriteLine("Please pick the game you wish to replay from the list below:");
                    for (var i = 0; i < GameData.AllGames.Count(); i++)
                    {
                        Console.WriteLine((i + 1) + ":    Game " + (i + 1));
                    }
                    int gameNumber = int.Parse(Console.ReadLine()) - 1;
                    initBoard();
                    reWatch(gameNumber);
                }
                else if (playerChoice == 4)
                {
                    displayGameTime();
                }
                else
                {
                    menu();
                }
            }


            //Method to display each recorded functions average runtime in the game for analytics and report purposes.
            void displayGameTime()
            {
                long GameMoveAvg = 0;
                long UpdateDisplayAvg = 0;
                long UndoMoveAvg = 0;
                long RedoMoveAvg = 0;
                long CheckWinAvg = 0;


                foreach (var item in gameMetrics.UpdateDisplayTimesAvg)
                {
                    GameMoveAvg += item;
                }
                foreach (var item in gameMetrics.UpdateDisplayTimesAvg)
                {
                    UpdateDisplayAvg += item;
                }
                foreach (var item in gameMetrics.UndoMoveTimeAvg)
                {
                    UndoMoveAvg += item;
                }
                foreach (var item in gameMetrics.RedoMoveTimeAvg)
                {
                    RedoMoveAvg += item;
                }
                foreach (var item in gameMetrics.CheckWinTimeAvg)
                {
                    CheckWinAvg += item;
                }

                Console.WriteLine("Average for players to make their move and update all stored data accordingly");
                Console.WriteLine(string.Format(GameMoveAvg / gameMetrics.MakeMoveAvg.Count() + " Nanoseconds average.").Pastel("#a4f542"));
                Console.WriteLine("\n");

                Console.WriteLine("Average time taken to update and display the game board in CMD window with new player moves:");
                Console.WriteLine(string.Format(UpdateDisplayAvg / gameMetrics.UpdateDisplayTimesAvg.Count() + " Milliseconds average.").Pastel("#a4f542"));
                Console.WriteLine("\n");
                if (gameMetrics.UndoMoveTimeAvg.Count() > 0)
                {
                    Console.WriteLine("Average time taken to undo a pair of moves in the game:");
                    Console.WriteLine(string.Format(UndoMoveAvg / gameMetrics.UndoMoveTimeAvg.Count() + " Nanoseconds average.").Pastel("#a4f542"));
                    Console.WriteLine("\n");
                }
                if (gameMetrics.RedoMoveTimeAvg.Count() > 0)
                {
                    Console.WriteLine("Average time taken to redo a pair of moves in the game:");
                    Console.WriteLine(string.Format(RedoMoveAvg / gameMetrics.RedoMoveTimeAvg.Count() + " Nanoseconds average.").Pastel("#a4f542"));
                    Console.WriteLine("\n");
                }
                Console.WriteLine("Average time taken to check for win conditions in the game:");
                Console.WriteLine(string.Format(CheckWinAvg / gameMetrics.CheckWinTimeAvg.Count() + " Nanoseconds average.").Pastel("#a4f542"));

                menu();
            }


            //Method to check win condition in each direction, vertical, horizontal, diagonal left, diagonal right.
            void checkWin()
            {
                var sw = new Stopwatch();
                sw.Start();
                bool one = false,
                    two = false,
                    three = false,
                    four = false;

                for (var y = 0; y < boardHeight; y++)
                {
                    for (var x = 0; x < boardWidth; x++)
                    {
                        //Horizontal Check (--)
                        if (x + 3 < boardWidth && board[y, x + 1] == 1 && board[y, x + 2] == 1 && board[y, x + 3] == 1)
                        {
                            foreach (Move move in GameData.allMoves)
                            {
                                if (move.player.id == 1)
                                {
                                    if (move.xPosition == x && move.yPosition == y)
                                    {
                                        one = true;
                                    }
                                    else if (move.xPosition == x + 1 && move.yPosition == y)
                                    {
                                        two = true;
                                    }
                                    else if (move.xPosition == x + 2 && move.yPosition == y)
                                    {
                                        three = true;
                                    }
                                    else if (move.xPosition == x + 3 && move.yPosition == y)
                                    {
                                        four = true;
                                    }

                                    if (one && two && three && four)
                                    {
                                        gameMetrics.CheckWinTimeAvg.Add(sw.ElapsedTicks);
                                        Win(p1);
                                        return;
                                    }
                                }
                            }
                            one = false;
                            two = false;
                            three = false;
                            four = false;
                            foreach (Move move in GameData.allMoves)
                            {
                                if (move.player.id == 2)
                                {
                                    if (move.xPosition == x && move.yPosition == y)
                                    {
                                        one = true;
                                    }
                                    else if (move.xPosition == x + 1 && move.yPosition == y)
                                    {
                                        two = true;
                                    }
                                    else if (move.xPosition == x + 2 && move.yPosition == y)
                                    {
                                        three = true;
                                    }
                                    else if (move.xPosition == x + 3 && move.yPosition == y)
                                    {
                                        four = true;
                                    }

                                    if (one && two && three && four)
                                    {
                                        gameMetrics.CheckWinTimeAvg.Add(sw.ElapsedTicks);
                                        Win(p2);
                                        return;
                                    }
                                }
                            }
                            one = false;
                            two = false;
                            three = false;
                            four = false;
                        }

                        //Vertical Check (|)
                        if (y + 3 < boardHeight && board[y + 1, x] == 1 && board[y + 2, x] == 1 && board[y + 3, x] == 1)
                        {
                            foreach (Move move in GameData.allMoves)
                            {
                                if (move.player.id == 1)
                                {
                                    if (move.xPosition == x && move.yPosition == y)
                                    {
                                        one = true;
                                    }
                                    else if (move.xPosition == x && move.yPosition == y + 1)
                                    {
                                        two = true;
                                    }
                                    else if (move.xPosition == x && move.yPosition == y + 2)
                                    {
                                        three = true;
                                    }
                                    else if (move.xPosition == x && move.yPosition == y + 3)
                                    {
                                        four = true;
                                    }

                                    if (one && two && three && four)
                                    {
                                        gameMetrics.CheckWinTimeAvg.Add(sw.ElapsedTicks);
                                        Win(p1);
                                        return;
                                    }
                                }
                            }
                            one = false;
                            two = false;
                            three = false;
                            four = false;
                            foreach (Move move in GameData.allMoves)
                            {
                                if (move.player.id == 2)
                                {
                                    if (move.xPosition == x && move.yPosition == y)
                                    {
                                        one = true;
                                    }
                                    else if (move.xPosition == x && move.yPosition == y + 1)
                                    {
                                        two = true;
                                    }
                                    else if (move.xPosition == x && move.yPosition == y + 2)
                                    {
                                        three = true;
                                    }
                                    else if (move.xPosition == x && move.yPosition == y + 3)
                                    {
                                        four = true;
                                    }

                                    if (one && two && three && four)
                                    {
                                        gameMetrics.CheckWinTimeAvg.Add(sw.ElapsedTicks);
                                        Win(p2);
                                        return;
                                    }
                                }
                            }
                            one = false;
                            two = false;
                            three = false;
                            four = false;
                        }


                        //Diagonal up Check (/)
                        if (y + 3 < boardHeight)
                        {
                            if (x + 3 < boardWidth && board[y + 1, x + 1] == 1 && board[y + 2, x + 2] == 1 && board[y + 3, x + 3] == 1)
                            {
                                foreach (Move move in GameData.allMoves)
                                {
                                    if (move.player.id == 1)
                                    {
                                        if (move.xPosition == x && move.yPosition == y)
                                        {
                                            one = true;
                                        }
                                        else if (move.xPosition == x + 1 && move.yPosition == y + 1)
                                        {
                                            two = true;
                                        }
                                        else if (move.xPosition == x + 2 && move.yPosition == y + 2)
                                        {
                                            three = true;
                                        }
                                        else if (move.xPosition == x + 3 && move.yPosition == y + 3)
                                        {
                                            four = true;
                                        }

                                        if (one && two && three && four)
                                        {
                                            gameMetrics.CheckWinTimeAvg.Add(sw.ElapsedTicks);
                                            Win(p1);
                                            return;
                                        }
                                    }
                                }
                                one = false;
                                two = false;
                                three = false;
                                four = false;
                                foreach (Move move in GameData.allMoves)
                                {
                                    if (move.player.id == 2)
                                    {
                                        if (move.xPosition == x && move.yPosition == y)
                                        {
                                            one = true;
                                        }
                                        else if (move.xPosition == x + 1 && move.yPosition == y + 1)
                                        {
                                            two = true;
                                        }
                                        else if (move.xPosition == x + 2 && move.yPosition == y + 2)
                                        {
                                            three = true;
                                        }
                                        else if (move.xPosition == x + 3 && move.yPosition == y + 3)
                                        {
                                            four = true;
                                        }

                                        if (one && two && three && four)
                                        {
                                            gameMetrics.CheckWinTimeAvg.Add(sw.ElapsedTicks);
                                            Win(p2);
                                            return;
                                        }
                                    }
                                }
                                one = false;
                                two = false;
                                three = false;
                                four = false;
                            }
                        }

                        //Diagonal down Check (\)
                        if (y - 3 >= 0)
                        {
                            if (x + 3 < boardWidth && board[y - 1, x + 1] == 1 && board[y - 2, x + 2] == 1 && board[y - 3, x + 3] == 1)
                            {
                                foreach (Move move in GameData.allMoves)
                                {
                                    if (move.player.id == 1)
                                    {
                                        if (move.xPosition == x && move.yPosition == y)
                                        {
                                            one = true;
                                        }
                                        else if (move.xPosition == x + 1 && move.yPosition == y - 1)
                                        {
                                            two = true;
                                        }
                                        else if (move.xPosition == x + 2 && move.yPosition == y - 2)
                                        {
                                            three = true;
                                        }
                                        else if (move.xPosition == x + 3 && move.yPosition == y - 3)
                                        {
                                            four = true;
                                        }

                                        if (one && two && three && four)
                                        {
                                            gameMetrics.CheckWinTimeAvg.Add(sw.ElapsedTicks);
                                            Win(p1);
                                            return;
                                        }
                                    }
                                }
                                one = false;
                                two = false;
                                three = false;
                                four = false;
                                foreach (Move move in GameData.allMoves)
                                {
                                    if (move.player.id == 2)
                                    {
                                        if (move.xPosition == x && move.yPosition == y)
                                        {
                                            one = true;
                                        }
                                        else if (move.xPosition == x + 1 && move.yPosition == y - 1)
                                        {
                                            two = true;
                                        }
                                        else if (move.xPosition == x + 2 && move.yPosition == y - 2)
                                        {
                                            three = true;
                                        }
                                        else if (move.xPosition == x + 3 && move.yPosition == y - 3)
                                        {
                                            four = true;
                                        }

                                        if (one && two && three && four)
                                        {
                                            gameMetrics.CheckWinTimeAvg.Add(sw.ElapsedTicks);
                                            Win(p2);
                                            return;
                                        }
                                    }
                                }
                                one = false;
                                two = false;
                                three = false;
                                four = false;
                            }
                        }
                    }
                }
                sw.Stop();
                gameMetrics.CheckWinTimeAvg.Add(sw.ElapsedTicks);
                return;
            }
        }
    }
}