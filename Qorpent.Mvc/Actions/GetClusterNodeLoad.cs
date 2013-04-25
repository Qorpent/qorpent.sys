namespace Qorpent.Mvc.Actions {
    /// <summary>
    ///     Возвращает информацию о текущей нагрузке сервера
    /// </summary>
    [Action("_sys.nodeload")]
    public class GetClusterNodeLoad : ActionBase {
        /// <summary>
        /// main process
        /// </summary>
        /// <returns></returns>
        protected override object MainProcess() {
            return ServiceStateBuilder.Build();
        }
    }
}
