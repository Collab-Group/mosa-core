var index,getUrls=async e=>{"use strict";var t=[];return Array.from((new DOMParser).parseFromString(await fetch(e).then(e=>e.text()),"text/xml").getElementsByTagName("loc")).forEach(e=>t.push(e.childNodes[0].nodeValue)),t},indexbuilder=e=>new Promise(async(t,a)=>{const i=await getUrls(e);var s=[];for(const e of i)await fetch(e).then(e=>e.text()).then(e=>(new DOMParser).parseFromString(e,"text/html")).then(t=>s.push({title:t.title,description:t.querySelector("meta[name='description']").content,url:e,icon:t.querySelector('link[rel*="icon"]').href}));t(new Fuse(s,{findAllMatches:!0,keys:["title","description"]}))});const sleep=e=>new Promise(t=>setTimeout(t,e));$("#site").hide(0,()=>$(()=>new Promise(e=>{new URLSearchParams(location.search).has("q")&&new URLSearchParams(location.search).get("q")?new Promise(async e=>{$("#loadinfo").text("Building search index"),await indexbuilder("https://elijah629.github.io/sitemap.xml").then(e=>index=e).then(e)}).then(()=>{$(".Content").html('<div id="searchresults"></div>'),$("#loadinfo").text("Generating search results");var e=index.search(new URLSearchParams(location.search).get("q"));for(const t of e)$("#searchresults").append(ui_searchResult(t.item.title,t.item.url,t.item.description,t.item.icon));$("#searchbox").val(new URLSearchParams(location.search).get("q"))}).then(e):($(".Content").html('<div class="banner"><h1>Elijah629\'s Projects</h1><hr/></div><div class="block-item" style="float:left;"><h3>Repositories:</h3><div id="repos"></div></div><div class="block-item" style="float:right;text-align: right;"><h3>Projects:</h3><div id="projects"></div></div>'),$("#loadinfo").text("Fetching repositories"),fetch("https://api.github.com/users/Elijah629/repos",{mode:"cors",headers:{Authorization:"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"}}).then(e=>e.json()).then(e=>e.forEach((e,t,a)=>{$("#loadinfo").text(`Fetching repositories (${t}/${a.length-1})`),$("#repos").append(`<a href="${e.html_url}">${e.name}</a></br>`)})).then(async()=>{$("#loadinfo").text("Fetching projects"),(await getUrls("https://elijah629.github.io/sitemap.xml")).forEach(async(e,t,a)=>{$("#loadinfo").text(`Fetching projects (${t+1}/${a.length})`),$("#projects").append(`<a href="${e}">${(new DOMParser).parseFromString(await fetch(e).then(e=>e.text()),"text/html").title}</a></br>`)})}).then(e))}).then(()=>{$("#loader").fadeOut("slow",()=>$("#site").show()),$("#searchbox").on("keydown",e=>{"Enter"!==e.key&&13!==e.keyCode||""==e.target.val()||(location.search=`?q=${$("#searchbox").val()}`)})})));var ui_searchResult=(e,t,a,i)=>`<div class="searchresult"><div class="Icon"><span><img src="${i}" alt="Icon"></span></div><div class="Url"><a href="${t}">${new URL(t).origin+new URL(t).pathname.replace(/\/$/,"").replace(/\//g," › ")}</a></div><div class="Title"><span>${e}</span></div><div class="Description"><span>${a}</span></div></div>`;