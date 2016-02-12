function btoab(base64) {
    var binary_string = window.atob(base64);
    var len = binary_string.length;
    var bytes = new Uint8Array(len);
    for (var i = 0; i < len; i++) {
        bytes[i] = binary_string.charCodeAt(i);
    }
    return bytes.buffer;

}
function thx(bytes) {
    return Array.from(bytes, function (byte) {
        return ('00' + (byte & 0xFF).toString(16)).slice(-2);
    }).join('').toUpperCase();
}

$(function () {
    var url = "your_url";
    $.get(url, function (data) {
        var bdoc = $('<div id="body-mock">' + data.replace(/^[\s\S]*<body.*?>|<\/body>[\s\S]*$/ig, '') + '</div>');
        var imageBase64 = $.trim(bdoc.find("#your_div_containing_base64_text_generated_by_this_app_representing_image").text());
        var buffer = btoab(imageBase64);
        var intReader = new Int32Array(buffer);
        var imgWidth = intReader[0];
        var imgHeight = intReader[1];
        var bitmapArray = new Uint8Array(buffer, 8, buffer.byteLength - 8);
        $(".left-topcon").click(function () {
            $("#leftpic").hide();
            $("#rightpic").hide();
            var imgDiv = $("#pcList");
            imgDiv.removeAttr("style");
            imgDiv.removeAttr("videoList-container");
            imgDiv.attr("id", "hahaha");
            imgDiv.empty();
            imgDiv.css('font-family', 'Consolas, monospace, sans-serif');
            imgDiv.css('font-size', 'xx-small');
            for (var y = 0, num = 0; y < imgHeight; y++) {
                var columnDiv = $('<div/>');
                for (var x = 0; x < imgWidth; x++, num += 3) {
                    var color = '#' + thx(bitmapArray.subarray(num, num + 3));
                    $('<span/>').css("color", color).text("#").appendTo(columnDiv);
                }
                columnDiv.appendTo(imgDiv);
            }
        });
    });
});


