using System;

namespace Qorpent.IoC {
    /// <summary>
    /// Описывает фабрику по умолчанию, в качестве страховки контейнера (#QPT-80)
    /// </summary>
    public interface IFactory {
	    /// <summary>
	    /// Реализует фабричную логику для указанного типа, тип возвращаемого значения должен быть верным
	    /// </summary>
	    /// <param name="serviceType"></param>
	    /// <param name="name"></param>
	    /// <returns></returns>
	    object Get(Type serviceType,string name = "");
    }
}