(function() {
    $(document).ready(function () {
        $.ajax({
            url: '/rest/account/external-login',
            success: function (result) {
                var html = [];
                result.forEach(function (oauth) {
                    html.push('<button type="submit" name="Provider" value="' + oauth.AuthenticationType + '">'
                        + oauth.Caption
                    + '</button>');
                });
                $('#oauth').html(html.join('<br/>'));
            }
        });
    })
})();
