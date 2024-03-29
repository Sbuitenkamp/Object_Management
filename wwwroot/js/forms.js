function onClickGiveOut(resId) {
    data = {
        QueryType: "giveOut",
        Reservations: [{
            reservation_number: resId,
            Objects: []
        }]
    }
    const container = document.querySelector("div.content");
    container.querySelectorAll("input").forEach(input => {
        if (!input.checked) return;
        const value = { object_number: parseInt(input.name) };
        data.Reservations[0].Objects.push(value);
    });
    post(data, window.location.href, "/reservations");
}

function onClickUpdate(type, table, isHidden) {
    data = {
        QueryType: type,
        [table + 's']: [] // create the list for the data to update with the correct name
    }
    markForms(isHidden);
    parseForms(table + 's');
    console.log(data);
    post(data, window.location.href);
}

// mark the forms as changed to prevent redundant data updating
function markForms(isHidden) {
    let container;
    if (isHidden) container = document.querySelector("div.hidden");
    else container = document.querySelector("div.content");
    container.querySelectorAll("input, option").forEach(inputField => {
        if (inputField.type === "hidden" || inputField.disabled || inputField.name === "toggle") return; // skip unusable fields and fields that don't have actual values
        const parentForm = document.querySelector(`form[id="${inputField.form.id}"]`); // get the form
        if (parentForm.getAttribute("changed") === "true") return; // skip already marked forms
        if (inputField.tagName.toLowerCase() === "input") {
            // check if the value on load matches with the current value
            if (((inputField.type === "text" || inputField.type === "number") && inputField.value !== inputField.defaultValue) || (inputField.type === "checkbox" && inputField.checked !== inputField.defaultChecked)) parentForm.setAttribute("changed", "true");
        } else if (inputField.tagName.toLowerCase() === "option") {
            // same as above but for option fields
            if (!inputField.selected) return;
            if (!inputField.hasAttribute("defaultSelected")) parentForm.setAttribute("changed", "true");
        }
    });
}

// turn all changed forms to an object
function parseForms(table, noNeedForChange, container) {
    let counter = 0;
    let forms;
    let searchContainer;
    if (container) searchContainer = container;
    else searchContainer = document;
    // get all the (changed) forms
    if (noNeedForChange) forms = searchContainer.querySelectorAll("form");
    else forms = searchContainer.querySelectorAll(`form[changed="true"]`);
    forms.forEach(form => {
        // initialize some data with special treatment for 2 tables
        if (/(object)(?!type){4}/g.test(table.toLowerCase())) data[table][counter] = { object_number: parseInt(form.id) };
        else if (/(reservation)/g.test(table.toLowerCase())) data[table][counter] = { reservation_number: parseInt(form.id) };
        else data[table][counter] = { id: parseInt(form.id) };
        
        // get all form fields from the current form
        searchContainer.querySelectorAll(`input[form="${form.id}"], select[form="${form.id}"] > option`).forEach(field => {
            if ((field.disabled && !noNeedForChange) || field.type === "hidden" || field.name === "toggle") return; // as usual skip unusable fields
            let value;
            if (field.tagName.toLowerCase() === "input") { // handle input
                if (field.type === "checkbox") value = field.checked;
                else if (field.type === "number") value = parseInt(field.value);
                else if (field.type === "text") {
                    if (/\d/.test(field.value) && !["adres", "telephone"].includes(field.name)) { // parse numbers
                        // dates that have (partially) spelled out months
                        if (/(jan|feb|maa|mar|apr|may|mei|jun|jul|aug|sep|oct|okt|nov|dec)/ig.test(field.value)) {
                            const newDate = new Date(field.value);
                            value = `${newDate.getDate()} ${monthNamesEnum[newDate.getMonth()]} ${newDate.getFullYear()}`;
                        } else if (field.value.includes('.')) value = parseFloat(field.value);
                        else value = parseInt(field.value);
                    } else value = field.value;
                }
            } else if (field.tagName.toLowerCase() === "option") { // handle select
                if (!field.selected) return;
                field.name = field.parentElement.name; // get the name of the select tag
                if (field.name === "object_type") { // special treatment for objectTypes
                    field.name = "Type";
                    value = { 
                        id: parseInt(field.value),
                        description: field.innerHTML // get description text
                    };
                } else if (/\d/.test(field.value)) { // parse numbers
                    if (field.value.includes('.')) value = parseFloat(field.value); // floats
                    else value = parseInt(field.value);
                } else value = field.value;
            }
            if (/object(?![_*-])([A-z])/g.test(field.name)) { // test for fields that start with lowercase object and then precede with an attached word without any symbols in between (for reservation sort)
                const newName = field.name.replace("object", "");
                const id = field.id.replace(newName, "obj");
                if (!data[table][counter].Objects) data[table][counter].Objects = [];
                if (!data[table][counter].Objects[id]) data[table][counter].Objects[id] = {};
                data[table][counter].Objects[id][newName] = value;
                return;
            }
            if (field.name.startsWith("sale")) { // special treatment for sales
                const id = parseInt(field.name.replace("sale", ''));
                const days = document.querySelector(`label[for="sale${id}"]`).innerHTML.split('-');
                const sale = {
                    id,
                    days_to_rent: parseInt(days[0]),
                    days_to_pay: parseInt(days[1]),
                    IsApplied: value
                }
                if (!data[table][counter].Sales) data[table][counter].Sales = []; // initialize array if not exists
                data[table][counter].Sales.push(sale);
            } else data[table][counter][field.name] = value;
        });
        counter++;
    });
}