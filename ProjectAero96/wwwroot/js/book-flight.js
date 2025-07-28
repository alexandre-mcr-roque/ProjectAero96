/** SS (Session Storage) Key */ const KEY_SS_BOOKING_DATA = 'bookingData';

function addTicket(data, idx) {
    const $list = $('#ticketsList');
    let index = idx || $('#ticketsList li[data-index]').length;
    // Create new list item
    let $li;
    if (data) {
        $li = $(`
                <li class="list-group-item border rounded px-4 py-2 mb-3" data-index="${index}">
                    <div class="form-group mb-3">
                        <label for="Tickets[${index}].FirstName">First Name</label>
                        <input class="form-control" type="text" id="Tickets[${index}].FirstName" maxlength="100" name="Tickets[${index}].FirstName" value="${data.FirstName}">
                        <span class="text-danger field-validation-valid" data-valmsg-for="Tickets[${index}].FirstName" data-valmsg-replace="true"></span>
                    </div>
                    <div class="form-group mb-3">
                        <label for="Tickets[${index}].LastName">Last Name</label>
                        <input class="form-control" type="text" id="Tickets[${index}].LastName" maxlength="100" name="Tickets[${index}].LastName" value="${data.LastName}">
                        <span class="text-danger field-validation-valid" data-valmsg-for="Tickets[${index}].LastName" data-valmsg-replace="true"></span>
                    </div>
                    <div class="form-group mb-3">
                        <label for="Tickets[${index}].Email">Email Address</label>
                        <input class="form-control" type="email" id="Tickets[${index}].Email" name="Tickets[${index}].Email" value="${data.Email}">
                        <span class="field-validation-valid" data-valmsg-for="Tickets[${index}].Email" data-valmsg-replace="true"></span>
                    </div>
                    <div class="form-group mb-3">
                        <label for="Tickets[${index}].Age">Age</label>
                        <input class="form-control" type="number" data-val="true" data-val-range="Please insert a valid age." data-val-range-max="200" data-val-range-min="0" data-val-required="The Age field is required." id="Tickets[${index}].Age" name="Tickets[${index}].Age" value="${data.Age}"><input name="__Invariant" type="hidden" value="Tickets[${index}].Age">
                        <span class="text-danger field-validation-valid" data-valmsg-for="Tickets[${index}].Age" data-valmsg-replace="true"></span>
                    </div>
                    <div class="form-group mb-3">
                        <label for="Tickets[${index}].SeatNumber">Seat Number</label>
                        <input class="form-control" type="text" id="Tickets[${index}].SeatNumber" name="Tickets[${index}].SeatNumber" value="${data.SeatNumber}">
                        <span class="text-danger field-validation-valid" data-valmsg-for="Tickets[${index}].SeatNumber" data-valmsg-replace="true"></span>
                    </div>
                   <button type="button" class="btn btn-danger btn-sm remove-ticket-btn" data-index="${index}">
                       <i class="bi bi-x"></i> Remove
                   </button>
                </li>
            `);
        debugger;
        let $existing = $(`#ticketsList li[data-index="${index}"]`);
        if ($existing.length) {
            $existing.replaceWith($li);
            return;
        }
    }
    else {
        $li = $(`
                <li class="list-group-item border rounded px-4 py-2 mb-3" data-index="${index}">
                    <div class="form-group mb-3">
                        <label for="Tickets[${index}].FirstName">First Name</label>
                        <input class="form-control" type="text" id="Tickets[${index}].FirstName" maxlength="100" name="Tickets[${index}].FirstName" value="">
                        <span class="text-danger field-validation-valid" data-valmsg-for="Tickets[${index}].FirstName" data-valmsg-replace="true"></span>
                    </div>
                    <div class="form-group mb-3">
                        <label for="Tickets[${index}].LastName">Last Name</label>
                        <input class="form-control" type="text" id="Tickets[${index}].LastName" maxlength="100" name="Tickets[${index}].LastName" value="">
                        <span class="text-danger field-validation-valid" data-valmsg-for="Tickets[${index}].LastName" data-valmsg-replace="true"></span>
                    </div>
                    <div class="form-group mb-3">
                        <label for="Tickets[${index}].Email">Email Address</label>
                        <input class="form-control" type="email" id="Tickets[${index}].Email" name="Tickets[${index}].Email" value="">
                        <span class="field-validation-valid" data-valmsg-for="Tickets[${index}].Email" data-valmsg-replace="true"></span>
                    </div>
                    <div class="form-group mb-3">
                        <label for="Tickets[${index}].Age">Age</label>
                        <input class="form-control" type="number" data-val="true" data-val-range="Please insert a valid age." data-val-range-max="200" data-val-range-min="0" data-val-required="The Age field is required." id="Tickets[${index}].Age" name="Tickets[${index}].Age" value="0"><input name="__Invariant" type="hidden" value="Tickets[${index}].Age">
                        <span class="text-danger field-validation-valid" data-valmsg-for="Tickets[${index}].Age" data-valmsg-replace="true"></span>
                    </div>
                    <div class="form-group mb-3">
                        <label for="Tickets[${index}].SeatNumber">Seat Number</label>
                        <input class="form-control" type="text" id="Tickets[${index}].SeatNumber" name="Tickets[${index}].SeatNumber" value="">
                        <span class="text-danger field-validation-valid" data-valmsg-for="Tickets[${index}].SeatNumber" data-valmsg-replace="true"></span>
                    </div>
                   <button type="button" class="btn btn-danger btn-sm remove-ticket-btn" data-index="${index}">
                       <i class="bi bi-x"></i> Remove
                   </button>
                </li>
            `);
    }

    $list.append($li);
}

function storageAvailable(type) {
    // https://developer.mozilla.org/en-US/docs/Web/API/Web_Storage_API/Using_the_Web_Storage_API
    let storage;
    try {
        storage = window[type];
        const x = "__storage_test__";
        storage.setItem(x, x);
        storage.removeItem(x);
        return true;
    } catch (e) {
        return (
            e instanceof DOMException &&
            e.name === "QuotaExceededError" &&
            // acknowledge QuotaExceededError only if there's something already stored
            storage &&
            storage.length !== 0
        );
    }
}

function loadValues(bookingData) {
    $('#FirstName').val(bookingData.FirstName);
    $('#LastName').val(bookingData.LastName);
    $('#Email').val(bookingData.Email);
    $('#BirthDate').val(bookingData.BirthDate);
    $('#Address1').val(bookingData.Address1);
    $('#Address2').val(bookingData.Address2);
    $('#City').val(bookingData.City);
    $('#Country').val(bookingData.Country);
    bookingData.Tickets.forEach(function (ticket, idx) {
        if (idx === 0) {
            let tickets = $.escapeSelector('Tickets[0].');
            $(`#${tickets}FirstName`).val(ticket.FirstName);
            $(`#${tickets}LastName`).val(ticket.LastName);
            $(`#${tickets}Email`).val(ticket.Email);
            $(`#${tickets}Age`).val(ticket.Age);
            $(`#${tickets}SeatNumber`).val(ticket.SeatNumber);
        }
        else addTicket(ticket, idx);
    });
}

/**
 * 
 * @param {HTMLInputElement} element
 * @param {any} bookingData
 */
function updateValue(element, bookingData) {
    if (element.id.startsWith('Tickets[')) {
        let index = $(element).closest('li').attr('data-index');
        let prop = element.id.split('.')[1];
        bookingData.Tickets[index][prop] = $(element).val();
    }
    else {
        bookingData[element.id] = $(element).val();
    }
    window.sessionStorage.setItem(KEY_SS_BOOKING_DATA, JSON.stringify(bookingData));
}

$(function () {
    var bookingData;
    if (storageAvailable("sessionStorage")) {
        // Get account values from input fields
        let email = $('#Email').val();
        let tickets = $.escapeSelector('Tickets[0].');
        bookingData = {
            sessionUser: email || "__anonymous__",
            FirstName: $('#FirstName').val(),
            LastName: $('#LastName').val(),
            Email: email,
            BirthDate: $('#BirthDate').val(),
            Address1: $('#Address1').val(),
            Address2: $('#Address2').val(),
            City: $('#City').val(),
            Country: $('#Country').val(),
            Tickets: [{
                FirstName: $(`#${tickets}FirstName`).val(),
                LastName: $(`#${tickets}LastName`).val(),
                Email: $(`#${tickets}Email`).val(),
                Age: $(`#${tickets}Age`).val(),
                SeatNumber: $(`#${tickets}SeatNumber`).val(),
            }]
        };
        let __bookingData = window.sessionStorage.getItem(KEY_SS_BOOKING_DATA);
        if (__bookingData) {
            oldBookingData = JSON.parse(__bookingData);
            if (oldBookingData.sessionUser == bookingData.sessionUser) {
                bookingData = oldBookingData;
                loadValues(bookingData);
            }
        }
        window.sessionStorage.setItem(KEY_SS_BOOKING_DATA, JSON.stringify(bookingData));
    }
    $(document).on('click', '#add-ticket-btn', function () {
        addTicket();
        // Update session storage
        bookingData.Tickets.push({
            FirstName: '',
            LastName: '',
            Email: '',
            Age: 0,
            SeatNumber: ''
        });
    });

    $(document).on('change blur', 'input.form-control', function () { updateValue(this, bookingData) });

    $(document).on('click', '.remove-ticket-btn', function () {
        let $li = $(this).closest('li');
        let index = $li.attr('data-index');
        $li.remove();

        // Re-index inputs
        $('#ticketsList li[data-index]').each(function (idx) {
            $(this).attr('data-index', idx);
            $(this).find('div.form-group').each(function () {
                let prop = $(this).find('label').attr('for').replace(/Tickets\[\d+]/, `Tickets[${idx}]`);
                $(this).find('label').attr('for', prop);
                $(this).find('input').each(function () {
                    if ($(this).attr('name') == '__Invariant') {
                        $(this).val(prop);
                    } else {
                        $(this).attr('id', prop);
                        $(this).attr('name', prop);
                    }
                });
                $(this).find('span').attr('data-valmsg-for', prop);
            });
            $(this).find('.remove-ticket-btn').attr('data-index', idx);
        });
        // Update session storage
        bookingData.Tickets.splice(index, 1);
        window.sessionStorage.setItem(KEY_SS_BOOKING_DATA, bookingData);
    });

    $('form').on('submit', function () {
        debugger;
        window.sessionStorage.removeItem(KEY_SS_BOOKING_DATA);
    });
})