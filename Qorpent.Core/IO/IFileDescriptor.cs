<<<<<<< HEAD:Qorpent.IO/IFileEntity.cs
﻿using System;

namespace Qorpent.IO {
    /// <summary>
    ///     Внутреннее представление файла
    /// </summary>
    public interface IFileEntity {
        /// <summary>
        ///     полный путь до файла
        /// </summary>
        string Path { get; }
        /// <summary>
        ///     Дата публикации файла в хранилище
        /// </summary>
        DateTime DateTime { get;}
        /// <summary>
        ///     Версия файла
        /// </summary>
        string Version { get; }
        /// <summary>
        ///     владелец файла
        /// </summary>
        string Owner { get; set; }
    }
=======
﻿using System;

namespace Qorpent.IO {
    /// <summary>
    ///     Внутреннее представление файла
    /// </summary>
    public interface IFileDescriptor {
        /// <summary>
        ///     полный путь до файла
        /// </summary>
        string Path { get; }
        /// <summary>
        ///     Дата публикации файла в хранилище
        /// </summary>
        DateTime DateTime { get;}
        /// <summary>
        ///     Версия файла
        /// </summary>
        string Version { get; }
        /// <summary>
        ///     владелец файла
        /// </summary>
        string Owner { get; set; }
    }
>>>>>>> origin/master:Qorpent.Core/IO/IFileDescriptor.cs
}