using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Qorpent.Config;

namespace Qorpent.Utils.Scaling {
    /// <summary>
    ///     ��������� ��� ��������������� �������� ����� ������ � �����
    /// </summary>
    public class ScaleApproximated : ConfigBase {
        /// <summary>
        ///     �������������� ������������ �������� ������� ���������
        /// </summary>
        private readonly double _baseMaximal;
        /// <summary>
        ///     �������������� ����������� �������� ������� ���������
        /// </summary>
        private readonly double _baseMinimal;
        /// <summary>
        ///     ������� ����, ��� ��������� ����� ���������
        /// </summary>
        private bool _isDone;
        /// <summary>
        ///     ������� ����, ��� ���������� �������� �����������
        /// </summary>
        private bool _isMinimalSet;
        /// <summary>
        ///     ������� ����, ��� ������������ �������� ���� �����������
        /// </summary>
        private bool _isMaximalSet;
        /// <summary>
        ///     ������� ����, ��� ��������� �������� �����������
        /// </summary>
        private bool _isBorderValueSet;
        /// <summary>
        ///     ����������, ��������� � ������ ������������� ������, �� ������� ������� ���������
        ///     <see cref="IsError"/>
        /// </summary>
        public Exception Exception { get; private set; }
        /// <summary>
        ///     ������� ����, ��� ��� ������������ ���� �������� ��������� ������
        ///     ������� ��������� ������������ �� ���� ���������
        /// </summary>
        public bool IsError { get; private set; }
        /// <summary>
        ///     ������� ����, ��� ��� ������� ������������ �� ��������� ������, ��������� ����
        /// </summary>
        public bool IsErrorBahaviorError { get; private set; }
        /// <summary>
        ///     ��������� �� ������, � ������� ��������� ������
        /// </summary>
        public ScaleNormalizeClause Clause { get; private set; }
        /// <summary>
        ///     ��������������� ������������� �����
        /// </summary>
        public ScaleNormalized Normalized { get; private set; }
        /// <summary>
        ///     ��������, ����������� ��� ������������� ��������� ������
        /// </summary>
        public Action<ScaleApproximated> ErrorBahavior { get; set; }
        /// <summary>
        ///     ���������� ������� ������������� 
        /// </summary>
        public bool IsDone {
            get { return _isDone; }
        }
        /// <summary>
        ///     ����������� �������� �� ������� ��������
        /// </summary>
        public double BaseMinimal {
            get { return _baseMinimal; }
        }
        /// <summary>
        ///     ������������ �������� �� ������� ��������
        /// </summary>
        public double BaseMaximal {
            get { return _baseMaximal; }
        }
        /// <summary>
        ///     ��� ������� �������������
        /// </summary>
        public string Hash {
            get { return MatchHash(); }
        }
        /// <summary>
        ///     ����������� �������� � ���������� ��� ������������ �������������
        /// </summary>
        public double Minimal {
            get {
                if (!_isMinimalSet) {
                    throw new Exception("The minimal value was not set");
                }

                return Get<double>("Minimal");
            }
            set {
                if (_isMinimalSet) {
                    throw new Exception("The minimal value was already set"); // ������ ������: ������ � ������ ��������
                }

                _isMinimalSet = true;
                Set("Minimal", value);
            }
        }
        /// <summary>
        ///     ������������ �������� � ���������� ��� ������������ �������������
        /// </summary>
        public double Maximal {
            get {
                if (!_isMaximalSet) {
                    throw new Exception("The maximal value was not set");
                }

                return Get<double>("Maximal");
            }
            set {
                if (_isMaximalSet) {
                    throw new Exception("The maximal values was already set"); // ������ ������: ������ � ������ ��������
                }

                _isMaximalSet = true;
                Set("Maximal", value);
            }
        }
        /// <summary>
        ///     ��������� ��������
        /// </summary>
        public double BorderValue {
            get {
                if (!_isBorderValueSet) {
                    throw new Exception("The border value value was not set"); // ������ ������: ������ � ������ ��������
                }

                return Get<double>("BorderValue");
            }
            set {
                if (_isBorderValueSet) {
                    throw new Exception("The border value was already set");
                }

                _isBorderValueSet = true;
                Set("BorderValue", value);
            }
        }
        /// <summary>
        ///     ������������ �������� �������� �����
        /// </summary>
        public IEnumerable<double> BaseValues { get; private set; }
        /// <summary>
        ///     ����� ����������� �������� �����
        /// </summary>
        public IEnumerable<double> Minimals { get; private set; }
        /// <summary>
        ///     ����� ������������ ������� ��� �����
        /// </summary>
        public IEnumerable<double> Maximals { get; private set; }
        /// <summary>
        ///     ��������� ��� ��������������� �������� ����� ������ � �����
        /// </summary>
        /// <param name="clause">������, � ������� ��������� ��������</param>
        /// <param name="baseValues">������� �������� �����</param>
        /// <param name="normalized">��������������� ������������� �����</param>
        public ScaleApproximated(ScaleNormalizeClause clause, IEnumerable<double> baseValues, ScaleNormalized normalized) {
            Clause = clause;
            BaseValues = baseValues;
            Normalized = normalized;

            _baseMaximal = BaseValues.Max();
            _baseMinimal = BaseValues.Min();
        }
        /// <summary>
        ///     ��������� ������������ ������������ ��������
        /// </summary>
        /// <param name="maximals">������������ ������������ ��������</param>
        public void SetMaximals(IEnumerable<double> maximals) {
            if (_isDone) {
                throw new Exception("Holden because handling is done");
            }

            Maximals = maximals;
        }
        /// <summary>
        ///     ��������� ������������ ����������� ��������
        /// </summary>
        /// <param name="minimals">������������ ����������� ��������</param>
        public void SetMinimals(IEnumerable<double> minimals) {
            if (_isDone) {
                throw new Exception("Holden because handling is done");
            }

            Minimals = minimals;
        }
        /// <summary>
        ///     ���������� ���������������� ��������
        /// </summary>
        /// <param name="variant">������������� ��������</param>
        public void AddVariant(ScaleNormalizedVariant variant) {
            if (_isDone) {
                throw new Exception("Holden because handling is done");
            }

            Normalized.AddVariant(variant);
        }
        /// <summary>
        ///     ���������� ���������������� ��������
        /// </summary>
        /// <param name="minimal">����������� ��������</param>
        /// <param name="maximal">������������ ��������</param>
        /// <param name="divlines">���������� ���������</param>
        public void AddVariant(double minimal, double maximal, int divlines) {
            if (_isDone) {
                throw new Exception("Holden because handling is done");
            }

            AddVariant(new ScaleNormalizedVariant { Divline = divlines, Minimal = minimal, Maximal = maximal });
        }
        /// <summary>
        ///     ��������� ��������� ��������� ������
        /// </summary>
        /// <param name="exception">����������, �������������� ������</param>
        /// <param name="runErrorBehavior">������� ����, ��� ����� ��������� �������� ������� �� ����</param>
        public void Error(Exception exception, bool runErrorBehavior) {
            IsError = true;
            _isDone = false;
            Exception = exception;

            if (runErrorBehavior) {
                if (ErrorBahavior == null) {
                    return;
                }

                try {
                    ErrorBahavior(this);
                } catch {
                    IsErrorBahaviorError = true;
                }
            }
        }
        /// <summary>
        ///     ������������� ������� ����, ��� ��������� ���������
        /// </summary>
        public void Done() {
            Normalized.Done();
            _isDone = true;
        }
        /// <summary>
        ///     ������������ ��� ������������������� �������������
        /// </summary>
        /// <returns>��� ������������������� �������������</returns>
        private string MatchHash() {
            return GetHashCode().ToString();
        }
    }
}