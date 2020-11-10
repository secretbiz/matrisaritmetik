////// internal modules
import { updateMathContent } from './mathjax_custom.js';


//////// matris ekleme fonksiyonu
function addMatrix(event) {
    var tkn = event.currentTarget.token;
    // From text
    if (event.currentTarget.getAttribute("currentTab") == "text") {
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
    else if (event.currentTarget.getAttribute("currentTab") == "special") {
        $.ajax(
            {
                type: 'POST',
                url: 'Matris?handler=AddMatrixSpecial',
                data:
                {
                    __RequestVerificationToken: tkn,
                    "name": document.getElementById("matris_name").value,
                    "func": document.getElementById("matris_vals_special").value,
                    "args": document.getElementById("matris_special_args").value
                },
                success: function (_data) { updateTable(tkn); updateHistoryPanel(tkn); },
                error: function (error) { console.log(error); }
            });
    }
}

//////// matris tablosu güncelleme
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
} export { updateTable }


//////// matris silme fonksiyonu
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


//////// komut gönder
function sendCmd(event) {
    var tkn = event.currentTarget.token;
    var filteredcmd = document.getElementById("matris_komut_satır").value.split("=").join("!__EQ!").split("./").join("!__REVMUL!");
    $.ajax(
        {
            type: 'POST',
            url: 'Matris?handler=SendCmd',
            data:
            {
                __RequestVerificationToken: tkn,
                "cmd": filteredcmd,
            },
            success: function (data) { updateHistoryPanel(tkn); },
            error: function (error) { console.log(error); }
        });
}

// komut gönderme butonu click event
const matris_komut_button = document.getElementById("matris_komut_button");
if (matris_komut_button) { matris_komut_button.addEventListener("click", sendCmd, false); }


//////// komut dropdown seçeneklerinde değişim
function placeAsCommand(event) {
    if (event.currentTarget.value == "komut_options_placeholder")
        return;

    var command_row = document.getElementById("matris_komut_satır");

    // Empty input area
    if (command_row.value == "")
        command_row.value = event.currentTarget.value;
    else
        command_row.value += event.currentTarget.value;

    event.currentTarget.selectedIndex = 0;
}

// komut dropdown change event
const matris_komut_options = document.getElementById("matris_komut_options");
if (matris_komut_options) { matris_komut_options.addEventListener("change", placeAsCommand, false); }


//////// özel matris dropdown seçeneklerinde değişim
function specialMatrisPick(event) {
    if (event.currentTarget.value == "special_options_placeholder")
        return;

    // Make special panel visible
    document.getElementById("matris_vals_special").style.display = "inherit";
    document.getElementById("matris_special_args").style.display = "inherit";

    // Replace args placeholder with minimal needed
    document.getElementById("matris_special_args").placeholder = event.currentTarget[event.currentTarget.selectedIndex].getAttribute("data");

    // Hide standard panel
    document.getElementById("matris_vals").style.display = "none";

    // Update "active" classes and styles
    document.getElementById("special_matris_options").style.backgroundColor = "white";
    document.getElementById("special_matris_options").style.setProperty("border","1px solid #d6d6d6","important");
    document.getElementById("matris_create_bytext_parent").classList.remove("active");
    //document.getElementById("matris_create_byfile_parent").classList.remove("active");

    // Add selected value
    document.getElementById("matris_vals_special").value = event.currentTarget.value;

    // Revert displayed selection
    event.currentTarget.selectedIndex = 0;

    // Change add button's attribute
    document.getElementById("matris_add_button").setAttribute("currentTab", "special");
}

// komut dropdown change event
const special_matris_options = document.getElementById("special_matris_options");
if (special_matris_options) { special_matris_options.addEventListener("change", specialMatrisPick, false); }


//////// Text matris panelini göster
function textMatrisPick(event) {
    
    // Hide special panels
    document.getElementById("matris_vals_special").style.display = "none";
    document.getElementById("matris_special_args").style.display = "none";

    // Show standard panel
    document.getElementById("matris_vals").style.display = "inherit";

    // Update "active" classes and styles
    document.getElementById("matris_create_bytext_parent").classList.add("active");
    document.getElementById("special_matris_options").style.backgroundColor = "inherit";
    document.getElementById("special_matris_options").style.border = null;
    //document.getElementById("matris_create_byfile_parent").classList.remove("active");

    // Change add button's attribute
    document.getElementById("matris_add_button").setAttribute("currentTab", "text");
}

// komut dropdown change event
const matris_create_bytext = document.getElementById("matris_create_bytext");
if (matris_create_bytext) { matris_create_bytext.addEventListener("click", textMatrisPick, false); }

//////// komut geçmişi panelini güncelle
function updateHistoryPanel(token) {
    $('#output_body').load('/Matris?handler=UpdateHistoryPanel', { __RequestVerificationToken: token }, function () {
        updateTable(token);
        }

    );
}