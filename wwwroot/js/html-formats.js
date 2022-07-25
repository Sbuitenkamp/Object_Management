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