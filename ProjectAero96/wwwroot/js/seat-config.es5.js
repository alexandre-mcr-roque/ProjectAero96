'use strict';

function validateSeatConfig(rows, cols, maxSeats, windowSeats) {
    if (rows < 1 || cols < 1 || maxSeats < 1 || windowSeats < 1 || maxSeats > 1000 || rows > 1000 || cols > 12 || windowSeats > 4) {
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
        var centerSeats = cols - windowSeats * 2;
        if (centerSeats < 0) {
            return 'Number of window seats must be at most ' + (cols / 2 | 0) + '.';
        }
        if (centerSeats > 4) {
            return 'Number of window seats must be at least ' + ((cols - 3) / 2 | 0) + '.';
        }
    }
    return '';
}

function renderSeatMap() {
    var $windowSeats = $('#WindowSeats');

    var rows = parseInt($('#SeatRows').val()) || 0;
    var cols = parseInt($('#SeatColumns').val()) || 0;
    var maxSeats = parseInt($('#MaxSeats').val()) || 0;
    var windowSeats = parseInt($windowSeats.val()) || 0;

    if (cols === 1) {
        windowSeats = 1;
        $windowSeats.val(windowSeats);
        $windowSeats.prop('disabled', true);
    } else {
        var centerSeats = cols - windowSeats * 2;
        if (centerSeats < 0) {
            windowSeats = cols / 2 | 0;
        }
        if (centerSeats > 4) {
            windowSeats = (cols - 3) / 2 | 0;
        }
        $windowSeats.val(windowSeats);
        $windowSeats.prop('disabled', false);
    }

    var error = validateSeatConfig(rows, cols, maxSeats, windowSeats);
    $('#seat-config-error').text(error);
    var $map = $('#seat-map');
    $map.empty();

    if (error.length > 0 || rows < 1 || cols < 1) return;

    for (var r = 0; r < rows; r++) {
        var $row = $('<div class="seat-row"></div>');
        if (cols === 1) {
            // Single seat in the row, center it
            var $center = $('<div class="seats-center"></div>');
            $center.append('<div class="seat"></div>');
            $row.append($center);
        } else {
            // Window seats left
            var $left = $('<div class="seats-left"></div>');
            for (var i = 0; i < windowSeats; i++) {
                $left.append('<div class="seat"></div>');
            }
            $row.append($left);

            // Center seats
            var centerSeats = cols - windowSeats * 2;
            var $center = $('<div class="seats-center"></div>');
            for (var i = 0; i < centerSeats; i++) {
                $center.append('<div class="seat"></div>');
            }
            $row.append($center);

            // Window seats right
            var $right = $('<div class="seats-right"></div>');
            for (var i = 0; i < windowSeats; i++) {
                $right.append('<div class="seat"></div>');
            }
            $row.append($right);
        }
        $map.append($row);
    }
}

function updateSetDefaultSeatConfigButton() {
    var $maxSeats = $('#MaxSeats');
    var $form = $maxSeats.closest('form');
    var validator = $form.data('validator') || $form.data('unobtrusiveValidation');
    var isInvalid = false;

    if (validator) {
        // Validate only the MaxSeats field and check if it has errors
        validator.element($maxSeats);
        isInvalid = validator.invalid['MaxSeats'] === true;
    } else {
        // Fallback to aria-invalid if validator is not available
        isInvalid = $maxSeats.attr('aria-invalid') === 'true';
    }

    $('#open-seat-config').prop('disabled', isInvalid || $maxSeats.val() <= '0'); // double check if MaxSeats is less or equal to 0
}

$(function () {
    var $maxSeats = $('#MaxSeats');
    var $seatRows = $('#SeatRows');
    var $seatColumns = $('#SeatColumns');
    var $windowSeats = $('#WindowSeats');

    var $airplaneModelSelect = $('#AirplaneModelId');
    if ($airplaneModelSelect.length) {
        $airplaneModelSelect.on('change', function () {
            var id = $(this).val();
            if (id == '0') {
                $maxSeats.val(0);
                $seatRows.val(1);
                $seatColumns.val(1);
                $windowSeats.val(1);
                renderSeatMap();
                updateSetDefaultSeatConfigButton();
            } else {
                $.get('/admin/airplane-models/seat-config/' + id).done(function (data) {
                    $maxSeats.val(data.maxSeats);
                    $seatRows.val(data.seatRows);
                    $seatColumns.val(data.seatColumns);
                    $windowSeats.val(data.windowSeats);
                }).fail(function () {
                    $maxSeats.val(0);
                    $seatRows.val(1);
                    $seatColumns.val(1);
                    $windowSeats.val(1);
                }).always(function () {
                    renderSeatMap();
                    updateSetDefaultSeatConfigButton();
                });
            }
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
        var rows = parseInt($('#SeatRows').val()) || 0;
        var cols = parseInt($('#SeatColumns').val()) || 0;
        var maxSeats = parseInt($('#MaxSeats').val()) || 0;
        var windowSeats = parseInt($('#WindowSeats').val()) || 0;

        var errors = validateSeatConfig(rows, cols, maxSeats, windowSeats);
        if (errors.length > 0) {
            e.preventDefault();
            return false;
        }
    });
});

