// Helper: Convert .NET DayOfWeek (0=Sunday) to JS index (0=Sunday
const days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

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
            { data: 'price' },
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
            `<dd><a href="/flights/book/${data.id}" role="button" class="btn btn-primary btn-sm">Book flight</a> <a href="/flights/unschedule/${data.id}" role="button" class="btn btn-danger btn-sm">Unschedule flight</a></dd>` +
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
    fromSelect.value = "";
    fromSelect.text = "";
    fromSelect.index = "";
    fromSelect.change = updateTable;
    toSelect.value = "";
    toSelect.text = "";
    toSelect.index = "";
    toSelect.change = updateTable;
});