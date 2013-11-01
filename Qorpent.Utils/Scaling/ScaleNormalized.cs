using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.Scaling {
    /// <summary>
    ///     �����, �������������� ��������������� �����
    /// </summary>
    public class ScaleNormalized {
        /// <summary>
        ///     ��������������� �������
        /// </summary>
        private ScaleNormalizedVariant _recommendedVariant;
        /// <summary>
        ///     ���������� ������ ���������
        /// </summary>
        private readonly IList<ScaleNormalizedVariant> _variants = new List<ScaleNormalizedVariant>();
        /// <summary>
        ///     �������� ������
        /// </summary>
        public ScaleNormalizeClause Clause { get; private set; }
        /// <summary>
        ///     ��������������� �������
        /// </summary>
        public ScaleNormalizedVariant RecommendedVariant {
            get { return _recommendedVariant; }
        }
        /// <summary>
        ///     ������������ ������� �����
        /// </summary>
        public double Maximal {
            get { return _recommendedVariant.Maximal; }
        }
        /// <summary>
        ///     ����������� �������� �����
        /// </summary>
        public double Minimal {
            get { return _recommendedVariant.Minimal; }
        }
        /// <summary>
        ///     ���������� ���������
        /// </summary>
        public int Divline {
            get { return _recommendedVariant.Divline; }
        }
        /// <summary>
        ///     �����, �������������� ��������������� �����
        /// </summary>
        /// <param name="clause">�������� ������</param>
        public ScaleNormalized(ScaleNormalizeClause clause) {
            Clause = clause;
        }
        /// <summary>
        ///     ������������ ��������� ������������, �������� �������������� ����� �����������
        /// </summary>
        public IEnumerable<ScaleNormalizedVariant> Variants {
            get { return _variants.AsEnumerable(); }
        }
        /// <summary>
        ///     ���������� �������� � ��������������� �������������
        /// </summary>
        /// <param name="variant">�������</param>
        public void AddVariant(ScaleNormalizedVariant variant) {
            _variants.Add(variant);
        }
        /// <summary>
        ///     ��������� ���������������� ��������
        /// </summary>
        /// <param name="recommendedVariant">��������������� �������</param>
        public void SetRecommendedVariant(ScaleNormalizedVariant recommendedVariant) {
            _recommendedVariant = recommendedVariant;
        }
    }
}