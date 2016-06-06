using System;

namespace Qorpent.Utils.Extensions {
    [Flags]
    public enum SimplifyOptions {
        None = 0,
        Trim = 1,
        LfOnly = 2,
        NoNewLines = 4,
        NoInlineWs = 8,
       
        NoWs = NoNewLines | NoInlineWs,
        SingleQuotes = 16,
        LowerCase = 32,
        Default = Trim | LfOnly ,

        NoUndescores = 64,
        NoDashes = 128,

        Full = Trim | NoWs | SingleQuotes | LowerCase | NoUndescores | NoDashes ,
        Test = LfOnly | SingleQuotes,
       
        
    }
}