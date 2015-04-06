using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Config;

namespace Qorpent.Utils.Scaling {
    /// <summary>
    ///     ������������� ������� �� �����������
    /// </summary>
    public class ScaleNormalizeClause : ConfigBase {
        /// <summary>
        ///     �������������� ��������, ����������� ������ ��� ����� ���������� ���� � ��������� �����
        /// </summary>
        private readonly IList<KeyValuePair<int, Action<ScaleApproximated>>> _appendixes;
        /// <summary>
        ///     ������� ����, ��� ����������� �������� ���� �����������
        /// </summary>
        private bool _isMinimalValueSet;
        /// <summary>
        ///     ������� ����, ��� ������������ �������� ���� �����������
        /// </summary>
        private bool _isMaximalValueSet;
        /// <summary>
        ///     ������� ����, ��� ����� ������������ ������������� ����������� ��������
        /// </summary>
        public bool UseMinimalValue { get; set; }
        /// <summary>
        ///     ������� ����, ��� ����� ������������ ������������� ������������ ��������
        /// </summary>
        public bool UseMaximalValue { get; set; }
        /// <summary>
        ///     ������� ����, ��� ����� ��������� ������ ������������ ��� ������ �������������� <see cref="Appendixes"/>,
        ///     ����������� ������������ ����� ������������
        /// </summary>
        public bool RunSlickNormalization { get; set; }
        /// <summary>
        ///     �������������� ��������, ����������� ������ ��� ����� ���������� ���� � ��������� �����
        /// </summary>
        public IEnumerable<KeyValuePair<int, Action<ScaleApproximated>>> Appendixes {
            get { return _appendixes.AsEnumerable(); }
        }
        /// <summary>
        ///     ������� ����, ��� ����� ������������ ���
        /// </summary>
        public bool UseCache { get; set; }
        /// <summary>
        ///     ���
        /// </summary>
        public string Hash {
            get { return MatchHash(); }
        }
        /// <summary>
        ///     ����������� ��������
        /// </summary>
        public double MinimalValue {
            get {
                if (!_isMinimalValueSet) {
                    throw new Exception("The minimal value was not set");
                }

                return Get<double>("MinimalValue");
            }
            set {
                _isMinimalValueSet = true;
                Set("MinimalValue", value);
            }
        }
        /// <summary>
        ///     ������������ ��������
        /// </summary>
        public double MaximalValue {
            get {
                if (!_isMaximalValueSet) {
                    throw new Exception("The maximal value was not set");
                }

                return Get<double>("MaximalValue");
            }
            set {
                _isMaximalValueSet = true;
                Set("MaximalValue", value);
            }
        }
        /// <summary>
        ///     ������������� ������� �� �����������
        /// </summary>
        public ScaleNormalizeClause() {
            _appendixes = new List<KeyValuePair<int, Action<ScaleApproximated>>>();

            RunSlickNormalization = true;
            UseCache = false;
        }
        /// <summary>
        ///     ���������� ��������������� �������� � ���������
        /// </summary>
        /// <param name="appendix">����, �������������� ��� ���� � ��������</param>
        public ScaleNormalizeClause AddAppendix(KeyValuePair<int, Action<ScaleApproximated>> appendix) {
            _appendixes.Add(appendix);
            return this;
        }
        /// <summary>
        ///     ���������� ��������������� �������� � ���������
        /// </summary>
        /// <param name="stepCode">��� ����, � ������� �������������� ��������</param>
        /// <param name="appendix">��������</param>
        public ScaleNormalizeClause AddAppendix(int stepCode, Action<ScaleApproximated> appendix) {
            return AddAppendix(new KeyValuePair<int, Action<ScaleApproximated>>(stepCode, appendix));
        }
        /// <summary>
        ///     ������������ ��� ������� �� ������������
        /// </summary>
        /// <returns>��� ������� �� ������������</returns>
        private string MatchHash() {
            return GetHashCode().ToString();
        }
    }
}