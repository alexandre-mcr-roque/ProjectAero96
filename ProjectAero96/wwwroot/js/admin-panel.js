$(function () {
    let sidebar = $('#dockSidebar')[0].ej2_instances[0];
    $('#main-content container-fluid col-md-12').removeClass('e-content-animation');
    $('#toggle').on('click', function () {
        sidebar.toggle();
    });
    $('[class*="link-"]').each(function () {
        $(this).on('click', function () {
            let path = this.className.split(' ').find(s => s.startsWith('link')).split('-');
            let pathname = '';
            for (let i = 1; i < path.length; i++) { pathname += `/${path[i]}`; }
            if (window.location.pathname === pathname) return false;
            window.location.pathname = pathname;
        });
    });
});