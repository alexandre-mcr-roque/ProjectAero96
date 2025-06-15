$(function () {
    function getFilteredUsers(json) {
        const showDisabled = $('#show-disabled').is(':checked');
        // If checked, show all users; otherwise, hide deleted users
        return json.users.filter(function (user) {
            return showDisabled || !user.deleted;
        });
    }

    var table = $('#table-users').DataTable({
        layout: {
            topStart: [
                'pageLength',
                {
                    div: {
                        className: 'dataTable-feature',
                        id: 'create-user-container',
                        html: '<a href="/admin/users/create" class="btn btn-sm btn-primary" role="button">Create new User</a>'
                    }
                }
            ],
            topEnd: [
                {
                    div: {
                        className: 'dataTable-feature',
                        id: 'role-selectbox'
                    }
                },
                {
                    search: {
                        text: 'Search user: _INPUT_'
                    }
                }
            ],
            top2End: {
                div: {
                    className: 'dataTable-feature',
                    id: 'show-disabled-container',
                    html: '<label for="show-disabled" class="form-check-label me-2">Show disabled users</label>'
                        + '<input type="checkbox" id="show-disabled" class="form-check-input">'
                }
            }
        },
        ajax: {
            url: '/admin/users/getall',
            dataSrc: getFilteredUsers
        },
        processing: true,
        columnDefs: [
            { className: 'align-middle', targets: '_all' }
        ],
        columns: [
            { data: 'fullName' },
            { data: 'email' },
            { data: 'roles', orderable: false },
            {
                orderable: false,
                searchable: false,
                width: '8em',
                data: 'id',
                render: function (uid, _, user) {
                    if (user.deleted) {
                        return '<div class="text-center">'
                                + `<a class="btn btn-sm btn-primary m-1" href="/admin/users/edit/${uid}" role="button"><i class="fa-solid fa-user-pen"></i></a>`
                                + `<button class="btn btn-sm btn-success m-1" id="restore-modal-toggle" restore-target="${uid}"><i class="fa-solid fa-user-plus"></i></button>`
                             + '</div>';
                    }
                    return '<div class="text-center">'
                            + `<a class="btn btn-sm btn-primary m-1" href="/admin/users/edit/${uid}" role="button"><i class="fa-solid fa-user-pen"></i></a>`
                            + `<button class="btn btn-sm btn-danger m-1" id="disable-modal-toggle" disable-target="${uid}"><i class="fa-solid fa-user-xmark"></i></button>`
                         + '</div>';
                }
            }
        ],
        rowCallback: function (row, data) {
            if (data.deleted) {
                $(row).addClass('row-disabled');
            }
        }
    });
    table.processing(false);
    // Show disabled users checkbox
    $('#show-disabled').on('change', function () {
        table.ajax.reload();
    });
    // Role select box
    $('#role-selectbox')
        .append('<label for="role-select">Filter by role:</label>')
        .append($('<select></select>', {
            'id': 'role-select',
            'class': 'form-select form-select-sm',
            html: '<option value="" selected>None</option>'
                + '<option value="Client">Client</option>'
                + '<option value="Employee">Employee</option>'
                + '<option value="Admin">Admin</option>',
            on: {
                change: function () {
                    $('#table-users').DataTable().column(2)
                        .search($(this).val())
                        .draw();
                }
            }
        }));
    // Disable modal
    $('#table-users').on('click', 'button[id="disable-modal-toggle"]', function () {
        const uid = $(this).attr('disable-target');
        $('#disable-userid').val(uid);
        $('#disable-modal').modal('show');
    });
    $('#confirm-disable').on('click', function () {
        const uid = $('#disable-userid').val();
        // Get the anti-forgery token value from the hidden input
        const token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: `/admin/users/disable/${uid}`,
            type: 'POST',
            headers: {
                'RequestVerificationToken': token // Add the token to the request headers
            },
            success: function () {
                table.ajax.reload();
            },
            error: function (xhr) {
                alert(`Error disabling user: ${xhr.responseText}`);
            },
            complete: function () {
                $('#disable-modal').modal('hide');
            },
        });
    });
    // Restore modal
    $('#table-users').on('click', 'button[id="restore-modal-toggle"]', function () {
        const uid = $(this).attr('restore-target');
        $('#restore-userid').val(uid);
        $('#restore-modal').modal('show');
    });
    $('#confirm-restore').on('click', function () {
        const uid = $('#restore-userid').val();
        // Get the anti-forgery token value from the hidden input
        const token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: `/admin/users/restore/${uid}`,
            type: 'POST',
            headers: {
                'RequestVerificationToken': token // Add the token to the request headers
            },
            success: function () {
                table.ajax.reload();
            },
            error: function (xhr) {
                alert(`Error disabling user: ${xhr.responseText}`);
            },
            complete: function () {
                $('#restore-modal').modal('hide');
            },
        });
    });
});