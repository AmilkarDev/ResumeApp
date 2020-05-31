$('input').iCheck({
    checkboxClass: 'icheckbox_flat'
    ,
    radioClass: 'iradio_flat'
});

//$(document).ready(function () {


    var inp = $('.icheckbox_flat').each(function () {
        $(this).removeClass('checked');
    });

    $('.mybutton').each(function () {
        var ss = "'" + $.trim(this.innerText) + "'";
        var final = "#usersCheck[name=" + ss + "]";
        var inp = $(final)[0];
        if (typeof inp !== 'undefined') {
            // console.log(inp.parentElement);
            var pr = (inp.parentElement);
            $(pr).addClass('checked');
            // console.log(pr);
        }
    });
    $('.myybutton').each(function () {
        var ss = "'" + $.trim(this.innerText) + "'";
        var final = "#ccsusersCheck[name=" + ss + "]";
        var inp = $(final)[0];
        if (typeof inp !== 'undefined') {
            //console.log(inp.parentElement);
            var pr = inp.parentElement;
            $(pr).addClass('checked');
            //console.log(pr);
        }
    });


//});


$('.iradio_flat').on('ifChecked', function (event) {
    var col = $('input[name=cvType]:checked').val();
    console.log(col);
    if (col == "cvtoColleague") {
        $("#subject").text("To Colleague");
        CKEDITOR.instances.editora.setData("To Colleague");
    }
    else {
        if (col == "cvtoClient") {
            $("#subject").text("To Client");
            CKEDITOR.instances.editora.setData("Go to Client");
        }
        else {
            $("#subject").text("Traitement des CVS");
            CKEDITOR.instances.editora.setData("To Manual");
        }

    }
});

$('.icheckbox_flat').on('ifChecked', function (event) {
    console.log(event.target.id);
    switch (event.target.id) {

        case 'usersCheck':
            // $('#principalRecipients').append('<span class="label label-default">  ' + event.target.name + '</span > ')
            var email1 = event.target.name;
            var str1 = ":contains('" + email1 + "')";
            // console.log($('#principalRecipients button').filter(str).length);
            if ($('#principalRecipients button').filter(str1).length === 0) {
                $('#principalRecipients').append(
                    '<button type="button" class="btn btn-default btn-sm mybutton">' +
                    event.target.name +
                    '   <span class="glyphicon glyphicon-remove-sign" onclick=remove(this)></span>' +
                    ' </button>'
                );
            }

            break;
        case 'ccsusersCheck':
            // $('#ccRecipients').append('<span class="label label-default">' + event.target.name + '</span > ')
            var email = event.target.name;
            var str = ":contains('" + email + "')";
            // console.log($('#principalRecipients button').filter(str).length);
            if ($('#ccRecipients button').filter(str).length === 0) {
                $('#ccRecipients').append(
                    '<button type="button" class="btn btn-default btn-sm myybutton">' +
                    event.target.name +
                    '   <span class="glyphicon glyphicon-remove-sign" onclick=removee(this)></span>' +
                    ' </button>'
                );
            }

            break;


        //case 'fileCheck':
        //    // $('#ccRecipients').append('<span class="label label-default">' + event.target.name + '</span > ')
        //    var fileName = event.target.name;
        //    var str = ":contains('" + fileName + "')";
        //    console.log($('#attachments button').filter(str).length);
        //    if ($('#attachments button').filter(str).length == 0) {
        //        $('#attachments').append(
        //            '<button type="button" class="btn btn-default btn-sm Filebutton">' +
        //            event.target.name +
        //            '   <span class="glyphicon glyphicon-remove-sign" onclick=removeFile(this)></span>' +
        //            ' </button>'
        //        )
        //    }
        //    break;
    }
});

$('.icheckbox_flat').on('ifUnchecked', function (event) {

    //console.log(event.target);
    var email = event.target.name;
    var str = ":contains('" + email + "')";
    console.log(str);
    switch (event.target.id) {

        case 'usersCheck':
            $('#principalRecipients button').filter(str).remove();
            break;
        case 'ccsusersCheck':
            $('#ccRecipients button').filter(str).remove();
            break;
        case 'fileCheck':
            $('#attachments button').filter(str).remove();
            break;

    }
});
function remove(e) {
    var ss = "'" + $.trim(e.parentElement.innerText) + "'";
    var final = "#usersCheck[name=" + ss + "]";
    var inp = $(final)[0];
    if (typeof inp !== 'undefined') {
        $(inp.parentElement).removeClass('checked');
    }
    e.parentElement.remove();
}
function removee(e) {
    var ss = "'" + $.trim(e.parentElement.innerText) + "'";
    var final = "#ccsusersCheck[name=" + ss + "]";
    var inp = $(final)[0];
    if (typeof inp !== 'undefined') {
        $(inp.parentElement).removeClass('checked');
    }
    e.parentElement.remove();
}
function removeFile(e) {
    var ss = "'" + $.trim(e.parentElement.innerText) + "'";
    var final = "#fileCheck[name=" + ss + "]";
    var inp = $(final)[0];
    if (typeof inp !== 'undefined') {
        $(inp.parentElement).removeClass('checked');
    }
    e.parentElement.remove();
}

//function flyNow() {
//    console.log();
//    $('input').iCheck({
//        checkboxClass: 'icheckbox_flat',
//        // radioClass: 'iradio_flat'
//    });





//    var inp = $('.icheckbox_flat').each(function () {
//        $(this).removeClass('checked');
//    });

//    $('.mybutton').each(function () {
//        var ss = "'" + $.trim(this.innerText) + "'";
//        var final = "#usersCheck[name=" + ss + "]";
//        var inp = $(final)[0];
//        if (typeof inp !== 'undefined') {
//            // console.log(inp.parentElement);
//            var pr = (inp.parentElement);
//            $(pr).addClass('checked');
//            // console.log(pr);
//        }
//    });
//    $('.myybutton').each(function () {
//        var ss = "'" + $.trim(this.innerText) + "'";
//        var final = "#ccsusersCheck[name=" + ss + "]";
//        var inp = $(final)[0];
//        if (typeof inp !== 'undefined') {
//            //console.log(inp.parentElement);
//            var pr = inp.parentElement;
//            $(pr).addClass('checked');
//            //console.log(pr);
//        }
//    });

//    $('.Filebutton').each(function () {
//        var ss = "'" + $.trim(this.innerText) + "'";
//        var final = "#fileCheck[name=" + ss + "]";
//        var inp = $(final)[0];
//        if (typeof inp != 'undefined') {
//            //console.log(inp.parentElement);
//            var pr = (inp.parentElement);
//            $(pr).addClass('checked');
//            //console.log(pr);
//        }
//    });
//}