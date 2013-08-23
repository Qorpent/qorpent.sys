using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qorpent.IoC;
using System;

namespace Qorpent.Mvc.Actions
{
   
   /// <summary>
   /// f
   /// </summary>
   public class DirectoryObjEntry
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Имя папки или файла
        /// </summary>

        public string ObjName { get; set; }
        /// <summary>
        /// Тайп (папка или файл)
        /// </summary>
        public string ObjType { get; set; }
    }

    /// <summary>
    /// Методы
    /// </summary>
    public class MetodsCollect
    {
        /// <summary>
        /// Метод вовращает список дирректориев
        /// </summary>
        /// <returns></returns>
        public static DirectoryInfo[] ListDir()
        {
            var di = new DirectoryInfo(EnvironmentInfo.RootDirectory);
            return di.GetDirectories();
        }

        /// <summary>
        /// Список Файлов
        /// </summary>
        /// <returns></returns>
        public static string[] ListFile()
        {
            return Directory.GetFiles(EnvironmentInfo.RootDirectory);
        }

        /// <summary>
        /// Добавляетв в коллекцию
        /// </summary>
        /// <returns></returns>
        public static List<DirectoryObjEntry> AddListFilesAndDirs()
        {
            var listFilesCollection = new List<DirectoryObjEntry>();
            for (int index = 0; index < ListDir().Count(); index++)
            {
                listFilesCollection.Add(new DirectoryObjEntry() { ID = index + 1, ObjName = ListDir()[index].ToString(), ObjType = "Dir" });
            }
            for (var index = 0; index < ListFile().Count(); index++)
            {
                listFilesCollection.Add(new DirectoryObjEntry() { ID = index + 1 + ListDir().Count(), ObjName = ListFile()[index], ObjType = "File" });
            }
            return listFilesCollection;
        }
    }


    /// <summary>
    /// 	Возвращает текущий откомпилированный манифест (полный XML)
    /// </summary>
    [Action("_sys.listfiles", Role = "DEVELOPER", Help = "Возвращает список всех файлов и папок, где находися приложение", Arm = "admin")]
    public class ListFilesAction : ActionBase
    {
        /// <summary>
        /// 	fh
        /// </summary>
        protected override object MainProcess()
        {
            return MetodsCollect.AddListFilesAndDirs();
        }
    }
}