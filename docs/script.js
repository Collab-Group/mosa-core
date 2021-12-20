$("#site").hide(0, () => $(() => new Promise(e => {
    // preload
    fetch("./articles/index.json").then(x => x.json()).then(x => x.forEach(article => {
        console.log(article.name);
    }));
    e();
}).then(() => {
    // afterload (don't use)
    $("#loader").fadeOut("slow", () => $("#site").show());
})));
var loadArticle = (url) => {

}