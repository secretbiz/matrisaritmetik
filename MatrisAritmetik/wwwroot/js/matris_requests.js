import { updateMathContent } from './mathjax_custom.js';

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

function updateTable(token) {
    $('#matris_table').load('/Matris?handler=UpdateMatrisTable', { __RequestVerificationToken: token }, function () { updateMathContent();  })
}

const matris_add_button = document.getElementById("matris_add_button");
if (matris_add_button) { matris_add_button.addEventListener("click", addMatrix, false); }
