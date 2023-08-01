// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function GetTheme() {
    console.log("Cookie: " + document.cookie);
    let theme = GetCookie("theme");
    console.log("Theme: " + theme);
    if (theme != "dark") {
        theme = "light"
    }
    return theme;
}

function SetTheme(theme) {
    console.log("Cookie: " + document.cookie);
    SetCookie("theme", theme);
    console.log("Setting theme to " + theme);
    if (typeof jscolor !== "undefined") {
        var themedColor = theme == "dark" ? "#333333FF" : "#FFFFFFFF";
        jscolor.presets.ThemedPreset.backgroundColor = themedColor;
        var colorPickers = document.getElementsByClassName("jscolor");
        for (var i = 0; i < colorPickers.length; i++) {
            colorPickers[i].jscolor.backgroundColor = themedColor;
        }
    }
}

function ReloadTheme() {
    console.log("Cookie: " + document.cookie);
    console.log("Reloading theme, which is set to: " + GetTheme());
    document.documentElement.dataset.bsTheme = GetTheme();
}

function SetCookie(name, value) {
    document.cookie = name + '=' + value + '; Path=/;';
}

function GetCookie(name) {
    const regex = new RegExp("(?<=" + name + "=)[^;]*", 'gm');
    const match = document.cookie.match(regex);
    if (match === null) {
        return "light";
    }
    return match[0];
}

/*
function TestWidget(id) {
    const form = document.getElementById("testWidgetForm-"+id);
    form.submit();
}
*/

function TestWidget(id, aft) {
    $.ajax({
        method: "POST",
        url: "?handler=TestTrigger&id="+id,
        headers: {
            "RequestVerificationToken": aft
        }
    });
}