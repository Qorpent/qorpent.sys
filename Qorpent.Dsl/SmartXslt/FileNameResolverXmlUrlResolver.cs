#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Dsl/FileNameResolverXmlUrlResolver.cs
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Qorpent.IO;
using Qorpent.Utils.Extensions;

namespace Qorpent.Dsl.SmartXslt {
	/// <summary>
	/// 	xml resolver, attached to FileNameResolver (treats ~/.../ names as resolved from
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class FileNameResolverXmlUrlResolver : XmlUrlResolver {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="FileNameResolverXmlUrlResolver" /> class.
		/// </summary>
		/// <param name="baseuri"> The baseuri. </param>
		/// <param name="resolver"> The resolver. </param>
		/// <param name="directSource"> </param>
		/// <remarks>
		/// </remarks>
		public FileNameResolverXmlUrlResolver(Uri baseuri, IFileNameResolver resolver,
		                                      IDictionary<string, string> directSource) {
			_baseuri = baseuri;
			_resolver = resolver;
			_directsource = directSource;
		}

		/// <summary>
		/// 	Resolves the absolute URI from the base and relative URIs.
		/// </summary>
		/// <param name="baseUri"> The base URI used to resolve the relative URI. </param>
		/// <param name="relativeUri"> The URI to resolve. The URI can be absolute or relative. If absolute, this value effectively replaces the <paramref
		/// 	 name="baseUri" /> value. If relative, it combines with the <paramref name="baseUri" /> to make an absolute URI. </param>
		/// <returns> A <see cref="T:System.Uri" /> representing the absolute URI, or null if the relative URI cannot be resolved. </returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="baseUri " />
		/// 	is null or
		/// 	<paramref name="relativeUri" />
		/// 	is null</exception>
		/// <remarks>
		/// </remarks>
		public override Uri ResolveUri(Uri baseUri, string relativeUri) {
			var _bu = baseUri ?? _baseuri;
			if (_bu.PathAndQuery.IsEmpty()) {
				_bu = _baseuri;
			}
			if (_directsource.ContainsKey(relativeUri)) {
				return new Uri("direct://" + relativeUri);
			}
			if (!relativeUri.StartsWith("~")) {
				return base.ResolveUri(_bu, relativeUri);
			}
			return new Uri(_resolver.Resolve(relativeUri));
		}

		/// <summary>
		/// 	Maps a URI to an object containing the actual resource.  - can resolve from directsource
		/// </summary>
		/// <returns> A System.IO.Stream object or null if a type other than stream is specified. </returns>
		/// <param name="absoluteUri"> The URI returned from <see
		/// 	 cref="M:System.Xml.XmlResolver.ResolveUri(System.Uri,System.String)" /> </param>
		/// <param name="role"> The current implementation does not use this parameter when resolving URIs. This is provided for future extensibility purposes. For example, this can be mapped to the xlink:role and used as an implementation specific argument in other scenarios. </param>
		/// <param name="ofObjectToReturn"> The type of object to return. The current implementation only returns System.IO.Stream objects. </param>
		/// <exception cref="T:System.Xml.XmlException">
		/// 	<paramref name="ofObjectToReturn" />
		/// 	is neither null nor a Stream type.</exception>
		/// <exception cref="T:System.UriFormatException">The specified URI is not an absolute URI.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="absoluteUri" />
		/// 	is null.</exception>
		/// <exception cref="T:System.Exception">There is a runtime error (for example, an interrupted server connection).</exception>
		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn) {
			if (absoluteUri.Scheme.StartsWith("direct")) {
				return new MemoryStream(Encoding.UTF8.GetBytes(_directsource[absoluteUri.PathAndQuery]), false);
			}
			return base.GetEntity(absoluteUri, role, ofObjectToReturn);
		}

		/// <summary>
		/// </summary>
		private readonly Uri _baseuri;

		private readonly IDictionary<string, string> _directsource;

		/// <summary>
		/// </summary>
		private readonly IFileNameResolver _resolver;
	}
}