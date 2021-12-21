var MDConverter;

$("#site").hide(0, () => $(() => new Promise(e => {
    showdown.extension("font", () => [{
        type: "output",
        filter: e => e.replace(/\$\{(.+)\}\((.+)\)/g, (e, o, a) => `<span style="font-family: ${o};">${a}</span>`)
      }]), showdown.extension("webpage", () => [{
        type: "output",
        filter: e => e.replace(/~\{(.+)\}\((\S+|(\S+) =(\d+)x(\d+))\)/g, (_, o, a, t, r, s) => `<iframe src="${t||a}" style="border:0" ${r?`width=${r}`:""} ${s?`height=${s}`:""} title="${o}">${o}</iframe>`)
      }]), showdown.extension("message", () => [{
        type: "output",
        filter: e => e.replace(/<\/?p[^>]*>/g, "")
      }]), showdown.extension("highlightjs", () => [{
        type: "output",
        filter: e => e.replace(/<code>(.*)<\/code>/g, (e, o) => `<pre><code>${hljs.highlightAuto(o).value}</code></pre>`)
      }]);
      MDConverter = new showdown.Converter({
        extensions: ["message", "font", "webpage", "highlightjs"],
        omitExtraWLInCodeBlocks: !0,
        noHeaderId: !0,
        parseImgDimensions: !0,
        strikethrough: !0,
        requireSpaceBeforeHeadingText: !0,
        emoji: !0,
        openLinksInNewWindow: !0
      });
    fetch("./articles/index.json").then(x => x.json()).then(x => x.forEach(article => {
        $("#articles").append(`<li onclick="loadArticle(\\\"${article.filepath}\\\")">${article.name}</li>`);
    }));
    e();
}).then(() => {
    // afterload (don't use)
    $("#loader").fadeOut("slow", () => $("#site").show());
})));
var loadArticle = (url) => {
    fetch(url).then(x => x.text()).then(x => {
        $(".Content").html(MDConverter.makeHtml(x));
    });
}