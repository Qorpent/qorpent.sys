namespace Qorpent.BSharp {
    /// <summary>
    /// Расширение компилятора BSharp
    /// </summary>
    public interface IBSharpCompilerExtension {
        /// <summary>
        /// Выполняет логику расширения
        /// </summary>
        /// <param name="compiler"></param>
        /// <param name="context"></param>
        /// <param name="cls"></param>
        /// <param name="phase"></param>
        void Execute(IBSharpCompiler compiler, IBSharpContext context, IBSharpClass cls, BSharpCompilePhase phase);
    }
}