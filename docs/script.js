var MDConverter;

$("#site").hide(0, () => $(() => new Promise(e => {
    /*showdown.extension("font", () => [{
        type: "output",
        filter: e => e.replace(/\$\{(.+)\}\((.+)\)/g, (e, o, a) => `<span style="font-family: ${o};">${a}</span>`)
      }]), showdown.extension("webpage", () => [{
        type: "output",
        filter: e => e.replace(/~\{(.+)\}\((\S+|(\S+) =(\d+)x(\d+))\)/g, (_, o, a, t, r, s) => `<iframe src="${t||a}" style="border:0" ${r?`width=${r}`:""} ${s?`height=${s}`:""} title="${o}">${o}</iframe>`)
      }]), showdown.extension("message", () => [{
        type: "output",
        filter: e => e.replace(/<\/?p[^>]*>/g, "")
      }]),*/
      MDConverter = new showdown.Converter({
        //extensions: ["message", "font", "webpage"],
        //omitExtraWLInCodeBlocks: !0,
        //noHeaderId: !0,
        //parseImgDimensions: !0,
        //strikethrough: !0,
        //requireSpaceBeforeHeadingText: !0,
        //emoji: !0,
        //openLinksInNewWindow: !0
      }),
      MDConverter.setFlavor('github');
      loadArticle('README.md');
    fetch("./articles/index.json").then(x => x.json()).then(x => Object.entries(x).forEach(catagory => {
        const [catagoryName,catagoryArticles] = catagory;
        if (catagoryName == "global") {
          catagoryArticles.forEach(article => {
            $("#articles").append(`<li class=\"article\" onclick=\"loadArticle(\'${article.filepath}\')\">${article.name}</li>`);
          });
        } else {
            var $el = $(`<li class="catagory">${catagoryName}<br><ul></ul></li>`);
            
            catagoryArticles.forEach((article,i,a) => {
                $($el[0].children[1]).append(`<li class=\"article\" onclick=\"loadArticle(\'${article.filepath}\')\">${article.name}</li>`)
                if (i == a.length - 1) {
                    $("#articles").append($el);
                }
            })
        }
    })).then(e);
}).then(() => $("#loader").fadeOut("slow", () => $("#site").show()))));
var loadArticle = (url) => {
    fetch(`./articles/${url}`).then(x => x.text()).then(x => {
        $(".Content").html(MDConverter.makeHtml(x)/*.replace(/<code>(.*)<\/code>/g, (e, o) => `<pre><code>${hljs.highlightAuto(o).value}</code></pre>`)*/);
    });
}