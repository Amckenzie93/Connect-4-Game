using Pastel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConnectFourAM
{

    class Player
    {
        public string name;
        public int id;
    }

    class Move
    {
        public int xPosition;
        public int yPosition;
        public Player player;
    }

    class GameData
    {
        public List<Queue<Move>> AllGames = new List<Queue<Move>>();
        public Stack<Move> allMoves = new Stack<Move>();
        public Stack<Move> undoneMoves = new Stack<Move>();
    }


    class GameMetrics
    {
        public List<long> UpdateDisplayTimesAvg = new List<long>();
        public List<long> UndoMoveTimeAvg = new List<long>();
        public List<long> RedoMoveTimeAvg = new List<long>();
        public List<long> CheckWinTimeAvg = new List<long>();
    }


    class ConnectFour
    {
        static void Main(string[] args)
        {
            GameData GameData = new GameData();
            GameMetrics GameTimeComplexity = new GameMetrics();
            int boardHeight = 6;
            int boardWidth = 7;
            int[,] board = new int[boardHeight, boardWidth];
            bool gameOver = false;
            int gameChoice;
            int playerChoice;


            Console.WriteLine("Welcome to Connect 4 \n");
            Console.WriteLine("\n");

            displayBoard();
            Console.WriteLine("\n");

            Player p1 = new Player();
            p1.id = 1;
            Console.WriteLine("Player 1 Please enter your name");
            p1.name = Console.ReadLine();

            Player p2 = new Player();
            p2.id = 2;

            menu();
            Console.ReadLine();

            void gameOptions()
            {
                do
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("1:     To make a move enter");
                    Console.WriteLine("2:     To undo the last move enter");
                    Console.WriteLine("3:     To redo the last move enter");
                    gameChoice = int.Parse(Console.ReadLine());
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

            void menu()
            {
                Console.WriteLine("\n");
                Console.WriteLine("1:    Do you want to play against a human?");
                Console.WriteLine("      OR");
                Console.WriteLine("2:    Do you want to play against a computer?");

                if (GameData.AllGames.Count > 0)
                {
                    Console.WriteLine("      OR");
                    Console.WriteLine("3:    Do you want to Re-watch a previous game?");
                    Console.WriteLine("      OR");
                    Console.WriteLine("4:    View game time complexity averages per function?");
                }
                Console.WriteLine("\n");
                playerChoice = int.Parse(Console.ReadLine());
                if (playerChoice == 1)
                {
                    gameOver = false;
                    initBoard();
                    Console.WriteLine("Player 2 Please enter your name");
                    p2.name = Console.ReadLine();
                    gameOptions();
                }
                else if (playerChoice == 2)
                {
                    gameOver = false;
                    initBoard();
                    p2.name = "HAL 9000";
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
                else if(playerChoice == 4)
                {
                    displayGameTime();
                }
            }

            void displayGameTime()
            {
                long UpdateDisplayAvg = 0;
                long UndoMoveAvg = 0;
                long RedoMoveAvg = 0;
                long CheckWinAvg = 0;



                foreach (var item in GameTimeComplexity.UpdateDisplayTimesAvg)
                {
                    UpdateDisplayAvg += item;
                }
                foreach (var item in GameTimeComplexity.UndoMoveTimeAvg)
                {
                    UndoMoveAvg += item;
                }
                foreach (var item in GameTimeComplexity.RedoMoveTimeAvg)
                {
                    RedoMoveAvg += item;
                }
                foreach (var item in GameTimeComplexity.CheckWinTimeAvg)
                {
                    CheckWinAvg += item;
                }

                Console.WriteLine("Average time taken to update and display the game board in CMD window with new player moves:");
                Console.WriteLine(UpdateDisplayAvg / GameTimeComplexity.UpdateDisplayTimesAvg.Count() + " miliseconds average.");
                Console.WriteLine("\n");

                if (GameTimeComplexity.UndoMoveTimeAvg.Count() > 0)
                {
                    Console.WriteLine("Average time taken to undo a pair of moves in the game:");
                    Console.WriteLine(UndoMoveAvg / GameTimeComplexity.UndoMoveTimeAvg.Count() + " miliseconds average.");
                    Console.WriteLine("\n");
                }
                if (GameTimeComplexity.RedoMoveTimeAvg.Count() > 0)
                {
                    Console.WriteLine("Average time taken to redo a pair of moves in the game:");
                    Console.WriteLine(RedoMoveAvg / GameTimeComplexity.RedoMoveTimeAvg.Count() + " miliseconds average.");
                    Console.WriteLine("\n");
                }

                Console.WriteLine("Average time taken to check for win conditions in the game:");
                Console.WriteLine(CheckWinAvg / GameTimeComplexity.CheckWinTimeAvg.Count() + " miliseconds average.");
                Console.WriteLine("\n");

                menu();
            }

            void initBoard()
            {
                for (var y = 0; y < boardHeight; y++)
                {
                    for (var x = 0; x < boardWidth; x++)
                    {
                        board[y, x] = 0;
                    }
                }
            }


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

            void Win(Player player)
            {
                string message;
                if (player == p1)
                {
                    message = string.Format(player.name + " has won the game").Pastel("#a4f542");
                }
                else
                {
                    message = string.Format(player.name + " has won the game").Pastel("#ff0000");
                }

                Console.WriteLine(message);

                Queue<Move> clonedStack = new Queue<Move>();
                clonedStack = new Queue<Move>(GameData.allMoves);
                GameData.AllGames.Add(clonedStack);
                GameData.allMoves.Clear();
                GameData.undoneMoves.Clear();
                gameChoice = 0;
                playerChoice = 0;
                menu();
            }

            void undoMove()
            {
                var sw = new Stopwatch();
                sw.Start();

                GameData.undoneMoves.Push(GameData.allMoves.Peek());
                Move lastmove = GameData.allMoves.Pop();
                board[lastmove.yPosition, lastmove.xPosition] = 0;

                GameData.undoneMoves.Push(GameData.allMoves.Peek());
                Move lastmove2 = GameData.allMoves.Pop();
                board[lastmove2.yPosition, lastmove2.xPosition] = 0;

                GameTimeComplexity.UndoMoveTimeAvg.Add(sw.ElapsedMilliseconds);

                updateBoard(GameData.allMoves);
            }

            void redoMove()
            {
                var sw = new Stopwatch();
                sw.Start();

                Move move = GameData.undoneMoves.Pop();
                board[move.yPosition, move.xPosition] = 1;
                GameData.allMoves.Push(move);

                Move move2 = GameData.undoneMoves.Pop();
                board[move2.yPosition, move2.xPosition] = 1;
                GameData.allMoves.Push(move2);

                GameTimeComplexity.RedoMoveTimeAvg.Add(sw.ElapsedMilliseconds);

                updateBoard(GameData.allMoves);

            }

            void makeMove(Player player)
            {
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
                    thisMove.xPosition = int.Parse(Console.ReadLine()) - 1;
                    Console.WriteLine("\n");
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
                updateBoard(GameData.allMoves);
                checkWin();
            }


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

                GameTimeComplexity.UpdateDisplayTimesAvg.Add(sw.ElapsedMilliseconds);
            }


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
                foreach (var item in allMovesQueue.Reverse())
                {
                    newStack.Push(item);
                    board[item.yPosition, item.xPosition] = 1;
                    updateBoard(newStack);
                    System.Threading.Thread.Sleep(200);
                }
            }



            void displayBoard()
            {
                int rowLength = board.GetLength(0);
                int colLength = board.GetLength(1);

                for (int y = 0; y < rowLength; y++)
                {
                    for (int x = 0; x < colLength; x++)
                    {
                        Console.Write(string.Format("  {0}  ", board[y, x]));
                    }
                    Console.Write(Environment.NewLine);
                }
                Console.WriteLine("  --------------------------------");
                Console.WriteLine("  1    2    3    4    5    6    7");
            }





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
                                        GameTimeComplexity.CheckWinTimeAvg.Add(sw.ElapsedMilliseconds);
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
                                        GameTimeComplexity.CheckWinTimeAvg.Add(sw.ElapsedMilliseconds);
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
                                        GameTimeComplexity.CheckWinTimeAvg.Add(sw.ElapsedMilliseconds);
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
                                        GameTimeComplexity.CheckWinTimeAvg.Add(sw.ElapsedMilliseconds);
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
                                            GameTimeComplexity.CheckWinTimeAvg.Add(sw.ElapsedMilliseconds);
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
                                            GameTimeComplexity.CheckWinTimeAvg.Add(sw.ElapsedMilliseconds);
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
                                            GameTimeComplexity.CheckWinTimeAvg.Add(sw.ElapsedMilliseconds);
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
                                            GameTimeComplexity.CheckWinTimeAvg.Add(sw.ElapsedMilliseconds);
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
                GameTimeComplexity.CheckWinTimeAvg.Add(sw.ElapsedMilliseconds);
                Console.WriteLine("check win time = " + sw.ElapsedMilliseconds);
                return;
            }
        }
    }
}


