using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Mvc.Loader
{
    /// <summary>
    /// Уровень загружаемого контента по доступу
    /// </summary>
    public enum LoadLevel
    {
        /// <summary>
        /// Доступен всем
        /// </summary>
        Guest,
        /// <summary>
        /// Доступен авторизованным
        /// </summary>
        Auth,
        /// <summary>
        /// Доступен админам
        /// </summary>
        Admin
    }
}
