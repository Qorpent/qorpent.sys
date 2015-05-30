using System;
using System.Linq;
using System.Resources;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace qorpent.v2.security.authentication
{
    public class Token {
        public string User { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expire { get; set; }
        public string ImUser { get; set; }
        public string Metrics { get; set; }
    }
}
