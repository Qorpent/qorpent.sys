using System.IO;
using System.Linq;
using Qorpent.IoC;
using System;

namespace Qorpent.Mvc.Actions
{
    internal abstract class CodeBase
    {
        public static string[] ListFilesString()
        {
            var listFilesStr = Directory.GetFiles(Environment.CurrentDirectory);
            return listFilesStr;
        }

        public static string ListDIrectrory()
        {
            var di = new DirectoryInfo(Environment.CurrentDirectory);
            var subDirs = di.GetDirectories();
            Console.WriteLine(Environment.CurrentDirectory);
            if (subDirs.Length > 0)
            {
                return subDirs.ToString();
            }
            else
            {
                Console.WriteLine("Пусто");
                return null;
            }
        }

    }


    /// <summary>
    /// 	Возвращает текущий откомпилированный манифест (полный XML)
    /// </summary>
    [Action("_sys.listfiles", Role = "DEVELOPER", Help = "!!!!Возвращает список всех файлов и папок, где находися приложение", Arm = "admin")]
    public class ListFilesAction : ActionBase
    {

        /// <summary>
        /// 	fh
        /// </summary>
        protected override object MainProcess()
        {

            //String dir1 = Environment.CurrentDirectory;//получаем текущую рабочую папку приложения
            //String dir2 = Application.StartupPath;//получаем папку из которой произошел запуск приложения

            return CodeBase.ListFilesString();
           // return ;
            //CodeBase.ListDIrectrory();


        }
    }
}