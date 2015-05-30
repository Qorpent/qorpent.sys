using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Qorpent.Experiments;
using Qorpent.IO.Http;

namespace qorpent.v2.security.authentication
{
    public interface IHttpAuthenticator {
       
        void Authenticate(IHttpRequestDescriptor request, IHttpResponseDescriptor response);
    }
}
