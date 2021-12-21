var e;
$("#site").hide(0, () => $(() => new Promise(i => {
  showdown.extension("highlightjs", function() {
    return [{
      type: "output",
      filter: function(e, t, i) {
        return showdown.helper.replaceRecursiveRegExp(e, function(e, t, i, l) {
          return t = t.replace(/&amp;/g, "&").replace(/&lt;/g, "<").replace(/&gt;/g, ">"), i + hljs.highlightAuto(t).value + l
        }, "<pre><code\\b[^>]*>", "</code></pre>", "g")
      }
    }]
  }), (e = new showdown.Converter({
    extensions: ["highlightjs"],
    omitExtraWLInCodeBlocks: !0,
    noHeaderId: !0,
    parseImgDimensions: !0,
    strikethrough: !0,
    emoji: !0,
    openLinksInNewWindow: !0
  })).setFlavor("github"), fetch("./articles/index.json").then(e => e.json()).then(e => {
      var params = new URLSearchParams(location.search);
      if (params.has("doc")) {
          var split = params.get("doc").split('.');
        if (split.length == 2) {
         if (e[split[0]] && e[split[0]][Number(split[1])]) {
            t(JSON.stringify(e[split[0]][Number(split[1])]));
        } else {
            t(JSON.stringify({
                title: "README",
                location: "README.md"
              }));
          }
        } else {
            t(JSON.stringify({
                title: "README",
                location: "README.md"
              }));
          }
      } else {
        t(JSON.stringify({
            title: "README",
            location: "README.md"
          }));
      }
      return e;
  }).then(e => Object.entries(e).forEach(e => {
    const [t, i] = e;
    if ("global" == t) i.forEach(e => {
      $("#articles").append(`<li class="article" onclick='t(\`${JSON.stringify(e)}\`)'>${e.title}</li>`)
    });
    else {
      var l = $(`<li class="catagory">${t}<br><ul></ul></li>`);
      i.forEach((e, t, i) => {
        $(l[0].children[1]).append(`<li class="article" onclick='t(\`${JSON.stringify(e)}\`)'>${e.title}</li>`), t == i.length - 1 && $("#articles").append(l)
      })
    }
  })).then(i);
}).then(() => $("#loader").fadeOut("slow", () => $("#site").show()))));
var t = t => {
  t = JSON.parse(t), $("#articleTitle").text(t.title), fetch(`./articles/${t.location}`).then(e => e.ok ? e : ! function() {
    throw new Error("Invalid filepath")
  }()).then(e => e.text()).then(t => $(".Content").html(e.makeHtml(t))).catch(e => alert(e))
};