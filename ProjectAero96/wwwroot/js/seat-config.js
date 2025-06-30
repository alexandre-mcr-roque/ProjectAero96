function validateSeatConfig(rows, cols, maxSeats, windowSeats) {
    if (rows < 1 || cols < 1 || maxSeats < 1 || windowSeats < 1
        || maxSeats > 1000
        || rows > 1000
        || cols > 12
        || windowSeats > 4) {
        return ' ';
    }

    if (rows * cols > maxSeats) {
        return 'Number of seats exceed the maximum amount of seats.';
    }
    if (cols == 1) {
        if (windowSeats != 1) {
            return 'This configuration of window seats is invalid.';
        }
    } else {
        let centerSeats = cols - windowSeats * 2;
        if (centerSeats < 0) {
            return `Number of window seats must be at most ${cols / 2 | 0}.`;
        }
        if (centerSeats > 4) {
            return `Number of window seats must be at least ${(cols - 3) / 2 | 0}.`;
        }
    }
    return '';
}

function renderSeatMap() {
    let $windowSeats = $('#WindowSeats');

    let rows = parseInt($('#SeatRows').val()) || 0;
    let cols = parseInt($('#SeatColumns').val()) || 0;
    let maxSeats = parseInt($('#MaxSeats').val()) || 0;
    let windowSeats = parseInt($windowSeats.val()) || 0;

    if (cols === 1) {
        windowSeats = 1;
        $windowSeats.val(windowSeats);
        $windowSeats.prop('disabled', true);
    } else {
        let centerSeats = cols - windowSeats * 2;
        if (centerSeats < 0) {
            windowSeats = cols / 2 | 0;
        }
        if (centerSeats > 4) {
            windowSeats = (cols - 3) / 2 | 0;
        }
        $windowSeats.val(windowSeats);
        $windowSeats.prop('disabled', false);
    }

    let error = validateSeatConfig(rows, cols, maxSeats, windowSeats);
    $('#seat-config-error').text(error);
    let $map = $('#seat-map');
    $map.empty();

    if (error.length > 0 || rows < 1 || cols < 1) return;

    for (let r = 0; r < rows; r++) {
        let $row = $('<div class="seat-row"></div>');
        if (cols === 1) {
            // Single seat in the row, center it
            let $center = $('<div class="seats-center"></div>');
            $center.append('<div class="seat"></div>');
            $row.append($center);
        } else {
            // Window seats left
            let $left = $('<div class="seats-left"></div>');
            for (let i = 0; i < windowSeats; i++) {
                $left.append('<div class="seat"></div>');
            }
            $row.append($left);

            // Center seats
            let centerSeats = cols - windowSeats * 2;
            let $center = $('<div class="seats-center"></div>');
            for (let i = 0; i < centerSeats; i++) {
                $center.append('<div class="seat"></div>');
            }
            $row.append($center);

            // Window seats right
            let $right = $('<div class="seats-right"></div>');
            for (let i = 0; i < windowSeats; i++) {
                $right.append('<div class="seat"></div>');
            }
            $row.append($right);
        }
        $map.append($row);
    }
}

function updateSetDefaultSeatConfigButton() {
    let $maxSeats = $('#MaxSeats');
    let $form = $maxSeats.closest('form');
    let validator = $form.data('validator') || $form.data('unobtrusiveValidation');
    let isInvalid = false;

    if (validator) {
        // Validate only the MaxSeats field and check if it has errors
        validator.element($maxSeats);
        isInvalid = validator.invalid['MaxSeats'] === true;
    } else {
        // Fallback to aria-invalid if validator is not available
        isInvalid = $maxSeats.attr('aria-invalid') === 'true';
    }

    $('#open-seat-config').prop('disabled', isInvalid);
}

$(function () {
    let $maxSeats = $('#MaxSeats');
    let $seatRows = $('#SeatRows');
    let $seatColumns = $('#SeatColumns');
    let $windowSeats = $('#WindowSeats');

    let $airplaneModelSelect = $('#AirplaneModelId');
    if ($airplaneModelSelect.length) {
        $maxSeats.val(0); // Reset max seats on page load
        $airplaneModelSelect.on('change', function () {
            let id = $(this).val();
            $.get(`/admin/airplanemodels/seat-config/${id}`)
                .done(function (data) {
                    $maxSeats.val(data.maxSeats);
                    $seatRows.val(data.seatRows);
                    $seatColumns.val(data.seatColumns);
                    $windowSeats.val(data.windowSeats);
                })
                .fail(function () {
                    $maxSeats.val(0);
                    $seatRows.val(1);
                    $seatColumns.val(1);
                    $windowSeats.val(1);
                })
                .always(renderSeatMap);
        });
    }
    // Run on page load and whenever MaxSeats changes or is validated
    updateSetDefaultSeatConfigButton();
    $maxSeats.on('input change', updateSetDefaultSeatConfigButton);

    // Also update after validation (for unobtrusive validation)
    $maxSeats.on('blur', function () {
        setTimeout(updateSetDefaultSeatConfigButton, 10);
    });

    $('#SeatRows, #SeatColumns, #MaxSeats, #WindowSeats').on('input', renderSeatMap);
    // Initial render
    renderSeatMap();

    $('form').on('submit', function (e) {
        let rows = parseInt($('#SeatRows').val()) || 0;
        let cols = parseInt($('#SeatColumns').val()) || 0;
        let maxSeats = parseInt($('#MaxSeats').val()) || 0;
        let windowSeats = parseInt($('#WindowSeats').val()) || 0;

        let errors = validateSeatConfig(rows, cols, maxSeats, windowSeats);
        if (errors.length > 0) {
            e.preventDefault();
            return false;
        }
    });
});