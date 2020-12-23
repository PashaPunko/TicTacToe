using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TicTacToe;

namespace TicTacToe.Hubs
{
    public interface ILobbyClient
    {
        Task UpadateEvaluableGames(Game[] games);
        Task UpadateEvaluableTags(string[] tags);
        Task CloseGame(string id);
    }

    public class LobbyHub : Hub<ILobbyClient>
    {
        private IGameRepository repository;
        private Dictionary<string, int> tags;

        public LobbyHub(IGameRepository repository)
        {
            this.repository = repository;
            tags = new Dictionary<string, int>();
            repository.Games.ForEach(game =>
            {
                game.Tags.ForEach(tag => {
                    if (tags.ContainsKey(tag)) tags[tag]++;
                    else tags.Add(tag, 1);
                });
            });
            
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Client(Context.ConnectionId)
                .UpadateEvaluableGames(repository.Games.Where(g => !g.InProgress && g.CurrentField != null).Take(10).ToArray());
            await Clients.Client(Context.ConnectionId)
                .UpadateEvaluableTags(this.tags.Keys.ToArray());
        }

        public async Task Find(string tags)
        {
            if (tags == null || tags == "") {
                await Clients.Client(Context.ConnectionId)
                .UpadateEvaluableGames(repository.Games.Where(g => !g.InProgress && g.CurrentField!=null).Take(10).ToArray());
            }
            else{
                List<string> input = new List<string>(tags.Split(' '));
                var games = repository.Games.Where(g => !g.InProgress && g.Tags.Intersect(input).Count() != 0 && g.CurrentField != null).Take(10);
                await Clients.Client(Context.ConnectionId)
                    .UpadateEvaluableGames(games.ToArray());
            }
            
        }

        public async Task Create(string name, string tags)
        {
            Game game = new Game();
            game.Id = Guid.NewGuid().ToString();
            game.Name = name != null && name != "" ? name : "New Game";
            game.Tags = tags != null && tags!="" ? new List<string>(tags.Split(' ')) : new List<string>();
            game.Player1.ConnectionId = Context.User.Identity.Name;
            game.Player1.Sign = Game.CrossCell;
            game.CurrentPlayer = game.Player1;
            repository.Games.Add(game);
            game.Tags.ForEach(tag => {
                    if (this.tags.ContainsKey(tag)) this.tags[tag]++;
                    else this.tags.Add(tag, 1);
            });
            await Clients.All
                .UpadateEvaluableTags(this.tags.Keys.ToArray());
        }
        public async Task Update()
        {
            await Clients.Client(Context.ConnectionId)
                .UpadateEvaluableGames( repository.Games.Where(g => !g.InProgress && g.CurrentField != null).Take(10).ToArray());
        }

        public async Task CloseGame(string id)
        {
            repository.Games.Find(g => g.Id == id).Tags.ForEach(tag =>{
             tags[tag]--;
             if (tags[tag] == 0) tags.Remove(tag);
            });
            await Clients.All
                .CloseGame(id);
            await Clients.All
                .UpadateEvaluableTags(this.tags.Keys.ToArray());
            }
    }
}