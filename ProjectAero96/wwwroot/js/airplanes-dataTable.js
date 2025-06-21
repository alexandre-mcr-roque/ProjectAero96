$(function () {
    function getFilteredAirplanes(json) {
        const showDisabled = $('#show-disabled').is(':checked');
        // If checked, show all airplanes; otherwise, hide deleted airplanes
        return json.airplanes.filter(function (airplane) {
            return showDisabled || !airplane.deleted;
        });
    }

    var table = $('#table-airplanes').DataTable({
        layout: {
            topStart: [
                'pageLength',
                {
                    div: {
                        className: 'dataTable-feature',
                        id: 'create-airplane-container',
                        html: '<a href="/admin/airplanes/create" class="btn btn-sm btn-primary" role="button">Create new Airplane</a>'
                    }
                }
            ],
            topEnd: {
                search: {
                    text: 'Search airplane: _INPUT_'
                }
            },
            top2End: {
                div: {
                    className: 'dataTable-feature',
                    id: 'show-disabled-container',
                    html: '<label for="show-disabled" class="form-check-label me-2">Show disabled airplanes</label>'
                        + '<input type="checkbox" id="show-disabled" class="form-check-input">'
                }
            }
        },
        order: [[1, 'asc']],
        ajax: {
            url: '/admin/airplanes/getall',
            dataSrc: getFilteredAirplanes
        },
        processing: true,
        columnDefs: [
            { className: 'align-middle', targets: '_all' }
        ],
        columns: [
            {
                orderable: false,
                searchable: false,
                width: '8em',   
                data: 'airline',
                render: function (airline, _, airplane) {
                    return `<img src="${airplane.airlineImagePath}" alt="${airline}" class="table-image">`;
                }
            },
            { data: 'descriptionStr' },
            {
                orderable: false,
                searchable: false,
                width: '8em',
                data: 'id',
                render: function (id, _, airplane) {
                    if (airplane.deleted) {
                        return '<div class="text-center">'
                                + `<a class="btn btn-sm btn-primary m-1" href="/admin/airplanes/edit/${id}" role="button"><i class="fa-solid fa-pen"></i></a>`
                                + `<button class="btn btn-sm btn-success m-1" id="restore-modal-toggle" restore-target="${id}"><i class="fa-solid fa-plus"></i></button>`
                             + '</div>';
                    }
                    return '<div class="text-center">'
                            + `<a class="btn btn-sm btn-primary m-1" href="/admin/airplanes/edit/${id}" role="button"><i class="fa-solid fa-pen"></i></a>`
                            + `<button class="btn btn-sm btn-danger m-1" id="disable-modal-toggle" disable-target="${id}"><i class="fa-solid fa-xmark"></i></button>`
                         + '</div>';
                }
            }
        ],
        rowCallback: function (row, airplane) {
            if (airplane.deleted) {
                $(row).addClass('row-disabled');
            }
        }
    });
    table.processing(false);
    // Show disabled cities checkbox
    $('#show-disabled').on('change', function () {
        table.ajax.reload();
    });
    // Disable modal
    $('#table-airplanes').on('click', 'button[id="disable-modal-toggle"]', function () {
        let id = $(this).attr('disable-target');
        $('#disable-airplaneid').val(id);
        $('#disable-modal').modal('show');
    });
    $('#confirm-disable').on('click', function () {
        let id = $('#disable-airplaneid').val();
        // Get the anti-forgery token value from the hidden input
        let token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: `/admin/airplanes/disable/${id}`,
            type: 'POST',
            headers: {
                'RequestVerificationToken': token // Add the token to the request headers
            },
            success: function () {
                table.ajax.reload();
            },
            error: function (xhr) {
                alert(`Error disabling airplane: ${xhr.responseText}`);
            },
            complete: function () {
                $('#disable-modal').modal('hide');
            },
        });
    });
    // Restore modal
    $('#table-airplanes').on('click', 'button[id="restore-modal-toggle"]', function () {
        let id = $(this).attr('restore-target');
        $('#restore-airplaneid').val(id);
        $('#restore-modal').modal('show');
    });
    $('#confirm-restore').on('click', function () {
        let id = $('#restore-airplaneid').val();
        // Get the anti-forgery token value from the hidden input
        let token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: `/admin/airplanes/restore/${id}`,
            type: 'POST',
            headers: {
                'RequestVerificationToken': token // Add the token to the request headers
            },
            success: function () {
                table.ajax.reload();
            },
            error: function (xhr) {
                alert(`Error restoring airplane: ${xhr.responseText}`);
            },
            complete: function () {
                $('#restore-modal').modal('hide');
            },
        });
    });
});