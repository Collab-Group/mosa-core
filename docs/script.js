/* jshint esversion:8 */
/*! JQ scrollTo Plugin */
!function(e){"use strict";"function"==typeof define&&define.amd?define(["jquery"],e):"undefined"!=typeof module&&module.exports?module.exports=e(require("jquery")):e(jQuery)}(function(e){"use strict";var t=e.scrollTo=function(t,o,n){return e(window).scrollTo(t,o,n)};function o(t){return!t.nodeName||-1!==e.inArray(t.nodeName.toLowerCase(),["iframe","#document","html","body"])}function n(e){return"function"==typeof e}function r(t){return n(t)||e.isPlainObject(t)?t:{top:t,left:t}}return t.defaults={axis:"xy",duration:0,limit:!0},e.fn.scrollTo=function(i,s,f){"object"==typeof s&&(f=s,s=0),"function"==typeof f&&(f={onAfter:f}),"max"===i&&(i=9e9),f=e.extend({},t.defaults,f),s=s||f.duration;var a=f.queue&&f.axis.length>1;return a&&(s/=2),f.offset=r(f.offset),f.over=r(f.over),this.each(function(){if(null!==i){var u,c=o(this),l=c?this.contentWindow||window:this,d=e(l),m=i,p={};switch(typeof m){case"number":case"string":if(/^([+-]=?)?\d+(\.\d+)?(px|%)?$/.test(m)){m=r(m);break}m=c?e(m):e(m,l);case"object":if(0===m.length)return;(m.is||m.style)&&(u=(m=e(m)).offset())}var h=n(f.offset)&&f.offset(l,m)||f.offset;e.each(f.axis.split(""),function(e,o){var n="x"===o?"Left":"Top",r=n.toLowerCase(),i="scroll"+n,s=d[i](),v=t.max(l,o);if(u)p[i]=u[r]+(c?0:s-d.offset()[r]),f.margin&&(p[i]-=parseInt(m.css("margin"+n),10)||0,p[i]-=parseInt(m.css("border"+n+"Width"),10)||0),p[i]+=h[r]||0,f.over[r]&&(p[i]+=m["x"===o?"width":"height"]()*f.over[r]);else{var w=m[r];p[i]=w.slice&&"%"===w.slice(-1)?parseFloat(w)/100*v:w}f.limit&&/^\d+$/.test(p[i])&&(p[i]=p[i]<=0?0:Math.min(p[i],v)),!e&&f.axis.length>1&&(s===p[i]?p={}:a&&(x(f.onAfterFirst),p={}))}),x(f.onAfter)}function x(t){var o=e.extend({},f,{queue:!0,duration:s,complete:t&&function(){t.call(l,m,f)}});d.animate(p,o)}})},t.max=function(t,n){var r="x"===n?"Width":"Height",i="scroll"+r;if(!o(t))return t[i]-e(t)[r.toLowerCase()]();var s="client"+r,f=t.ownerDocument||t.document,a=f.documentElement,u=f.body;return Math.max(a[i],u[i])-Math.min(a[s],u[s])},e.Tween.propHooks.scrollLeft=e.Tween.propHooks.scrollTop={get:function(t){return e(t.elem)[t.prop]()},set:function(t){var o=this.get(t);if(t.options.interrupt&&t._last&&t._last!==o)return e(t.elem).stop();var n=Math.round(t.now);o!==n&&(e(t.elem)[t.prop](n),t._last=this.get(t))}},t});
var e;
$("#site").hide(0, () => $(() => new Promise(i => {
  showdown.extension("highlightjs", () => [{
    type: "output",
    filter: e => showdown.helper.replaceRecursiveRegExp(e, (e, t, i, l) => (t = t.replace(/&amp;/g, "&").replace(/&lt;/g, "<").replace(/&gt;/g, ">"), i + hljs.highlightAuto(t).value + l), "<pre><code\\b[^>]*>", "</code></pre>", "g")
  }]), (e = new showdown.Converter({
    extensions: ["highlightjs"],
    omitExtraWLInCodeBlocks: !0,
    parseImgDimensions: !0,
    strikethrough: !0,
    emoji: !0,
    openLinksInNewWindow: !0
  })).setFlavor("github"), fetch("./articles/index.json").then(e => e.json()).then(e => {
    var i = new URLSearchParams(location.search);
    if (i.has("doc")) {
      var l = i.get("doc").split(".");
      2 == l.length && e[l[0]] && e[l[0]][Number(l[1])] ? t(JSON.stringify(e[l[0]][Number(l[1])])) : t(JSON.stringify({
        title: "README",
        location: "README.md"
      }));
    } else t(JSON.stringify({
      title: "README",
      location: "README.md"
    }));
    return e;
  }).then(e => Object.entries(e).forEach(e => {
    const [t, i] = e;
    if ("global" == t) i.forEach(e => $("#articles").append(`<li class="article" onclick='t(\`${JSON.stringify(e)}\`)'>${e.title}</li>`));
    else {
      var l = $(`<li class="catagory">${t}<br><ul></ul></li>`);
      i.forEach((e, t, i) => {
        $(l[0].children[1]).append(`<li class="article" onclick='t(\`${JSON.stringify(e)}\`)'>${e.title}</li>`), t == i.length - 1 && $("#articles").append(l);
      });
    }
  })).then(i);
}).then(() => $("#loader").fadeOut("slow", () => $("#site").show()))));
var t = t => {
  t = JSON.parse(t), $("#articleTitle").text(t.title), fetch(`./articles/${t.location}`).then(e => e.ok ? e : (() => {
    throw new Error("Invalid filepath");
  })()).then(e => e.text()).then(t => $(".Content").html(e.makeHtml(t))).catch(e => alert(e));
};