// Helper: Convert .NET DayOfWeek (0=Sunday) to JS index (0=Sunday
const days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

$(function () {
    function getFilteredFlights(json) {
        const showDisabled = $('#show-disabled').is(':checked');
        // If checked, show all flights; otherwise, hide deleted flights
        return json.flights.filter(function (flight) {
            return showDisabled || !flight.deleted;
        });
    }

    let fromSelect = $('#select-from-city')[0].ej2_instances[0];
    let toSelect = $('#select-to-city')[0].ej2_instances[0];
    let table = $('#table-flights').DataTable({
        layout: {
            topStart: null,
            topEnd: {
                div: {
                    className: 'dataTable-feature',
                    id: 'show-disabled-container',
                    html: '<label for="show-disabled" class="form-check-label me-2">Show disabled flights</label>'
                        + '<input type="checkbox" id="show-disabled" class="form-check-input">'
                }
            },
            bottomEnd: [
                'paging',
                'pageLength'
            ]
        },
        //order: [[1, 'asc']],
        ajax: {
            url: '/flights/getall/include-deleted',
            dataSrc: getFilteredFlights
        },
        processing: true,
        columnDefs: [
            { className: 'align-middle', targets: '_all' }
        ],
        columns: [
            { data: 'departureCity.name' },
            { data: 'arrivalCity.name' },
            { data: 'price' },
            {
                className: 'dt-control',
                orderable: false,
                data: null,
                defaultContent: ''
            }
        ],
        rowCallback: function (row, flight) {
            if (flight.deleted) {
                $(row).addClass('row-disabled');
            }
        }
    });
    table.processing(false);

    // Formatting function for row details
    function format(data) {
        if (data.deleted) {
            return (
                '<dl>' +
                    `<dd>Departure Time: Every ${days[data.dayOfWeek]}, ${data.departureTime}</dd>` +
                    `<dd>Flight Duration: ${data.flightDuration}</dd>` +
                    `<dd><a href="/flights/edit/${data.id}" role="button" class="btn btn-primary btn-sm me-1">Edit flight</a>` +
                    `<a href="/flights/restore/${data.id}" role="button" class="btn btn-primary btn-sm">Restore flight</a></dd>` +
                '</dl>'
            );
        }
        return (
            '<dl>' +
                `<dd>Departure Time: ${data.departureTime}</dd>` +
                `<dd>Flight Duration: ${data.flightDuration}</dd>` +
                `<dd><a href="/flights/edit/${data.id}" role="button" class="btn btn-primary btn-sm me-1">Edit flight</a>` +
                `<a href="/flights/disable/${data.id}" role="button" class="btn btn-danger btn-sm">Disable flight</a></dd>` +
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
    fromSelect.value = null;
    fromSelect.change = updateTable;
    toSelect.value = null;
    toSelect.change = updateTable;
});