using System;
using System.Collections.Generic;
using Qorpent.Config;

namespace Qorpent.Utils.Scaling {
    /// <summary>
    ///     ��������� ��� ��������������� �������� ����� ������ � �����
    /// </summary>
    public class ScaleApproximated : ConfigBase {
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
        /// <param name="baseLimits">������� �������� �����</param>
        /// <param name="normalized">��������������� ������������� �����</param>
        public ScaleApproximated(ScaleNormalizeClause clause, IEnumerable<double> baseLimits, ScaleNormalized normalized) {
            Clause = clause;
            BaseValues = baseLimits;
            Normalized = normalized;
        }
        /// <summary>
        ///     ��������� ������������ ������������ ��������
        /// </summary>
        /// <param name="maximals">������������ ������������ ��������</param>
        public void SetMaximals(IEnumerable<double> maximals) {
            Maximals = maximals;
        }
        /// <summary>
        ///     ��������� ������������ ����������� ��������
        /// </summary>
        /// <param name="minimals">������������ ����������� ��������</param>
        public void SetMinimals(IEnumerable<double> minimals) {
            Minimals = minimals;
        }
        /// <summary>
        ///     ���������� ���������������� ��������
        /// </summary>
        /// <param name="variant">������������� ��������</param>
        public void AddVariant(ScaleNormalizedVariant variant) {
            Normalized.AddVariant(variant);
        }
        /// <summary>
        ///     ���������� ���������������� ��������
        /// </summary>
        /// <param name="minimal">����������� ��������</param>
        /// <param name="maximal">������������ ��������</param>
        /// <param name="divlines">���������� ���������</param>
        public void AddVariant(double minimal, double maximal, int divlines) {
            AddVariant(new ScaleNormalizedVariant { Divline = divlines, Minimal = minimal, Maximal = maximal });
        }
        /// <summary>
        ///     ��������� ��������� ��������� ������
        /// </summary>
        /// <param name="exception">����������, �������������� ������</param>
        /// <param name="runErrorBehavior">������� ����, ��� ����� ��������� �������� ������� �� ����</param>
        public void Error(Exception exception, bool runErrorBehavior) {
            IsError = true;
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
    }
}