$(function () {
    let adminCheck = $("#IsAdmin");
    let employeeCheck = $("#IsEmployee");
    let clientCheck = $("#IsClient");
    let errorSpan = $('span[data-valmsg-for="HasRole"]');
    let form = $('form');
    let modal = $('#modal');
    let openModal = $('#open-modal');
    let modalEmail = $('#modal-user-email');
    let submitForm = $('#submit-form');
    let submitted = false;
    function validateRoles() {
        if (!submitted) return true;
        let anyChecked = adminCheck.is(':checked') || employeeCheck.is(':checked') || clientCheck.is(':checked')
        if (!anyChecked) {
            errorSpan.text('Please select at least 1 role.');
            return false;
        } else {
            errorSpan.text('');
            return true;
        }
    }

    form.on('submit', function (e) {
        submitted = true;
        if (!validateRoles()) {
            e.preventDefault();
        }
    });

    adminCheck.on('change', validateRoles);
    employeeCheck.on('change', validateRoles);
    clientCheck.on('change', validateRoles);

    openModal.on('click', function () {
        debugger;
        if (modalEmail != null) modalEmail.text('Email: ' + $('#Email').val());
        modal.modal('show');
    });

    submitForm.on('click', function () {
        form.trigger('submit');
        modal.modal('hide');
    })
});