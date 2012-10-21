#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : EntityTypeConfigurationExtensions.cs
// Project: Qorpent.Data
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Data.Entity.ModelConfiguration;
using Qorpent.Model;

namespace Qorpent.Data.Mapping {
	/// <summary>
	/// </summary>
	public static class EntityTypeConfigurationExtensions {
		/// <summary>
		/// </summary>
		/// <param name="typeconf"> </param>
		/// <typeparam name="T"> </typeparam>
		public static void Call<T>(EntityTypeConfiguration<T> typeconf) where T : class {
			if (typeof (IEntity).IsAssignableFrom(typeof (T))) {
				var method = typeof (EntityTypeConfigurationExtensions)
					.GetMethod("CallEntity");
				method.MakeGenericMethod(typeof (T)).Invoke(null, new[] {typeconf});
			}
			if (typeof (IWithHierarchy<T>).IsAssignableFrom(typeof (T))) {
				var method = typeof (EntityTypeConfigurationExtensions)
					.GetMethod("CallHierarchy");
				method.MakeGenericMethod(typeof (T)).Invoke(null, new[] {typeconf});
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="tconf"> </param>
		/// <typeparam name="T"> </typeparam>
		public static void CallHierarchy<T>(EntityTypeConfiguration<T> tconf) where T : class, IWithHierarchy<T>, IEntity {
			tconf.Property(x => x.ParentId); //.HasColumnName(tconf.Rename("ParentId")).IsOptional();
			tconf.Property(x => x.Path); //.HasColumnName(tconf.Rename("Path")).IsOptional();
			tconf.HasOptional(x => x.Parent)
				.WithMany(x => x.Children)
				.HasForeignKey(x => x.ParentId);
		}

		/// <summary>
		/// </summary>
		/// <param name="tconf"> </param>
		/// <typeparam name="T"> </typeparam>
		public static void CallEntity<T>(EntityTypeConfiguration<T> tconf) where T : class, IWithHierarchy<T>, IEntity {
			tconf.HasKey(x => x.Id);
			tconf.Property(x => x.Code).IsRequired(); //.HasColumnName(tconf.Rename("Code"));
			tconf.Property(x => x.Name).IsRequired(); //.HasColumnName(tconf.Rename("Name"));
			tconf.Property(x => x.Idx); //.HasColumnName(tconf.Rename("Idx")).IsOptional();
			tconf.Property(x => x.Version); //.HasColumnName(tconf.Rename("Version"));
			tconf.Property(x => x.Comment); //.IsMaxLength().HasColumnName(tconf.Rename("Comment")).IsOptional();
			tconf.Property(x => x.Tag); //.IsVariableLength().HasColumnName(tconf.Rename("Tags")).IsOptional();
		}
	}
}