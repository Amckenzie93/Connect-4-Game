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

    class ConnectFour
    {
        static void Main(string[] args)
        {
            Stack<Move> allMoves = new Stack<Move>();
            Stack<Move> undoneMoves = new Stack<Move>();
            int boardHeight = 6;
            int boardWidth = 6;
            int[,] board = new int[boardHeight, boardWidth];
            bool gameOver = false;

            Console.WriteLine("Welcome to Connect 4 \n");

            Player p1 = new Player();
            p1.id = 1;
            Console.WriteLine("Player 1 Please enter your player name");
            p1.name = Console.ReadLine();

            Player p2 = new Player();
            p2.id = 2;
            Console.WriteLine("Player 2 Please enter your player name");
            p2.name = Console.ReadLine();

            displayBoard();


            do
            {
                Console.WriteLine("To make a move enter:          1");
                Console.WriteLine("To undo the last move enter:   2");
                Console.WriteLine("To redo the last move enter:   3");

                int choice = int.Parse(Console.ReadLine());
                if (choice == 1)
                {
                    makeMove(p1);
                    makeMove(p2);
                }
                else if (choice == 2)
                {
                    undoMove();
                }
                else if (choice == 3)
                {
                    redoMove();
                }

            } while (gameOver == false);

            Console.ReadLine();


            void undoMove()
            {
                undoneMoves.Push(allMoves.Peek());
                Move lastmove = allMoves.Pop();
                board[lastmove.yPosition, lastmove.xPosition] = 0;
                updateBoard(allMoves);
            }

            void redoMove()
            {
                Move move = undoneMoves.Pop();
                board[move.yPosition, move.xPosition] = 1;
                allMoves.Push(move);
                updateBoard(allMoves);
            }

            void makeMove(Player player)
            {
                Move thisMove = new Move();
                Console.WriteLine("Player " + player.id + "'s move");
                Console.WriteLine("Please enter the column of your move as a whole number(1 - 6)");
                thisMove.xPosition = int.Parse(Console.ReadLine()) - 1;

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
                allMoves.Push(thisMove);
                updateBoard(allMoves);
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
                Console.WriteLine("  ---------------------------");
                Console.WriteLine("  1    2    3    4    5    6");
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
                Console.WriteLine("  ---------------------------");
                Console.WriteLine("  1    2    3    4    5    6");
            }
        }
    }
}