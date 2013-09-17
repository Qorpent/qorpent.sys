namespace Qorpent.Mvc.Actions {
    /// <summary>
    ///     Возвращает информацию о текущей нагрузке сервера
    /// </summary>
    [Action("_sys.nodeload",Role="DEFAULT")]
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
