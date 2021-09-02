(function () {

    $('form#StatusForm').find('#ParkedStatus').on('change', function (e) {
        e.preventDefault();
        let parked = $("#ParkedStatus").val();
        $('#parkedStatus').val(parked);
        $('form#StatusForm').submit();
    })

    $('form#StatusForm').find('#UnParkedStatus').on('change', function (e) {
        e.preventDefault();
        let unparked = $("#UnParkedStatus").val();
        $('#parkedStatus').val(unparked);
        $('form#StatusForm').submit();
    });

    $('form#StatusForm').find('#AllStatus').on('change', function (e) {
        e.preventDefault;
        let all = $("#AllStatus").val();
        $('#parkedStatus').val(all);
        $('form#StatusForm').submit();
    })
})();