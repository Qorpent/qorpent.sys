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
qwiki.createTOC = function(text, logwriter){
	var result = {
		text : text,
		lines /* as string[] */: null,
		processed : [],
		html /* as string */: null,
		log : logwriter||function(sender,text){},

		processReference : function(match, addr, p2, name, p3, tail,newitem) {
			var addr = (addr || "").trim();
			var title = (name || addr|| "").trim();
			var tail = (tail||"").trim();
			newitem.addr = addr;
			newitem.title = title;
			newitem.extension = tail;
		},
		
		processDefault : function( curline ) {
				var level = 0;
				//HEADERS SUPPORT
				if (curline.match(/^======/)){
					level = 6;
					curline = curline.substring(6).trim();
				}else if (curline.match(/^=====/)){
					level = 5;
					curline = curline.substring(5).trim();
				}else if (curline.match(/^====/)){
					level = 4;
					curline = curline.substring(4).trim();
				}else if (curline.match(/^===/)){
					level = 3;
					curline = curline.substring(3).trim();
				}else if (curline.match(/^==/)){
					level = 2;
					curline = curline.substring(2).trim();
				}else if (curline.match(/^=/)){
					level = 1;
					curline = curline.substring(1).trim();
				}
				if(level == 0 ) return null;
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
				
				var newitem = { level : level, raw : curline, items : []  };
				// references
				
				var self = this;
				curline = curline.replace(/\[([^\s]+?)(\s+([^|~]+?))?([\|~]([\s\S]+?))?\]/, function(match, addr, p2, name, p3, tail){
					return self.processReference(match, addr, p2, name, p3, tail,newitem);
				});
				
				
				if (1==level) {
					this.json.items.push(newitem);
					newitem.parent = this.json;
				}else{
					if (null!=this.last) {
						if ( level == this.last.level ) {
							this.last.parent.items.push(newitem);
							newitem.parent = this.last.parent;
						}else if( level == this.last.level + 1) {
							this.last.items.push(newitem);
							newitem.parent = this.last;
						}else if (level < this.last.level) {
							var target = this.last;
							while (level <= target.level) {
								target = target.parent;
							}
							target.items.push(newitem);
							newitem.parent = target;
						}
					}
				}
				
				this.currentLevel = level;
				this.last = newitem;
				this.json.all = this.json.all || [];
				this.json.all.push(newitem);
		},
		
		//state control
		idx : 0,
		curline : "",
		currentLevel : 0,
		last : null,
		process : function(){
			this.processed = [];
			this.json = {items:[]};
			this.log(this,"start");
			var preprocessedText = '\n'+this.text.trim()+'\n';
			preprocessedText = preprocessedText.replace(/\r/g,'');
			// firstly we must meet 1-st feature - we unify and process line delimiters
			preprocessedText = preprocessedText.replace(/\n\n+/g,"\n\n[BR]\n\n");
			preprocessedText = preprocessedText.replace(/\n([\=\%\!\-\|])([^\n]+)/g,'\n\n$1$2\n\n');
			// then we must split lines for block elements
			preprocessedText = preprocessedText.replace(/(\[\[\/?\w+\]\])/g,"\n\n\n\n$1\n\n\n\n");
			
			// and finally remove ambigous lines
			preprocessedText = preprocessedText.replace(/\n\n+/g,"__LINER__");
			preprocessedText = preprocessedText.replace(/\n/g,"&nbsp;");
			preprocessedText = preprocessedText.replace(/__LINER__/g,"\n");
			
			this.log(this,"lines merged");
			// then we setup array of 
			this.lines = preprocessedText.split(/\n/);
			this.log(this,"text splited");
			for (this.idx = 0; this.idx < this.lines.length; this.idx++){
				this. processDefault( this.lines[this.idx] );
				
			}

			if(this.json.all){
				for(var i = 0;i<this.json.all.length;i++){
					delete(this.json.all[i].parent);
				}
			
			}
			delete (this.json.all);
			
		},
	};
	this.setup(result);
	return result;
}
qwiki.create = function(text, logwriter){
	var result = {
		text : text,
		lines /* as string[] */: null,
		processed : [],
		html /* as string */: null,
		log : logwriter||function(sender,text){},
		defaultReference: function(addr,name,tail){
			if(addr.match(/\.((png)|(gif)|(jpg)|(jpeg))$/)){
				return "<img src='"+addr+"' title='"+name+"' "+tail+" />";
			}else{
				return "<a href='" + addr + "' "+tail+" >" + name + "</a>";
			}
		},
		processReference : function(match, addr, p2, name, p3, tail) {
			return this.defaultReference((addr||"").trim(),(name||addr||"").trim(),(tail||"").trim());
		},
		processCode : function(curline ) {
			curline = curline.replace(/\&nbsp;/g,' __BR__ ');
			curline = curline.replace(/\[BR\]/g,'');
			curline = curline.replace(/\s{4}/g,' __TAB__ ');
			curline = curline.replace(/\t/g,' __TAB__ ');
			//CODE BLOCKS
			curline = curline.replace(
				/([!=+\-*\.\\\/;<>%\&\^\:\|]+)/g,
				"<span _CLASS_ATTR_'operator'>$1</span>");
			curline = curline.replace(/\/\*/g,"<span _CLASS_ATTR_'comment'>");
			curline = curline.replace(/\*\//g,"</span>");
			curline = curline.replace (/(\#[^"']+)$/g,"<span _CLASS_ATTR_'comment'>$1</span>");
			curline = curline.replace(
				/\b((var)|(for)|(return)|(foreach)|(while)|(case)|(switch)|(in)|(out)|(private)|(public)|(protected)|(void)|(function)|(class)|(namespace)|(using)|(select)|(where)|(group by)|(order by)|(null)|(true)|(false))\b/g,
				"<span _CLASS_ATTR_'keyword'>$1</span>");
			curline = curline.replace(
				/\b((int)|(string)|(DateTime)|(decimal)|(bool)|(nvarchar)|(datetime)|(bit)|(byte)|(float)|(long)|(bigint))\b/g,
				"<span _CLASS_ATTR_'type'>$1</span>");
			
			curline = curline.replace(
				/([\{\}\[\]\(\),]+)/g,
				"<span _CLASS_ATTR_'delimiter'>$1</span>");
			
			curline = curline.replace(/\\"/g,'_EQ_');
			curline = curline.replace(/""/g,'_DQ_');
			curline = curline.replace(/"([\s\S]+?)"/g,"<span _CLASS_ATTR_'string'>$1</span>");
			curline = curline.replace(/_EQ_/g,'\\"');
			curline = curline.replace(/_DQ_/g,'""');
			
			curline = curline.replace(/(\b-?\d+(\.\d+)?)/g,"<span _CLASS_ATTR_'number'>$1</span>");
			
			
			curline+="<br/>";
			curline = curline.replace(/_CLASS_ATTR_/g,'class=');
			curline = curline.replace(/__TAB__/g,'&nbsp;&nbsp;&nbsp;&nbsp;');
			curline = curline.replace(/__BR__/g,'<br/>');
			return curline;
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
				//subtext new version
				curline = curline.replace(/,,([\s\S]+?),,/,'<sub>$1</sub>');
				//supertext
				curline = curline.replace(/::([\s\S]+?)::/,'<sup>$1</sup>');
				//custom style
				curline = curline.replace(/\{style:([\s\S]+?)\}([\s\S]+?)\{style\}/,'<span style="$1">$2</span>');
				// references
				var self = this;
				curline = curline.replace(/\[([^\s]+?)(\s+([^|~]+?))?([\|~]([\s\S]+?))?\]/, function(match, addr, p2, name, p3, tail){
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
		table : false,
		firstrow : false,
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
			preprocessedText = preprocessedText.replace(/\n([\=\%\!\-\|])([^\n]+)/g,'\n\n$1$2\n\n');
			// then we must split lines for block elements
			preprocessedText = preprocessedText.replace(/(\[\[\/?\w+\]\])/g,"\n\n\n\n$1\n\n\n\n");
			
			
			
			// and finally remove ambigous lines
			preprocessedText = preprocessedText.replace(/\n\n+/g,"__LINER__");
			preprocessedText = preprocessedText.replace(/\n/g,"&nbsp;");
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
					continue;
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
				
				
				
				if (curline.match(/^\|/)) {
					if (!this.table) {
						this.processed.push("<table>");
						this.processed.push("<thead>");
						this.table = true;
						this.firstrow = true;
						this.headclosed = false;
					}
				}else{
					if(this.table){
						this.processed.push("</tbody>");
						this.processed.push("</table>");
						this.table = false;
					}
				}
								
				if ( this.table ) {
					var tde = this.firstrow ? "th" : "td" ;
					var keephead  = false;
					if(!this.firstrow){
						if(curline.match(/^\|\{\+\}/)){
							curline = curline.replace(/^\|\{\+\}/,'|');
							this.firstrow = true;
							suffix = "";
							tde = "th";
						}
					}
					var items = curline.split(/\|/);
					var row = "";
					if(!this.firstrow && this.headclosed){
						row+="</thead></tbody>";
						this.headclosed = true;
					}
					row += "<tr>";
					for(var i = 0;i<items.length;i++){
						if(i==0||i==items.length-1)continue; //ignore left-right starters
						var cell = items[i].trim();
						cell = this.processDefault(cell).trim();
						var spanmatch = cell.match(/^\{(\d+)?(,(\d+))?\}/);
						if(spanmatch){
							cell = cell.replace(/^\{(\d+)?(,(\d+))?\}/,'');
							var rowspan = 1;
							var colspan = 1;
							
							if(spanmatch[1]){
								colspan = spanmatch[1];
							}
							if (spanmatch[3]){
								rowspan = spanmatch[3];
							}
							row += "<"+tde+" rowspan='"+rowspan+"' colspan='"+colspan+"' >" + cell+"</"+tde+">";
						}else {
							row += "<"+tde+">" + cell+"</"+tde+">";
						}
					}
					
					
					row+="</tr>";
					this.processed.push(row);
					this.firstrow = false;
					continue;
				}
				if (curline=="[[sample]]") {
						this.processed.push("<div class='sample'>");
						continue;
					}
				
					if (curline=="[[/sample]]") {
						this.processed.push("</div>");
						continue;
					}
					
				
				if (curline=="----"){
					this.processed.push("<hr/>");
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
				var str = this.processed[i].replace(/__BLOCK__/g,'[[');
				this.html+=str;
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
qwiki.toTOC = function(text) {
	var processor = this.createTOC(text);
	processor.process();
	var result = processor.json;
	delete(processor);
	return result;

}

})();