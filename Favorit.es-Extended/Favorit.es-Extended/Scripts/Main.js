//$(document).ready(function () {
//    //a tags have an event associated with them, so we'll represent that with a parameter called event
//    $('body').on('click', '.photo a', function (event) {
//        //when we click on a <a> under an element with the class of photo, do this...

//        //we want to prevent the default behaviour of the <a> tag
//        event.preventDefault();

//        var theATag = $(this);
//        //for theATag object, we want the value of the attribute (attr) href
//        var theATagsHREF = theATag.attr('href');

//        //make the AJAX request
//        //$.get(url, function(data) {...})
//        $.get(theATagsHREF, function (data) {
//            if (data == "OK") {
//                //everything worked great, expected response was "OK"
//                //add the class "heart" to the div
//                theATag.children('div').addClass('heart')
//            }
//        });
//    });

//    $('body').on('click', '.favorite a', function (event) {
//        event.preventDefault();
//        var theATag = $(this);
//        var theATagsHREF = theATag.attr('href');
        
//        $.get(theATagsHREF, function (data) {
//            if (data == "OK") {
//                theATag.children('div').removeClass('heart')
//            }
//        });
//    });
//});

$(document).ready(function () {
    $('body').on('click', '.photo a, .favorite a', function (event) {
        //do an AJAX GET request to the href. based on the response, either add or remove the heart from the child div
        event.preventDefault();
        var theATag = $(this);
        var theATagsHREF = theATag.attr('href');
        $.get(theATagsHREF, function (data) {
            //theATag.children('div').toggleClass('heart')
            
            if (data == "favorited") {
                theATag.children('div').addClass('heart')
            }
            else if (data == "unfavorited") {
                theATag.children('div').removeClass('heart')
            }
        });
    });
});