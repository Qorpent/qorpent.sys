using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qorpent.IoC;
using System;
using Qorpent.Mvc.Binding;

namespace Qorpent.Mvc.Actions
{

    /// <summary>
    /// f
    /// </summary>
    public class DirectoryObjEntry
    {
        ///// <summary>
        ///// ID
        ///// </summary>
        //public int ID { get; set; }

        /// <summary>
        /// Имя папки или файла 
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// полное имя папки
        /// </summary>
        public string FullPath { get; set; }

        

        
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
        //public static string[] ListFile(string fMask)
            public static string[] ListFile(string filMask)
        {
            return filMask != null ? Directory.GetFiles(EnvironmentInfo.RootDirectory, filMask) : Directory.GetFiles(EnvironmentInfo.RootDirectory);
        }

            /// <summary>
            /// Локальное имя файла
            /// </summary>
            /// <returns></returns>
            public static string[] ListFileLocalName(string fileMask)
            {
                var localNameFile = new string[ListFile(fileMask).Count()];
                for (int index = 0; index < ListFile(fileMask).Count(); index++)
                {
                    localNameFile[index] = Path.GetFileName(ListFile(fileMask)[index]);
                }
                return localNameFile;


            }

        /// <summary>
        /// Добавляетв в коллекцию
        /// </summary>
        /// <returns></returns>
        public static List<DirectoryObjEntry> AddListFilesAndDirs(string fMask, bool sDir, bool sFile)
        {

            var listFilesCollection = new List<DirectoryObjEntry>();
           
            
            if (sDir)
            {
                for (int index = 0; index < ListDir().Count(); index++)
                {
                    listFilesCollection.Add(new DirectoryObjEntry() { /*ID = index + 1,*/ LocalPath = ListDir()[index].ToString(), FullPath = Path.GetFullPath(EnvironmentInfo.RootDirectory) + ListDir()[index].ToString(), ObjType = "Dir" });
                }
            }

            if (sFile)
            {
                for (var index = 0; index < ListFile(fMask).Count(); index++)
                {
                    listFilesCollection.Add(new DirectoryObjEntry() { /*ID = index + 1 + ListDir().Count(), */LocalPath = ListFileLocalName(fMask)[index], FullPath = ListFile(fMask)[index], ObjType = "File" });
                }
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
        /// Имя скрипта
        /// </summary>
        [Bind]
        protected string FileMask;

        /// <summary>
        /// Показывать папки
        /// </summary>
        [Bind]
        protected bool ShowDirs;
        
        /// <summary>
        /// Показывать файлы
        /// </summary>
        [Bind]
        protected bool ShowFiles;

        /// <summary>
        /// 	fh
        /// </summary>
        protected override object MainProcess()
        {
            //if (ShowDirs)
            //{
                //return MetodsCollect.AddListFilesAndDirs(FileMask, ShowDirs, ShowFiles);
            //}

            return MetodsCollect.AddListFilesAndDirs(FileMask, ShowDirs, ShowFiles);
        }

      

    }
}