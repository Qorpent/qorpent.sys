using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qorpent.IoC;
using System;

namespace Qorpent.Mvc.Actions
{
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
            var dirs = ListDirectrory();
            var files = ListFiles();

            var result = new List<string>();

            result.AddRange(dirs);
            result.AddRange(files);

            return result;
        }

        private IEnumerable<string> ListDirectrory()
        {
            var di = new DirectoryInfo(Environment.CurrentDirectory);
            if (null == di)
            {
                throw new Exception("Cannot get directory info");
            }

            return di.GetDirectories().Select(directoryInfo => directoryInfo.Name).ToList();
        }


        private IEnumerable<string> ListFiles()
        {
            
        }
    }
}