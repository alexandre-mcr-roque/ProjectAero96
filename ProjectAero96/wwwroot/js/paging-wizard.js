$(function () {
    let currStep = '1';

    $('.paging-wz').each(function () {
        $(this).on('click', function () {
            let step = $(this).attr('step');
            if (step === currStep) return;

            let curr = $(`#step${currStep}`);
            let next = $(`#step${step}`);
            currStep = step;

            // Fade out current step
            curr.css('opacity', 0);
            setTimeout(() => {
                curr.addClass('d-none');
                // Fade in next step
                // Reset opacity to 0 in case of first time load
                next.css('opacity', 0)
                    .removeClass('d-none');
                setTimeout(() => next.css('opacity', 1), 10);
            }, 400); // Match the CSS transition duration
        });
    });
});