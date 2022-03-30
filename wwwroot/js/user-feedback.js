window.onload = () => {
    document.querySelectorAll("h1.warning, h1.confirmation").forEach(e => {
        if (e.innerHTML !== "") return e.style.display = "block";
    });
}