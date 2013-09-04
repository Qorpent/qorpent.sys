using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Dot
{
	/// <summary>
	/// Полный граф DOT
	/// </summary>
	public class Graph : GraphElementBase
	{
		/// <summary>
		/// 
		/// </summary>
		public Graph() {
			Nodes = new List<Node>();
			Edges = new List<Edge>();
		}
		/// <summary>
		/// Узлы
		/// </summary>
		public IList<Node> Nodes { get; private set; }
		/// <summary>
		/// Ребра
		/// </summary>
		public IList<Edge> Edges { get; private set; }	

		/// <summary>
		/// Направление графа
		/// </summary>
		public RankDirType RankDir {
			get { return Get(DotConstants.RankDirAttribute).To<RankDirType>(); }
			set { Set(DotConstants.RankDirAttribute, value.ToStr()); }
		}

        /// <summary>
        /// Если "Да", то расположение графа центировано, а если "Нет", то не центировано
        /// </summary>
        public bool Center
        {
            get { return Get(DotConstants.CenterAttribute).To<bool>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.CenterAttribute, str);

            }
        }
        /// <summary>
        /// Если "Да", то позволяет стрелкам идти между подграфами (См. Lhead Ltail )
        /// </summary>
        public bool Compound
        {
            get { return Get(DotConstants.CompoundAttribute).To<bool>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.CompoundAttribute, str);

            }
        }
        /// <summary>
        /// Сконцентрированное расположение узлов и подграфов графа
        /// </summary>
        public bool Concentrate
        {
            get { return Get(DotConstants.ConcentrateAttribute).To<bool>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.ConcentrateAttribute, str);

            }
        }
        /// <summary>
        /// Если "Да", то график отображается в ландшафтном режиме, т.е. переварачивается на 90 градусов
        /// </summary>
        public bool Landscape
        {
            get { return Get(DotConstants.LandscapeAttribute).To<bool>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.LandscapeAttribute, str);

            }
        }
        /// <summary>
        /// Разварачивает граф на альбомный лист. В значении д.б. указана любая английская Л - l*, L* или полностью Landscape Н-р, orientation="Landscape"
        /// </summary>
        public string Orientation
        {
            get { return Get(DotConstants.OrientationAttribute).To<string>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.OrientationAttribute, str);

            }
        }
        /// <summary>
        /// Квантум увеличивает размеры узла графа на указанное значение. Н-р, quantum=0.5
        /// </summary>
        public double Quantum
        {
            get { return Get(DotConstants.QuantumAttribute).To<double>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.QuantumAttribute, str);

            }
        }
        /// <summary>
        /// Если "Да" и есть несколько графов, то ?????
        /// </summary>
        public bool Remincross
        {
            get { return Get(DotConstants.RemincrossAttribute).To<bool>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.RemincrossAttribute, str);

            }
        }
        /// <summary>
        /// Если установить значение 90, то произойдет переориентации графа в альбомную страницу
        /// </summary>
        public int Rotate
        {
            get { return Get(DotConstants.RotateAttribute).To<int>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.RotateAttribute, str);

            }
        }
        


		/// <summary>
		/// Автонастройка
		/// </summary>
		public override void AutoTune()
		{
			base.AutoTune();
			if (Edges.Any(e => 
				e.HasAttribute(DotConstants.LheadAttribute)
				||e.HasAttribute(DotConstants.LtailAttribute))) {
				if (!HasAttribute(DotConstants.CompoundAttribute)) {
					Compound = true;
				}
			}
		}
	}
}
