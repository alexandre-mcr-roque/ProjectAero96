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
        order: [[1, 'asc']],
        ajax: {
            url: function () {
                let from = Number(fromSelect.value);
                let to = Number(toSelect.value);
                if (from > 0 && to > 0)
                    return `/admin/flights/getall?from=${from}&to=${to}`;
                return '/admin/flights/getall';
            },
            dataSrc: getFilteredFlights
        },
        processing: true,
        columnDefs: [
            { className: 'align-middle', targets: '_all' }
        ],
        columns: [
            { data: 'id' },
            { data: 'price' },
            {
                orderable: false,
                searchable: false,
                width: '8em',
                data: 'id',
                render: function (id, _, flight) {
                    if (city.deleted) {
                        return '<div class="text-center">'
                            + `<a class="btn btn-sm btn-primary m-1" href="/admin/cities/edit/${id}" role="button"><i class="fa-solid fa-pen"></i></a>`
                            + `<button class="btn btn-sm btn-success m-1" id="restore-modal-toggle" restore-target="${id}"><i class="fa-solid fa-plus"></i></button>`
                            + '</div>';
                    }
                    return '<div class="text-center">'
                        + `<a class="btn btn-sm btn-primary m-1" href="/admin/cities/edit/${id}" role="button"><i class="fa-solid fa-pen"></i></a>`
                        + `<button class="btn btn-sm btn-danger m-1" id="disable-modal-toggle" disable-target="${id}"><i class="fa-solid fa-xmark"></i></button>`
                        + '</div>';
                }
            }
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