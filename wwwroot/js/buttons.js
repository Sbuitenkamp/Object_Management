function openHidden() {
    const hidden = document.querySelector("div.hidden");
    hidden.style.display = "flex";
    hidden.style.opacity = "100";
}

function closeHidden() {
    const hidden = document.querySelector("div.hidden");
    hidden.style.display = "none";
    hidden.style.opacity = "0";
}

function deleteType(id) {
    const data = {
        Type: "drop",
        Table: "ObjectType",
        Forms: [ { id } ]
    }
    const http = new XMLHttpRequest();
    http.open("POST", "/Objects");
    http.setRequestHeader("Content-type", "application/json");
    http.setRequestHeader("RequestVerificationToken", document.querySelector(`input[name="__RequestVerificationToken"]`).value);
    http.send(JSON.stringify(data));
    http.onload = () => location.reload();
}