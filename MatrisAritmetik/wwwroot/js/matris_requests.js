// internal modules
import { updateMathContent } from './mathjax_custom.js';

// matris ekleme fonksiyonu
function addMatrix(event) {
    var tkn = event.currentTarget.token;
    $.ajax(
        {
            type: 'POST',
            url: 'Matris?handler=AddMatrix',
            data:
            {
                __RequestVerificationToken: tkn,
                "name": document.getElementById("matris_name").value,
                "vals": document.getElementById("matris_vals").value
            },
            success: function (data) { updateTable(tkn); },
            error: function (error) { console.log(error); }
        });
}

// matris tablosu güncelleme
function updateTable(token) {
    $('#matris_table').load('/Matris?handler=UpdateMatrisTable', { __RequestVerificationToken: token }, function ()
    {
        updateMathContent();
        // Butonları bul & eventlistener ekle
        var buttons = document.getElementsByName("matris_table_delbutton");
        for (var i = 0; i < buttons.length; i++)
        {
            buttons[i].addEventListener("click", deleteMatrix, false);
            buttons[i].token = token;
        }
        
    });
}

// matris silme fonksiyonu
function deleteMatrix(event) {
    var tkn = event.currentTarget.token;
    $.ajax(
        {
            type: 'POST',
            url: 'Matris?handler=DeleteMatrix',
            data:
            {
                __RequestVerificationToken: tkn,
                "name": event.currentTarget.id,
            },
            success: function (data) { updateTable(tkn); },
            error: function (error) { console.log(error); }
        });
}

// matris ekleme butonu click event
const matris_add_button = document.getElementById("matris_add_button");
if (matris_add_button) { matris_add_button.addEventListener("click", addMatrix, false); }

// mouse hover -> ipucu gösterme
$(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

// komut gönder
function sendCmd(event) {
    var tkn = event.currentTarget.token;
    $.ajax(
        {
            type: 'POST',
            url: 'Matris?handler=SendCmd',
            data:
            {
                __RequestVerificationToken: tkn,
                "cmd": document.getElementById("matris_komut_satır").value,
            },
            success: function (data) { updateTable(tkn); },
            error: function (error) { console.log(error); }
        });
}

// komut gönderme butonu click event
const matris_komut_button = document.getElementById("matris_komut_button");
if (matris_komut_button) { matris_komut_button.addEventListener("click", sendCmd, false); }