let data = {};
let rightNow;
let offset = 0;

window.onload = () => {
    // custom defaultSelected attribute for dropdowns because nothing can ever be easy in html
    document.querySelectorAll("option").forEach(option => {
        if (!option.selected) return;
        option.setAttribute("defaultSelected", "true");
    });

    // custom input number fields to prevent negative numbers
    document.querySelectorAll(`input[type="number"]`).forEach(input => {
        input.addEventListener("input", function (e) {
            if (input.value <= 0) input.value = 0;
        });
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