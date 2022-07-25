function sortBy(table, field, descending) {
    const thead = document.querySelector(`thead.content__table__head`);
    const tbody = document.querySelector(`tbody.content__table__body`);
    data = {
        [table]: [] // create the list for the data to update with the correct name
    }
    parseForms(table, true);
    
    if (descending) data[table].sort((a,b) => (b[field] > a[field]) ? 1 : ((a[field] > b[field] ? -1 : 0)));
    else data[table].sort((a,b) => (a[field] > b[field]) ? 1 : ((b[field] > a[field] ? -1 : 0)));
    
    // backup some important functional stuff
    const requestToken = tbody.querySelector("input[name='__RequestVerificationToken']");
    const hidden = tbody.querySelectorAll("input[type='hidden']");

    console.log(requestToken)
    // empty table and repopulate with the important stuff
    tbody.innerHTML = ""; 
    tbody.appendChild(requestToken)
    for (const hiddenInput of hidden) tbody.appendChild(hiddenInput);

    // window[table] = the individual item of data[table], window[table+"Row"] = the function that returns the HTML row as defined in html-formats.js
    for (window[table] of data[table]) tbody.innerHTML += window[table + "Row"](window[table]);
    
    // visual indicators
    const button = thead.querySelector(`a[onclick="sortBy('${table}', '${field}', ${descending})"]`);
    button.children[0].classList.remove("fa-sort");
    button.children[0].classList.remove(descending ? "fa-sort-up" : "fa-sort-down");
    button.children[0].classList.add(descending ? "fa-sort-down" : "fa-sort-up");
    // flip the descending parameter
    button.setAttribute("onclick", `sortBy('${table}', '${field}', ${!descending})`);
}

function selectMore(offset, table) {
    const data = {
        QueryType: "select",
        [table + 's']: [ { } ],
        Offset: offset
    };
    // TODO: Auto sort when loading in
    post(data, window.location.href);
}

function toggleHidden(element) {
    const id = element.id.split('-')[1];
    const content = document.querySelector(`tr#reservation-body-${id}`);
    const label = document.querySelector(`label[for="toggle-${id}"]`).children[0];

    // change + to - and show content or vice versa
    if (!element.checked) {
        content.style.display = "none";
        label.classList.remove("fa-minus");
        label.classList.add("fa-plus");
    } else {
        content.style.display = "table-row";
        label.classList.remove("fa-plus");
        label.classList.add("fa-minus");
    }
}

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

function deleteType(id, name) {
    if (!confirm(`Weet u zeker dat u fietssoort ${name} wilt verwijderen?`)) return;
    const data = {
        QueryType: "drop",
        ObjectTypes: [ { id } ]
    };
    post (data, "/Objects");
}

function deleteObject(id) {
    if (!confirm(`Weet u zeker dat u object ${id} wilt verwijderen?`)) return;
    const data = {
        QueryType: "drop",
        Objects: [ { object_number: id } ]
    };
    post (data, "/Objects");
}

function deleteCustomer(id, name) {
    if (!confirm(`Weet u zeker dat u klant ${name} wilt verwijderen?`)) return;
    const data = {
        QueryType: "drop",
        Customers: [ { id }]
    };
    post (data, "/Customers");
}

function returnReservation(id) {
    if (!confirm(`Weet u zeker dat u reservering ${id} wilt inleveren?`)) return;
    const paid = document.querySelector(`input[type="checkbox"][form="${id}"]`).checked;
    if (!paid) return alert(`Reservering ${id} is nog niet betaald!`);

    const data = {
        QueryType: "edit",
        Reservations: [ { reservation_number: id, returned: true, paid }]
    };
    post (data, "/Reservations");
}

function deleteReservation(id) {
    if (!confirm(`Weet u zeker dat u reservering ${id} wilt verwijderen?`)) return;
    const data = {
        QueryType: "drop",
        Reservations: [ { reservation_number: id }]
    };
    post (data, "/Reservations");
}

function deleteSale(id, days_to_pay, days_to_rent) {
    if (!confirm(`Weet u zeker dat u aanbieding ${days_to_rent}-${days_to_pay} wilt verwijderen?`)) return;
    const data = {
        QueryType: "drop",
        Sales: [ { id }]
    };
    post (data, "/objects/sales");
}

function payReservation(id) {
    if (!confirm(`Weet u zeker dat u reservering ${id} als betaald wilt markeren?`)) return;
    const data = {
        QueryType: "edit",
        Reservations: [ { reservation_number: id, paid: true, returned: false }]
    };
    post (data, "/Reservations");
}

function unReserveObject(id, resId, name) {
    if (!confirm(`Weet u zeker dat u objectsoort ${name} uit reservering ${resId} wilt verwijderen?`)) return;
    const data = {
        QueryType: "drop",
        Reservations: [ { 
            reservation_number: resId, 
            Objects: [ { Type: { id } }]
        }]
    };
    post (data, "/Reservations");
}

function addWeek(reverse) {
    const inputField = document.querySelector(`input[name="SelectedDate"]`);
    const daysToAdd = reverse ? -7 : 7;
    const value = new Date(inputField.value);
    if (value.addDays(daysToAdd).getTime() >= rightNow) inputField.value = value.addDays(daysToAdd).formatISO();
}

function changeDate() {
    const dateToChange = document.querySelector(`input[name="SelectedDate"]`).value;
    window.location.href = `/index?selectedDate=${dateToChange}`;
}

// xmlhttprequest function
function post(data, url, redirect) {
    const http = new XMLHttpRequest();
    http.open("POST", url);
    http.setRequestHeader("Content-type", "application/json");
    http.setRequestHeader("RequestVerificationToken", document.querySelector(`input[name="__RequestVerificationToken"]`).value);
    http.send(JSON.stringify(data));
    http.onload = () => {
        let ready = true;
        const result = JSON.parse(http.responseText);
        if (result) {
            if (result.success || result.warning) {
                const type = result.success ? "confirmation" : "warning"; // set the type for dynamic class injection
                const negType = result.success ? "warning" : "confirmation";
                const textField = document.querySelector(`h1.${type}`);
                const negField = document.querySelector(`h1.${negType}`);
                if (textField.classList.contains(`${type}--activated`)) {
                    textField.classList.remove(`${type}--activated`);
                    textField.classList.add(`${type}--deactivated`);
                    ready = false;
                }
                new Promise((resolve) => {
                    if (ready) return resolve();
                    setTimeout(() => {
                        textField.classList.add(`${type}--hidden`);
                        ready = true;
                        resolve();
                    }, 300);
                }).then(() => setTimeout(() => message(textField, result.success ?? result.warning, type, negType, negField), 100)); // small unnoticeable timeout to make a second edit without refreshing show up too
            } else {
                const table = document.querySelector(`tbody.content__table__body`);
                const button = document.querySelector(`button.button--load-more`);
                if (result.customers) {
                    for (const customer of result.customers) table.innerHTML += customerRow(customer);
                    button.setAttribute("onclick", `selectMore(${result.offset}, "Customer")`);
                    offset = result.offset;
                }
            }
        } else {
            console.log(offset)
            if (offset !== 0) {
                let newSearch = "";
                if (window.location.search) newSearch = window.location.search + "loadMore=" + offset;
                else newSearch = "?loadmore=" + offset;
                console.log(window.location.href.toString() + newSearch);
                return window.location.replace(window.location.href.toString() + newSearch);
            } else if (redirect) return window.location.replace(redirect);
            window.location.reload();
        }
    }
}

// resultmsg function
function message(textField, msg, type, negType, negField) {
    if (type === "warning") msg = `Er is een fout opgetreden: "${msg}" Probeer het later nog eens. Contacteer uw administrator als dit blijft gebeuren.`;
    if (negField.classList.contains(`${negType}--activated`)) { // remove error
        negField.classList.remove(`${negType}--activated`);
        negField.classList.add(`${negType}--deactivated`);
        setTimeout(() => negField.classList.add(`${negType}--hidden`), 300);
    }
    textField.innerHTML = msg;
    textField.classList.remove(`${type}--hidden`);
    textField.classList.remove(`${type}--deactivated`);
    textField.classList.add(`${type}--activated`);

    setTimeout(() => {
        textField.classList.remove(`${type}--activated`);
        textField.classList.add(`${type}--deactivated`);
        setTimeout(() => textField.classList.add(`${type}--hidden`), 300);
    }, 5000);
}

// date methods
Date.prototype.addDays = function(days) {
    const date = new Date(this);
    date.setDate(date.getDate() + days);
    return date;
}

Date.prototype.formatISO = function() {
    let d = new Date(this),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('-');
}