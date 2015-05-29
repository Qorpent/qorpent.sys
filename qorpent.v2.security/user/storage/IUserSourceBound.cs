using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qorpent.v2.security.user.storage
{
    public interface IUserSourceBound
    {
        IUserSource UserSource { get; set; }
    }
}
