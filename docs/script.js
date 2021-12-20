$("#site").hide(0, () => $(() => new Promise(e => {
    // preload
    fetch("./articles/index.json").then(x => x.json()).then(x => console.log(x));
    e();
}).then(() => {
    // afterload (don't use)
    $("#loader").fadeOut("slow", () => $("#site").show());
})));