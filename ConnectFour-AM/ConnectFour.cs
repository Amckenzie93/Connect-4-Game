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

    class ConnectFour
    {
        static void Main(string[] args)
        {

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

            Console.ReadLine();

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