﻿namespace Qorpent.Graphs
{
    /// <summary>
    /// Интерфейс класса, пригодного для конвертации в графического представление графа
    /// </summary>
    public interface IGraphConvertible {
        /// <summary>
        /// Возвращает скрипт графа на целевом языке
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        string GenerateGraphScript(GraphOptions parameters = null);

        /// <summary>
        /// Получить класс построителя
        /// </summary>
        /// <returns></returns>
        IGraphBuilder GetBuilder();
    }
}
