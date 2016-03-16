using System;

namespace Qorpent.Utils.LogicalExpressions {
    /// <summary>
    /// 	tokens, available in logical expression
    /// </summary>
    [Flags]
    public enum LETokenType {
        /// <summary>
        /// 	undefined
        /// </summary>
        None = 0,

        /// <summary>
        /// 	open brace (block of nodes)
        /// </summary>
        OpenBlock =1 ,

        /// <summary>
        /// 	close breace (end of block)
        /// </summary>
        CloseBlock = 1<<1,

        /// <summary>
        /// 	term reference
        /// </summary>
        Literal = 1 << 2,

        /// <summary>
        /// 	Equal operator ==
        /// </summary>
        Eq = 1 << 3,
        Greater = 1 << 4,
        Lower = 1 << 5,

        /// <summary>
        /// 	Not equal operator !=
        /// </summary>
        Neq = 1 << 6,

        /// <summary>
        /// 	Negate operator ![LITERAL|BLOCK]
        /// </summary>
        Not = 1 << 7, 

        /// <summary>
        /// 	And operator - &amp;
        /// </summary>
        And = 1 << 8,

        /// <summary>
        /// 	Or operator - |
        /// </summary>
        Or = 1 << 9,

        /// <summary>
        /// 	Str const "..."
        /// </summary>
        String = 1 << 10,
        /// <summary>
        /// Number const
        /// </summary>
        Number = 1 << 11,

        /// <summary>
        /// 	Block of nodes
        /// </summary>
        Block = 1 << 12,
        GreaterOrEq = 1 << 13,
        LowerOrEq = 1 << 14,
        Regex = 1 << 15,
        Compare = Eq | Neq | Greater | Lower | GreaterOrEq | LowerOrEq | Regex,
        
    }
}