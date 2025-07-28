$(function () {
    let fromSelect = $('#select-from-city')[0].ej2_instances[0];
    let toSelect = $('#select-to-city')[0].ej2_instances[0];
    let table = $('#table-flights').DataTable({
        layout: {
            topStart: null,
            topEnd: null,
            bottomEnd: [
                'paging',
                'pageLength'
            ]
        },
        //order: [[1, 'asc']],
        ajax: {
            url: '/flights/getall',
            dataSrc: 'flights'
        },
        processing: true,
        columnDefs: [
            { className: 'align-middle', targets: '_all' }
        ],
        columns: [
            { data: 'departureCity.name' },
            { data: 'arrivalCity.name' },
            { data: (data) => data.departureDate.split('T', 1)[0].replace(/-/g, '/'), className: 'dt-left' },
            { data: 'price', type: 'num-fmt', className: 'dt-left' },
            {
                className: 'dt-control',
                orderable: false,
                data: null,
                defaultContent: ''
            }
        ]
    });
    table.processing(false);

    // Formatting function for row details
    function format(data) {
        debugger;
        return (
            '<dl>' +
                `<dd>Departure Date: ${new Date(data.departureDate).toLocaleString()}</dd>` +
                `<dd>Flight Duration: ${data.flightDuration}</dd>` +
                `<dd><a href="/flights/book/${data.id}" role="button" class="btn btn-primary btn-sm">Book flight</a> <button id="unschedule-modal-toggle" unschedule-target="${data.id}" type="button" class="btn btn-danger btn-sm">Unschedule flight</a></dd>` +
            '</dl>'
        );
    }
    // Add event listener for opening and closing details
    table.on('click', 'td.dt-control', function (e) {
        let tr = e.target.closest('tr');
        let row = table.row(tr);

        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
        }
        else {
            // Open this row
            row.child(format(row.data())).show();
        }
    });

    function updateTable() {
        let from = Number(fromSelect.value);
        let to = Number(toSelect.value);
        if ((from > 0 || to > 0) && from != to) {
            table.ajax.url(`/flights/getall?from=${from}&to=${to}`);
        } else {
            table.ajax.url('/flights/getall');
        }
        table.ajax.reload();
    }
    // Clear city filters
    fromSelect.value = "";
    fromSelect.text = "";
    fromSelect.index = "";
    fromSelect.change = updateTable;
    toSelect.value = "";
    toSelect.text = "";
    toSelect.index = "";
    toSelect.change = updateTable;

    // Unschedule modal
    $('#table-flights').on('click', 'button[id="unschedule-modal-toggle"]', function () {
        let id = $(this).attr('unschedule-target');
        $('#unschedule-flightid').val(id);
        $('#unschedule-modal').modal('show');
    });
    $('#confirm-unschedule').on('click', function () {
        let id = $('#unschedule-flightid').val();
        // Get the anti-forgery token value from the hidden input
        let token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: `/flights/unschedule/${id}`,
            type: 'POST',
            headers: {
                'RequestVerificationToken': token // Add the token to the request headers
            },
            success: function () {
                table.ajax.reload();
            },
            error: function (xhr) {
                alert(`Error unscheduling flight: ${xhr.responseText}`);
            },
            complete: function () {
                $('#unschedule-modal').modal('hide');
            },
        });
    });
});