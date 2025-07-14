
// Helper: Convert .NET DayOfWeek (0=Sunday) to JS index (0=Sunday)
const days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];// --- Flight calendar script ---

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
});