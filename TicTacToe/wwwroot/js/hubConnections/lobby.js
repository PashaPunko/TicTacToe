const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/Lobby")
    .build();

hubConnection.serverTimeoutInMilliseconds = 1000 * 60 * 10;
let tagifyCreate = new Tagify(document.getElementById('createTags'), {
    originalInputValueFormat: valuesArr => valuesArr.map(item => item.value).join(' '),
})
possibleTags = []
let tagifyFind = new Tagify(document.getElementById('tagsToFind'), {
    originalInputValueFormat: valuesArr => valuesArr.map(item => item.value).join(' '),
    whitelist: possibleTags,
    enforceWhitelist: true,
})
hubConnection.on('UpadateEvaluableTags', function (tags) {
    possibleTags = tags
    tagifyFind.destroy()
    tagifyFind = new Tagify(document.getElementById('tagsToFind'), {
        originalInputValueFormat: valuesArr => valuesArr.map(item => item.value).join(' '),
        whitelist: possibleTags,
        enforceWhitelist: true,
    })
})
document.getElementById('update').addEventListener("click", () => {
    hubConnection.invoke('Update')
});
document.getElementById('find').addEventListener("click", () => {
    hubConnection.invoke('Find', document.getElementById('tagsToFind').value)
});
document.getElementById('create').addEventListener("click", async () => {
    await hubConnection.invoke('Create',
        document.getElementById('createName').value,
        document.getElementById('createTags').value);
    window.location.href = `/Game/Index`
});

hubConnection.on('UpadateEvaluableGames', function (games) {
    container = document.getElementById('games');
    while (container.firstChild) {
        container.removeChild(container.lastChild);
    }
    games.forEach(el => {
        game = document.createElement("div");
        game.classList.add("col-lg-3");
        game.classList.add("mb-3");
        game.classList.add("col-md-4");
        game.classList.add("col-sm-5");
        click = document.createElement("img");
        click.id = el.id;
        click.classList.add("img-fluid");
        click.classList.add("shadow-on-hover")
        click.classList.add("black-shadow")
        click.src = "../../../../img/" + el.currentField;
        click.onclick = function () {
            Close(el.id)
        };
        game.appendChild(click)
        container.appendChild(game)
    });
    if (games.length == 0) {
        div = document.createElement("div");
        div.classList.add("col-10");
        div.classList.add("m-2");
        div.classList.add("justify-content-center");
        div.innerHTML = "<h5>No available games now. Please, Update or Create new one<h5>"
        container.appendChild(div)
    }
});
hubConnection.on('CloseGame', function (id) {
    if (document.getElementById(id) !== null) {
        document.getElementById(id).src = "../../../../img/closed.png"
        document.getElementById(id).onclick = null
    }
});

hubConnection.start();
async function Close(id) {
    await hubConnection.invoke('CloseGame', id);
    window.location.href = `/Game/Index?id=${id}`
}
