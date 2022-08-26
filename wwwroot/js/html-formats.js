function customerRow(customer) {
    return `
        <form method="post" id="${customer.id}"></form>
        <p class="indicator"></p>
        <tr class="content__table__body__row">
            <td class="content__table__body__row__element">
                <input type="text" name="name" form="${customer.id}" value="${customer.name}">
            </td>
            <td class="content__table__body__row__element">
                <input type="text" name="telephone" form="${customer.id}" value="${customer.telephone}">
            </td>
            <td class="content__table__body__row__element">
                <input type="text" name="email" form="${customer.id}" value="${customer.email}">
            </td>
            <td class="content__table__body__row__element">
                <input type="text" name="adres" form="${customer.id}" value="${customer.adres}">
            </td>
            <td class="content__table__body__row__element">
                <button onclick="deleteCustomer(${customer.id}, ${customer.name})" class="content__table__body__row__element button button--delete"><i class="fa-solid fa-trash-can"></i></button>
            </td>
        </tr>
    `;
}

function objectRow(obj, { objectTypes }) {
    const type = objectTypes.find(e => e.id === obj.Type.id);
    let objectTypesHtml = `<select name="object_type" form="${obj.object_number}">`;
    let sizesHtml = `
        <select form="${obj.object_number}" name="size"> 
        <option value="">Geen</option>
    `;
    let salesHtml = ``;
    
    for (const objectType of objectTypes) objectTypesHtml += `<option ${objectType.id === obj.Type.id ? "selected" : ""} value="${objectType.id}">${objectType.description}</option>`;
    for (const size in sizesEnum) sizesHtml += `<option ${obj.size === size ? "selected" : ""} value="${size}">${size.toUpperCase()}</option>`
    for (const sale of type.Sales) if (sale.IsApplied) salesHtml += `<p class="float">${sale.days_to_rent}-${sale.days_to_pay}</p>`;
    
    if (salesHtml === ``) salesHtml = `<p>Geen</p>`;

    // closing tags
    objectTypesHtml += "</select>";
    sizesHtml += "</select>";
    return `
        <form method="post" id="${obj.object_number}"></form>
        <tr class="content__table__body__row">
            <td class="content__table__body__row__element">
                <input disabled="disabled" type="text" name="object_number" form="${obj.object_number}" value="${obj.object_number}">
            </td>
            <td class="content__table__body__row__element">
                ${objectTypesHtml}
            </td>
            <td class="content__table__body__row__element">
                &euro;<input disabled="disabled" type="text" name="price" form="${obj.object_number}" value="${type.price.formatCurrency()}"">
            </td>
            <td class="content__table__body__row__element">
                <input disabled="disabled" type="checkbox" name="loaned_out" form="${obj.object_number}" ${obj.loaned_out ? "checked" : ""}>
            </td>
            <td class="content__table__body__row__element">
                <input type="checkbox" name="in_service" form="${obj.object_number}" ${obj.in_service ? "checked" : ""}>
            </td>
            <td class="content__table__body__row__element">
                ${sizesHtml}
            </td>
            <td class="content__table__body__row__element">
                ${salesHtml}
            </td>
            <td class="content__table__body__row__element">
                <button onclick="deleteObject(${obj.object_number})" class="content__table__body__row__element button button--delete"><i class="fa-solid fa-trash-can"></i></button>
            </td>
        </tr>
    `;
}

function saleRow(sale) {
    return `
        <form method="post" id="${sale.id}"></form>
        <td class="content__table__body__row__element">
            <input type="number" name="days_to_rent" form="${sale.id}" value="${sale.days_to_rent}">
        </td>
        <td class="content__table__body__row__element">
            <input type="number" name="days_to_pay" form="${sale.id}" value="${sale.days_to_pay}">
        </td>
        <td class="content__table__body__row__element">
            <button onclick="deleteSale(${sale.id}, ${sale.days_to_pay}, ${sale.days_to_rent})" class="content__table__body__row__element button button--delete"><i class="fa-solid fa-trash-can"></i></button>
        </td>
    `;
}

function reservationRowUpper(reservation) {
    const today = new Date();
    today.setHours(0,0,0,0)
    let reservationHtml = `
        <form method="post" id="${reservation.reservation_number}"></form>
        <p class="indicator"></p>
        <tr class="content__table__body__row ${new Date(reservation.return_date) < today ? "warning warning--tr" : ""}">
            <td class="content__table__body__row__element content__table__body__row__element--toggle">
                <label for="toggle-${reservation.reservation_number}"><i class="fa-solid fa-plus pointer"></i></label>
                <input onclick="toggleHidden(this)" type="checkbox" name="toggle" id="toggle-${reservation.reservation_number}" data-toggle="toggle">
            </td>
            <td class="content__table__body__row__element">
                <input disabled="disabled" type="text" name="reservation_number" form="${reservation.reservation_number}" value="${reservation.reservation_number}">
            </td>
            <td class="content__table__body__row__element">
                <input disabled="disabled" type="text" name="customerName" form="" value="${reservation.Customer.Name}">
            </td>
            <td class="content__table__body__row__element">
                <input disabled="disabled" type="text" name="payment_method" form="${reservation.reservation_number}" value="${reservation.payment_method}">
            </td>
            <td class="content__table__body__row__element">
                <input disabled="disabled" type="text" name="start_date" form="${reservation.reservation_number}" value="${reservation.start_date}">
            </td>
            <td class="content__table__body__row__element">
                <input disabled="disabled" type="text" name="return_date" form="${reservation.reservation_number}" value="${reservation.return_date}">
            </td>
            <td class="content__table__body__row__element">
                &euro;<input disabled="disabled" type="text" name="total" form="${reservation.reservation_number}" value="${reservation.total.formatCurrency()}">
            </td>
            <td class="content__table__body__row__element">
                <input type="checkbox" name="paid" form="${reservation.reservation_number}" ${reservation.paid ? 'checked' : ''}>
            </td>
            <td class="content__table__body__row__element">
                <button onclick="returnReservation(${reservation.reservation_number})" class="content__table__body__row__element button"><i class="fa-solid fa-arrow-left"></i></button>
            </td>
            <td class="content__table__body__row__element">
                <button onclick="deleteReservation(${reservation.reservation_number})" class="content__table__body__row__element button button--delete"><i class="fa-solid fa-trash-can"></i></button>
            </td>
        </tr>
        <tr class="content__table__body__row content__table__body__row--hidden hide ${new Date(reservation.return_date) < today ? "warning warning--tr" : ""}" id="reservation-body-${reservation.reservation_number}">
            <td colspan="3">
                <table class="content__table__body__row--hidden__customer-table">
                    <tr>
                        <th>Klantnaam</th>
                        <td>
                            <input type="text" disabled="disabled" name="customerName" form="${reservation.reservation_number}" value="${reservation.Customer.Name}">
                        </td>
                    </tr>
                    <tr>
                        <th>Verblijf</th>
                        <td>
                            <input type="text" disabled="disabled" name="residence" form="${reservation.reservation_number}" value="${reservation.residence}">
                        </td>
                    </tr>
                    <tr>
                        <th>Email</th>
                        <td>
                            <input type="text" disabled="disabled" name="customerEmail" form="${reservation.reservation_number}" value="${reservation.Customer.Email}">
                        </td>
                    </tr>
                    <tr>
                        <th>Telefoonnummer</th>
                        <td>
                            <input type="text" disabled="disabled" name="customerTelephone" form="${reservation.reservation_number}" value="${reservation.Customer.Telephone}">
                        </td>
                    </tr>
                    <tr>
                        <th>Adres</th>
                        <td>
                            <input type="text" disabled="disabled" name="customerAdres" form="${reservation.reservation_number}" value="${reservation.Customer.Adres}">
                        </td>
                    </tr>
                </table>
            </td>
            <td colspan="10">
                <table class="content__table__body__row--content__objects-table">
                <thead>
                <tr>
                    <th>Fietsnummer</th>
                    <th>Fietssoort</th>
                    <th>Prijs</th>
                    <th></th>
                </tr>
                </thead>
                <tbody>
    `;
    
    // add the objects
    for (const objectData of reservation.Objects.sort((a, b) => a.TypeDescription > b.TypeDescription ? 1 : a.TypeDescription < b.TypeDescription ? -1 : 0)) { 
        reservationHtml += `    
            <tr>
                <td>
                    <input type="text" disabled="disabled" id="ObjectNumber${objectData.ObjectNumber}" name="objectObjectNumber" form="${reservation.reservation_number}" value="${objectData.ObjectNumber}">
                </td>
                <td>
                    <input type="text" disabled="disabled" id="TypeDescription${objectData.ObjectNumber}" name="objectTypeDescription" form="${reservation.reservation_number}" value="${objectData.TypeDescription}">
                </td>
                <td class="currency">
                    &euro;<input type="text" disabled="disabled" id="PriceSingle${objectData.ObjectNumber}" name="objectPriceSingle" form="${reservation.reservation_number}" value="${objectData.PriceSingle}">
                </td>
            </tr>
        `;
    }
    
    // closing tags
    reservationHtml += `
                        </tbody>
                    </table>
                    </td>
                </tr>
            </tbody>
        </table>
    `;
    
    return reservationHtml;
}

function reservationRow(reservation) {
    let reservationHtml = `
        <form method="post" id="${reservation.reservation_number}"></form>
        <p class="indicator"></p>
        <tr class="content__table__body__row">
            <td class="content__table__body__row__element content__table__body__row__element--toggle">
                <label for="toggle-${reservation.reservation_number}"><i class="fa-solid fa-plus pointer"></i></label>
                <input onclick="toggleHidden(this)" type="checkbox" name="toggle" id="toggle-${reservation.reservation_number}" data-toggle="toggle">
            </td>
            <td class="content__table__body__row__element">
                <input disabled="disabled" type="text" name="reservation_number" form="${reservation.reservation_number}" value="${reservation.reservation_number}">
            </td>
            <td class="content__table__body__row__element">
                <input disabled="disabled" type="text" name="customerName" form="" value="${reservation.Customer.Name}">
            </td>
            <td class="content__table__body__row__element">
                <input disabled="disabled" type="text" name="payment_method" form="${reservation.reservation_number}" value="${reservation.payment_method}">
            </td>
            <td class="content__table__body__row__element">
                <input disabled="disabled" type="text" name="start_date" form="${reservation.reservation_number}" value="${reservation.start_date}">
            </td>
            <td class="content__table__body__row__element">
                <input disabled="disabled" type="text" name="return_date" form="${reservation.reservation_number}" value="${reservation.return_date}">
            </td>
            <td class="content__table__body__row__element">
                &euro;<input disabled="disabled" type="text" name="total" form="${reservation.reservation_number}" value="${reservation.total.formatCurrency()}">
            </td>
            <td class="content__table__body__row__element">
                <input type="checkbox" name="paid" form="${reservation.reservation_number}" ${reservation.paid ? 'checked' : ''}">
            </td>
            <td class="content__table__body__row__element">
                <a href="/reservations/giveout?resId=${reservation.reservation_number}" class="content__table__body__row__element button"><i class="fa-solid fa-person-biking"></i></a>
            </td>
            <td class="content__table__body__row__element">
                <button onclick="deleteReservation(${reservation.reservation_number})" class="content__table__body__row__element button button--delete"><i class="fa-solid fa-trash-can"></i></button>
            </td>
        </tr>
        <tr class="content__table__body__row content__table__body__row--hidden hide" id="reservation-body-${reservation.reservation_number}">
            <td colspan="3">
                <table class="content__table__body__row--hidden__customer-table">
                    <tr>
                        <th>Klantnaam</th>
                        <td>
                            <input type="text" disabled="disabled" name="customerName" form="${reservation.reservation_number}" value="${reservation.Customer.Name}">
                        </td>
                    </tr>
                    <tr>
                        <th>Verblijf</th>
                        <td>
                            <input type="text" disabled="disabled" name="residence" form="${reservation.reservation_number}" value="${reservation.residence}">
                        </td>
                    </tr>
                    <tr>
                        <th>Email</th>
                        <td>
                            <input type="text" disabled="disabled" name="customerEmail" form="${reservation.reservation_number}" value="${reservation.Customer.Email}">
                        </td>
                    </tr>
                    <tr>
                        <th>Telefoonnummer</th>
                        <td>
                            <input type="text" disabled="disabled" name="customerTelephone" form="${reservation.reservation_number}" value="${reservation.Customer.Telephone}">
                        </td>
                    </tr>
                    <tr>
                        <th>Adres</th>
                        <td>
                            <input type="text" disabled="disabled" name="customerAdres" form="${reservation.reservation_number}" value="${reservation.Customer.Adres}">
                        </td>
                    </tr>
                    <tr>
                        <th>Opmerking</th>
                    </tr>
                        <td>
                            <input type="text" disabled="disabled" name="comment" form="${reservation.reservation_number}" value="${reservation.comment}">
                        </td>
                </table>
            </td>
            <td colspan="10">
                <table class="content__table__body__row--content__objects-table">
                <thead>
                <tr>
                    <th>Fietssoort</th>
                    <th>Aantal</th>
                    <th>Prijs</th>
                    <th></th>
                </tr>
                </thead>
                <tbody>
    `;

    for (const objectData of reservation.Objects.sort((a, b) => a.TypeDescription > b.TypeDescription ? 1 : a.TypeDescription < b.TypeDescription ? -1 : 0)) {
        reservationHtml += ` 
            <tr>
                <td>
                    <input type="text" disabled="disabled" id="TypeDescription${reservation.Objects.indexOf(objectData)+1}" name="objectTypeDescription" form="${reservation.reservation_number}" value="${objectData.TypeDescription}">
                </td>
                <td>
                    <input type="text" id="Amount${reservation.Objects.indexOf(objectData)+1}" disabled="disabled" name="objectAmount" form="${reservation.reservation_number}" value="${objectData.Amount}">
                </td>
                <td class="currency">
                    &euro;<input type="text" disabled="disabled" id="PriceSingle${reservation.Objects.indexOf(objectData)+1}" name="objectPriceSingle" form="${reservation.reservation_number}" value="${objectData.PriceSingle.formatCurrency()}">
                </td>
                ${reservation.Objects.Count > 1 ?
                    '<td><button onclick="unReserveObject(' + objectData.Type.id + ', ' + reservation.reservation_number + ', \'' + objectData.Type.description + '\')" class="button button--delete"><i class="fa-solid fa-trash-can"></i></button></td>'
                    :
                    '<td><button onclick="deleteReservation(' + reservation.reservation_number + ')" class="button button--delete"><i class="fa-solid fa-trash-can"></i></button></td>'
                }
            </tr>
        `;
    }
    
    reservationHtml += `
                </tbody>
            </table>
            </td>
        </tr>
    `;
    return reservationHtml;
}