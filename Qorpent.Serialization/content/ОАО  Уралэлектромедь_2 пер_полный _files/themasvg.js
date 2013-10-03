overlays.themasvg = {
	start : function(){
		qweb.defineGlobal("zeta.ta.node");
			Object.extend(zeta.ta.node, {
				clicked : function(event, code, file){
					if(event.shiftKey){
						window.open("/z3adm/filemanager/index.rails?autosearch="+file,"_blank");
					}else{
						document.location = document.location.href.replace(/themacode=[\w\d]*/,'themacode='+code);
					}
				}
			});
			
				$$('a.thema_node_ref').each(function(e){
					Event.observe(e,"click",function(event){
						zeta.ta.node.clicked(event, e.getAttribute("nodecode"), e.getAttribute("nodefile"));
						event.stop();
						return false;
					});
				});
			
	}
}
overlays.items.push(overlays.themasvg);

