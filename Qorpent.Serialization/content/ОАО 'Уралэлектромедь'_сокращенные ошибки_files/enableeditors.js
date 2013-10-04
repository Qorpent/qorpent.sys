overlays.enableeditors = {
	start : function(){
		$$('.editor').each(function(e){
			e.show()
		});
		
	}
}
overlays.items.push(overlays.enableeditors);

