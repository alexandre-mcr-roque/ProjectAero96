$(function () {
    function getFilteredCities(json) {
        const showDisabled = $('#show-disabled').is(':checked');
        // If checked, show all cities; otherwise, hide deleted cities
        return json.cities.filter(function (city) {
            return showDisabled || !city.deleted;
        });
    }

    var table = $('#table-cities').DataTable({
        layout: {
            topStart: [
                'pageLength',
                {
                    div: {
                        className: 'dataTable-feature',
                        id: 'create-city-container',
                        html: '<a href="/admin/cities/create" class="btn btn-sm btn-primary" role="button">Create new City</a>'
                    }
                }
            ],
            topEnd: {
                search: {
                    text: 'Search city: _INPUT_'
                }
            },
            top2End: {
                div: {
                    className: 'dataTable-feature',
                    id: 'show-disabled-container',
                    html: '<label for="show-disabled" class="form-check-label me-2">Show disabled cities</label>'
                        + '<input type="checkbox" id="show-disabled" class="form-check-input">'
                }
            }
        },
        order: [[1, 'asc']],
        ajax: {
            url: '/admin/cities/getall',
            dataSrc: getFilteredCities
        },
        processing: true,
        columnDefs: [
            { className: 'align-middle', targets: '_all' }
        ],
        columns: [
            { data: 'name' },
            { data: 'country' },
            {
                orderable: false,
                searchable: false,
                width: '8em',
                data: 'id',
                render: function (id, _, city) {
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
        ],
        rowCallback: function (row, city) {
            if (city.deleted) {
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
    $('#table-cities').on('click', 'button[id="disable-modal-toggle"]', function () {
        let id = $(this).attr('disable-target');
        $('#disable-cityid').val(id);
        $('#disable-modal').modal('show');
    });
    $('#confirm-disable').on('click', function () {
        let id = $('#disable-cityid').val();
        // Get the anti-forgery token value from the hidden input
        let token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: `/admin/cities/disable/${id}`,
            type: 'POST',
            headers: {
                'RequestVerificationToken': token // Add the token to the request headers
            },
            success: function () {
                table.ajax.reload();
            },
            error: function (xhr) {
                alert(`Error disabling city: ${xhr.responseText}`);
            },
            complete: function () {
                $('#disable-modal').modal('hide');
            },
        });
    });
    // Restore modal
    $('#table-cities').on('click', 'button[id="restore-modal-toggle"]', function () {
        let id = $(this).attr('restore-target');
        $('#restore-cityid').val(id);
        $('#restore-modal').modal('show');
    });
    $('#confirm-restore').on('click', function () {
        let id = $('#restore-cityid').val();
        // Get the anti-forgery token value from the hidden input
        let token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: `/admin/cities/restore/${id}`,
            type: 'POST',
            headers: {
                'RequestVerificationToken': token // Add the token to the request headers
            },
            success: function () {
                table.ajax.reload();
            },
            error: function (xhr) {
                alert(`Error restoring city: ${xhr.responseText}`);
            },
            complete: function () {
                $('#restore-modal').modal('hide');
            },
        });
    });
});