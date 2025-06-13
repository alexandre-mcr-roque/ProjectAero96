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
                render: function (data) {
                    return '<div class="text-center">'
                            + `<a class="btn btn-sm btn-primary m-1" href="/admin/users/edit/${data}" role="button"><i class="fa-solid fa-user-pen"></i></a>`
                            + `<button class="btn btn-sm btn-danger m-1" id="disable-modal-toggle" delete-target="${data}"><i class="fa-solid fa-user-xmark"></i></button>`
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

    $('#show-disabled').on('change', function () {
        table.ajax.reload();
    });

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
    $('#table-users').on('click', 'button[id="disable-modal-toggle"]', function () {
        const userId = $(this).attr('delete-target');
        $('#disable-userid').val(userId);
        $('#disable-modal').modal('show');
    });
    $('#confirm-disable').on('click', function () {
        const userId = $('#disable-userid').val();
        // Get the anti-forgery token value from the hidden input
        const token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: `/admin/users/disable/${userId}`,
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
});