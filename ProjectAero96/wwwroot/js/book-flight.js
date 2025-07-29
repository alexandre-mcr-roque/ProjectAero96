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
                        <button type="button" class="form-control text-start" data-bs-target="#seat-map-modal" ticket-index="${index}">${data.SeatNumber || 'None'}</button>
                        <input type="hidden" id="Tickets[${index}].SeatNumber" name="Tickets[${index}].SeatNumber" value="${data.SeatNumber}">
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
                        <button type="button" class="form-control text-start" data-bs-target="#seat-map-modal" ticket-index="${index}">None</button>
                        <input type="hidden" id="Tickets[${index}].SeatNumber" name="Tickets[${index}].SeatNumber" value="">
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
            $(`button[data-bs-target="#seat-map-modal"][ticket-index="${idx}"]`).text(ticket.SeatNumber || 'None');
        }
        else addTicket(ticket, idx);
    });
}

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

// Helper: Convert zero-indexed seat index to code (e.g., col=2, row=4 => "C5")
function seatCode(col, row) {
    return String.fromCharCode(65 + col) + (row + 1);
}
// Helper to get the seat code
function getInputSeat(idx) {
    let inputName = $.escapeSelector(`Tickets[${idx}].SeatNumber`);
    return $('#' + inputName).val();
}
// Helper to set the seat code
function setInputSeat(idx, value, bookedSeats) {
    let inputName = $.escapeSelector(`Tickets[${idx}].SeatNumber`);
    $('#' + inputName).val(value).trigger('change');
    bookedSeats[idx] = value;
    $('#selected-seat-span').text(value || 'None');
    $(`button[data-bs-target="#seat-map-modal"][ticket-index="${idx}"]`).text(value || 'None');
}

function renderSeats(flightId, bookedSeats, cfg) {
    // Fetch occupied seats
    $.get(`/flights/${flightId}/occupied-seats`, function (occupiedSeats) {
        const rows = cfg.seatRows;
        const cols = cfg.seatColumns;
        const windowSeats = cfg.windowSeats;
        const rightWindowSeats = cols - windowSeats;

        // Get selected idx
        let selectedIdx = $('#ticket-idx-modal').val();

        // Clear inputs whose value is now unavailable
        $('input[id$="SeatNumber"]').each(function () {
            let $input = $(this);
            let seat = $input.val();
            if (occupiedSeats.includes(seat)) {
                let idx = $input.closest('li').attr('data-index');
                setInputSeat(idx, '', bookedSeats);
            }
        });

        // Build seat map
        let $map = $('<div id="seat-map"></div>');
        for (let r = 0; r < rows; r++) {
            let $row = $('<div class="seat-row"></div>');
            let $center = $('<div class="seats-center"></div>');
            let $left = $('<div class="seats-left"></div>');
            let $right = $('<div class="seats-right"></div>');
            for (let c = 0; c < cols; c++) {
                let code = seatCode(c, r);
                let $seat = $('<div class="seat"></div>');
                $seat.attr('data-seat', code);

                if (bookedSeats.includes(code)) {
                    $seat.addClass('occupied');
                    if (selectedIdx && code === getInputSeat(selectedIdx)) {
                        $seat.addClass('selected');
                    }
                } else if (occupiedSeats.includes(code)) {
                    $seat.addClass('unavailable');
                } else {
                    $seat.addClass('available');
                }

                if (cols === 1) {
                    $center.append($seat);
                }
                else {
                    if (c < windowSeats) $left.append($seat);
                    else if (c >= rightWindowSeats) $right.append($seat);
                    else $center.append($seat);
                }
                if (cols > 1) {
                    $row.append($left)
                        .append($center)
                        .append($right);
                }
                else $row.append($center);
            }
            $map.append($row);
        }
        $('#seat-map').replaceWith($map);

        // Attach click handlers after rendering
        $('#seat-map .seat.available, #seat-map .seat.selected').off('click')
            .on('click', function () {
                let $seat = $(this);
                if ($seat.hasClass('selected')) {
                    // Unselect seat for this idx
                    $seat.removeClass('selected occupied').addClass('available');
                    setInputSeat(selectedIdx, '', bookedSeats);
                    return true;
                }
                // Unselect previous seat for this idx
                $('#seat-map .seat.selected').removeClass('selected occupied').addClass('available');

                // Mark this seat as occupied and selected
                $seat.removeClass('available').addClass('occupied selected');

                // Update the input
                let seatCodeVal = $seat.data('seat');
                setInputSeat(selectedIdx, seatCodeVal, bookedSeats);
            });
    });
}

function openModal(element, flightId, bookedSeats, cfg) {
    let idx = $(element).attr('ticket-index');
    $('#ticket-idx-modal').val(idx);

    let inputName = $.escapeSelector(`Tickets[${idx}].SeatNumber`);
    $('#selected-seat-span').text($('#' + inputName).val() || 'None');

    renderSeats(flightId, bookedSeats, cfg);
    $('#seat-map-modal').modal('show');
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

    let flightId = $('#FlightId').val();
    var bookedSeats = bookingData.Tickets.map(t => t.SeatNumber);
    $.get(`/flights/${flightId}/seat-config/`, function (config) {
        $(document).on('click', 'button[data-bs-target="#seat-map-modal"]', function () { openModal(this, flightId, bookedSeats, config) });
    });

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
                $(this).find('button[data-bs-target="#seat-map-modal"]').attr('ticket-index', idx);
                $(this).find('span').attr('data-valmsg-for', prop);
            });
            $(this).find('.remove-ticket-btn').attr('data-index', idx);
        });
        // Update session storage
        bookingData.Tickets.splice(index, 1);
        window.sessionStorage.setItem(KEY_SS_BOOKING_DATA, JSON.stringify(bookingData));
        // Update booked seats
        bookedSeats.splice(index, 1);
    });

    $('form').on('submit', function () {
        window.sessionStorage.removeItem(KEY_SS_BOOKING_DATA);
    });
})