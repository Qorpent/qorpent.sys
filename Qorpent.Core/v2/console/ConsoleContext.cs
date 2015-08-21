using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent;
using Qorpent.Bxl;
using Qorpent.Config;
using Qorpent.IoC;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.console {
    public class ConsoleContext : IConsoleContext {

        public ConsoleContext() {
              
        }
        public IConsoleContext Parent { get; set; }
        private bool _initialized = true;

        public ConsoleContext(ConsoleCallInfo info) {
            this.Info = info;
            _initialized = false;
        }

        public IConsoleContext GetProxy(Func<IConsoleContext,IConsoleContext> setup = null) {
            var result = (IConsoleContext)new ConsoleContext {Parent = this};
            if (null != setup) {
                result = setup(result);
            }
            return result;
        }

        public void Initialize() {
            if (_initialized) return;
            Info.Normalize();
            if (null == this.Parameters) {
                InitializeParameters();
            }
            this.Container = this.Container ?? Info.Container;
            if (null == Container) {
                InitializeContainer();
            }
            _initialized = true;
        }



        private void InitializeContainer() {
            var container = Qorpent.IoC.ContainerFactory.CreateDefault();
            var loader = container.GetLoader();
            if (null != Info.Libraries) {
                foreach (var library in Info.Libraries) {
                    loader.LoadAssembly(Assembly.Load(library));
                }
            }
            if (null != Parameters && null != Parameters.Definition) {
                container.Set<IConfigProvider>(new GenericConfiguration(
                    Parameters.Definition,
                    Parameters.BSharpContext
                    ));
                var libs = Parameters.Definition.Elements("lib");
                foreach (var element in libs) {
                    loader.LoadAssembly(Assembly.Load(element.Attr("code")));
                }
            }
        }

        private void InitializeParameters() {
            var args = new ConsoleApplicationParameters {
                RepositoryPath = Info.RepositoryPath,
                WorkingDirectory = Info.CurrentDirectory,
                ConfigSet = Info.ConfigSet,
            };
            args.Initialize(Info.Arguments);
            Parameters = args;
        }


        public IScope Scope
        {
            get { return _scope ?? (_scope= GetScope()); }
            set { _scope = value; }
        }

        private IScope GetScope() {
            if (null == Parent) return new Scope();
            return new Scope(Parent.Scope);
        }

        public ConsoleCallInfo Info
        {
            get { return _info ?? (Parent==null?null:Parent.Info); }
            set { _info = value; }
        }

        public async Task<ConsoleCommandResult> Execute(string commandname, string commandstring=null, IScope scope = null) {
        
            var command = GetCommand(commandname, commandstring, scope);
            if (null == command) {
                throw new Exception("cannot find command "+commandname );
            }
            var proxy = this.GetProxy();
            proxy.Scope.Set("commandname",commandname);
            proxy.Scope.Set("commandstring",commandstring); 
            return await command.Execute(proxy, commandname, commandstring, scope);
        }

        public XElement GetBxl() {
            var callcontext = Scope;
            var command = callcontext.Get("commandname", "");
            var commandstring = callcontext.Get("commandstring", "");
            var bxl = command + " " + commandstring;
            if (!string.IsNullOrWhiteSpace(bxl)) {
               return new BxlParser().Parse(bxl, "", BxlParserOptions.ExtractSingle);
            }
            return new XElement("stub");
        }

        public IConsoleCommand GetCommand(string commandname, string commandstring = null, IScope scope = null) {
            if (null != Parent) {
                return Parent.GetCommand(commandname, commandstring, scope);
            }
            if (null == Container) return null;
            var commands = Container.All2(typeof(IConsoleCommand),null).OfType<IConsoleCommand>().OrderByDescending(_=>_.Priority);
            var command = commands.FirstOrDefault(_ => _.IsMatch(commandname, commandstring,scope));
            return command;
        }

        private TextWriter _output;
        private TextWriter _error;
        private ConsoleApplicationParameters _parameters;
        private IContainer _container;
        private ConsoleCallInfo _info;
        private Stream _outStream;
        private IScope _scope;

        public Stream OutStream { get; set; }

        public TextWriter Out
        {
            get { return _output ?? (Parent != null ? Parent.Out : Console.Out) ; }
            set { _output = value; }
        }

        public TextWriter Error
        {
            get { return _error ?? (Parent != null ? Parent.Error : Console.Error) ; }
            set { _error = value; }
        }

        public event Func<string, string> OnReadLine;
        public event Func<ConsoleKeyInfo> OnReadKey;
        public event Action<ConsoleColor> OnSetColor;
        public event Action OnRestoreColor;
        public event Action<string, bool> OnWrite;

        public string ReadLine(string message=null) {
            if (null == OnReadLine) {
                
                if (!string.IsNullOrWhiteSpace(message)) {
                     Out.Write(message + ": ");
                }
                if (null != Parent) {
                    return Parent.ReadLine();
                }
                return Console.ReadLine();
            }
            return OnReadLine(message);
        }

        public ConsoleKeyInfo ReadKey() {
            if (null == OnReadKey)
            {
                if (null != Parent) {
                    return Parent.ReadKey();
                }
                return Console.ReadKey();
            }
            return OnReadKey();
        }

        public IConsoleContext SetColor(ConsoleColor color) {
            if (null == OnSetColor) {
                if (null != Parent) {
                    Parent.SetColor(color);
                }
                else {
                    Console.ForegroundColor = color;
                }
            }
            else {
                OnSetColor(color);
            }
            return this;
        }

        public IConsoleContext ResetColor() {
            if (null == OnRestoreColor)
            {
                if (null != Parent) {
                    Parent.ResetColor();
                }
                else {
                    Console.ResetColor();
                }
            }
            else
            {
                OnRestoreColor();
            }
            return this;
        }

        public IConsoleContext Write(string data) {
            if (null == OnWrite) {
                Out.Write(data);
            }
            else {
                OnWrite(data, false);
            }
            return this;
        }

        public IConsoleContext WriteLine(string data) {
            if (null == OnWrite)
            {
                Out.Write(data);
            }
            else
            {
                OnWrite(data, true);
            }
            return this;
        }

        public ConsoleApplicationParameters Parameters
        {
            get
            {
                if (null != Parent) {
                    return _parameters ?? Parent.Parameters;
                }
                Initialize();
                return _parameters;
            }
            set { _parameters = value; }
        }

        public IContainer Container
        {
            get
            {
                if (null != Parent) {
                    return _container ?? Parent.Container;
                }
                Initialize();
                return _container;
            }
            set { _container = value; }
        }
    }
}