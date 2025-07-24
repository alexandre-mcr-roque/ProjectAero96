var flights = [];

function renderCalendar() {
    // TODO redo this function to use the new flight date structure
    //// Prepare a 7-day structure
    //let calendar = { };
    //days.forEach(day => calendar[day] = []);

    //// Fill calendar with flights
    //flights.forEach(f => {
    //    const dayName = days[f.dayOfWeek];
    //    calendar[dayName].push(f);
    //});

    //// Build HTML table
    //let html = '<table class="table table-bordered"><thead><tr>';
    //days.forEach(day => html += `<th>${day}</th>`);
    //html += '</tr></thead><tbody><tr>';

    //days.forEach(day => {
    //    html += '<td>';
    //    if (calendar[day].length === 0) {
    //        html += '<span class="text-muted">No flights</span>';
    //    } else {
    //    calendar[day].forEach(f => {
    //        html += `<div class="mb-2"><strong>Dep: </strong>${f.departureTime}<br/><strong> Duration: </strong>${f.flightDuration}</div>`;
    //        });
    //    }
    //    html += '</td>';
    //});

    //html += '</tr></tbody></table>';
    //$('#weekly-calendar').html(html);
}

function setDepartureTimes() {
    var $list = $('#departureDatesList');
    $list.empty();
    flights.forEach(flight => {
        // Create new list item
        let dateObj = new Date(flight.departureDate);
        let $li = $(`
                <li class="list-group-item d-flex justify-content-between align-items-center">
                    <div>
                        <div><strong>Departure:</strong> ${dateObj.toUTCString()}</div>
                        <div><strong>Duration:</strong> ${flight.flightDuration}</div>
                    </div>
                </li>
            `);

        $list.append($li);
    });
}

function fetchFlights(airplaneId) {
    if (!airplaneId || airplaneId == '0') {
        $('#weekly-calendar').html('');
        return;
    }
    $.get(`/airplanes/${airplaneId}/flights`, function (data) {
        flights = data.flights;
        renderCalendar();
        setDepartureTimes();
    });
}
// --- Price auto-update script ---
// Cache for airplane price per hour
let airplanePricePerHour = null;
// Fetch price per hour for the selected airplane
function fetchPricePerHour(airplaneId) {
    if (!airplaneId || airplaneId == '0') {
        airplanePricePerHour = null;
        return;
    }
    $.get(`/airplanes/${airplaneId}/price-per-hour`, function (data) {
        airplanePricePerHour = parseFloat(data);
        if (airplanePricePerHour > 0) updatePriceIfValid();
    });
}

function toggleButton(airplaneId) {
    $('button[step="2"]').toggleClass('disabled', !airplaneId || airplaneId == '0')
}

// Update the Price field if flight duration is valid and price per hour is known
function updatePriceIfValid() {
    const $hours = $('#Hours');
    const $minutes = $('#Minutes');
    const $form = $('form');
    if (airplanePricePerHour === null) return;

    // Use jQuery validation to check if the field is valid
    if ($form.validate().element($hours)
     && $form.validate().element($minutes)) {
        let hours = (parseInt($('#Hours').val()) | 0) + (parseInt($('#Minutes').val()) | 0) / 60;
        let price = (hours * airplanePricePerHour).toFixed(2);
        $('#Price').val(price);
    }
}

function flightDurationString(hours, minutes) {
    if (hours <= 0) return `${minutes} minute${minutes == 1 ? '' : 's'}`;
    if (minutes <= 0) return `${hours} hour${hours == 1 ? '' : 's'}`;
    return `${hours} hour${hours == 1 ? '' : 's'} and ${minutes} minute${minutes == 1 ? '' : 's'}`
}

function addDepartureDate() {
    // Get flight duration from input fields
    let hours = parseInt($('#Hours').val(), 10) || 0;
    let minutes = parseInt($('#Minutes').val(), 10) || 0;
    if (hours <= 0 && minutes <= 0) {
        alert('Please enter a valid flight duration first.');
        return;
    }

    let departureDate = $('#newDepartureDate').val();

    if (!departureDate) {
        alert('Please enter a departure date.');
        return;
    }
    if (!departureDate.charAt(departureDate.length - 1) != 'Z') departureDate = departureDate + 'Z';
    let dateObj = new Date(departureDate);

    let minDate = new Date(); // Now
    minDate.setDate(minDate.getDate() + 30); // Add 30 days
    if (dateObj < minDate) {
        alert('Cannot schedule a flight in under 30 days.');
        return;
    }

    // Overlap verification
    let durationMs = (hours * 60 + minutes) * 60 * 1000;
    let newStart = dateObj.getTime();
    let newEnd = newStart + durationMs;
    let hasOverlap = false;
    flights.forEach(flight => {
        let existingStart = new Date(flight.departureDate).getTime();
        let existingEnd = new Date(flight.arrivalDate).getTime();
        // Overlap if intervals intersect
        if (newStart <= existingEnd && existingStart <= newEnd) {
            hasOverlap = true;
            return false; // break loop
        }
    });
    if (hasOverlap) {
        alert('Date and duration overlapped with existing flight.');
        return;
    }
    $('#departureDatesList input[type="hidden"][name^="DepartureDates"]').each(function () {
        let existingStart = new Date($(this).val()).getTime();
        let existingEnd = existingStart + durationMs; // Use current duration for all
        // Overlap if intervals intersect
        if (newStart <= existingEnd && existingStart <= newEnd) {
            hasOverlap = true;
            return false; // break loop
        }
    });
    if (hasOverlap) {
        alert('Date and duration overlapped with existing flight.');
        return;
    }

    var index = $('#departureDatesList li[data-index]').length;
    var $list = $('#departureDatesList');
    // Create new list item
    let $li = $(`
                <li class="list-group-item d-flex justify-content-between align-items-center" data-index="${index}">
                    <div>
                        <div><strong>Departure:</strong> ${dateObj.toUTCString()}</div>
                        <div><strong>Duration:</strong> ${flightDurationString(hours,minutes)}</div>
                    </div>
                    <button type="button" class="btn btn-danger btn-sm remove-date-btn" data-index="${index}">
                        <i class="bi bi-x"></i> Remove
                    </button>
                    <input type="hidden" name="DepartureDates[${index}]" value="${departureDate}" />
                </li>
            `);

    $list.append($li);

    // Clear input
    //$('#newDepartureDate').val('');
}

$(function () {
    // Initial load if value is set
    const initialId = $('#AirplaneId').val();
    fetchFlights(initialId);
    fetchPricePerHour(initialId);
    toggleButton(initialId);

    // On airplane select change fetch flights and price per hour
    $('#AirplaneId').on('change', function () {
        let id = $(this).val();
        fetchFlights(id);
        fetchPricePerHour(id);
        toggleButton(id);
    });

    $('#Hours, #Minutes').on('change blur', updatePriceIfValid);
    // Clear all departure dates if duration changes
    $('#Hours, #Minutes').on('change', function () {
        setDepartureTimes();
    });

    $(document).on('click', '#addDepartureDateBtn', addDepartureDate);

    // Delegate remove button clicks
    $(document).on('click', '.remove-date-btn', function () {
        $(this).closest('li').remove();

        // Re-index hidden inputs
        $('#departureDatesList li[data-index]').each(function (idx) {
            $(this).attr('data-index', idx);
            $(this).find('input').each(function () {
                if (this.name.includes('DepartureDate'))
                    this.name = `DepartureDates[${idx}]`;
            });
            $(this).find('.remove-date-btn').attr('data-index', idx);
        });
    });
});