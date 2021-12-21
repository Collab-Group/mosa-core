var MDConverter;

$("#site").hide(0, () => $(() => new Promise(e => {
      showdown.extension('highlightjs', function() {
        function htmlunencode(text) {
          return (
            text
              .replace(/&amp;/g, '&')
              .replace(/&lt;/g, '<')
              .replace(/&gt;/g, '>')
            );
        }
        return [
          {
            type: 'output',
            filter: function (text, converter, options) {
              // use new shodown's regexp engine to conditionally parse codeblocks
              var left  = '<pre><code\\b[^>]*>',
                  right = '</code></pre>',
                  flags = 'g',
                  replacement = function (wholeMatch, match, left, right) {
                    // unescape match to prevent double escaping
                    match = htmlunencode(match);
                    return left + hljs.highlightAuto(match).value + right;
                  };
              return showdown.helper.replaceRecursiveRegExp(text, replacement, left, right, flags);
            }
          }
        ];
      }),
      MDConverter = new showdown.Converter({
        extensions: ["highlightjs"],
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
    fetch(`./articles/${url}`).then(x => {
      return x.ok ? x : !function() { throw new Error("Invalid filepath") }();
    }).then(x => x.text()).then(x => {
        $(".Content").html(MDConverter.makeHtml(x)/*.replace(/<code>(.*)<\/code>/g, (e, o) => `<pre><code>${hljs.highlightAuto(o).value}</code></pre>`)*/);
    }).catch(x => {
      alert(x);
    });
}