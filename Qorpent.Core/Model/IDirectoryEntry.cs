using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Model
{
    /// <summary>
    /// d
    /// </summary>
    public interface IDirectoryEntry
    {
        /// <summary>
        /// ID
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Имя папки или файла
        /// </summary>
        string ObjName { get; set; }
        /// <summary>
        /// Тайп (папка или файл)
        /// </summary>
        string ObjType { get; set; }
    }
}
