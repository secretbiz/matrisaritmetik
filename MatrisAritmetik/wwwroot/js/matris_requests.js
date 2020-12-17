function updateMathContent() {
    try {
        var math = document.getElementById("matris_table");
        MathJax.Hub.Queue(["Typeset", MathJax.Hub, math]);

        var hist = document.getElementById("output_body");
        MathJax.Hub.Queue(["Typeset", MathJax.Hub, hist]);
        //console.log("rerendering math...");
    }
    catch (err) {
        console.log(err.message);
    }
}

//////// matris ekleme fonksiyonu
function addMatrix(event) {
    var tkn = event.currentTarget.token;
    var delim = document.getElementById("displaySelectedSpacer").value;
    var newline = document.getElementById("displaySelectedLiner").value;

    if (delim == "")
        delim = " ";
    if (newline == "")
        newline = "\n";

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
                    "vals": document.getElementById("matris_vals").value,
                    "delimiter": delim,
                    "newline": newline
                },
                success: function (data) { updateTable(tkn); updateHistoryPanel(tkn);},
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
            success: function (data) { updateTable(tkn); updateHistoryPanel(tkn); },
            error: function (error) { console.log(error); }
        });
}

// matris ekleme butonu click event
const matris_add_button = document.getElementById("matris_add_button");
if (matris_add_button) { matris_add_button.addEventListener("click", addMatrix, false); }


//////// komut gönder
function sendCmd(event) {
    var tkn = event.currentTarget.token;
    var filteredcmd = document.getElementById("matris_komut_satır").textContent
        .split("=").join("!__EQ!")
        .split("./").join("!__REVMUL!")
        .replace(/\u00a0/g, " ");

    if (filteredcmd.trim() == "")
        return;

    $.ajax(
        {
            type: 'POST',
            url: 'Matris?handler=SendCmd',
            data:
            {
                __RequestVerificationToken: tkn,
                "cmd": filteredcmd,
            },
            success: function (data) { updateHistoryPanel(tkn); updateMathContent(); },
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
    if (command_row.textContent == "")
        command_row.textContent = event.currentTarget.value;
    else
        command_row.textContent += event.currentTarget.value;

    event.currentTarget.selectedIndex = 0;
    colorInput();
}

// komut dropdown change event
const matris_komut_options = document.getElementById("matris_komut_options");
if (matris_komut_options) { matris_komut_options.addEventListener("change", placeAsCommand, false); }

//////// dosya yükleme
async function uploadFiles(event) {
    var tkn = event.currentTarget.token;
    var input = document.getElementById("matris_file_button");
    var files = input.files;

    if (files == null)
        return;
    if (files.length != 1)
        return;

    if (files[0].size > 5e+7) {
        resetFilePanel(null);
        alert("Dosya boyutu en fazla 5MB olabilir!");
        return;
    }

    if (files[0].type != "text/plain" && files[0].type != "text/csv" && files[0].type != "application/vnd.ms-excel") {
        resetFilePanel(null);
        alert("Dosya olarak yalnızca text(.txt) ve comma-seperated-values(.csv) uzantılı dosyalar yüklenebilir!");
        return;
    }
    let formData = new FormData();

    formData.append("file", files[0]);
    formData.append("type", files[0].type);
    formData.append("name", document.getElementById("matris_name").value);
    formData.append("delim", document.getElementById("displaySelectedSpacer").value);
    formData.append("newline", document.getElementById("displaySelectedLiner").value);

    await fetch('Matris?handler=UploadFile',
        {
            method: "POST",
            body: formData,
            headers:
                { "RequestVerificationToken": tkn }
        }
    ).then(function (data) { updateTable(tkn); updateHistoryPanel(tkn); });
    
}

function resetFilePanel(args) {
    document.getElementById("matris_file_button").value = '';
    document.getElementById("displaySelectedLiner").value = "";
    document.getElementById("displaySelectedSpacer").value = "";
    document.getElementById("matris_name").value = "";
}

function placeDefaultValues(event) {
    var input = document.getElementById("matris_file_button");
    var files = input.files;

    if (files == null)
        return;
    if (files.length != 1)
        return;

    if (files[0].size > 5e+7) {
        resetFilePanel(null);
        alert("Dosya boyutu en fazla 5MB olabilir!");
        return;
    }

    if (files[0].type != "text/plain" && files[0].type != "text/csv" && files[0].type != "application/vnd.ms-excel") {
        resetFilePanel(null);
        alert("Dosya olarak yalnızca text(.txt) ve comma-seperated-values(.csv) uzantılı dosyalar yüklenebilir!");
        return;
    }

    let name = files[0].name;
    document.getElementById("matris_name").value = name.substring(0, name.lastIndexOf("."));

    switch (files[0].type) {
        case "text/plain":
            {
                document.getElementById("displaySelectedSpacer").value = " ";
                document.getElementById("displaySelectedLiner").value = "\\n";
                break;
            }
        case "text/csv":
            {
                document.getElementById("displaySelectedSpacer").value = ",";
                document.getElementById("displaySelectedLiner").value = "\\n";
                break;
            }
        case "application/vnd.ms-excel": // csv?
            {
                document.getElementById("displaySelectedSpacer").value = ",";
                document.getElementById("displaySelectedLiner").value = "\\n";
                break;
            }
        default:
            {
                resetFilePanel(null);
                alert("Dosya uzantısı .csv veya .txt olmalı!");
                return;
            }
    }
}

// dosya yükleme butonu click event
const matris_fromfile_create_button = document.getElementById("matris_fromfile_create_button");
if (matris_fromfile_create_button) { matris_fromfile_create_button.addEventListener("click", uploadFiles, false); }
// dosya seçme paneli change event
const matris_file_button = document.getElementById("matris_file_button");
if (matris_file_button) { matris_file_button.addEventListener("change", placeDefaultValues, false); }

//////// Text matris panelini göster
function textMatrisPick(event) {
    
    // Hide special panels
    document.getElementById("matris_vals_special").style.display = "none";
    document.getElementById("matris_special_args").style.display = "none";
    document.getElementById("matris_file_button").style.display = "none";

    // Show standard panel
    document.getElementById("matris_vals").style.display = "inherit";
    document.getElementById("matris_options_column").style.display = "block";

    // Update "active" classes and styles
    document.getElementById("matris_create_bytext_parent").classList.add("active");
    document.getElementById("matris_create_fromfile_parent").classList.remove("active");
    document.getElementById("special_matris_options").style.backgroundColor = "inherit";
    document.getElementById("special_matris_options").style.border = null;
    //document.getElementById("matris_create_byfile_parent").classList.remove("active");

    // Change add button's attribute
    document.getElementById("matris_add_button").setAttribute("currentTab", "text");

    document.getElementById("matris_fromfile_create_button").style.display = "none";
    document.getElementById("matris_add_button").style.display = "inline-block";
}

// yazıdan matris tab click event
const matris_create_bytext = document.getElementById("matris_create_bytext");
if (matris_create_bytext) { matris_create_bytext.addEventListener("click", textMatrisPick, false); }

//////// Dosya ile matris panelini göster
function fileMatrisPick(event) {

    // Hide other panels
    document.getElementById("matris_vals_special").style.display = "none";
    document.getElementById("matris_special_args").style.display = "none";
    document.getElementById("matris_vals").style.display = "none";
    document.getElementById("matris_options_column").style.display = "none";

    // Show upload button and options
    document.getElementById("matris_file_button").style.display = "block";
    document.getElementById("matris_options_column").style.display = "block";

    // Update "active" classes and styles
    document.getElementById("matris_create_fromfile_parent").classList.add("active");
    document.getElementById("matris_create_bytext_parent").classList.remove("active");
    document.getElementById("special_matris_options").style.backgroundColor = "inherit";
    document.getElementById("special_matris_options").style.border = null;
    //document.getElementById("matris_create_byfile_parent").classList.remove("active");

    // Change add button's attribute
    document.getElementById("matris_add_button").setAttribute("currentTab", "file");

    document.getElementById("matris_fromfile_create_button").style.display = "inline-block";
    document.getElementById("matris_add_button").style.display = "none";
}

// dosyadan matris tab click event
const matris_create_fromfile = document.getElementById("matris_create_fromfile");
if (matris_create_fromfile) { matris_create_fromfile.addEventListener("click", fileMatrisPick, false); }

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
    document.getElementById("matris_options_column").style.display = "none";
    document.getElementById("matris_file_button").style.display = "none";

    // Update "active" classes and styles
    document.getElementById("special_matris_options").style.backgroundColor = "white";
    document.getElementById("special_matris_options").style.setProperty("border", "1px solid #d6d6d6", "important");
    document.getElementById("matris_create_bytext_parent").classList.remove("active");
    document.getElementById("matris_create_fromfile_parent").classList.remove("active");
    //document.getElementById("matris_create_byfile_parent").classList.remove("active");

    // Add selected value
    document.getElementById("matris_vals_special").value = event.currentTarget.value;

    // Revert displayed selection
    event.currentTarget.selectedIndex = 0;

    // Change add button's attribute
    document.getElementById("matris_add_button").setAttribute("currentTab", "special");

    document.getElementById("matris_fromfile_create_button").style.display = "none";
    document.getElementById("matris_add_button").style.display = "inline-block";
}

// komut dropdown change event
const special_matris_options = document.getElementById("special_matris_options");
if (special_matris_options) { special_matris_options.addEventListener("change", specialMatrisPick, false); }

//////// komut geçmişi panelini güncelle
function updateHistoryPanel(token) {
    $('#output_body').load('/Matris?handler=UpdateHistoryPanel', { __RequestVerificationToken: token }, function () {
        updateTable(token);
        }

    );
}


// Token highlighting ( individual )
function colorInput(event) {
    var append = false;
    var parentspan = window.getSelection();
    var parentnode = null;
    var ind = null;

    parentnode = parentspan.baseNode.parentNode.id;
    var baseind = parentnode.split("_")[1];
    if (parentnode.substring(0, 3) == "cmd") {

        ind = parseInt(baseind);
        ind += parentspan.baseNode.textContent.length - 1;

        let chars = document.getElementById("matris_komut_satır").childNodes;
        let lastind = parseInt(chars[chars.length - 1].id.split("_")[1]);

        if (lastind > chars.length - 1) {
            if (chars.length > 0) {
                if (chars[0].id != "cmd_0")
                    ind = chars[0].textContent.length - 1;
                else if (parentspan.baseNode.textContent != parentspan.baseNode.parentElement.innerHTML) {
                    let len = parentspan.baseNode.parentElement.innerHTML.length;
                    for (var ch in chars) {
                        if (chars[ch].textContent.length > 1) {
                            ind = parseInt(ch) + len - 1;
                            break;
                        }
                    }
                }
                else
                    ind += 1;
            }
        }
        else if ( ind != chars.length)
        {
            if (baseind != "0") {
                ind += 1;
            }
            else if (chars.length >= 1) {
                ind += 1;
            }
        }
        else
            append = true;

    }
    else {
        append = true;
    }

    let inputSpan = document.getElementById("matris_komut_satır");
    let newText = Highlight(inputSpan.textContent);
    inputSpan.innerHTML = newText;
    if (append)
        setCaretPos(inputSpan);
    else
        setCaretPos(inputSpan, ind);

}

const operators = ["+", "-", "*", "/", ".", "%", "^", "="];
const seperators = ["(", ")", ","];

function Highlight(text) {
    var htmlText = text;

    var span = "";
    var lengthoffset = 0;
    var length = htmlText.length;
    var offsetindex = 0;
    var char = "";
    var docsStarted = false;
    var settingsStarted = false;
    var anySettingNameRead = false;
    var settingattValStarted = false;

    for (var i = 0; i < length; i++) {
        offsetindex = lengthoffset;
        if (offsetindex == htmlText.length)
            offsetindex -= 1;
        char = htmlText[offsetindex];

        if (docsStarted) { // Docs
            if (char == ";") {
                span = "<span class='settings_sep' id='cmd_" + i + "'>";
                docsStarted = false;
                settingsStarted = true;
            }
            else {
                span = "<span class='docs_term' id='cmd_" + i + "'>";
            }
        }
        else if (settingsStarted) { // Settings "; apply:setting value ;"

            if (char == ";") {  // Start a new settings block
                span = "<span class='settings_sep' id='cmd_" + i + "'>";
                settingattValStarted = false;
                anySettingNameRead = false;
            }
            else if (char == ":") { // Start reading the attribute value
                span = "<span class='settings_setapplysep' id='cmd_" + i + "'>";
            }
            else if (char.trim() == "") {      // Attribute val starting to be read or at start
                span = "<span id='cmd_" + i + "'>";
                if (anySettingNameRead) {
                    settingattValStarted = true;
                    anySettingNameRead = false;
                }
            }
            else if (settingattValStarted) {       // Attribute val is currently being read
                span = "<span class='settings_attvalue' id='cmd_" + i + "'>";
            }
            else {// Attribute name is currently read
                span = "<span class='settings_attname' id='cmd_" + i + "'>";
                anySettingNameRead = true;
            }
            
        }
        else
        {   // Any other operation
            if (operators.includes(char))
                span = "<span class='operator' id='cmd_" + i + "'>";
            else if (seperators.includes(char))
                span = "<span class='seperator' id='cmd_" + i + "'>";
            else if (char == "!")
                span = "<span class='function' id='cmd_" + i + "'>";
            else if (char == "?") {
                span = "<span class='docs' id='cmd_" + i + "'>";
                docsStarted = true;
            }
            else if (char == ";") {
                span = "<span class='settings_sep' id='cmd_" + i + "'>";
                docsStarted = false;
                settingsStarted = true;
            }
            else
                span = "<span class='text_or_num' id='cmd_" + i + "'>";

        }
        lengthoffset += span.length + 8;
        span += char + "</span>";

        htmlText = htmlText.substring(0, offsetindex) +
            span +
            htmlText.substring(offsetindex + 1, length + lengthoffset);

    }
    return htmlText;
}

function setCaretPos(elem,o = null) {
    let range = document.createRange();
   
    if (o != null) {
        range.setStart(elem,o);
        range.collapse(false);
    }
    else {
        range.selectNodeContents(elem);
        range.collapse(false);
    }
    let selection = window.getSelection();
    selection.removeAllRanges();
    selection.addRange(range);
}

// Command syntax 
const matris_komut_satır = document.getElementById("matris_komut_satır");
if (matris_komut_satır) { matris_komut_satır.addEventListener("input", colorInput, false); }
if (matris_komut_satır) {
    matris_komut_satır.addEventListener('copy', (event) => {
        event.clipboardData.setData('text', document.getSelection());
        event.preventDefault();
    }); }
