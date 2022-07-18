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
            const type = result.success ? "confirmation" : "warning"; // set the type for dynamic class injection
            const negType = result.success ? "warning" : "confirmation";
            const textField = document.querySelector(`h1.${type}`);
            const negField = document.querySelector(`h1.${negType}`);
            if (textField.classList.contains(`${type}--activated`)) {
                textField.classList.remove(`${type}--activated`);
                textField.classList.add(`${type}--deactivated`);
                ready = false;
            }
            new Promise ((resolve) => {
                if (ready) return resolve();
                setTimeout(() => {
                    textField.classList.add(`${type}--hidden`);
                    ready = true;
                    resolve();
                }, 300);
            }).then(() => setTimeout(() => message(textField, result.success ?? result.warning, type, negType, negField), 100)); // small unnoticeable timeout to make a second edit without refreshing show up too
        } else {
            if (redirect) return window.location.replace(redirect);
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