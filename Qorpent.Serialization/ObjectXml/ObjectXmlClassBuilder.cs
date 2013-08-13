using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.ObjectXml {
	/// <summary>
	/// 
	/// </summary>
	public class ObjectXmlClassBuilder {
		private readonly ObjectXmlCompiler _compiler;
		private readonly ObjectXmlClass _cls;
		private readonly ObjectXmlCompilerIndex _index;

		private IObjectXmlCompilerConfig GetConfig() {
			return _compiler.GetConfig();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="compiler"></param>
		/// <param name="cls"></param>
		/// <param name="index"></param>
		public static void Build(ObjectXmlCompiler compiler, ObjectXmlClass cls, ObjectXmlCompilerIndex index) {
			new ObjectXmlClassBuilder(compiler, cls, index).Build();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="compiler"></param>
		/// <param name="cls"></param>
		/// <param name="index"></param>
		public static Task BuildAsync(ObjectXmlCompiler compiler, ObjectXmlClass cls, ObjectXmlCompilerIndex index) {
			var task = new Task(() => new ObjectXmlClassBuilder(compiler, cls, index).Build());
			cls.BuildTask = task;
			task.Start();
			return task;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectXmlCompiler"></param>
		/// <param name="cls"></param>
		/// <param name="index"></param>
		protected ObjectXmlClassBuilder(ObjectXmlCompiler objectXmlCompiler, ObjectXmlClass cls, ObjectXmlCompilerIndex index) {
			_compiler = objectXmlCompiler;
			_cls = cls;
			_index = index;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Build()
		{
			lock (_cls) {
				if (CheckExistedBuild()) return;
				Console.WriteLine("start build " + _cls.FullName);
				_cls.InBuiltMode = true;	
				InternalBuild();
				_cls.InBuiltMode = false;
				_cls.IsBuilt = true;
				Console.WriteLine("fininsh build " + _cls.FullName);
			}
			
		}

		private void InternalBuild() {
			InitializeBuildIndexes();
			IntializeMergeIndexes();
			MergeSimpleInternalsNonStatic();
			InterpolateFields();
			BindParametersToCompiledClass();
			InterpolateElements();
			MergeSimpleInternalsStatic();
			CleanupPrivateMembers();
		}

		private bool CheckExistedBuild() {
			if (_cls.IsBuilt) return true;
			lock (_cls) {
				if (_cls.InBuiltMode) {
					if (null != _cls.BuildTask) {
						_cls.BuildTask.Wait();
					}
					else {
						for (var i = 0; i <= 10; i++) {
							Thread.Sleep(10);
							if (_cls.IsBuilt) {
								break;
							}
						}
					}
					if (_cls.IsBuilt) return true;
				}
			}
			return false;
		}

		private void IntializeMergeIndexes()
		{
			_cls.AllMergeDefs = _cls.CollectMerges().ToList();
		}

		private void MergeSimpleInternalsStatic()
		{
			foreach (var e in _cls.CollectImports().Where(_ => _.Static))
			{
				_cls.Compiled.Add(e.Compiled.Elements());
			}
		}

		private void MergeSimpleInternalsNonStatic()
		{
			foreach (var e in _cls.CollectImports().Where(_ => !_.Static))
			{
				_cls.Compiled.Add(e.Source.Elements());
			}
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
				if (e.Attr("code") == e.Attr("id"))
				{
					e.Attribute("id").Remove();
				}
				e.Attributes().Where(_ => _.Name.LocalName.StartsWith("_") || string.IsNullOrEmpty(_.Value)).Remove();
			}

		}

		private void InitializeBuildIndexes()
		{
			_cls.Compiled = new XElement(_cls.Source);
			_cls.ParamSourceIndex = BuildParametersConfig();
			_cls.ParamIndex = new ConfigBase();
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
			foreach (var i in _cls.CollectImports().Union(new[] { _cls }))
			{
				var selfconfig = new ConfigBase();
				selfconfig.Set("_class_", _cls.FullName);
				selfconfig.SetParent(current);
				current = selfconfig;
				if (i.Static && _cls != i)
				{
					if (!i.IsBuilt) {
						Build(_compiler, i, _index);
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

		private void InterpolateElements()
		{
			if (GetConfig().UseInterpolation)
			{
				var xi = new XmlInterpolation();
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