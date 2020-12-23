using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TicTacToe.Controllers
{
    public class GameController : Controller
    {
        private IGameRepository repository;
        public GameController( IGameRepository repository)
        {
            this.repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? id)
        {
            if (!User.Identity.IsAuthenticated) {
                return RedirectToAction("Login", "Account");
            }
            if (id is null)
            {
                if (repository.Games.FirstOrDefault(g => g.HasPlayer(User.Identity.Name)) != null) {
                    return View();
                }
                return RedirectToAction("Index", "Home");
            }
            var game = repository.Games.FirstOrDefault(g => g.Id==id);
            if (game is null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (game.Player2.ConnectionId == User.Identity.Name || game.Player1.ConnectionId == User.Identity.Name)
            {
                return View();
            }
            if (game.Player2.ConnectionId is null)
            {
                game.Player2.ConnectionId = User.Identity.Name;
                game.Player2.Sign = Game.ZeroCell;
                game.InProgress = true;
                game.CurrentPlayer = game.Player2;
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Index", "Home");
        }

    }
}
