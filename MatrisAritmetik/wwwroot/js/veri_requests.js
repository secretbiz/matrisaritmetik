function updateMathContentDf() {
    try {
        var mat = document.getElementById("matris_table_base");
        MathJax.Hub.Queue(["Typeset", MathJax.Hub, mat]);

        var df = document.getElementById("veri_table_base");
        MathJax.Hub.Queue(["Typeset", MathJax.Hub, df]);

        var hist = document.getElementById("output_body_df");
        MathJax.Hub.Queue(["Typeset", MathJax.Hub, hist]);
        //console.log("rerendering math...");
    }
    catch (err) {
        console.log(err.message);
    }
}

function getExtras() {
    var st = "";
    var opts = document.getElementById("extra_options").selectedOptions;
    for (var i = 0; i < opts.length; i++) {
        st += opts[i].value;
        if (i != opts.length - 1) {
            st += ",";
        }
    }
    return st;
        
}
//////// df ekleme fonksiyonu
function addDataframe(event) {
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
                url: 'Veri?handler=AddDataframe',
                data:
                {
                    __RequestVerificationToken: tkn,
                    "name": document.getElementById("veri_name").value,
                    "vals": document.getElementById("veri_vals").value,
                    "delimiter": delim,
                    "newline": newline,
                    "extras": getExtras()
                },
                success: function (data) { updateMatTable(tkn); updatedfTable(tkn); updateHistoryPanelDf(tkn); },
                error: function (error) { console.log(error); }
            });
    }
    else if (event.currentTarget.getAttribute("currentTab") == "special") {
        $.ajax(
            {
                type: 'POST',
                url: 'Veri?handler=AddDataframeSpecial',
                data:
                {
                    __RequestVerificationToken: tkn,
                    "name": document.getElementById("veri_name").value,
                    "func": document.getElementById("veri_vals_special").value,
                    "args": document.getElementById("veri_special_args").value,
                    "extras": getExtras()
                },
                success: function (_data) { updateMatTable(tkn); updatedfTable(tkn); updateHistoryPanelDf(tkn); },
                error: function (error) { console.log(error); }
            });
    }
}

//////// df ve matris tablosu güncelleme
export function updatedfTable(token) {
    $('#veri_table_base').load('/Veri?handler=UpdateDataframeTable', { __RequestVerificationToken: token }, function () {
        var math = document.getElementById("veri_table_base");
        MathJax.Hub.Queue(["Typeset", MathJax.Hub, math]);
        // Butonları bul & eventlistener ekle
        var buttons = document.getElementsByName("veri_table_delbutton");
        for (var i = 0; i < buttons.length; i++) {
            buttons[i].addEventListener("click", deleteDataframe, false);
            buttons[i].token = token;
        }

    });
}

export function updateMatTable(token) {
    $('#matris_table_base').load('/Veri?handler=UpdateMatrisTable', { __RequestVerificationToken: token }, function () {
        var math = document.getElementById("matris_table_base");
        MathJax.Hub.Queue(["Typeset", MathJax.Hub, math]);
        // Butonları bul & eventlistener ekle
        var buttons = document.getElementsByName("matris_table_delbutton");
        for (var i = 0; i < buttons.length; i++) {
            buttons[i].addEventListener("click", deleteMatrixDf, false);
            buttons[i].token = token;
        }

    });
}


//////// df ve matris silme fonksiyonu
function deleteDataframe(event) {
    var tkn = event.currentTarget.token;
    $.ajax(
        {
            type: 'POST',
            url: 'Veri?handler=DeleteDataframe',
            data:
            {
                __RequestVerificationToken: tkn,
                "name": event.currentTarget.id,
            },
            success: function (data) { updateMatTable(tkn); updatedfTable(tkn); updateHistoryPanelDf(tkn); },
            error: function (error) { console.log(error); }
        });
}

function deleteMatrixDf(event) {
    var tkn = event.currentTarget.token;
    $.ajax(
        {
            type: 'POST',
            url: 'Veri?handler=DeleteMatrix',
            data:
            {
                __RequestVerificationToken: tkn,
                "name": event.currentTarget.id,
            },
            success: function (data) { updateMatTable(tkn); updatedfTable(tkn); updateHistoryPanelDf(tkn); },
            error: function (error) { console.log(error); }
        });
}

// df ekleme butonu click event
const veri_add_button = document.getElementById("veri_add_button");
if (veri_add_button) { veri_add_button.addEventListener("click", addDataframe, false); }


//////// komut gönder
function sendCmdDf(event) {
    var tkn = event.currentTarget.token;
    var filteredcmd = document.getElementById("matris_komut_satır_df").textContent
        .split("=").join("!!__EQ!!")
        .split("&").join("!!__AND!!")
        .split("./").join("!!__REVMUL!!")
        .replace(/\u00a0/g, " ");

    if (filteredcmd.trim() == "")
        return;

    $.ajax(
        {
            type: 'POST',
            url: 'Veri?handler=SendCmd',
            data:
            {
                __RequestVerificationToken: tkn,
                "cmd": filteredcmd,
            },
            success: function (data) { updateMatTable(tkn); updatedfTable(tkn); updateHistoryPanelDf(tkn); updateMathContentDf(); },
            error: function (error) { console.log(error); }
        });
}

// komut gönderme butonu click event
const veri_komut_button = document.getElementById("veri_komut_button");
if (veri_komut_button) { veri_komut_button.addEventListener("click", sendCmdDf, false); }


//////// komut dropdown seçeneklerinde değişim
function placeAsCommandDf(event) {
    if (event.currentTarget.value == "komut_options_placeholder")
        return;

    var command_row = document.getElementById("matris_komut_satır_df");

    // Empty input area
    if (command_row.textContent == "")
        command_row.textContent = event.currentTarget.value;
    else
        command_row.textContent += event.currentTarget.value;

    event.currentTarget.selectedIndex = 0;
    colorInputDf();
}

// komut dropdown change event
const veri_komut_options = document.getElementById("veri_komut_options");
if (veri_komut_options) { veri_komut_options.addEventListener("change", placeAsCommandDf, false); }

//////// dosya yükleme
async function uploadFilesDf(event) {
    var tkn = event.currentTarget.token;
    var input = document.getElementById("veri_file_button");
    var files = input.files;

    if (files == null)
        return;
    if (files.length != 1)
        return;

    if (files[0].size * 2 > 5e+6) {
        resetFilePanelDf(null);
        alert("Dosya boyutu en fazla 5MB olabilir!");
        return;
    }

    if (files[0].type != "text/plain" && files[0].type != "text/csv" && files[0].type != "application/vnd.ms-excel") {
        resetFilePanelDf(null);
        alert("Dosya olarak yalnızca text(.txt) ve comma-seperated-values(.csv) uzantılı dosyalar yüklenebilir!");
        return;
    }
    let formData = new FormData();

    formData.append("file", files[0]);
    formData.append("type", files[0].type);
    formData.append("name", document.getElementById("veri_name").value);
    formData.append("delim", document.getElementById("displaySelectedSpacer").value);
    formData.append("newline", document.getElementById("displaySelectedLiner").value);
    formData.append("extras", getExtras());

    await fetch('Veri?handler=UploadFile',
        {
            method: "POST",
            body: formData,
            headers:
                { "RequestVerificationToken": tkn }
        }
    ).then(function (data) { updateMatTable(tkn); updatedfTable(tkn); updateHistoryPanelDf(tkn); });

}

function resetFilePanelDf(args) {
    document.getElementById("veri_file_button").value = '';
    document.getElementById("displaySelectedLiner").value = "";
    document.getElementById("displaySelectedSpacer").value = "";
    document.getElementById("veri_name").value = "";
}

function placeDefaultValuesDf(event) {
    var input = document.getElementById("veri_file_button");
    var files = input.files;

    if (files == null)
        return;
    if (files.length != 1)
        return;

    if (files[0].size > 5e+7) {
        resetFilePanelDf(null);
        alert("Dosya boyutu en fazla 5MB olabilir!");
        return;
    }

    if (files[0].type != "text/plain" && files[0].type != "text/csv" && files[0].type != "application/vnd.ms-excel") {
        resetFilePanelDf(null);
        alert("Dosya olarak yalnızca text(.txt) ve comma-seperated-values(.csv) uzantılı dosyalar yüklenebilir!");
        return;
    }

    let name = files[0].name;
    document.getElementById("veri_name").value = name.substring(0, name.lastIndexOf("."));

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
                resetFilePanelDf(null);
                alert("Dosya uzantısı .csv veya .txt olmalı!");
                return;
            }
    }
}

// dosya yükleme butonu click event
const veri_fromfile_create_button = document.getElementById("veri_fromfile_create_button");
if (veri_fromfile_create_button) { veri_fromfile_create_button.addEventListener("click", uploadFilesDf, false); }
// dosya seçme paneli change event
const veri_file_button = document.getElementById("veri_file_button");
if (veri_file_button) { veri_file_button.addEventListener("change", placeDefaultValuesDf, false); }

//////// Text df panelini göster
function textVeriPickDf(event) {

    // Hide special panels
    document.getElementById("veri_vals_special").style.display = "none";
    document.getElementById("veri_special_args").style.display = "none";
    document.getElementById("veri_file_button").style.display = "none";

    // Show standard panel
    document.getElementById("veri_vals").style.display = "inherit";
    document.getElementById("veri_options_column").style.display = "block";

    // Update "active" classes and styles
    document.getElementById("veri_create_bytext_parent").classList.add("active");
    document.getElementById("veri_create_fromfile_parent").classList.remove("active");
    document.getElementById("special_veri_options").style.backgroundColor = "inherit";
    document.getElementById("special_veri_options").style.border = null;
    //document.getElementById("veri_create_byfile_parent").classList.remove("active");

    // Change add button's attribute
    document.getElementById("veri_add_button").setAttribute("currentTab", "text");

    document.getElementById("veri_fromfile_create_button").style.display = "none";
    document.getElementById("veri_add_button").style.display = "inline-block";
}

// yazıdan df tab click event
const veri_create_bytext = document.getElementById("veri_create_bytext");
if (veri_create_bytext) { veri_create_bytext.addEventListener("click", textVeriPickDf, false); }

//////// Dosya ile df panelini göster
function fileVeriPickDf(event) {

    // Hide other panels
    document.getElementById("veri_vals_special").style.display = "none";
    document.getElementById("veri_special_args").style.display = "none";
    document.getElementById("veri_vals").style.display = "none";
    document.getElementById("veri_options_column").style.display = "none";

    // Show upload button and options
    document.getElementById("veri_file_button").style.display = "block";
    document.getElementById("veri_options_column").style.display = "block";

    // Update "active" classes and styles
    document.getElementById("veri_create_fromfile_parent").classList.add("active");
    document.getElementById("veri_create_bytext_parent").classList.remove("active");
    document.getElementById("special_veri_options").style.backgroundColor = "inherit";
    document.getElementById("special_veri_options").style.border = null;
    //document.getElementById("veri_create_byfile_parent").classList.remove("active");

    // Change add button's attribute
    document.getElementById("veri_add_button").setAttribute("currentTab", "file");

    document.getElementById("veri_fromfile_create_button").style.display = "inline-block";
    document.getElementById("veri_add_button").style.display = "none";
}

// dosyadan df tab click event
const veri_create_fromfile = document.getElementById("veri_create_fromfile");
if (veri_create_fromfile) { veri_create_fromfile.addEventListener("click", fileVeriPickDf, false); }

//////// özel df dropdown seçeneklerinde değişim
function specialVeriPickDf(event) {
    if (event.currentTarget.value == "special_options_placeholder")
        return;

    // Make special panel visible
    document.getElementById("veri_vals_special").style.display = "inherit";
    document.getElementById("veri_special_args").style.display = "inherit";

    // Replace args placeholder with minimal needed
    document.getElementById("veri_special_args").placeholder = event.currentTarget[event.currentTarget.selectedIndex].getAttribute("data");

    // Hide standard panel
    document.getElementById("veri_vals").style.display = "none";
    document.getElementById("veri_options_column").style.display = "none";
    document.getElementById("veri_file_button").style.display = "none";

    // Update "active" classes and styles
    document.getElementById("special_veri_options").style.backgroundColor = "white";
    document.getElementById("special_veri_options").style.setProperty("border", "1px solid #d6d6d6", "important");
    document.getElementById("veri_create_bytext_parent").classList.remove("active");
    document.getElementById("veri_create_fromfile_parent").classList.remove("active");
    //document.getElementById("veri_create_byfile_parent").classList.remove("active");

    // Add selected value
    document.getElementById("veri_vals_special").value = event.currentTarget.value;

    // Revert displayed selection
    event.currentTarget.selectedIndex = 0;

    // Change add button's attribute
    document.getElementById("veri_add_button").setAttribute("currentTab", "special");

    document.getElementById("veri_fromfile_create_button").style.display = "none";
    document.getElementById("veri_add_button").style.display = "inline-block";
}

// komut dropdown change event
const special_veri_options = document.getElementById("special_veri_options");
if (special_veri_options) { special_veri_options.addEventListener("change", specialVeriPickDf, false); }

//////// komut geçmişi panelini güncelle
function updateHistoryPanelDf(token) {
    $('#output_body_df').load('/Veri?handler=UpdateHistoryPanel', { __RequestVerificationToken: token }, function () {
        updateMatTable(token);
    }

    );
}

// Token highlighting ( individual )
function colorInputDf(_event) {
    var append = false;
    var parentspan = window.getSelection();
    var parentnode = null;
    var ind = null;

    parentnode = parentspan.baseNode.parentNode.id;
    var baseind = parentnode.split("_")[1];
    if (parentnode.substring(0, 3) == "cmd") {

        ind = parseInt(baseind);
        ind += parentspan.baseNode.textContent.length - 1;

        let chars = document.getElementById("matris_komut_satır_df").childNodes;
        let lastind = parseInt(chars[chars.length - 1].id.split("_")[1]);

        if (lastind > chars.length - 1) {
            if (chars.length > 0) {
                if (chars[0].id != "cmd_0")
                    ind = chars[0].textContent.length - 1;
                else if (parentspan.baseNode.textContent != parentspan.baseNode.parentElement.innerHTML) {
                    let len = parentspan.baseNode.parentElement.innerHTML.length;
                    for (var ch in chars) {
                        if (chars[ch].textContent == null) {
                            continue;
                        }
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
        else if (ind != chars.length) {
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

    let inputSpan = document.getElementById("matris_komut_satır_df");
    const maxlen = 128;
    if (inputSpan.textContent.length > maxlen) {
        inputSpan.textContent = inputSpan.textContent.substring(0, maxlen);
        alert("Komut karakter limiti: " + maxlen);
        setCaretPosDf(inputSpan);
        return;
    }
    let newText = HighlightDf(inputSpan.textContent);
    inputSpan.innerHTML = newText;
    if (append)
        setCaretPosDf(inputSpan);
    else
        setCaretPosDf(inputSpan, ind);

}

const operators = ["+", "-", "*", "/", ".", "%", "^", "="];
const seperators = ["(", ")", ","];

function HighlightDf(text) {
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
        else {   // Any other operation
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

function setCaretPosDf(elem, o = null) {
    let range = document.createRange();

    if (o != null) {
        var _temp = true;
        while (_temp) {
            try {
                range.setStart(elem, o);
            } catch (e) {
                o--;
                if (o == 0) {
                    _temp = false;
                }
            }
            _temp = false;
        }
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
const matris_komut_satır_df = document.getElementById("matris_komut_satır_df");
if (matris_komut_satır_df) { matris_komut_satır_df.addEventListener("input", colorInputDf, false); }
if (matris_komut_satır_df) {
    matris_komut_satır_df.addEventListener('copy', (event) => {
        event.clipboardData.setData('text', document.getSelection());
        event.preventDefault();
    });
}