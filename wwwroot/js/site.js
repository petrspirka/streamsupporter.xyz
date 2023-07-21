// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function GetTheme() {
    let themeRegex = document.cookie.match(/(?<=theme=)[^;]*/gm);
    if (themeRegex === null) {
        return "light";
    }
    let theme = themeRegex[0];
    console.log("Theme: " + theme);
    if (theme != "dark") {
        theme = "light"
    }
    return theme;
}

function SetTheme(theme) {
    document.cookie = "theme=" + theme;
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
    console.log("Reloading theme, which is set to: " + GetTheme());
    document.documentElement.dataset.bsTheme = GetTheme();
}