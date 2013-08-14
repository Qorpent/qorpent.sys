using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.LogicalExpressions;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.LogicalExpressions;

namespace Qorpent.BSharp {
	/// <summary>
	/// 
	/// </summary>
	public class BSharpClassBuilder {
		private readonly IBSharpCompiler _compiler;
		private readonly IBSharpClass _cls;
		private readonly IBSharpContext _context;

		private IBSharpConfig GetConfig() {
			return _compiler.GetConfig();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="compiler"></param>
		/// <param name="cls"></param>
		/// <param name="context"></param>
		public static void Build(IBSharpCompiler compiler, IBSharpClass cls, IBSharpContext context) {
			new BSharpClassBuilder(compiler, cls, context).Build();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="compiler"></param>
		/// <param name="cls"></param>
		/// <param name="context"></param>
		public static Task BuildAsync(BSharpCompiler compiler, IBSharpClass cls, BSharpContext context) {
			var task = new Task(() => new BSharpClassBuilder(compiler, cls, context).Build());
			cls.BuildTask = task;
			task.Start();
			return task;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bSharpCompiler"></param>
		/// <param name="cls"></param>
		/// <param name="context"></param>
		protected BSharpClassBuilder(IBSharpCompiler bSharpCompiler, IBSharpClass cls, IBSharpContext context) {
			_compiler = bSharpCompiler;
			_cls = cls;
			_context = context;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Build()
		{
			lock (_cls) {
				if (CheckExistedBuild()) return;
				_cls.Set(BSharpClassAttributes.InBuild);
				InternalBuild();
				_cls.Remove(BSharpClassAttributes.InBuild);
				_cls.Set(BSharpClassAttributes.Built);
			}
		}

		private void InternalBuild() {
			InitializeBuildIndexes();
			IntializeMergeIndexes();
			InterpolateFields();
			BindParametersToCompiledClass();

			CleanupElementsWithConditions();
			
			MergeInternals();
			InterpolateElements();
			PerformMergingWithElements();
			
			CleanupElementsWithConditions();

			ProcessIncludes();

			CleanupPrivateMembers();
		}

		private void ProcessIncludes() {
			XElement[] includes;
			var needReInterpolate = false;
			while ((includes = _cls.Compiled.Descendants("include").ToArray()).Length != 0) {
				foreach (var i in includes) {
					needReInterpolate = ProcessInclude(i, needReInterpolate);
				}				
			}
			if (needReInterpolate) {
				InterpolateElements('%');
			}
		}

		private bool ProcessInclude(XElement i, bool needReInterpolate) {
			var code = i.Attr("code");
			var includecls = _context.Get(code, _cls.Namespace);
			if (null == includecls) {
				i.Remove();
			}
			else {
				Build(_compiler,includecls,_context);
				var includeelement = new XElement(includecls.Compiled);
				var usebody = null!=i.Attribute("body")||i.Attr("name")=="body";

				needReInterpolate = needReInterpolate || includeelement.HasAttributes(contains: "%{", skipself: usebody);

				if (usebody) {
					var elements = includeelement.Elements().ToArray();
					foreach (var e in elements) {
						StoreIncludeParameters(i, e);
					}
					i.ReplaceWith(includeelement.Elements());
				}
				else {
					StoreIncludeParameters(i, includeelement);
					includeelement.Name = includeelement.Attr("fullcode");
					includeelement.Attribute("fullcode").Remove();
					includeelement.Attribute("code").Remove();
					includeelement.Attribute("id").Remove();
					i.ReplaceWith(includeelement);
				}
			}
			return needReInterpolate;
		}

		private void StoreIncludeParameters(XElement src, XElement trg) {
			foreach (var a in src.Attributes()) {
				if(a.Name.LocalName=="code") continue;
				if (a.Name.LocalName == "name") continue;
				if (a.Name.LocalName == "body") continue;
				trg.SetAttributeValue(a.Name,a.Value);
			}
		}

		private void CleanupElementsWithConditions() {
				_cls.Compiled.Descendants().Where(_ => !IsMatch(_)).Remove();

		}

		private void PerformMergingWithElements() {
			foreach (var root in _cls.AllElements.Where(_ => _.Type == BSharpElementType.Define).ToArray()) {
				var allroots = _cls.Compiled.Elements(root.Name).ToArray();
				var groupedroots = allroots.GroupBy(_ => _.Attr("code"));
				foreach(var doublers in groupedroots.Where(_=>_.Count()>1)) {
					doublers.Skip(1).Remove();
				}
				var alloverrides =
					_cls.AllElements.Where(_ => _.Type != BSharpElementType.Define && _.TargetName == root.Name).ToArray();
//				foreach (var over in alloverrides) {
					foreach (var g in groupedroots) {
						var e = g.First();
						//реверсировать надо для правильного порядка обхода
						var candidates = e.ElementsBeforeSelf().Reverse().Where(_=>_.Attr("code")==g.Key).ToArray();

						foreach (var o in candidates) {
							var over = alloverrides.FirstOrDefault(_ => _.Name == o.Name.LocalName);
							if (null != over) {
								if (over.Type == BSharpElementType.Override) {
									foreach (var a in o.Attributes()) {
										e.SetAttributeValue(a.Name, a.Value);
									}
									if (o.HasElements) {
										e.Elements().Remove();
										e.Add(o.Elements());
									}
								}else if (over.Type == BSharpElementType.Extension) {
									foreach (var a in o.Attributes())
									{
										if (null == e.Attribute(a.Name)) {
											e.SetAttributeValue(a.Name, a.Value);
										}
									}
									if (o.HasElements)
									{
										e.Add(o.Elements());
									}
								}

								o.Remove();	
							}
						}
						
					}
	//			}
			}
			
		}

		private bool CheckExistedBuild() {
			if (_cls.Is(BSharpClassAttributes.Built)) return true;
			lock (_cls) {
				if (_cls.Is(BSharpClassAttributes.InBuild))
				{
					if (null != _cls.BuildTask) {
						_cls.BuildTask.Wait();
					}
					else {
						for (var i = 0; i <= 10; i++) {
							Thread.Sleep(10);
							if (_cls.Is(BSharpClassAttributes.Built))
							{
								break;
							}
						}
					}
					if (_cls.Is(BSharpClassAttributes.Built)) return true;
				}
			}
			return false;
		}

		private void IntializeMergeIndexes()
		{
			foreach (var m in _cls.AllElements) {
				m.Name = m.Name.ConvertToXNameCompatibleString();
				if (!string.IsNullOrWhiteSpace(m.TargetName)) {
					m.TargetName = m.TargetName.ConvertToXNameCompatibleString();
				}
			}
			var allroots = _cls.AllElements.Where(_ => _.Type == BSharpElementType.Define).Select(_ => _.Name).ToArray();
			foreach (var root in allroots) {
				var defoverride = new BSharpElement {Name = "__TILD__" + root, TargetName = root, Type = BSharpElementType.Override};
				if (!_cls.AllElements.Contains(defoverride)) {
					_cls.AllElements.Add(defoverride);
				}
				var defext = new BSharpElement { Name = "__PLUS__" + root, TargetName = root, Type = BSharpElementType.Extension };
				if (!_cls.AllElements.Contains(defext))
				{
					_cls.AllElements.Add(defext);
				}
			}
		}

		

		private void MergeInternals()
		{//реверс нужен для правильного порядка наката элементов
			
			foreach (var e in _cls.AllImports.Reverse())
			{
				if (e.Is(BSharpClassAttributes.Static)) {
					if (!e.Is(BSharpClassAttributes.Built)) {
						Build(_compiler,e,_context);
					}
					_cls.Compiled.Add(e.Compiled.Elements().Where(IsMatch));
				}
				else {
					_cls.Compiled.Add(e.Source.Elements().Where(IsMatch));
				}
			}

			
		}
		
		private bool IsMatch(XElement arg) {
			var cond = arg.Attr("if");
			if (string.IsNullOrWhiteSpace(cond)) {
				return true;
			}
			//мы должны пропускать интерполяции, так как сверить их все равно нельзя пока
			if (cond.Contains("${")) return true;
			var src = new DictionaryTermSource(_cls.ParamIndex);
			return new LogicalExpressionEvaluator().Eval(cond, src);
		}

		private void CleanupPrivateMembers()
		{
			var name = _cls.Compiled.Attr("name");
			if (name == "abstract" || name == "static")
			{
				_cls.Compiled.Attribute("name").Remove();
			}
			_cls.Compiled.Attributes("abstract").Remove();
			_cls.Compiled.Attributes("static").Remove();
			_cls.Compiled.Elements("import").Remove();
			_cls.Compiled.Elements("element").Remove();
			_cls.Compiled.Descendants("embed").Remove();
			_cls.Compiled.Descendants("include").Remove();

			foreach (var e in _cls.Compiled.DescendantsAndSelf())
			{
				if (null != e.Attribute("id")) {
					if (e.Attr("code") == e.Attr("id")) {
						e.Attribute("id").Remove();
					}

				}
				if (null != e.Attribute("if")) {
					e.Attribute("if").Remove();
				}
				e.Attributes().Where(_ => _.Name.LocalName.StartsWith("_") || string.IsNullOrEmpty(_.Value)).Remove();
			}

			foreach (var m in _cls.AllElements.Where(_ => _.Type != BSharpElementType.Define).Select(_ => _.Name).Distinct()) {
				_cls.Compiled.Elements(m).Remove();
			}

		}

		private void InitializeBuildIndexes()
		{
			_cls.Compiled = new XElement(_cls.Source);
			_cls.Compiled.Elements("import").Remove();
			_cls.Compiled.Elements("element").Remove();
			_cls.ParamSourceIndex = BuildParametersConfig();
			_cls.ParamIndex = new ConfigBase();
			_cls.Compiled.SetAttributeValue("fullcode",_cls.FullName);
			foreach (var p in _cls.ParamSourceIndex)
			{
				_cls.ParamIndex.Set(p.Key, p.Value);
			}

		}

		/// <summary>
		///     Возвращает XML для резолюции атрибутов
		/// </summary>
		/// <returns></returns>
		private IConfig BuildParametersConfig()
		{

			var result = new ConfigBase();
			ConfigBase current = result;
			foreach (var i in _cls.AllImports.Union(new[] { _cls }))
			{
				var selfconfig = new ConfigBase();
				selfconfig.Set("_class_", _cls.FullName);
				selfconfig.SetParent(current);
				current = selfconfig;
				if (i.Is(BSharpClassAttributes.Static) && _cls != i)
				{
					if (!i.Is(BSharpClassAttributes.Built)) {
						Build(_compiler, i, _context);
					}
					foreach (var p in i.ParamIndex)
					{
						current.Set(p.Key, p.Value);
					}
				}
				else
				{
					foreach (XAttribute a in i.Source.Attributes())
					{
						current.Set(a.Name.LocalName, a.Value);
					}
				}
			}
			return current;
		}

		private  void BindParametersToCompiledClass()
		{
			foreach (var e in _cls.ParamIndex)
			{
				_cls.Compiled.SetAttributeValue(e.Key, e.Value.ToStr());
			}
		}

		private void InterpolateElements(char ancor='$')
		{
			if (GetConfig().UseInterpolation)
			{
				var xi = new XmlInterpolation{AncorSymbol = ancor};
				xi.Interpolate(_cls.Compiled);
			}
		}

		private void InterpolateFields()
		{
			if (GetConfig().UseInterpolation)
			{
				var si = new StringInterpolation();

				for (int i = 0; i <= 3; i++)
				{
					KeyValuePair<string, object>[] _substs =
						_cls.ParamIndex.Where(_ => (_.Value is string) && ((string)_.Value).Contains("${"))
						   .ToArray();
					if (0 == _substs.Length)
					{
						break;
					}
					foreach (var s in _substs)
					{
						_cls.ParamIndex.Set(s.Key, si.Interpolate((string)s.Value, _cls.ParamSourceIndex));
					}
				}
			}
		}

	}
}