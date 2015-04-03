namespace Qorpent.Data {
    public static class DbCommandExecutorExtesions {
        public static object GetResultSync(this IDbCommandExecutor executor, DbCommandWrapper command) {
            executor.Execute(command).Wait();
            if (command.Ok) {
                return command.Result;
            }
            throw command.Error;
        }
    }
}