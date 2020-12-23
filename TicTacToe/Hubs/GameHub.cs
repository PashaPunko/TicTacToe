using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TicTacToe;

namespace TicTacToe.Hubs
{
    public interface IGameClient
    {
        Task RenderBoard(string[][] board, string name, string[] tags);
        Task Disconnect();
        Task Turn(string sign, int id, bool curPlayer);
        Task Victory(string result);
    }

    public class GameHub : Hub<IGameClient>
    {
        private IGameRepository repository;

        public GameHub(IGameRepository repository)
        {
            this.repository = repository;
        }


        public async Task TryMove(int id)
        {
            int row = id / 3;
            int column = id % 3;

            var game = repository.Games.FirstOrDefault(g => g.HasPlayer(Context.User.Identity.Name));
            if (game is null)
            {
                return;
            }
            if (Context.User.Identity.Name != game.CurrentPlayer.ConnectionId)
            {
                return;
            }
            game.Move(row, column, game.CurrentPlayer.Sign);
            if (game.Player2.ConnectionId is null) {
                game.CurrentField = id.ToString() + ".png";
            }
            await Clients.GroupExcept(game.Id, Context.ConnectionId).Turn(game.CurrentPlayer.Sign, id, true);
            await Clients.Client(Context.ConnectionId).Turn(game.CurrentPlayer.Sign, id, false);
            if (game.CheckVictory(row, column))
            {
                await Clients.Client(Context.ConnectionId).Victory("You win!");
                await Clients.GroupExcept(game.Id, Context.ConnectionId).Victory("You lose(");
                repository.Games.Remove(game);
                return;
            }
            else if(game.CheckDraw())
            {
                await Clients.Group(game.Id).Victory("Draw game...");
                repository.Games.Remove(game);
                return;
            }
            game.NextPlayer();
        }

        public override async Task OnConnectedAsync()
        {
            var game = repository.Games.FirstOrDefault(g => g.HasPlayer(Context.User.Identity.Name));
            if (game != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
                await Clients.Client(Context.ConnectionId).RenderBoard(game.Board, game.Name, game.Tags.ToArray());
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var game = repository.Games.FirstOrDefault(g => 
            g.Player1.ConnectionId == Context.User.Identity.Name || 
            g.Player2.ConnectionId == Context.User.Identity.Name);
            if (!(game is null))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.Id);
                await Clients.Group(game.Id).Disconnect();
                repository.Games.Remove(game);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}