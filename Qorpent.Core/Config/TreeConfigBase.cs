using System.Collections.Generic;

namespace Qorpent.Config {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeConfigBase<T> : ConfigBase where T : TreeConfigBase<T> {
        /// <summary>
        /// 
        /// </summary>
        public const string ParentElementName = "__parent_element";
        /// <summary>
        /// 
        /// </summary>
        public const string ChildrenElementName = "__children_elements";
        /// <summary>
        /// 
        /// </summary>
        public T ParentElement {
            get { return Get<T>(ParentElementName); }
            set {
                Set(ParentElementName,value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public IList<T> ChildrenElements {
            get {
                var result = Get<IList<T>>(ChildrenElementName);
                if (null == result) {
                    Set(ChildrenElementName,result=new TreeConfigBaseElementsList<T>((T)this));
                }
                return result;
            }
        } 


    }
}