let data = {};
let rightNow;

// custom defaultSelected attribute for dropdowns because nothing can ever be easy in html
window.onload = () => {
    document.querySelectorAll("option").forEach(option => {
        if (!option.selected) return;
        option.setAttribute("defaultSelected", "true");
    });
    
    // limit checkboxes
    document.querySelectorAll(`div.checkContainer[limit] div.checkWrap input[type="checkbox"]`).forEach(check => {
        const container = check.parentElement.parentElement
        const max = parseInt(container.getAttribute("limit"));
        check.onclick = selectiveCheck;

        function selectiveCheck (event) {
            const checkedChecks = document.querySelectorAll(`div#${container.id}[limit] input[type="checkbox"]:checked`);
            const type = document.querySelector(`div#${container.id}[limit] p.type`).innerHTML;
            if (checkedChecks.length >= max + 1) {
                alert(`Er zijn al ${max} van het type ${type} geselecteerd.`);
                return false;
            }
        }
    });
    const date = new Date().formatISO();
    rightNow = new Date(date).getTime();
}