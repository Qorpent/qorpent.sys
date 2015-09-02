using System.Xml.Linq;
using Qorpent.BSharp;

namespace Qorpent.Config {
    public class GenericConfiguration : IConfigProvider {
        private readonly XElement _config;
        private readonly IBSharpContext _context;

        public GenericConfiguration(XElement config, IBSharpContext context) {
            _config = config;
            _context = context;
        }
        public XElement GetConfig() {
            return _config;
        }

        public IBSharpContext GetContext() {
            return _context;
        }
        public object Custom { get; set; }
    }
}