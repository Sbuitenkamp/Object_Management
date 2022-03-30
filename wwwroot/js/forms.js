let data = {
    Forms: [],
    Type: "",
    Table: ""
};

// custom defaultSelected attribute for dropdowns because nothing can ever be easy in html
window.onload = () => {
    document.querySelectorAll("option").forEach(option => {
        if (!option.selected) return;
        option.setAttribute("defaultSelected", "true");
    });
}

function onClickUpdate(type, table, isHidden) {
    data.Type = type;
    data.Table = table;
    markForms(isHidden);
    parseForms();
    console.log(data);
    sendForms();
}

// mark the forms as changed to prevent redundant data updating
function markForms(isHidden) {
    let container;
    if (isHidden) container = document.querySelector("div.hidden");
    else container = document.querySelector("div.content");
    container.querySelectorAll("input, option").forEach(inputField => {
        if (inputField.type === "hidden" || inputField.disabled) return;
        const parentForm = document.querySelector(`form[id="${inputField.form.id}"]`);
        if (parentForm.getAttribute("changed") === "true") return;
        if (inputField.tagName.toLowerCase() === "input") {
            if ((inputField.type === "text" && inputField.value !== inputField.defaultValue) || (inputField.type === "checkbox" && inputField.checked !== inputField.defaultChecked)) parentForm.setAttribute("changed", "true");
        } else if (inputField.tagName.toLowerCase() === "option") {
            if (!inputField.selected) return;
            console.log(inputField.attributes);
            if (!inputField.hasAttribute("defaultSelected")) parentForm.setAttribute("changed", "true");
        }
    });
}

// turn all changed forms to an object
function parseForms() {
    let counter = 0;
    document.querySelectorAll(`form[changed="true"]`).forEach(form => {
        data.Forms[counter] = { id: parseInt(form.id) };
        document.querySelectorAll(`input[form="${form.id}"], select[form="${form.id}"] > option`).forEach(field => {
            if (field.disabled || field.type === "hidden") return;
            let value;
            if (field.tagName.toLowerCase() === "input") {
                if (field.type === "checkbox") value = field.checked;
                else if (field.type === "text") {
                    if (/\d/.test(field.value)) { // parse numbers
                        if (field.value.includes('.')) value = parseFloat(field.value);
                        else value = parseInt(field.value);
                    } else value = field.value;
                }
            } else if (field.tagName.toLowerCase() === "option") {
                if (!field.selected) return;
                field.name = field.parentElement.name;
                if (/\d/.test(field.value)) { // parse numbers
                    if (field.value.includes('.')) value = parseFloat(field.value);
                    else value = parseInt(field.value);
                } else value = field.value;
            }
            if (field.name.startsWith("sale")) {
                const id = parseInt(field.name.replace("sale", ''));
                const sale = {
                    id,
                    object_type_id: parseInt(form.id),
                    is_applied: value
                }
                if (!data.Forms[counter].sales) data.Forms[counter].sales = [];
                data.Forms[counter].sales.push(sale);
            } else data.Forms[counter][field.name] = value;
        });
        counter++;
    });
}

function sendForms() {
    const http = new XMLHttpRequest();
    http.open("POST", "/HandlePost");
    http.setRequestHeader("Content-type", "application/json");
    http.setRequestHeader("RequestVerificationToken", document.querySelector(`input[name="__RequestVerificationToken"]`).value);
    http.send(JSON.stringify(data));
    http.onload = () => location.reload();
}