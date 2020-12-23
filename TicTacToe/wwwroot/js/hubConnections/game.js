const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/Game")
    .build();
hubConnection.serverTimeoutInMilliseconds = 1000 * 60 * 10;
let curPlayer = false
hubConnection.on('Turn', function (sign, id, isCur) {
    document.getElementById(id).src = "../../../../img/" + sign;
    document.getElementById(id).onclick = null;
    curPlayer = isCur
});
hubConnection.on('Disconnect', function () {
    modal = new bootstrap.Modal(document.getElementById('victory'), {
        keyboard: false,
        backdrop: 'static'
    })
    gameResult = document.getElementById('game-result')
    gameResult.innerHTML = `<h3>Your Opponent has been disconnected</h3>`
    modal.show()
});
hubConnection.on('Victory', function (result) {
    modal = new bootstrap.Modal(document.getElementById('victory'), {
        keyboard: false,
        backdrop: 'static'
    })
    gameResult = document.getElementById('game-result')
    gameResult.innerHTML = `<h3>${result}</h3>`
    modal.show()
});
hubConnection.on('RenderBoard', function (board, name, tags) {
    for (i = 0; i < 3; i++) {
        for (j = 0; j < 3; j++) {
            document.getElementById(i * 3 + j).src = "../../../../img/" + board[i][j];
        }
    }
    
    nameOfGame = document.getElementById('name-of-game')
    nameOfGame.innerHTML = `<h1>${name}</h1>`
    tagsOfGame = document.getElementById('tags-of-game')
    tags.forEach(tag => {
        divtag = document.createElement("div");
        divtag.classList.add('tag');
        divtag.innerHTML = `<h6>#${tag}</h6>`;
        tagsOfGame.appendChild(divtag);
    })
    curPlayer = true;
});

hubConnection.start();
function Move(id) {
    if (curPlayer) {
        hubConnection.invoke('TryMove', id);
        curPlayer = false;
    }
    
}
function ToHomePage() {
    window.location.href = "/Home/Index"
}