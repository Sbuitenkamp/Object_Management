function sortBy(button, table, field, descending, subField, upperTable, clear) {
    const thead = document.querySelector(`thead.content__table__head`);
    let container;
    data = {
        [table]: [], // create the list for the data to update with the correct name
        objectTypes: []
    };
    memory.sort = { field, descending, subField };
    const selector = table === "reservation" && upperTable ? "table#table-upper" : table === "reservation" ? "table#table-lower" : "";
    if (selector) container = document.querySelector(selector);
    parseForms(table, true, container);
    if (table === "reservation") {
        for (const reservation of data[table]) {
            if (!reservation.Customer) reservation.Customer = {};
            for (const property of Object.getOwnPropertyNames(reservation)) {
                if (/customer(?![_*-])([A-z])/g.test(property)) {
                    const newName = property.replace("customer", "");
                    reservation.Customer[newName] = reservation[property];
                    delete reservation[property];
                }
            }
            // set the object array to proper indexes
            let counter = 0;
            for (const obj in reservation.Objects) {
                if (typeof reservation.Objects[obj] === 'function') continue;
                reservation.Objects[counter] = reservation.Objects[obj];
                delete reservation.Objects[obj];
                counter++;
            }
        }
    }
    // switch the types into a different array
    for (const item of data[table]) {
        if (!(item.description)) continue;
        item.id = item.object_number;
        delete item.object_number;
        data.objectTypes.push(item);
        delete data[table][data[table].indexOf(item)];
    }
    data[table] = data[table].clean();

    console.log(data);
    sortData(data[table], memory.sort);

    clearTable(container);
    // window[table] = the individual item of data[table], window[table+"Row"] = the function that returns the HTML row as defined in html-formats.js
    for (window[table] of data[table]) renderTable(window[`${table}Row${upperTable ? "Upper" : ""}`](window[table], data), container);
    
    // reset other buttons
    thead.querySelectorAll("a[onclick]").forEach(button => {
        const arrow = button.children[0];
        if (!arrow) return;
        if ((arrow.className.includes("fa-sort-down") || arrow.className.includes("fa-sort-up"))) {
            arrow.classList.remove("fa-sort-up", "fa-sort-down");
            arrow.classList.add("fa-sort");
            const newClick = button.getAttribute("onclick").replace("true", "false");
            button.setAttribute("onclick", newClick.toString());
        }
    });
    if (!clear) { // no flip for the reset button
        // visual indicators
        button.children[0].classList.remove("fa-sort", descending ? "fa-sort-up" : "fa-sort-down");
        button.children[0].classList.add(descending ? "fa-sort-down" : "fa-sort-up");
        // flip the descending parameter TODO fix uppertable param
        button.setAttribute("onclick", `sortBy(this, '${table}', '${field}', ${!descending}, ${subField ? `'${subField}'` : null}, ${upperTable ? `${upperTable}` : null})`);
        // set the uppertable for the clear button
        document.querySelector("a#empty-button").setAttribute("onclick", `sortBy(this, '${table}', '${field}', ${!descending}, ${subField ? `'${subField}'` : null}, ${upperTable ? `${upperTable}` : null}, true)`);
    }
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
    window.location.reload();
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
function post(dataToSend, url, redirect) {
    const http = new XMLHttpRequest();
    http.open("POST", url);
    http.setRequestHeader("Content-type", "application/json");
    http.setRequestHeader("RequestVerificationToken", document.querySelector(`input[name="__RequestVerificationToken"]`).value);
    http.send(JSON.stringify(dataToSend));
    http.onload = () => {
        let ready = true;
        console.log(http.responseText)
        const result = JSON.parse(http.responseText);
        if (result) {
            console.log(result);
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
                new Promise((resolve) => { // animation timing
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
            if (result.body) {
                const { table } = result.body;
                console.log(data);
                data = { [table]: [], objectTypes: [] };
                parseForms(table, true);

                // switch the types into a different array
                for (const item of data[table]) {
                    if (!item.description) continue;
                    item.id = item.object_number;
                    data.objectTypes.push(item);
                }
                
                // no need for the existing data since it's outdated
                delete data[table];
                clearTable(); // todo upper table container
                // const selector = table === "reservation" && upperTable ? "table#table-upper" : table === "reservation" ? "table#table-lower" : "";

                for (const row of result.body.data) {
                    row.Type = row.type; // annoying capitalisation stuff
                    delete row.type;
                }

                if (memory.sort) sortData(result.body.data, memory.sort);
                for (const row of result.body.data) {
                    // todo keep sorted when rendering post result client side, then include in all other pages
                    const html = window[table.toLowerCase() + "Row"](row, data);
                    renderTable(html);
                }
            }
        } else {
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

// result message function
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

// sort
function sortData(dataToSort, {field, descending, subField}) {
    if (field === "sale") { // sales are too complicated to do procedurally
        dataToSort.sort((a,b) => {
            let saleA = data.objectTypes.find(x => x.id === a.Type.id).Sales.find(x => x.IsApplied);
            let saleB = data.objectTypes.find(x => x.id === b.Type.id).Sales.find(x => x.IsApplied);
            if (!saleA) saleA = { days_to_rent: 0 };
            if (!saleB) saleB = { days_to_rent: 0 };
            if (descending) return (saleB.days_to_rent > saleA.days_to_rent) ? 1 : ((saleA.days_to_rent > saleB.days_to_rent ? -1 : 0));
            return (saleA.days_to_rent > saleB.days_to_rent) ? 1 : ((saleB.days_to_rent > saleA.days_to_rent ? -1 : 0));
        })
    } else if (subField) {
        if (descending) dataToSort.sort((a,b) => (b[field][subField] > a[field][subField]) ? 1 : ((a[field][subField] > b[field][subField] ? -1 : 0)));
        else dataToSort.sort((a,b) => (a[field][subField] > b[field][subField]) ? 1 : ((b[field][subField] > a[field][subField] ? -1 : 0)));
    } else {
        if (descending) dataToSort.sort((a, b) => (b[field] > a[field]) ? 1 : ((a[field] > b[field] ? -1 : 0)));
        else dataToSort.sort((a, b) => (a[field] > b[field]) ? 1 : ((b[field] > a[field] ? -1 : 0)));
    }
}

// re-render table
function clearTable(container) {
    const searchContainer = container ?? document;
    const tbody = searchContainer.querySelector(`tbody.content__table__body`);
    // backup some important functional stuff
    const requestToken = tbody.querySelector("input[name='__RequestVerificationToken']");
    const hidden = tbody.querySelectorAll("input[type='hidden']");

    // empty table and repopulate with the important stuff
    tbody.innerHTML = "";
    tbody.appendChild(requestToken)
    for (const hiddenInput of hidden) tbody.appendChild(hiddenInput);
}
function renderTable(html, container) {
    const renderContainer = container ?? document;
    renderContainer.querySelector(`tbody.content__table__body`).innerHTML += html;
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

// array methods
Array.prototype.clean = function() {
    const result = [];
    let index = -1;
    let resIndex = -1;

    while (++index < this.length) {
        const value = this[index];
        if (value) result[++resIndex] = value;
    }

    return result;
}

Number.prototype.formatCurrency = function () {
    return currencyFormatter.format(this).replace(/(?:\$|€|£)/, '');
}