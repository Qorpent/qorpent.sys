using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.FuzzyLogic;

namespace Qorpent.Utils.Scaling {
    /// <summary>
    ///     �����, �������������� ��������������� �����
    /// </summary>
    public class ScaleNormalized {
        /// <summary>
        ///     ������� ����, ��� ������������ ���������
        /// </summary>
        private bool _isDone;
        /// <summary>
        ///     ��������������� �������
        /// </summary>
        private ScaleNormalizedVariant _recommendedVariant;
        /// <summary>
        ///     ���������� ������ ���������
        /// </summary>
        private readonly IFuzzySet<ScaleNormalizedVariant> _variants = new FuzzySet<ScaleNormalizedVariant>();
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
        ///     ���������� ������� ������������� ��������� �����
        /// </summary>
        public bool IsDone {
            get { return _isDone; }
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
        public IFuzzySet<ScaleNormalizedVariant> Variants {
            get { return _variants; }
        }
        /// <summary>
        ///     ���������� �������� � ��������������� �������������
        /// </summary>
        /// <param name="variant">�������</param>
        public void AddVariant(ScaleNormalizedVariant variant) {
            if (_isDone) {
                throw new Exception("Holden because handling is done");
            }

            _variants.Insert(variant);
        }
        /// <summary>
        ///     ��������� ���������������� ��������
        /// </summary>
        /// <param name="recommendedVariant">��������������� �������</param>
        public void SetRecommendedVariant(ScaleNormalizedVariant recommendedVariant) {
            if (_isDone) {
                throw new Exception("Holden because handling is done");
            }

            _recommendedVariant = recommendedVariant;
        }
        /// <summary>
        ///     ��������� ������� ����, ��� ��������� ���������
        /// </summary>
        public void Done() {
            if (_isDone) {
                throw new Exception("Holden because handling is done");
            }

            _isDone = true;
        }
    }
}