using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe
{
    public interface IGameRepository
    {
        List<Game> Games { get; }
    }

    public class GameRepository : IGameRepository
    {
        public GameRepository()
        {
        }
        public List<Game> Games { get; } = new List<Game>();
    }
}
