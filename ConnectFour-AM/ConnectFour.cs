using Pastel;
using System;
using System.Collections;
using System.Collections.Generic;
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


    class ConnectFour
    {
        static void Main(string[] args)
        {
            GameData GameData = new GameData();
            int boardHeight = 6;
            int boardWidth = 7;
            int[,] board = new int[boardHeight, boardWidth];
            bool gameOver = false;

            Console.WriteLine("Welcome to Connect 4 \n");

            displayBoard();
            Console.WriteLine("\n");


            Player p1 = new Player();
            p1.id = 1;
            Console.WriteLine("Player 1 Please enter your player name");
            p1.name = Console.ReadLine();

            Player p2 = new Player();
            p2.id = 2;

            int playerChoice = 0;

            menu();

            do
            {
                Console.WriteLine("To make a move enter:          1");
                Console.WriteLine("To undo the last move enter:   2");
                Console.WriteLine("To redo the last move enter:   3");
                Console.WriteLine("\n");
                int gameChoice = int.Parse(Console.ReadLine());
                if (gameChoice == 1)
                {
                    makeMove(p1);
                    if (playerChoice == 1)
                    {
                        makeMove(p2);
                    }
                    else
                    {
                        makeMove(p2);
                    }
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



            Console.ReadLine();






            void menu()
            {
                Console.WriteLine("\n");
                Console.WriteLine("Do you want to play against another human?:       1");
                Console.WriteLine("OR");
                Console.WriteLine("Do you want to play against a computer?:          2");

                if (GameData.AllGames.Count > 0)
                {
                    Console.WriteLine("OR");
                    Console.WriteLine("Do you want to Re-watch a previous game?:     3");
                }
                Console.WriteLine("\n");
                playerChoice = int.Parse(Console.ReadLine());
                if (playerChoice == 1)
                {
                    gameOver = false;
                    initBoard();
                    Console.WriteLine("Player 2 Please enter your player name");
                    p2.name = Console.ReadLine();
                }
                else if (playerChoice == 2)
                {
                    gameOver = false;
                    initBoard();
                    p2.name = "HAL 9000";
                }
                else if (playerChoice == 3)
                {
                    Console.WriteLine("Please pick the game you wish to replay from the list below:");

                    for (var i = 0; i < GameData.AllGames.Count(); i++)
                    {

                        Console.WriteLine("Game: " + (i + 1));
                    }

                    int gameNumber = int.Parse(Console.ReadLine()) - 1;
                    initBoard();
                    reWatch(gameNumber);
                }
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
                    message = string.Format("player " + player.name + " has won the game").Pastel("#a4f542");
                }
                else
                {
                    message = string.Format("player " + player.name + " has won the game").Pastel("#ff0000");
                }

                Console.WriteLine(message);

                Queue<Move> clonedStack = new Queue<Move>();
                clonedStack = new Queue<Move>(GameData.allMoves);
                GameData.AllGames.Add(clonedStack);
                GameData.allMoves.Clear();
                GameData.undoneMoves.Clear();
                menu();
            }

            void undoMove()
            {
                GameData.undoneMoves.Push(GameData.allMoves.Peek());
                Move lastmove = GameData.allMoves.Pop();
                board[lastmove.yPosition, lastmove.xPosition] = 0;

                GameData.undoneMoves.Push(GameData.allMoves.Peek());
                Move lastmove2 = GameData.allMoves.Pop();
                board[lastmove2.yPosition, lastmove2.xPosition] = 0;

                updateBoard(GameData.allMoves);
            }

            void redoMove()
            {
                Move move = GameData.undoneMoves.Pop();
                board[move.yPosition, move.xPosition] = 1;
                GameData.allMoves.Push(move);

                Move move2 = GameData.undoneMoves.Pop();
                board[move2.yPosition, move2.xPosition] = 1;
                GameData.allMoves.Push(move2);

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
                    Console.WriteLine("Player " + player.id + "'s move");
                    Console.WriteLine("Please enter the column of your move as a whole number(1 - 7)");
                    thisMove.xPosition = int.Parse(Console.ReadLine()) - 1;
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
                bool one = false,
                    two = false,
                    three = false,
                    four = false;

                for (var y = 0; y < boardHeight; y++)
                {
                    for (var x = 0; x < boardWidth; x++)
                    {

                        //Horizontal Check
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

                                        Win(p1);
                                        break;
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
                                        Win(p2);
                                        break;
                                    }
                                }
                            }
                            one = false;
                            two = false;
                            three = false;
                            four = false;
                        }

                        //Vertical Check
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
                                        Win(p1);
                                        break;
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
                                        Win(p2);
                                        break;
                                    }
                                }
                            }
                            one = false;
                            two = false;
                            three = false;
                            four = false;

                        }


                        //Diagonal up Check
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
                                            Win(p1);
                                            break;
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
                                            Win(p2);
                                            break;
                                        }
                                    }
                                }
                                one = false;
                                two = false;
                                three = false;
                                four = false;


                            }
                        }

                        //Diagonal down Check
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
                                            Win(p1);
                                            break;
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
                                            Win(p2);
                                            break;
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
            }
        }
    }
}


