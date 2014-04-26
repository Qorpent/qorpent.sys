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

qwiki.beforePreprocess = null;
qwiki.afterPreprocess = null;
qwiki.preprocess = function(processor){
	processor.text = processor.text || "";
	var preprocessedText  = processor.text ;
	if(this.beforePreprocess){
		preprocessedText = this.beforePreprocess(processor,preprocessedText);
	}
	var preprocessedText = '\n'+processor.text.trim()+'\n';
	preprocessedText = preprocessedText.replace(/\r/g,'');
	// firstly we must meet 1-st feature - we unify and process line delimiters
	preprocessedText = preprocessedText.replace(/\n\n+/g,"\n\n[BR]\n\n");
	preprocessedText = preprocessedText.replace(/\n([\=\%\!\-\|№])([^\n]+)/g,'\n\n$1$2\n\n');
	// then we must split lines for block elements
	preprocessedText = preprocessedText.replace(/(\[\[\/?\w+([^\]]*)\]\])/g,"\n\n\n\n$1\n\n\n\n");
	// and finally remove ambigous lines
	preprocessedText = preprocessedText.replace(/\n\n+/g,"__LINER__");
	preprocessedText = preprocessedText.replace(/\n/g,"&nbsp;");
	preprocessedText = preprocessedText.replace(/__LINER__/g,"\n");
	if(this.afterPreprocess){
		preprocessedText = this.afterPreprocess(processor,preprocessedText);
	}
	return preprocessedText;
};
qwiki.beforeInline = null;
qwiki.afterInline = null;
qwiki.processInline = function(processor, curline){
	if(this.beforeInline){
		curline = this.beforeInline(processor,curline);
	}
	//LETER STYLE SUPPORT
	//bold
	curline = curline.replace(/\*\*\*([\s\S]+?)\*\*\*/g,'<strong>$1</strong>');
	//italic
	curline = curline.replace(/\*\*([\s\S]+?)\*\*/g,'<em>$1</em>');
	//underline
	curline = curline.replace(/__([\s\S]+?)__/g,'<ins>$1</ins>');
	//strikeout
	curline = curline.replace(/--([\s\S]+?)--/g,'<del>$1</del>');
	//subtext new version
	curline = curline.replace(/,,([\s\S]+?),,/g,'<sub>$1</sub>');
	//supertext
	curline = curline.replace(/::([\s\S]+?)::/g,'<sup>$1</sup>');
	//custom style
	curline = curline.replace(/\{style:([\s\S]+?)\}([\s\S]+?)\{style\}/g,'<span style="$1">$2</span>');
	if(this.afterInline){
		curline = this.afterInline(processor,curline);
	}
	return curline;
};
qwiki.beforeLine = null;
qwiki.afterLine = null;
qwiki.processLine =  function(processor,curline){
	if(this.beforeLine){
		curline = this.beforeLine(processor,curline);
	}
    curline = curline.trim();
	//HEADERS SUPPORT
	if (curline.match(/^======/)){
		curline = "<h6 id='h_"+ (processor.idcount++) +"'>" + curline.substring(6) + "</h6>";
	}else if (curline.match(/^=====/)){
		curline = "<h5 id='h_"+ (processor.idcount++) +"'>" + curline.substring(5) + "</h5>";
	}else if (curline.match(/^====/)){
		curline = "<h4 id='h_"+ (processor.idcount++) +"'>" + curline.substring(4) + "</h4>";
	}else if (curline.match(/^===/)){
		curline = "<h3 id='h_"+ (processor.idcount++) +"'>" + curline.substring(3) + "</h3>";
	}else if (curline.match(/^==/)){
		curline = "<h2 id='h_"+ (processor.idcount++) +"'>" + curline.substring(2) + "</h2>";
	}else if (curline.match(/^=/)){
		curline = "<h1 id='h_"+ (processor.idcount++) +"'>" + curline.substring(1) + "</h1>";
	}

	//LIST SUPPORT
	else if (curline.match(/^%%%%%%/)){
		curline = "<div class='wiki-list wiki-list-6'>" + curline.substring(6) + "</div>";
	}else if (curline.match(/^%%%%%/)){
		curline = "<div class='wiki-list wiki-list-5'>" + curline.substring(5) + "</div>";
	}else if (curline.match(/^%%%%/)){
		curline = "<div class='wiki-list wiki-list-4'>" + curline.substring(4) + "</div>";
	}else if (curline.match(/^%%%/)){
		curline = "<div class='wiki-list wiki-list-3'>" + curline.substring(3) + "</div>";
	}else if (curline.match(/^%%/)){
		curline = "<div class='wiki-list wiki-list-2'>" + curline.substring(2) + "</div>";
	}else if (curline.match(/^%/)){
		curline = "<div class='wiki-list wiki-list-1'>" + curline.substring(1) + "</div>";
	}
	
	else if (curline.match(/^№№№№№№/)){
		curline = "<div class='wiki-list wiki-list-6 number'>" + curline.substring(6) + "</div>";
	}else if (curline.match(/^№№№№№/)){
		curline = "<div class='wiki-list wiki-list-5 number'>" + curline.substring(5) + "</div>";
	}else if (curline.match(/^№№№№/)){
		curline = "<div class='wiki-list wiki-list-4 number'>" + curline.substring(4) + "</div>";
	}else if (curline.match(/^№№№/)){
		curline = "<div class='wiki-list wiki-list-3 number'>" + curline.substring(3) + "</div>";
	}else if (curline.match(/^№№/)){
		curline = "<div class='wiki-list wiki-list-2 number'>" + curline.substring(2) + "</div>";
	}else if (curline.match(/^№/)){
		curline = "<div class='wiki-list wiki-list-1 number'>" + curline.substring(1) + "</div>";
	}
    else if (curline.match(/^\//)) {
        curline = "<div class='wiki-outline'>" + curline.substring(1) + "</div>";
	}
	
	
	else{
		curline = "<p>"+curline+"</p>";
	}
	if(this.afterLine){
		curline = this.afterLine(processor,curline);
	}
	return curline;
};
qwiki.beforeCode = null;
qwiki.afterCode = null;
qwiki.processCode = function (processor, curline) {
 
	if(this.beforeCode){
		curline = this.beforeCode(processor,curline);
	}
    
	curline = curline.replace(/\&nbsp;/g,' __BR__ ');
	curline = curline.replace(/\[BR\]/g,' __BR__ ');
	curline = curline.replace(/\x20{4}/g,' __TAB__ ');
	curline = curline.replace(/\t/g, ' __TAB__ ');
	curline = curline.replace(/</g, '__LT__');
	curline = curline.replace(/&lt;/g, '__LT__');
	curline = curline.replace(/&gt;/g, '__GT__');
	curline = curline.replace(/&amp;/g, '__AMP__');
	//CODE BLOCKS
	curline = curline.replace(
		/([!=+\-*\.\\\/;<>%\&\^\:\|]+)/g,
		"<span _CLASS_ATTR_'operator'>$1</span>");
	curline = curline.replace(/\/\*/g,"<span _CLASS_ATTR_'comment'>");
	curline = curline.replace(/\*\//g,"</span>");
	curline = curline.replace(/(\#[^"']+)$/g, "<span _CLASS_ATTR_'comment'>$1</span>");
	curline = curline.replace(/(^\#[\s\S]+)(?=__BR__)/g, "<span _CLASS_ATTR_'comment'>$1</span>");
	curline = curline.replace(
		/\b((var)|(for)|(return)|(foreach)|(while)|(case)|(switch)|(in)|(out)|(include)|(embed)|(private)|(public)|(protected)|(void)|(function)|(class)|(namespace)|(using)|(select)|(where)|(group by)|(order by)|(null)|(true)|(false))\b/g,
		"<span _CLASS_ATTR_'keyword'>$1</span>");
	curline = curline.replace(
		/\b((int)|(string)|(DateTime)|(decimal)|(bool)|(nvarchar)|(datetime)|(bit)|(byte)|(float)|(long)|(bigint))\b/g,
		"<span _CLASS_ATTR_'type'>$1</span>");
	
	curline = curline.replace(
		/([\{\}\[\]\(\),]+)/g,
		"<span _CLASS_ATTR_'delimiter'>$1</span>");
	
	curline = curline.replace(/\\"/g,'_EQ_');
	curline = curline.replace(/""/g,'_DQ_');
	curline = curline.replace(/"([\s\S]+?)"/g, "<span _CLASS_ATTR_'string'>\"$1\"</span>");
	curline = curline.replace(/_EQ_/g,'\\"');
	curline = curline.replace(/_DQ_/g,'""');
	
	curline = curline.replace(/(\b-?\d+(\.\d+)?)/g,"<span _CLASS_ATTR_'number'>$1</span>");
	
	
	//curline+="<br/>";
	curline = curline.replace(/_CLASS_ATTR_/g,'class=');
	curline = curline.replace(/__TAB__/g,'&nbsp;&nbsp;&nbsp;&nbsp;');
	curline = curline.replace(/__BR__/g, '<br/>');
	curline = curline.replace(/&amp;lt;/g, '&lt;');
	curline = curline.replace(/&amp;gt;/g, '&gt;');
	curline = curline.replace(/__LT__/g, '&lt;');
	curline = curline.replace(/__GT__/g, '&gt;');
	curline = curline.replace(/__AMP__/g, '&amp;');
	
	curline = curline + '<br/>';
    
	if(this.afterCode){
		curline = this.afterCode(processor,curline);
	}
	return curline;
};
qwiki.referenceRegex = /\[([^\s\]]+?)(\s+([^|~\]]+?))?([\|~]([\s\S]+?))?\]/g;
qwiki.processReferences = function(processor,curline, callback, context){
	curline = curline.replace(qwiki.referenceRegex, function(match, addr, p2, name, p3, tail){
		return callback(processor,match, addr, p2, name, p3, tail,context);
	});
	return curline;
};
qwiki.defaultProcessReference = function(processor,match, addr, p2, name, p3, tail,context) {
			return qwiki.getReference(processor,(addr||"").trim(),(name||addr||"").trim(),(tail||"").trim());
};
qwiki.getCustomReference = null;
qwiki.getReference = function(processor,addr,name,tail){

	if(this.getCustomReference){
		var result = null;
		result = this.getCustomReference(processor,addr,name,tail);
		if(null!=result){
			return result;
		}
	}	
	
	if(addr.match(/\.((png)|(gif)|(jpg)|(jpeg))$/)){
		return "<img src='"+addr+"' title='"+name+"' "+tail+" />";
	}else if (addr.match(/^http/)) {
	    return "<a href='" + addr + "' " + tail + " >" + name + "</a>";
	}else if (addr.match(/^href:/)) {
	    return "<a href='" + addr.replace('href:','') + "' " + tail + " >" + name + "</a>";
	}
	else {
		return "<a href='/wiki?" + addr + "' "+tail+" >" + name + "</a>";
	}
	
};
qwiki.setup  = function(wikiprocessor){};
qwiki.create = function(text, logwriter){
	var result = {
		text : text,
		lines /* as string[] */: null,
		processed : [],
		html /* as string */: null,
		log : logwriter||function(sender,text){},
		
		
		
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
					//this.processed.push("<br/>");
					return;
				}
				
				curline = qwiki.processInline(this,curline);
				// references
				curline = qwiki.processReferences(this,curline, qwiki.defaultProcessReference, null);
				
				curline = qwiki.processLine(this,curline);
				
				return curline;
		},
		
		//state control
		idx : 0,
		idcount :1,
		curline : "",
		nowikiblock : false,
		codeblock : false,
		table : false,
		firstrow : false,
		idcount : 1,
		// main function
		process : function(){
			this.processed = [];
			this.html = "";
			this.processed.push("<div class='wiki'>");
			this.log(this,"start");
			var preprocessedText = qwiki.preprocess(this);
			this.log(this,"lines merged");
			// then we setup array of 
			this.lines = preprocessedText.split(/\r?\n/);
			this.log(this,"text splited");
			for (this.idx = 0; this.idx < this.lines.length; this.idx++){
				var curline = this.lines[this.idx];
				if (curline.match(/^\s*$/))continue;
				
				// CODE BLOCK SUPPORT
				if (curline=="[[/code]]") {
					this.codeblock = false;
					this.processed.push("</div>");
					continue;
				}
				if (curline=="[[code]]") {
				    this.codeblock = true;
				    this.lastcode = "";
					this.processed.push("<div class='wiki-code'>");
					continue;
				}
				
				if (this.codeblock) {
				    this.lastcode += curline.replace(/(&nbsp;)|(\[BR\])/g, "\n") + "\r\n";
					this.processed.push( qwiki.processCode(this, curline)) ;
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

			    //WIKI IGNORANCE SUPPORT WITH BLOCK AND INLINE
				if (curline == "[[/script]]") {
				    this.scriptblock = false;
				    var defaultScript = true;
				    if (qwiki.scriptHandler) {
				        var customscript = qwiki.scriptHandler(this,this.scripttype, this.script);
				        if (customscript) {
				            defaultScript = false;
				            this.processed.push(customscript);
				        }
				    }
				    if (defaultScript) {
				        this.processed.push("<script type='" + this.scripttype + "'>");
				        this.processed.push(this.script);
				        this.processed.push("</script>");
				    }
				    continue;
				}
				if (curline.match(/\[\[script\s+type=([\w/]+)\]\]/)) {
				    this.scriptblock = true;
				    this.scripttype = curline.match(/type=([\w/]+)/)[1];
				    this.script = "";
				    continue;
				}
				if (curline.match(/\[\[script-last\s+type=([\w/]+)\]\]/)) {
				    this.scripttype = curline.match(/type=([\w/]+)/)[1];
				    this.script = this.lastcode;
				    var defaultScript = true;
				    if (qwiki.scriptHandler) {
				        var customscript = qwiki.scriptHandler(this, this.scripttype, this.script);
				        if (customscript) {
				            defaultScript = false;
				            this.processed.push(customscript);
				        }
				    }
				    if (defaultScript) {
				        this.processed.push("<script type='" + this.scripttype + "'>");
				        this.processed.push(this.script);
				        this.processed.push("</script>");
				    }
				    continue;
				}
				if (this.scriptblock)
				{
				    this.script += curline.replace(/(&nbsp;)|(\[BR\])/g, "\n");
				    continue;
				}

			
				if (curline.match(/^\|/)) {
					if (!this.table) {
						this.processed.push("<table>");
						this.processed.push("<thead>");
						this.table = true;
						this.ishead = true;
					}
				}
				else if (curline=="[[row]]"){
					if (this.cellstart) {
						this.cellstart = false;
						this.processed.push("</td>");
					}
					if (this.rowstart) {
						this.processed.push("</tr>");
					}
					if(!this.table){
						this.processed.push("<table><tbody>");
						this.table = true;
					}
					this.rowstart = true;
					this.processed.push("<tr>");
					continue;
				}
				else if (curline=="[[cell]]"){
					if (this.cellstart) {
						this.processed.push("</td>");
					}
					if(!this.table){
						this.processed.push("<table><tbody><tr>");
						this.table = true;
						this.rowstart = true;
					}
					this.cellstart = true;
					this.processed.push("<td>");
					continue;
				}else if(curline=="[[/cell]]"){
					this.cellstart =  false;
					this.processed.push("</td>");
					continue;
				}
				else if(curline=="[[/row]]"){
					if(this.cellstart){
						this.processed.push("</td>");
					}
					this.cellstart =  false;
					this.rowstart =  false;
					this.processed.push("</tr>");
					continue;
				}
				else{
				
					if(this.table){
						if(!this.cellstart) {
							if (this.rowstart){
								this.processed.push("</tr>");
							}
							this.processed.push("</tbody>");
							this.processed.push("</table>");
							this.table = false;
							this.cellstart= false;
							this.rowstart =false;
						
						}
					}
				}
								
				if ( this.table &&  !this.cellstart ) {
					var tde = this.ishead ? "th" : "td" ;
					if(!this.ishead){
						if(curline.match(/^\|\{\+\}/)){
							curline = curline.replace(/^\|\{\+\}/,'|');
							this.ishead = true;
							suffix = "";
							tde = "th";
						}
					}
					var items = curline.split(/\|/);
					var row = "";
					if(!this.ishead){
						row+="</thead></tbody>";
					}
					this.ishead = false;
					
					row += "<tr>";
					for(var i = 0;i<items.length;i++){
						if(i==0||i==items.length-1)continue; //ignore left-right starters
						var cell = items[i].trim();
						cell = qwiki.processInline(this,cell);
						cell = qwiki.processReferences(this,cell, qwiki.defaultProcessReference, null);
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


qwiki.createTOC = function(text, logwriter){
	var result = {
		text : text,
		lines /* as string[] */: null,
		processed : [],
		html /* as string */: null,
		log : logwriter||function(sender,text){},

		processReference : function(processor,match, addr, p2, name, p3, tail,newitem) {
			var addr = (addr || "").trim();
			var title = (name || addr|| "").trim();
			var tail = (tail||"").trim();
			newitem.addr = addr;
			newitem.title = title;
			newitem.extension = tail;
		},
		
		processDefault : function( curline ) {
				if(this.nowiki)return;
				if(curline=="[[nowiki]]"){
					this.nowiki = true;
					return;
				}
				if(curline=="[[/nowiki]]"){
					this.nowiki = false;
					return;
				}
				if(curline.match(/^\!/))return;
				if(!curline.match(/^\=/))return;
				
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
				
				curline = qwiki.processInline(this,curline);
				
				var newitem = { level : level, raw : curline, items : []  };
				// references
				
				curline = qwiki.processReferences(this,curline, this.processReference, newitem);
				
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
		nowiki : false,
		process : function(){
			this.processed = [];
			this.json = {items:[]};
			this.log(this,"start");
			var preprocessedText = qwiki.preprocess(this);
			
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

    qwiki.toHTML = function(text) {
        var processor = this.create(text);
        processor.process();
        var result = processor.html;
        delete(processor);
        return result;
    };
    qwiki.toTOC = function(text) {
        var processor = this.createTOC(text);
        processor.process();
        var result = processor.json;
        delete(processor);
        return result;

    };
    var _id = 1;
    qwiki.scriptHandler = function(processor, type, script) {
        if (type == "bxl" || type=="bsharp") {
            _id++;
            var cid = _id;
            window.setTimeout(function() {
                $.ajax({ method: 'POST', url: 'toxml?lang='+type+'&format=wiki', data: script }).done(function(_) {
                    $('#dynwiki_' + cid).html(_);
                });
            }, 300);
            return "<div id='dynwiki_" + cid + "' class='dynwiki " + type + "'></div>";
        }
        return null;
    };

})();