/*
Features 
1) single line delimiter is ignored
2) multiple line delimiters collapsed to single BR element
3) where are 3 types of blocks :
	a) inline (affect enclosed block in SINGLE line range) bold,italic, etc
	b) line-wide (affects whole line and use in prefix line form) - headers, lists and so on
	c) blocks - enclose group of rows, all blocks are in same syntax = [[CODE]] .. [[/CODE]] 
4) wiki ignorance - prefix any line with ! as first symbol - cause non processing for this row

5) inlines :
	a)	***BOLD***
	b)  **ITALIC**
	c)  __UNDERSCORE__
	d)  --STRIKEOUT--
	e)  {style:CSS-CODE}STYLED CONTENT{style}
	f)  [ HREF ; REFNAME ; ADDITION ATTRIBUTES ] - reference, REFNAME, ADDITION ATTRIBUTES are optional ; used to offen | usage in boot strap
	
6) line markers
	a) ! line with ignore any wiki
	b) =[====...] N-th level header N = 1..6
	c) %[%%%%...] N-th level doted list N = 1..6

7) blocks :
	a) [[nowiki]]...[[/nowiki]] - disable wiki formatting in enclosed block

*/
(function(){
var qwiki = window.qwiki = window.qwiki || {};
qwiki.setup  = function(wikiprocessor){};
qwiki.create = function(text, logwriter){
	var result = {
		text : text,
		lines /* as string[] */: null,
		processed : [],
		html /* as string */: null,
		log : logwriter||function(sender,text){},
		defaultReference: function(addr,name,tail){
			return "<a href='" + addr + "' "+tail+" >" + name + "</a>";
		},
		processReference : function(match, addr, p2, name, p3, tail) {
			return this.defaultReference((addr||"").trim(),(name||"").trim(),(tail||"").trim());
		},
		processCode : function(curline ) {
			return line;
		},
		processDefault : function( curline ) {
			
				// BY LINE IGNORE
				if (curline.match(/^\!/)){ //must ignore any wiki syntax this row
					curline = curline.substring(1);
					this.processed.push(curline);
					return;
				}
				/////////////////////////////////////////////////////
			
				// LINE BREAK SUPPORT
				if (curline == "[BR]") {
					this.processed.push("<br/>");
					return;
				}
				
				//LETER STYLE SUPPORT
				//bold
				curline = curline.replace(/\*\*\*([\s\S]+?)\*\*\*/,'<strong>$1</strong>');
				//italic
				curline = curline.replace(/\*\*([\s\S]+?)\*\*/,'<em>$1</em>');
				//underline
				curline = curline.replace(/__([\s\S]+?)__/,'<ins>$1</ins>');
				//strikeout
				curline = curline.replace(/--([\s\S]+?)--/,'<del>$1</del>');
				//subtext
				curline = curline.replace(/,,([\s\S]+?),,/,'<sub>$1</sub>');
				//supertext
				curline = curline.replace(/::([\s\S]+?)::/,'<sup>$1</sup>');
				//custom style
				curline = curline.replace(/\{style:([\s\S]+?)\}([\s\S]+?)\{style\}/,'<span style="$1">$2</span>');
				// references
				var self = this;
				curline = curline.replace(/\[([^\s]+?)(\s+([^|]+?))?(|([\s\S]+?))?\]/, function(match, addr, p2, name, p3, tail){
					return self.processReference(match, addr, p2, name, p3, tail);
				});
				
				//HEADERS SUPPORT
				if (curline.match(/^======/)){
					curline = "<h6>" + curline.substring(6) + "</h6>";
				}else if (curline.match(/^=====/)){
					curline = "<h5>" + curline.substring(5) + "</h5>";
				}else if (curline.match(/^====/)){
					curline = "<h4>" + curline.substring(4) + "</h4>";
				}else if (curline.match(/^===/)){
					curline = "<h3>" + curline.substring(3) + "</h3>";
				}else if (curline.match(/^==/)){
					curline = "<h2>" + curline.substring(2) + "</h2>";
				}else if (curline.match(/^=/)){
					curline = "<h1>" + curline.substring(1) + "</h1>";
				}

				//LIST SUPPORT
				if (curline.match(/^%%%%%%/)){
					curline = "<div class='wiki-list-6'>" + curline.substring(6) + "</div>";
				}else if (curline.match(/^%%%%%/)){
					curline = "<div class='wiki-list-5'>" + curline.substring(5) + "</div>";
				}else if (curline.match(/^%%%%/)){
					curline = "<div class='wiki-list-4'>" + curline.substring(4) + "</div>";
				}else if (curline.match(/^%%%/)){
					curline = "<div class='wiki-list-3'>" + curline.substring(3) + "</div>";
				}else if (curline.match(/^%%/)){
					curline = "<div class='wiki-list-2'>" + curline.substring(2) + "</div>";
				}else if (curline.match(/^%/)){
					curline = "<div class='wiki-list-1'>" + curline.substring(1) + "</div>";
				}
				
				return curline;
		},
		
		//state control
		idx : 0,
		curline : "",
		nowikiblock : false,
		codeblock : false,
		// main function
		process : function(){
			this.processed = [];
			this.html = "";
			this.processed.push("<div class='wiki'>");
			this.log(this,"start");
			var preprocessedText = '\n'+this.text.trim()+'\n';
			preprocessedText = preprocessedText.replace(/\r/g,'');
			// firstly we must meet 1-st feature - we unify and process line delimiters
			preprocessedText = preprocessedText.replace(/\n\n+/g,"\n\n[BR]\n\n");
			preprocessedText = preprocessedText.replace(/\n([\=\%\!])([^\n]+)/g,'\n\n$1$2\n\n');
			// then we must split lines for block elements
			preprocessedText = preprocessedText.replace(/(\[\[\/?\w+\]\])/g,"\n\n$1\n\n");
			// and finally remove ambigous lines
			preprocessedText = preprocessedText.replace(/\n\n+/g,"__LINER__");
			preprocessedText = preprocessedText.replace(/\n/g," ");
			preprocessedText = preprocessedText.replace(/__LINER__/g,"\n");
			
			this.log(this,"lines merged");
			// then we setup array of 
			this.lines = preprocessedText.split(/\n/);
			this.log(this,"text splited");
			for (this.idx = 0; this.idx < this.lines.length; this.idx++){
				var curline = this.lines[this.idx];
				// CODE BLOCK SUPPORT
				if (curline=="[[/code]]") {
					this.codeblock = false;
					this.processed.push("</div>");
					continue;
				}
				if (curline=="[[code]]") {
					this.codeblock = true;
					this.processed.push("<div class='wiki-code'>");
					continue;
				}
				
				if (this.codeblock) {
					this.processed.push( this.processCode( curline)) ;
				}
				
				
				//WIKI IGNORANCE SUPPORT WITH BLOCK AND INLINE
				if (curline=="[[/nowiki]]") {
					this.nowikiblock = false;
					continue;
				}
				if (curline=="[[nowiki]]") {
					this.nowikiblock = true;
					continue;
				}
				if (this.nowikiblock){
					this.processed.push(curline);
					continue;
				}
				var defaultLine = this. processDefault( curline );
				if(defaultLine ){
					this.processed.push( defaultLine);
				}
			}
			this.processed.push("</div>");
			
			this.html = "";
			for (var i=0;i<this.processed.length;i++){
				this.html+=this.processed[i];
			}
		},
	};
	this.setup(result);
	return result;
}
qwiki.toHTML = function(text){
	var processor = this.create(text);
	processor.process();
	var result = processor.html;
	delete(processor);
	return result;
}

})();