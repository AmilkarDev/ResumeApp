$('input').iCheck({
    checkboxClass: 'icheckbox_flat',
    radioClass: 'iradio_flat'
});
var cvFile = {};

$(document).ready(function () {
    $('input').iCheck('uncheck');
    cvFile = {};
});
//function removeFile() {
//    console.log(cvFile);
//    if (jQuery.isEmptyObject(cvFile))
//        iziToast.error({
//            title: 'Ouch',
//            message: 'Veuillez sélectionner un profil pour supprimer !',
//            icon: 'fa fa-times'
//        });
//    else {
//        $.get("removeFile/" + cvFile.Id,
//            function (data) {
//                iziToast.success({
//                    title: 'Hola',
//                    message: 'Profil supprimé avec succés !',
//                    icon: 'fa fa-check-square'
//                });
//                $("#displayFiles").empty();
//                $("#displayFiles").load("/Home/DisplayFiles");
//            });
//    }

//}

//function PostValid() {

//    var profile = {
//        FullName: $('#Name').val(),
//        PhoneNumber: $('#Phone').val(),
//        Email: $('#Email').val(),
//        Nationality: $('#Nationality').val(),
//        Id: $('#Id').val()
//    };
//    $.ajax({
//        url: "/Home/Valid",
//        data: JSON.stringify(profile),
//        type: "POST",
//        contentType: "application/json;charset=utf-8",
//        dataType: "json",
//        success: function (result) {
//            console.log(result);
//            $("#displayFiles").empty();
//            $("#displayFiles").load("/Home/DisplayFiles");
//            iziToast.success({
//                title: 'Hola',
//                message: 'Données validé avec succés !',
//                icon: 'fa fa-check-square'
//            });
//        },
//        error: function (errormessage) {
//            console.log(errormessage.responseText);
//        }
//    });
//}

$('.iradio_flat').on('ifChecked', function (event) {
    // console.log(event);
    cvFile.name = event.target.id;
    cvFile.Id = event.target.value;
});

