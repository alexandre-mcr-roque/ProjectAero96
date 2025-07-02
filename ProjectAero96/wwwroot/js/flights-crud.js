// --- Flight calendar script ---
// Helper: Convert .NET DayOfWeek (0=Sunday) to JS index (0=Sunday)
const days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

function renderCalendar(flights) {
    // Prepare a 7-day structure
    let calendar = { };
    days.forEach(day => calendar[day] = []);

    // Fill calendar with flights
    flights.forEach(f => {
        const dayName = days[f.dayOfWeek];
        calendar[dayName].push(f);
    });

    // Build HTML table
    let html = '<table class="table table-bordered"><thead><tr>';
    days.forEach(day => html += `<th>${day}</th>`);
    html += '</tr></thead><tbody><tr>';

    days.forEach(day => {
        html += '<td>';
        if (calendar[day].length === 0) {
            html += '<span class="text-muted">No flights</span>';
        } else {
        calendar[day].forEach(f => {
            html += `<div class="mb-2"><strong>Dep: </strong>${f.departureTime}<br/><strong> Duration: </strong>${f.flightDuration}</div>`;
            });
        }
        html += '</td>';
    });

    html += '</tr></tbody></table>';
    $('#weekly-calendar').html(html);
}

function fetchFlights(airplaneId) {
    if (!airplaneId || airplaneId == '0') {
        $('#weekly-calendar').html('');
        return;
    }
    $.get(`/airplanes/${airplaneId}/flights`, function (data) {
        renderCalendar(data.flights);
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
    // Example endpoint: /airplanes/getpriceperhour/{id}
    // The endpoint should return a decimal value (e.g., 120.00)
    $.get(`/airplanes/${airplaneId}/price-per-hour`, function (data) {
        airplanePricePerHour = parseFloat(data);
        if (airplanePricePerHour > 0) updatePriceIfValid();
    });
}

// Parse a TimeSpan string (hh:mm or hh:mm:ss) to hours as decimal
function parseTimeSpanToHours(timespan) {
    if (!timespan) return 0;
    // Split on '.' to separate days if present
    let days = 0, timePart = timespan;
    if (timespan.includes('.')) {
        const parts = timespan.split('.');
        days = parseInt(parts[0], 10) || 0;
        timePart = parts[1];
    }
    const timeParts = timePart.split(':');
    if (timeParts.length < 2) return 0;
    const hours = parseInt(timeParts[0], 10) || 0;
    const minutes = parseInt(timeParts[1], 10) || 0;
    // Total hours = days*24 + hours + minutes/60
    return days * 24 + hours + (minutes / 60);
}

// Update the Price field if FlightDuration is valid and price per hour is known
function updatePriceIfValid() {
    const $duration = $('#FlightDuration');
    const $form = $duration.closest('form');
    if (!$duration.length || airplanePricePerHour === null) return;

    // Use jQuery validation to check if the field is valid
    if ($form.length && $form.validate().element($duration)) {
        const hours = parseTimeSpanToHours($duration.val());
        if (hours > 0) {
            const price = (hours * airplanePricePerHour).toFixed(2);
            $('#Price').val(price);
        }
    }
}

$(function () {
    // Initial load if value is set
    const initialId = $('#AirplaneId').val();
    if (initialId) {
        fetchFlights(initialId);
        fetchPricePerHour(initialId);
    }

    // On airplane select change fetch flights and price per hour
    $('#AirplaneId').on('change', function () {
        fetchFlights($(this).val());
        fetchPricePerHour($(this).val());
    });

    // On FlightDuration change, update price if valid
    $('#FlightDuration').on('change blur', function () {
        updatePriceIfValid();
    });
});