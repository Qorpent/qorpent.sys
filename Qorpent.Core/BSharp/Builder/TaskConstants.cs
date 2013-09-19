namespace Qorpent.Integration.BSharp.Builder.Tasks {
    /// <summary>
    /// Константы, относящиеся к задачам
    /// </summary>
    public static class TaskConstants {
        /// <summary>
        /// Индекс данной задачи
        /// </summary>
        public const int LoadAllSourcesTaskIndex = 0;
        /// <summary>
        /// Индекс данной задачи
        /// </summary>
        public const int ResolveClassesAndNamespacesTaskIndex = LoadAllSourcesTaskIndex + INDEX_STEP;
        /// <summary>
        /// Индекс данной задачи
        /// </summary>
        public const int CompileBSharpTaskIndex = ResolveClassesAndNamespacesTaskIndex + INDEX_STEP;
        /// <summary>
        /// Индекс данной задачи
        /// </summary>
        public const int CleanUpTaskIndex = CompileBSharpTaskIndex + INDEX_STEP;
        /// <summary>
        /// Индекс данной задачи
        /// </summary>
        public const int WriteWorkingOutputTaskIndex = CleanUpTaskIndex + INDEX_STEP;
        /// <summary>
        /// Индекс данной задачи
        /// </summary>
        public const int GenerateIndexTaskIndex = WriteWorkingOutputTaskIndex + INDEX_STEP;
        /// <summary>
        /// Индекс данной задачи
        /// </summary>
        public const int GenerateSrcPackageTaskIndex = GenerateIndexTaskIndex + INDEX_STEP;
        /// <summary>
        /// Индекс данной задачи
        /// </summary>
        public const int GenerateClassGraphTaskIndex = GenerateSrcPackageTaskIndex + INDEX_STEP;

        /// <summary>
        /// Индекс данной задачи
        /// </summary>
        public const int WriteOrphansOutputTaskIndex = GenerateClassGraphTaskIndex + INDEX_STEP;



        /// <summary>
        /// Индекс данной задачи
        /// </summary>
        public const int WriteErrorInfoTaskIndex = WriteOrphansOutputTaskIndex + INDEX_STEP;
        /// <summary>
        /// Шаг прироста индекса задач
        /// </summary>
        public const int INDEX_STEP = 100;

       

        

        

       

       
        
    }
}