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
	public class EntityMap<T> : EntityTypeConfiguration<T> where T : class, IEntity {
		/// <summary>
		/// Override to provide custom naming for fields
		/// </summary>
		/// <param name="fieldname"></param>
		/// <returns></returns>
		protected virtual string Rename(string fieldname) {
			return fieldname;
		}
		/// <summary>
		/// 
		/// </summary>
		public EntityMap(string tablename, string schema = "") {

			ToTable(tablename, schema);
			HasKey(x => x.Id);
			Property(x => x.Code).IsRequired().HasColumnName(Rename("Code"));
			Property(x => x.Name).IsRequired().HasColumnName(Rename("Name"));
			Property(x => x.Idx).HasColumnName(Rename("Idx")).IsOptional();
			Property(x => x.Version).HasColumnName(Rename("Version"));
			Property(x => x.Comment).IsMaxLength().HasColumnName(Rename("Comment")).IsOptional();
			Property(x => x.Tags).IsVariableLength().HasColumnName(Rename("Tags")).IsOptional();
			if(typeof(IContextEntity).IsAssignableFrom(typeof(T))) {
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
			}
		}
	}
}