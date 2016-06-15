using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.Graphs.Dot.Builder
{
    public class DTable:XElement
    {

        public DTable(params object[] content):base("TABLE",(object[])content)
        {
        
        }


        public int Border
        {
            get { return this.Attr("BORDER","1").ToInt(); }
            set { this.SetAttributeValue("BORDER", value); }
        }
        public int CellBorder
        {
            get { return this.Attr("CELLBORDER").ToInt(); }    
            set { this.SetAttributeValue("CELLBORDER", value); }    
        }

        public int CellSpacing
        {
            get { return this.Attr("CELLSPACING","2").ToInt(); }
            set { this.SetAttributeValue("CELLSPACING", value); }
        }
        public int CellPadding
        {
            get { return this.Attr("CELLPADDING", "2").ToInt(); }
            set { this.SetAttributeValue("CELLPADDING", value); }
        }
    }
}
