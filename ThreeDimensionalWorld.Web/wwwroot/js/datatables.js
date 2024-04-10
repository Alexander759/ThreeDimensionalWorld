$(document).ready(function () {
    $('table').DataTable({
        "autoWidth": false,
        responsive: true,
        language: {
            "emptyTable": "Няма информация в таблицата",
            "lengthMenu": "Показва _MENU_ записа",
            "info": "Показва от _START_ до _END_ от общо _TOTAL_ записа",
            "infoEmpty": "Показва от 0 до 0 от общо 0 записа",
            "search": "Търсене:",
            "paginate": {
                "first": "Първа",
                "last": "Последна",
                "next": "Следваща",
                "previous": "Предишна"
            },
        }
    });
});