$('input').iCheck({
    checkboxClass: 'icheckbox_flat',
    radioClass: 'iradio_flat'
});
var cvFile = {};

$(document).ready(function () {
    $('input').iCheck('uncheck');
    cvFile = {};
});
function removeFile() {
    console.log(cvFile);
    if (jQuery.isEmptyObject(cvFile))
        iziToast.error({
            title: 'Ouch',
            message: 'Veuillez sélectionner un profil pour supprimer !',
            icon: 'fa fa-times'
        });
    else {
        $.get("removeFile/" + cvFile.Id,
            function (data) {
                iziToast.success({
                    title: 'Hola',
                    message: 'Profil supprimé avec succés !',
                    icon: 'fa fa-check-square'
                });
                $("#displayFiles").empty();
                $("#displayFiles").load("/Home/DisplayFiles");
            });
    }
    
}
function Details() {
    if (jQuery.isEmptyObject(cvFile))
        $('.modal-details').html('<div class="alert alert-danger">Veuillez sélectionner un profil pour afficher ses détails !</div>');
    else {
        $.get("Details/" + cvFile.Id,
            function (data) { $('.modal-details').html(data); }
        );     
    }
}
function validData() {
    //console.log(cvFile);
    //console.log(jQuery.isEmptyObject(cvFile));
    if (jQuery.isEmptyObject(cvFile))
        $('.modal-valid').html('<div class="alert alert-danger">Veuillez sélectionner un profil pour valider ses données !</div>  <button type="button" class="btn btn-default" data-dismiss="modal" style="Text-align:center;">Fermer</button>');
    else {
        $.get("Valid/" + cvFile.Id,
            function (data) {
                $('.modal-valid').html(' <div class="row"> <div class="col-md-6" id="cv"><br /> <iframe style="width:700px; height:600px;" frameborder="0" id="frimaValid"></iframe> </div> <div class="col-md-6" id="form"></div> </div>');

                $('#form').html(data);
                var val = cvFile.name.replace("'", "");
                $('#frimaValid').attr('src', "/UploadedFiles/" + val);
            }
        );
    }
}
function setSource() {
    if (jQuery.isEmptyObject(cvFile))
        iziToast.error({
            title: 'Ouch',
            message: 'Veuillez sélectionner un profil pour afficher !',
            icon: 'fa fa-times'
        });
    else {
        var val = cvFile.name.replace("'", "");
        $('#frima').attr('src', "/UploadedFiles/" + val);
    }
}
function Download() {
    if (jQuery.isEmptyObject(cvFile))
        iziToast.error({
            title: 'Ouch',
            message: 'Veuillez sélectionner un profil pour télécharger son CV !',
            icon: 'fa fa-times'
        });
    else {
        var myStr = cvFile.name;
        var s = myStr.indexOf(".");
        var subStr = myStr.substring(0, s);
        window.location = '/Home/Download/' + subStr;
    }
}

function PostValid() {

    var profile = {
        FullName: $('#Name').val(),
        PhoneNumber: $('#Phone').val(),
        Email: $('#Email').val(),
        Nationality: $('#Nationality').val(),
        Id: $('#Id').val()
    };
    $.ajax({
        url: "/Home/Valid",
        data: JSON.stringify(profile),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            console.log(result);
            $("#displayFiles").empty();
            $("#displayFiles").load("/Home/DisplayFiles");
            iziToast.success({
                title: 'Hola',
                message: 'Données validé avec succés !',
                icon: 'fa fa-check-square'
            });
        },
        error: function (errormessage) {
            console.log(errormessage.responseText);
        }
    });
}

$('.iradio_flat').on('ifChecked', function (event) {
   // console.log(event);
    cvFile.name = event.target.id;
    cvFile.Id = event.target.value;
});

