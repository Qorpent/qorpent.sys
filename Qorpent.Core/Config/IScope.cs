﻿using System.Collections.Generic;

namespace Qorpent.Config {
    /// <summary>
    ///     Интерфейс Scope коллекции
    /// </summary>
    public interface IScope : IDictionary<string, object> {
        object this[string key, ScopeOptions options] { get; }
        ScopeOptions Options { get; set; }
        bool UseInheritance { get; set; }

        /// <summary>
        ///     Устанваливает локальное значение скоупа
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Set(string key, object value);

        /// <summary>
        ///     Получает значение с указанием параметров обхода
        /// </summary>
        /// <param name="key"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        object Get(string key, ScopeOptions options = default (ScopeOptions));

        /// <summary>
        ///     Проверяет наличие ключа
        /// </summary>
        /// <param name="key"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        bool ContainsKey(string key, ScopeOptions options);

        /// <summary>
        ///     Возвращает все доступные ключи
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        IEnumerable<string> GetKeys(ScopeOptions options = default(ScopeOptions));

        /// <summary>
        ///     Добавляет родительский скоуп
        /// </summary>
        /// <param name="parent"></param>
        void AddParent(IScope parent);

        /// <summary>
        ///     Удаляет родительский скоуп
        /// </summary>
        /// <param name="parent"></param>
        void RemoveParent(IScope parent);

        /// <summary>
        ///     Возвращает все родительские скоупы
        /// </summary>
        /// <returns></returns>
        IEnumerable<IScope> GetParents();

        /// <summary>
        ///     Зачищает все родительские скоупы
        /// </summary>
        void ClearParents();

        /// <summary>
        ///     Изолирует скоуп от родительских с копированием всех параметров
        /// </summary>
        /// <returns></returns>
        void Stornate();
        /// <summary>
        /// Типизированный вариант возврата со значением по умолчанию
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        T Get<T>(string key, T def = default(T), ScopeOptions options = null);

        /// <summary>
        ///     Устанавливает родительский конфиг
        /// </summary>
        /// <param name="config"></param>
        void SetParent(IScope config);

        /// <summary>
        ///     Получение родителя
        /// </summary>
        /// <returns></returns>
        IScope GetParent();
    }
}