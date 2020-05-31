var selectedKb=null;
var $jq = jQuery.noConflict();
$('input').iCheck({
    checkboxClass: 'icheckbox_flat',
    radioClass: 'iradio_flat'
});

$(document).ready(function () {
    $("#newCountry").prop("disabled", true);
    $("#newSkill").prop("disabled", true);
    $("#newTool").prop("disabled", true);
    $("#newLanguage").prop("disabled", true);
    $("#newTitle").prop("disabled", true); 
});



function getbyID(kbID) {
    $('#Name').css('border-color', 'lightgrey');
    $('#Description').css('border-color', 'lightgrey');
    $.ajax({
        url: "/KnowledgeBases/GetbyID/" + kbID,
        typr: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            console.log(result);
            var data = JSON. parse(result);
            $('#kbId').val(data.Id);
            $('#Name').val(data.Name);
            $('#Description').val(data.Description);
            $('#myModal').modal('show');
            $('#btnUpdate').show();
            $('#btnAdd').hide();
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
    return false;
}
        function Addkb() {
                var res = validate();
                if (res == false) {
                    return false;
                }
                var KbObj = {
                    Name: $('#Name').val(),
                    Description: $('#Description').val(),
                };
                $.ajax({
                    url: "/KnowledgeBases/Add",
                    data: JSON.stringify(KbObj),
                    type: "POST",
                    contentType: "application/json;charset=utf-8",
                    dataType: "json",
                    success: function (result) {
                        $("#kbDiv").load("/KnowledgeBases/listKb");
                        $('#myModal').modal('hide');
                    },
                    error: function (errormessage) {
                        alert(errormessage.responseText);
                    }
                });
            }

        //Valdidation using jquery

        function validate() {
            var isValid = true;
            if ($('#Name').val().trim() == "") {
                $('#Name').css('border-color', 'Red');
                isValid = false;
            }
            else {
                $('#Name').css('border-color', 'lightgrey');
            }
            return isValid;
        }
        function Update() {
            var res = validate();
            if (res == false) {
                return false;
            }
            var kbObj = {
                Id: $('#kbId').val(),
                Name: $('#Name').val(),
                Description: $('#Description').val(),
            };
            $.ajax({
                url: "/KnowledgeBases/Update",
                data: JSON.stringify(kbObj),
                type: "POST",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                success: function (result) {
                    $("#kbDiv").load("/KnowledgeBases/listKb");
                    $('#myModal').modal('hide');
                    $('#kbId').val("");
                    $('#Name').val("");
                    $('#Description').val("");
                },
                error: function (errormessage) {
                    alert(errormessage.responseText);
                }
            });

        }
        //function for deleting employee's record

        function Delele(ID) {
            var ans = confirm("T'es sure de supprimer cette base de connaissance ?");
            if (ans) {
                $.ajax({
                    url: "/KnowledgeBases/Delete/" + ID,
                    type: "POST",
                    contentType: "application/json;charset=UTF-8",
                    dataType: "json",
                    success: function (result) {
                        $("#kbDiv").load("/KnowledgeBases/listKb");
                    },
                    error: function (errormessage) {
                        alert(errormessage.responseText);
                    }
                });
            }
        }
        $("#myBtn").click(function () {
            $('#myModal').modal('show');
            $('#btnUpdate').hide();
            $('#btnAdd').show();
        });
        $('.iradio_flat').on('ifChecked', function (event) {
            //console.log("Test");
            //console.log(event);
            //console.log(event.target.id);
            //console.log(event.target.value);
            selectedKb = event.target.value;
            $("#newCountry").prop("disabled", false);
            $("#newSkill").prop("disabled", false);
            $("#newTool").prop("disabled", false);
            $("#newLanguage").prop("disabled", false);
            $("#newTitle").prop("disabled", false);
            var kbId = event.target.value;
            console.log(kbId);
            $.ajax({
                url: "/KnowledgeBases/GetbyID/" + kbId,
                typr: "GET",
                contentType: "application/json;charset=UTF-8",
                dataType: "json",
                success: function (result) {
                    var data = JSON. parse(result);
                    console.log(data);
                    if (data.Countries.length == 0)
                        $('#countries').html('<p id="tt">Aucun pays pour cette base de connaissance</p>');
                    else{
                        $('#countries').html('');
                        $.each(data.Countries, function (index, value) {
                        

                            $('#countries').append(
                                    '<button type="button" class="btn btn-default btn-sm Filebutton">' +
                                    value.Text +
                                    '   <span class="glyphicon glyphicon-remove-sign" onclick=removeSpan(this)></span>' +
                                    ' </button>'
                                );
                      
                       //alert(index + ": " + value);
                        });
                    }


                    if (data.Languages.length == 0)
                        $('#languages').html('<p id="ttl">Aucun langue pour cette base de connaissance</p>');
                    else
                    {
                        $('#languages').html('');
                        $.each(data.Languages, function (index, value) {
                            $('#languages').append(
                                    '<button type="button" class="btn btn-default btn-sm Filebutton">' +
                                    value.Text +
                                    '   <span class="glyphicon glyphicon-remove-sign" onclick=removeSpan(this)></span>' +
                                    ' </button>'
                                );
                      
                       //alert(index + ": " + value);
                        });
                    }


                   if (data.Skills.length == 0)
                        $('#skills').html('<p id="tts">Aucune compétence pour cette base de connaissance</p>');
                    else
                    {
                        $('#skills').html('');
                        $.each(data.Skills, function (index, value) {
                            $('#skills').append(
                                    '<button type="button" class="btn btn-default btn-sm Filebutton">' +
                                    value.Text +
                                    '   <span class="glyphicon glyphicon-remove-sign" onclick=removeSpan(this)></span>' +
                                    ' </button>'
                                );
                      
                       //alert(index + ": " + value);
                        });
                    }


                   if (data.Tools.length == 0)
                        $('#tools').html('<p id="ttTool">Aucun outils pour cette base de connaissance</p>');
                    else
                    {
                        $('#tools').html('');
                        $.each(data.Tools, function (index, value) {
                            $('#tools').append(
                                    '<button type="button" class="btn btn-default btn-sm Filebutton">' +
                                    value.Text +
                                    '   <span class="glyphicon glyphicon-remove-sign" onclick=removeSpan(this)></span>' +
                                    ' </button>'
                                );
                      
                       //alert(index + ": " + value);
                        });
                    }


                    if (data.Titles.length == 0)
                        $('#titles').html('<p id="ttt">Aucun titre pour cette base de connaissance</p>');
                    else
                    {
                        $('#titles').html('');
                        $.each(data.Titles, function (index, value) {
                            $('#titles').append(
                                    '<button type="button" class="btn btn-default btn-sm Filebutton">' +
                                    value.Text +
                                    '   <span class="glyphicon glyphicon-remove-sign" onclick=removeSpan(this)></span>' +
                                    ' </button>'
                                );
                      
                       //alert(index + ": " + value);
                        });
                    }


                   








                },
                error: function (errormessage) {
                    console.log(errormessage.responseText);
                }

            });


        });

        function AddCountry() {
            if ($('#newCountry').val().length>0) {
                var val = $('#newCountry').val();
                console.log(val);


                $('#tt').remove();
                $('#countries').append(
                                   '<button type="button" class="btn btn-default btn-sm Filebutton">' +
                                   val +
                                   '   <span class="glyphicon glyphicon-remove-sign" onclick=removeSpan(this)></span>' +
                                   ' </button>'
                               );
                $('#newCountry').val('');
            }
        }
        function AddLanguage() {
            if ($('#newLanguage').val().length>0) {
                var val = $('#newLanguage').val();
                console.log(val);


                $('#ttl').remove();
                $('#languages').append(
                                   '<button type="button" class="btn btn-default btn-sm Filebutton">' +
                                   val +
                                   '   <span class="glyphicon glyphicon-remove-sign" onclick=removeSpan(this)></span>' +
                                   ' </button>'
                               );
                $('#newLanguage').val('');
            }
        }
       

        function AddSkill() {
            if ($('#newSkill').val().length>0) {
                var val = $('#newSkill').val();
                console.log(val);


                $('#tts').remove();
                $('#skills').append(
                                   '<button type="button" class="btn btn-default btn-sm Filebutton">' +
                                   val +
                                   '   <span class="glyphicon glyphicon-remove-sign" onclick=removeSpan(this)></span>' +
                                   ' </button>'
                               );
                $('#newSkill').val('');
            }
        }



        function AddTool() {
            if ($('#newTool').val().length>0) {
                var val = $('#newTool').val();
                console.log(val);


                $('#ttTool').remove();
                $('#tools').append(
                                   '<button type="button" class="btn btn-default btn-sm Filebutton">' +
                                   val +
                                   '   <span class="glyphicon glyphicon-remove-sign" onclick=removeSpan(this)></span>' +
                                   ' </button>'
                               );
                $('#newTool').val('');
            }
        }



        function AddTitle() {
            if ($('#newTitle').val().length>0) {
                var val = $('#newTitle').val();
                console.log(val);


                $('#ttt').remove();
                $('#titles').append(
                                   '<button type="button" class="btn btn-default btn-sm Filebutton">' +
                                   val +
                                   '   <span class="glyphicon glyphicon-remove-sign" onclick=removeSpan(this)></span>' +
                                   ' </button>'
                               );
                $('#newTitle').val('');
            }
        }

        function removeSpan(e) {
            e.parentElement.remove();             
        }




        function saveCountries() {
            kbButtons = document.querySelectorAll('#countries button');
            var countries = [];
            $.each(kbButtons, function (index, value) {
                //console.log(value.innerText);
                countries.push(value.innerText);
            });
            var Data = JSON.stringify({
                'Content': countries,
                'KbId': selectedKb
            });
            //console.log(selectedKb);
            if(selectedKb==null){
                iziToast.error({
                        title: 'Ouch',
                        message: 'Veuillez sélectionner une base de connaissance pour modifier ses donnés !',
                        icon: 'fa fa-times'
                    });
            }
            else{
            //console.log(tools.length);
            if(countries.length==0)
                {
                    iziToast.error({
                        title: 'Ouch',
                        message: 'Veuillez ajouter des pays avant de sauvegarder !',
                        icon: 'fa fa-times'
                    });
                }
                else{
            $.ajax({
                url: "/KnowledgeBases/SaveCountries",
                data: Data,
                type: "POST",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                success: function (result) {
                    console.log("saved successfully");
                    iziToast.success({
                        title: 'Hola',
                        message: 'Pays sauvegardés avec succés !',
                        icon: 'fa fa-check-square',
                    });
                },
                error: function (errormessage) {
                     iziToast.error({
                        title: 'Ouch',
                        message: 'On a rencontré une erreur , veuillez réessayer dans quelques minutes !',
                        icon: 'fa fa-times'
                    });
                }
            });
            }}
        }





        function saveLanguages() {
            kbButtons = document.querySelectorAll('#languages button');
            var languages = [];
            $.each(kbButtons, function (index, value) {
                //console.log(value.innerText);
                languages.push(value.innerText);
            });
            var Data = JSON.stringify({
                'Content': languages,
                'KbId': selectedKb
            });
            //console.log(selectedKb);
            if(selectedKb==null){
                iziToast.error({
                        title: 'Ouch',
                        message: 'Veuillez sélectionner une base de connaissance pour modifier ses donnés !',
                        icon: 'fa fa-times'
                    });
            }
            else{
            //console.log(tools.length);
            if(languages.length==0)
                {
                    iziToast.error({
                        title: 'Ouch',
                        message: 'Veuillez ajouter des langues avant de sauvegarder !',
                        icon: 'fa fa-times'
                    });
                }
                else{
            $.ajax({
                url: "/KnowledgeBases/SaveLanguages",
                data: Data,
                type: "POST",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                success: function (result) {
                    console.log("saved successfully");
                    iziToast.success({
                        title: 'Hola',
                        message: 'Langues sauvegardés avec succés !',
                        icon: 'fa fa-check-square',
                    });
                },
                error: function (errormessage) {
                    iziToast.error({
                        title: 'Ouch',
                        message: 'On a rencontré une erreur , veuillez réessayer dans quelques minutes !',
                        icon: 'fa fa-times'
                    });
                }
            });
            }
        }
        }




        function saveSkills() {
            kbButtons = document.querySelectorAll('#skills button');
            var skills = [];
            $.each(kbButtons, function (index, value) {
                //console.log(value.innerText);
                skills.push(value.innerText);
            });
            var Data = JSON.stringify({
                'Content': skills,
                'KbId': selectedKb
            });
            //console.log(selectedKb);
            if(selectedKb==null){
                iziToast.error({
                        title: 'Ouch',
                        message: 'Veuillez sélectionner une base de connaissance pour modifier ses donnés !',
                        icon: 'fa fa-times'
                    });
            }
            else{
            //console.log(tools.length);
            if(skills.length==0)
                {
                    iziToast.error({
                        title: 'Ouch',
                        message: 'Veuillez ajouter des compétences avant de sauvegarder !',
                        icon: 'fa fa-times'
                    });
                }
                else{
            $.ajax({
                url: "/KnowledgeBases/SaveSkills",
                data: Data,
                type: "POST",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                success: function (result) {
                    console.log("saved successfully");
                    iziToast.success({
                        title: 'Hola',
                        message: 'Compétences sauvegardés avec succés !',
                        icon: 'fa fa-check-square',
                    });
                },
                error: function (errormessage) {
                     iziToast.error({
                        title: 'Ouch',
                        message: 'On a rencontré une erreur , veuillez réessayer dans quelques minutes !',
                        icon: 'fa fa-times'
                    });
                }
            });
            }
        }

        }



        function saveTools() {
            kbButtons = document.querySelectorAll('#tools button');
            var tools = [];
            $.each(kbButtons, function (index, value) {
                //console.log(value.innerText);
                tools.push(value.innerText);
            });
            var Data = JSON.stringify({
                'Content': tools,
                'KbId': selectedKb
            });
           // console.log(selectedKb);
            if(selectedKb==null){
                iziToast.error({
                        title: 'Ouch',
                        message: 'Veuillez sélectionner une base de connaissance pour modifier ses donnés !',
                        icon: 'fa fa-times'
                    });
            }
            else{
            //console.log(tools.length);
            if(tools.length==0)
                {
                    iziToast.error({
                        title: 'Ouch',
                        message: 'Veuillez ajouter des outils avant de sauvegarder !',
                        icon: 'fa fa-times'
                    });
                }
                else{
            $.ajax({
                url: "/KnowledgeBases/SaveTools",
                data: Data,
                type: "POST",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                success: function (result) {
                    console.log("saved successfully");
                    iziToast.success({
                        title: 'Hola',
                        message: 'Outils sauvegardés avec succés !',
                        icon: 'fa fa-check-square',
                    });
                },
                error: function (errormessage) {
                     iziToast.error({
                        title: 'Ouch',
                        message: 'On a rencontré une erreur , veuillez réessayer dans quelques minutes !',
                        icon: 'fa fa-times'
                    });
                }
            });
        }
        }

        }





        function saveTitles() {
            kbButtons = document.querySelectorAll('#titles button');
            var titles = [];
            $.each(kbButtons, function (index, value) {
                //console.log(value.innerText);
                titles.push(value.innerText);
            });
            console.log(selectedKb);
            if(selectedKb==null){
                iziToast.error({
                        title: 'Ouch',
                        message: 'Veuillez sélectionner une base de connaissance pour modifier ses donnés !',
                        icon: 'fa fa-times'
                    });
            }
            else{

            if(titles.length==0)
                {
                    iziToast.error({
                        title: 'Ouch',
                        message: 'Veuillez ajouter des titres avant de sauvegarder !',
                        icon: 'fa fa-times'
                    });
                }
                else{
            var Data = JSON.stringify({
                'Content': titles,
                'KbId': selectedKb
            });
            
            $.ajax({
                url: "/KnowledgeBases/SaveTitles",
                data: Data,
                type: "POST",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                success: function (result) {
                    console.log("saved successfully");
                   iziToast.success({
                        title: 'Hola',
                        message: 'Titres sauvegardés avec succés !',
                        icon: 'fa fa-check-square',
                    });
                   
                },
                error: function (errormessage) {
                    //alert(errormessage.responseText);
                    iziToast.error({
                        title: 'Ouch',
                        message: 'On a rencontré une erreur , veuillez réessayer dans quelques minutes !',
                        icon: 'fa fa-times'
                    });
                }
            });
           }
        }

        }