$(function () {
    function getFilteredFlights(json) {
        return json.flights.filter(flight => !flight.deleted);
    }

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
        ajax: {
            //url: function () {
            //    let from = Number(fromSelect.value);
            //    let to = Number(toSelect.value);
            //    if (from > 0 && to > 0)
            //        return `/flights/getall?from=${from}&to=${to}`;
            //    return '/flights/getall';
            //},
            url: "/flights/getall",
            dataSrc: getFilteredFlights
        },
        processing: true,
        columnDefs: [
            { className: 'align-middle', targets: '_all' }
        ],
        columns: [
            { data: 'id' },
            { data: 'price' }
        ]
    });
    table.processing(false);

    function updateTable() {

        let from = Number(fromSelect.value);
        let to = Number(toSelect.value);
        if (from > 0 && to > 0 && from != to) {
            table.ajax.reload();
        }
    }
    fromSelect.change = updateTable;
    toSelect.change = updateTable;
});