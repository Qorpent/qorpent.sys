using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Qorpent.IoC;
using System;
using Qorpent.Mvc;
using Qorpent.Mvc.Actions;
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

       //// public string FullPath { get; set; }

        

        
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
            return di.GetDirectories("*.*",SearchOption.AllDirectories);
        }

        /// <summary>
        /// Список Файлов
        /// </summary>
        /// <returns></returns>
        //public static string[] ListFile(string fMask)
            public static string[] ListFile()
        {
            return Directory.GetFiles(EnvironmentInfo.RootDirectory,"*.*", SearchOption.AllDirectories);
        }

            /// <summary>
            /// Локальное имя файла
            /// </summary>
            /// <returns></returns>
            public static string[] ListFileLocalName()
            {
                var localNameFile = new string[ListFile().Count()];
                for (int index = 0; index < ListFile().Count(); index++)
                {
                    localNameFile[index] = Path.GetFileName(ListFile()[index]);
                }
                return localNameFile;


            }

        /// <summary>
        /// Добавляетв в коллекцию
        /// </summary>
        /// <returns></returns>
        public static List<DirectoryObjEntry> AddListFilesAndDirs( bool sDir, bool sFile)
        {

            var listFilesCollection = new List<DirectoryObjEntry>();
           
            
            if (sDir)
            {
                for (int index = 0; index < ListDir().Count(); index++)
                {
                    //listFilesCollection.Add(new DirectoryObjEntry() { /*ID = index + 1,*/ LocalPath = ListDir()[index].ToString(), FullPath = Path.GetFullPath(EnvironmentInfo.RootDirectory) + ListDir()[index].ToString(), ObjType = "Dir" });
                    listFilesCollection.Add(new DirectoryObjEntry() { /*ID = index + 1,*/ LocalPath = ListDir()[index].ToString(),  ObjType = "Dir" });
                }
            }
            if (sFile)
            {
                for (var index = 0; index < ListFile().Count(); index++)
                {
                    //listFilesCollection.Add(new DirectoryObjEntry() { /*ID = index + 1 + ListDir().Count(), */LocalPath = ListFileLocalName()[index], FullPath = ListFile()[index], ObjType = "File" });
                    listFilesCollection.Add(new DirectoryObjEntry() { /*ID = index + 1 + ListDir().Count(), */LocalPath = ListFileLocalName()[index], ObjType = "File" });
                }
            }
            return listFilesCollection;
        }


        //  /// <summary>
        ///// Проверка соответствия имени файла маске
        ///// </summary>
        ///// <param name="fileName">Имя проверяемого файла</param>
        ///// <param name="inputMasksMassS">Маска файла</param>
        ///// <param name="i"></param>
        ///// <returns>true - файл удовлетворяет маске, иначе false</returns>
        //static public bool CheckMask(string[] fileName, string inputMasksMassS, int i)
        //{
            
        //    //Console.WriteLine(counter);
        //    string pattern = string.Empty;
        //    foreach (string ext in OuputMasksMassive(inputMasksMassS))
        //        {
        //            if ((ext.IndexOf(".") > -1) || (ext.IndexOf("?") > -1) || ((ext.IndexOf("*") > -1)))
        //            {
        //                pattern += @"^"; //признак начала строки
        //                foreach (char symbol in ext)
        //                    switch (symbol)
        //                    {
        //                        case '.':
        //                            pattern += @"\.";
        //                            break;
        //                        case '?':
        //                            pattern += @".";
        //                            break;
        //                        case '*':
        //                            pattern += @".*";
        //                            break;
        //                        default:
        //                            pattern += symbol;
        //                            break;

        //                            // default: pattern += @".*(" + symbol + @").*"; break;
        //                            //^.*(sdg).*$
        //                    }
        //                pattern += @"$|"; //признак окончания строки
        //            }
        //            else
        //            {
        //                pattern += @"^.*(" + ext + @").*$|";
        //               // ^.*(sdg).*$
        //            }
        //            //Console.WriteLine(pattern);
        //       }
        //    if (pattern.Length == 0)
        //        {
        //            return false;
        //        }
        //        {
        //            pattern = pattern.Remove(pattern.Length - 1);
        //            var mask = new Regex(pattern, RegexOptions.IgnoreCase);
        //            return mask.IsMatch(Path.GetFileName(fileName[i]));
        //        }

        //    }

        //static public void CollectionAfterMask(string inputMasksMassS)

         /// <summary>
         /// Массив масок
         /// </summary>
         /// <param name="inputstring"></param>
         /// <returns></returns>
         public static string[] OuputMasksMassive(string inputstring)
        {
            if (inputstring != "")
            {
                string[] exts = inputstring.Split('|', ',', ';', ' ');
                int counter = 0;
                for (int index = 0; index < exts.Length; index++)
                {
                    string ext = exts[index];
                    if (VyvodPatterna().IsMatch(ext))
                    {
                        counter++;
                    }
                }
                var resultmassive = new string[exts.Length - counter];
                counter = 0;
                for (int index = 0; index < exts.Length; index++)
                {
                    //string ext = exts[index];
                    if (exts[index] != "")
                    {
                        resultmassive[counter] = exts[index];
                        counter++;
                    }

                }
                //Console.WriteLine(counter);
                return resultmassive;
            }
            else
            {
                return null;
            }
           
        }

         /// <summary>
         /// Паттерн, убирающий пробелы
         /// </summary>
         /// <returns></returns>
         public static Regex VyvodPatterna()
            {
                string pattern = @"^\s*$";
                var newReg = new Regex(pattern);
                return newReg;
            }
       

        
        /// <summary>
        /// Вывод коллекции после применения маски
        /// </summary>
        /// <param name="inputMasksMassS"></param>
        /// <param name="inputCollection"></param>
        /// <returns></returns>
        static public List<DirectoryObjEntry> CollectionAfterMask(string inputMasksMassS, List<DirectoryObjEntry> inputCollection )
                    {
                        if (inputMasksMassS != null)
                         {
                        var pattern = new string[OuputMasksMassive(inputMasksMassS).Count()];
                             for (int index = 0; index < OuputMasksMassive(inputMasksMassS).Length; index++)
                             {
                                 string ext = OuputMasksMassive(inputMasksMassS)[index];
                                 if ((ext.IndexOf(".") > -1) || (ext.IndexOf("?") > -1) || ((ext.IndexOf("*") > -1)))
                                 {
                                     pattern[index] += @"^"; //признак начала строки
                                     foreach (char symbol in ext)
                                         switch (symbol)
                                         {
                                             case '.':
                                                 pattern[index] += @"\.";
                                                 break;
                                             case '?':
                                                 pattern[index] += @".";
                                                 break;
                                             case '*':
                                                 pattern[index] += @".*";
                                                 break;
                                             default:
                                                 pattern[index] += symbol;
                                                 break;

                                                 // default: pattern += @".*(" + symbol + @").*"; break;
                                                 //^.*(sdg).*$
                                         }
                                     pattern[index] += @"$"; //признак окончания строки
                                 }
                                 else
                                 {
                                     pattern[index] += @"^.*(" + ext + @").*$";
                                     // ^.*(sdg).*$
                                 }
                             }
                             if (pattern.Length == 0)
                            {
                              return inputCollection;
                            }
                           // pattern = pattern.Remove(pattern.Length - 1);
                             var mask = new Regex[pattern.Length];
                            int limit = inputCollection.Count;
                            for (int index = 0; index < pattern.Length; index++)
                            {
                                //var s = pattern[index];
                                mask[index] = new Regex(pattern[index], RegexOptions.IgnoreCase);
                            }
                            var massivObjName = new string[limit];
                            for (int index = 0; index < limit; index++)
                            {
                                var myClass = inputCollection[index];
                                massivObjName[index] = myClass.LocalPath;
                            }
                            for (int index = limit - 1; index >= 0; index--)
                            {
                                for (int internalindex = 0; internalindex < pattern.Length; internalindex++)
                                {
                                    if (mask[internalindex].IsMatch(massivObjName[index]))
                                    {
                                        //Console.WriteLine("Совпадение в элеменете " + (index + 1));
                                    }
                                    else
                                    {
                                        //Console.WriteLine("Удаляем едемент с индексом " + (index + 1));
                                        inputCollection.RemoveAt(index);
                                        break;
                                    }
                                }
                             }
                             return inputCollection;
                        }
                        return inputCollection;
                    }
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
            

            return MetodsCollect.CollectionAfterMask(FileMask, MetodsCollect.AddListFilesAndDirs(ShowDirs,ShowFiles));
        }

      

    }
