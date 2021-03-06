﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Qorpent.Json;

namespace Qorpent.BSharp{
	/// <summary>
	/// </summary>
	public class BSharpJsonCompiler{
		private readonly BSharpCompiler _bSharpCompiler;
		private readonly IList<string> _jsonContext;
		private readonly JsonParser _jsonParser;
		private IBSharpContext _bSharpContext;

		/// <summary>
		/// </summary>
		public BSharpJsonCompiler(){
			_jsonContext = new List<string>();
			_bSharpContext = new BSharpContext();
			_bSharpCompiler = new BSharpCompiler();
			_jsonParser = new JsonParser();
		}

		/// <summary>
		///     Получение текущего B# контекста
		/// </summary>
		/// <returns></returns>
		public IBSharpContext GetBSharpContext(){
			return _bSharpContext;
		}

		/// <summary>
		///     Парсит текущий JSON контекст
		/// </summary>
		public void CompileContext(){
			BSharpConfig config = GetBSharpConfig();
			var list = new List<XElement>();

			foreach (string json in _jsonContext){
				list.AddRange(
					RebuildJsonToBSharpStyle(json)
					);
			}

			_bSharpCompiler.Initialize(config);
			IBSharpContext compiled = _bSharpCompiler.Compile(list, (IBSharpContext) null);
			_bSharpContext.Merge(compiled);
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		private BSharpConfig GetBSharpConfig(){
			return new BSharpConfig{UseInterpolation = true};
		}

		/// <summary>
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		private IEnumerable<XElement> RebuildJsonToBSharpStyle(string json){
			var xmlList = new List<XElement>();
			XElement classElements = _jsonParser.ParseXml(json).Element("class");

			if (classElements == null){
				return xmlList;
			}

			foreach (XElement item in classElements.Elements()){
				var root = new XElement("root");
				RebuildXElementToBSharpStyle(item);
				root.Add(item);
				xmlList.Add(root);
			}

			return xmlList;
		}

		/// <summary>
		/// </summary>
		/// <param name="xElement"></param>
		private void RebuildXElementToBSharpStyle(XElement xElement){
		}

		/// <summary>
		///     Очистить текущий B# контекст
		/// </summary>
		public void CleanBSharpContext(){
			_bSharpContext = new BSharpContext();
		}

		/// <summary>
		///     Очистить текущий JSON контекст
		/// </summary>
		public void CleanJsonContext(){
			_jsonContext.Clear();
		}

		/// <summary>
		///     Загрузка JSON контекста
		/// </summary>
		/// <param name="path">Директория, в которой начать поиск по маске</param>
		/// <param name="mask">Маска для поиска</param>
		/// <param name="searchOptions">Опции для поиск</param>
		public void LoadJsonContext(string path, string mask, SearchOption searchOptions){
			string fullpath = Path.GetFullPath(path);
			string[] gotfiles = Directory.GetFiles(fullpath, mask, searchOptions);

			foreach (string file in gotfiles){
				LoadJsonContext(file);
			}
		}

		/// <summary>
		///     Загрузка JSON контекста
		/// </summary>
		/// <param name="path">Целевой файл</param>
		/// <param name="json"></param>
		public void LoadJsonContext(string path = null, string json = null){
			if (path != null){
				_jsonContext.Add(
					File.ReadAllText(path)
					);
				return;
			}

			if (json != null){
				_jsonContext.Add(json);
				return;
			}

			throw new Exception("Incorrect JSON context");
		}
	}
}