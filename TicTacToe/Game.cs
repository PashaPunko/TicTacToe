using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class Game
    {
        public const string EmptyCell = "empty.png";
        public const string CrossCell = "Cross.png";
        public const string ZeroCell = "Circle.png";
        public const int NumberOfRows = 3;
        public const int NumberOfColumns = 3;

        public Game()
        {
            PopulateBoard();
            Tags = new List<string>();
            Player1 = new Player();
            Player2 = new Player();
        }
        public bool HasPlayer(string connectionId)
        {
            if (Player1 != null && Player1.ConnectionId == connectionId)
            {
                return true;
            }
            if (Player2 != null && Player2.ConnectionId == connectionId)
            {
                return true;
            }
            return false;
        }

        public string Id { get; set; }

        private void PopulateBoard()
        {
            Board = new string[NumberOfRows][];

            for (int x = 0; x < NumberOfRows; x++)
            {
                Board[x] = new string[NumberOfColumns];
                for (int y = 0; y < NumberOfColumns; y++)
                {
                    Board[x][y] = EmptyCell;
                }
            }
        }


        private bool Check(int row, int column, string playerSign)
        {
            int i = 0;
            for (;i< NumberOfRows; i++) {
                if (Board[i][column] != playerSign) break;
            }
            if (i == 3) return true;
            i = 0;
            for (; i < NumberOfColumns; i++)
            {
                if (Board[row][i] != playerSign) break;
            }
            if (i == 3) return true;
            i = 0;
            for (; i < Math.Min(NumberOfRows, NumberOfColumns); i++)
            {
                if (Board[i][i] != playerSign) break;
            }
            if (i == 3) return true;
            i = 0;
            for (; i < Math.Min(NumberOfRows, NumberOfColumns); i++)
            {
                if (Board[i][Math.Min(NumberOfRows, NumberOfColumns)-i-1] != playerSign) break;
            }
            if (i == 3) return true;
            return false;
        }
        public bool CheckDraw()
        {
            for (int i =0; i < NumberOfRows; i++)
            {
                for (int j=0; j <  NumberOfColumns; j++)
                {
                    if (Board[i][j] == EmptyCell) return false;
                }
            }
            return true;
        }


        public void Move(int row, int column, string sign)
        {
            if (Board[row][column] == EmptyCell)
            Board[row][column] = sign;
        }

        public void NextPlayer()
        {
            if (CurrentPlayer == Player1)
            {
                CurrentPlayer = Player2;
            }
            else
            {
                CurrentPlayer = Player1;
            }
        }

        public bool CheckVictory(int row, int column)
        {
            var playerSign = Board[row][column];

            if (Check(row, column, playerSign))
            {
                return true;
            }
            return false;
        }


        public Player Player1 { get; private set; }

        public Player Player2 { get; private set; }

        public Player CurrentPlayer { get; set; }

        public string[][] Board { get; private set; }
        public bool InProgress { get; set; }
        public string Name { get; set; }
        public string CurrentField { get; set; }
        public List<string> Tags { get; set; }

    }

    public class Player
    {
        public string ConnectionId { get; set; }
        public string Sign { get; set; }
    }
}
