overlays.redirectreport = {
	start : function(){
		
		$$('.redirectreport').each(function(e){
			from = e.getAttribute("replace");
			to = e.getAttribute("to");
			url = document.location.href.replace(new RegExp(from), to);
			anchor = new Element("a");
			anchor.setAttribute("href", url);
			anchor.setAttribute("target","_blank");
			anchor.update(e.innerHTML);
			e.update("");
			e.appendChild(anchor);
		});
	}
}
overlays.items.push(overlays.redirectreport);

