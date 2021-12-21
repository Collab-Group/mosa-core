! function() {
    var e;
    $("#site").hide(0, () => $(() => new Promise(i => {
      showdown.extension("highlightjs", function() {
        return [{
          type: "output",
          filter: function(e, t, i) {
            return showdown.helper.replaceRecursiveRegExp(e, function(e, t, i, n) {
              return t = t.replace(/&amp;/g, "&").replace(/&lt;/g, "<").replace(/&gt;/g, ">"), i + hljs.highlightAuto(t).value + n
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
      })).setFlavor("github"), t(JSON.stringify({
        title: "README",
        location: "README.md"
      })), fetch("./articles/index.json").then(e => e.json()).then(e => Object.entries(e).forEach(e => {
        const [t, i] = e;
        if ("global" == t) i.forEach(e => {
          $("#articles").append(`<li class="article" onclick='loadArticle(\"${JSON.stringify(e)}\")'>${e.title}</li>`)
        });
        else {
          var n = $(`<li class="catagory">${t}<br><ul></ul></li>`);
          i.forEach((e, t, i) => {
            $(n[0].children[1]).append(`<li class="article" onclick='loadArticle(\"${JSON.stringify(e)}\")'>${e.title}</li>`), t == i.length - 1 && $("#articles").append(n)
          })
        }
      })).then(i)
    }).then(() => $("#loader").fadeOut("slow", () => $("#site").show()))));
    var t = t => {
      t = JSON.parse(t), $("#articleTitle").text(t.title), fetch(`./articles/${t.location}`).then(e => e.ok ? e : ! function() {
        throw new Error("Invalid filepath")
      }()).then(e => e.text()).then(t => $(".Content").html(e.makeHtml(t))).catch(e => alert(e))
    }
  }();