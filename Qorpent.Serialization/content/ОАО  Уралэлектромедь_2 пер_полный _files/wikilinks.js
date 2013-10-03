overlays.wikilinks = {
	start : function(){
		$$('.wikipage').each(function(e){
			editdiv = new Element("div");
			editdiv.addClassName("wikieditlink");
			code = e.getAttribute("code");
			anchor = new Element("a");
			anchor.setAttribute("href", siteroot+"/wiki/edit.rails?asworkspace=1&code="+code);
			anchor.setAttribute("target","_blank");
			anchor.addClassName("screen_only");
			anchor.update("править Wiki");
			editdiv.appendChild(anchor);
			e.appendChild(editdiv);
		});
		$$('.wikilink').each(function(e){
			code = e.getAttribute("code");
			anchor = new Element("a");
			anchor.setAttribute("href", siteroot+"/wiki/edit.rails?asworkspace=1&code="+code);
			anchor.setAttribute("target","_blank");
			anchor.addClassName("screen_only");
			anchor.update(e.innerHTML);
			e.update("");
			e.appendChild(anchor);
		});
	}
}
overlays.items.push(overlays.wikilinks);

