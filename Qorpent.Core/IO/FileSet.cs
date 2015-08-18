using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Bxl;

namespace Qorpent.IO {
    public class FileSet {
        public FileSet() {
            
        }

        public FileSet(string root, params string[] masks) {
            this.Directories = new[] {root};
            this.Masks = masks;
            

        }
        public string[] Directories { get; set; }
        public string[] Masks { get; set; }
        public string[] Excludes { get; set; }
        public string[] Includes { get; set; }
        public bool Recursive { get; set; }
 

        protected bool Exclusive
        {
            get { return null != Includes && 0 != Includes.Length; }
        }

        protected bool Filtered
        {
            get { return null != Excludes && 0 != Excludes.Length; }
        }
        public IEnumerable<string> Collect() {
            Normalize();
            return 
                from directory in Directories
                from mask in Masks
                let opts = Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
                from file in Directory.GetFiles(directory,mask,opts)
                where IsMatch(file) select file;
        }

        public IEnumerable<XElement> CollectBxl(BxlParserOptions options = BxlParserOptions.None) {
            var bxl = new BxlParser();
            return 
                from file in Collect()
                let text = File.ReadAllText(file)
                select bxl.Parse(text, file, options);
        }

        private bool IsMatch(string file) {
            if (Exclusive) {
                if (!Includes.Any(include => Regex.IsMatch(file, include))) return false;

            }
            if (Filtered) {
                return Excludes.All(exclude => !Regex.IsMatch(file, exclude));
            }
            return true;
        }

        private void Normalize() {
            if (this.Masks == null || 0 == this.Masks.Length)
            {
                this.Masks = new[] { "*.*" };
            }
            if (this.Directories == null || 0 == this.Directories.Length) {
                this.Directories = new[] {Environment.CurrentDirectory};
            }
        }
    }
}