using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using Qorpent.Model;

namespace Qorpent.Data.Mapping {
	/// <summary>
	/// Мапинг строк
	/// </summary>
	public class EntityMap<T> : EntityTypeConfiguration<T> where T : class {
		/// <summary>
		/// Override to provide custom naming for fields
		/// </summary>
		/// <param name="fieldname"></param>
		/// <returns></returns>
		public virtual string Rename(string fieldname) {
			return fieldname;
		}
		/// <summary>
		/// 
		/// </summary>
		public EntityMap(string tablename, string schema = "") {
			ToTable(tablename, schema);	
			
			/*if(typeof(IContextEntity).IsAssignableFrom(typeof(T))) {
				Property(x => ((IContextEntity) x).Active).HasColumnName(Rename("Active")).IsOptional();
				Property(x => ((IContextEntity)x).Role).HasColumnName(Rename("Role")).IsOptional();
				Property(x => ((IContextEntity)x).Start).HasColumnName(Rename("Start")).IsOptional();
				Property(x => ((IContextEntity)x).Finish).HasColumnName(Rename("Start")).IsOptional();
			}
			if(typeof(IWithHierarchy<T>).IsAssignableFrom(typeof(T))) {
				Property(x => ((IWithHierarchy<T>)x).ParentId).HasColumnName(Rename("ParentId")).IsOptional();
				Property(x => ((IWithHierarchy<T>)x).Path).HasColumnName(Rename("Path")).IsOptional();
				HasOptional(x => ((IWithHierarchy<T>)x).Parent)
					.WithMany(x => ((IWithHierarchy<T>)x).Children)
					.HasForeignKey(x => ((IWithHierarchy<T>)x).ParentId);
			}
			if(typeof(IWithFormula).IsAssignableFrom(typeof(T))) {
				Property(x => ((IWithFormula)x).IsFormula).HasColumnName(Rename("IsFromula")).IsOptional();
				Property(x => ((IWithFormula)x).Formula).HasColumnName(Rename("Formula")).IsOptional();
				Property(x => ((IWithFormula)x).FormulaType).HasColumnName(Rename("FormulaType")).IsOptional();
			}*/
		}
	}


	/// <summary>
	/// 
	/// </summary>
	public static class EntityTypeConfigurationExtensions {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="typeconf"></param>
		/// <typeparam name="T"></typeparam>
		public static void Call<T>(EntityTypeConfiguration<T> typeconf) where T : class {
			if (typeof(IEntity).IsAssignableFrom(typeof(T)))
			{
				var method = typeof(EntityTypeConfigurationExtensions)
					.GetMethod("CallEntity");
				method.MakeGenericMethod(typeof(T)).Invoke(null, new[] { typeconf });
			}
			if(typeof(IWithHierarchy<T>).IsAssignableFrom(typeof(T))) {
				var method = typeof (EntityTypeConfigurationExtensions)
					.GetMethod("CallHierarchy");
				method.MakeGenericMethod(typeof (T)).Invoke(null,new[]{ typeconf});
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tconf"></param>
		/// <typeparam name="T"></typeparam>
		public static void CallHierarchy<T>(EntityTypeConfiguration<T> tconf ) where T: class, IWithHierarchy<T>, IEntity {
			tconf.Property(x => x.ParentId);//.HasColumnName(tconf.Rename("ParentId")).IsOptional();
			tconf.Property(x => x.Path);//.HasColumnName(tconf.Rename("Path")).IsOptional();
			tconf.HasOptional(x => x.Parent)
				.WithMany(x => x.Children)
				.HasForeignKey(x => x.ParentId);
		} 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tconf"></param>
		/// <typeparam name="T"></typeparam>
		public static void CallEntity<T>(EntityTypeConfiguration<T> tconf ) where T: class, IWithHierarchy<T>, IEntity {
			tconf.HasKey(x => x.Id);
			tconf.Property(x => x.Code).IsRequired();//.HasColumnName(tconf.Rename("Code"));
			tconf.Property(x => x.Name).IsRequired();//.HasColumnName(tconf.Rename("Name"));
			tconf.Property(x => x.Idx);//.HasColumnName(tconf.Rename("Idx")).IsOptional();
			tconf.Property(x => x.Version);//.HasColumnName(tconf.Rename("Version"));
			tconf.Property(x => x.Comment);//.IsMaxLength().HasColumnName(tconf.Rename("Comment")).IsOptional();
			tconf.Property(x => x.Tag);//.IsVariableLength().HasColumnName(tconf.Rename("Tags")).IsOptional();
		} 

		
	}
}