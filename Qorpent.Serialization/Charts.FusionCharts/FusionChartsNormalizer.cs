using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Config;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    ///     
    /// </summary>
    public class FusionChartNormalizer : ConfigBase {
        private readonly IList<IChartNormalizer> _normalizers = new List<IChartNormalizer>();
        /// <summary>
        ///     Добавочная коллекция нормалайзеров
        /// </summary>
        public IList<IChartNormalizer> Normalizers {
            get { return _normalizers; }
        }
        /// <summary>
        ///     Произвести нормализацию чарта
        /// </summary>
        /// <param name="chart">Чарт</param>
        /// <returns>Нормализованный чарт</returns>
        public IChart Normalize(IChart chart) {
            var normalized = new ChartNormalized();
            
            GetWellKnownNormalizers().Union(Normalizers).DoForEach(
                _ => _.Normalize(chart, normalized)
            );

            normalized.Apply(chart);

            return chart;
        }
        /// <summary>
        ///     Создаёт перечисление хорошо известных нормалайзеров
        /// </summary>
        /// <returns>Перечисление хорошо известных нормалайзеров</returns>
        private IEnumerable<IChartNormalizer> GetWellKnownNormalizers() {
            yield return new FusionChartsScaleNormalizer();
            yield return new FusionChartsAnchorsNormalizer();
            yield return new FusionChartsColorNormalizer();
            yield return new FusionChartsNumberScalingNormalizer();
            yield return new FusionChartsPositionNormalizer();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class FusionChartsNormalizeFactory {
        /// <summary>
        ///     Представление списка нормалайзеров, исключённых из выполнения вызывающим компонентом
        /// </summary>
        private readonly IList<int> _excludedNormalizers = new List<int>();
        /// <summary>
        ///     Внутренняя коллекция нормалайзеров, которые будут использоваться для построения плана нормализации
        /// </summary>
        private readonly IList<IChartNormalizer> _normalizers = new List<IChartNormalizer> {
            new FusionChartsScaleNormalizer(),
            new FusionChartsAnchorsNormalizer(),
            new FusionChartsColorNormalizer(),
            new FusionChartsNumberScalingNormalizer(),
            new FusionChartsPositionNormalizer()
        };
        /// <summary>
        ///     Перечисление нормалайзеров
        /// </summary>
        public IEnumerable<IChartNormalizer> Normalizers {
            get { return _normalizers.AsEnumerable(); }
        }
        /// <summary>
        ///     Добавление дополнительного нормалайзера в коллекцию
        /// </summary>
        /// <param name="normalizer"></param>
        public void AddNormalizer(IChartNormalizer normalizer) {
            if (normalizer.Code < 1000) {
                throw new Exception("You attempt to set a normalizer which have the code number from the base space");
            }

            _normalizers.Add(normalizer);
        }
        /// <summary>
        ///     Исключение нормалайзера из плана выполнения по коду
        /// </summary>
        /// <param name="code">Код нормалайзера для исключения</param>
        public void ExcludeNormalizer(int code) {
            if (!_excludedNormalizers.Contains(code)) {
                _excludedNormalizers.Add(code);
            }
        }
        /// <summary>
        ///     Прокат операции выполнения нормализации 
        /// </summary>
        /// <param name="chart">Представление чарта, который требуется нормализовать</param>
        /// <param name="config">Словарь конфигов для нормалиаторов типа КОД_НОРМАЛАЙЗЕРА:КОНФИГ</param>
        /// <returns></returns>
        public IChart Normalize(IChart chart, IDictionary<int, IConfig> config = null) {
            if (config == null) {
                config = new Dictionary<int, IConfig>();
            }

            var schedule = MakeSchedule(chart, config);
            
            ExecuteSchedule(schedule);
            ApplyResult(schedule);

            return chart;
        }
        /// <summary>
        ///     Сборка плана нормализации исходя из переданного контекста
        /// </summary>
        /// <param name="chart">Представление графика, реализующего <see cref="IChart"/>, над которым будет производиться нормализация</param>
        /// <param name="config">Опционайльный конфиг нормализатора</param>
        /// <returns>Настроенный план нормализации, готовый к <see cref="ChartNormalizeSchedule.Execute"/></returns>
        private ChartNormalizeSchedule MakeSchedule(IChart chart, IDictionary<int, IConfig> config = null) {
            var schedule = new ChartNormalizeSchedule(chart);

            foreach (var normalizer in Normalizers) {
                
            }

            return schedule;
        }
        /// <summary>
        ///     Поиск и настройка нормалайзера по переданному конфигу
        /// </summary>
        /// <param name="code">Код нормалайзера для настройки</param>
        /// <param name="config">Конфиг для нормалайзера</param>
        /// <returns>Настроенный экземпляр нормалайзера</returns>
        private IChartNormalizer SetupNormalizer(int code, IConfig config = null) {
            var normalizer = Normalizers.FirstOrDefault(_ => _.Code == code);

            if (
                (normalizer != null)
                    &&
                (config != null)
            ) {
                normalizer.SetParent(config);
            }

            return normalizer;
        }
        /// <summary>
        ///     Выполнение плана нормализации по переданному контексту нормализации
        /// </summary>
        /// <param name="schedule">План нормализации</param>
        private void ExecuteSchedule(ChartNormalizeSchedule schedule) {
            schedule.Execute();
        }
        /// <summary>
        ///     Применение результата выполнения плана нормализации
        /// </summary>
        /// <param name="schedule">План нормализации</param>
        private void ApplyResult(ChartNormalizeSchedule schedule) {
            schedule.Apply();
        }
        /// <summary>
        ///     Определяет признак исключённости нормалайзера по коду из плана выполнения
        /// </summary>
        /// <param name="code">Код нормалайзера</param>
        /// <returns>Признак исключённости нормалайзера по коду из плана выполнения</returns>
        private bool IsExcludedNormalizer(int code) {
            return _excludedNormalizers.Contains(code);
        }
    }
    /// <summary>
    ///     План нормализации чарта
    /// </summary>
    internal class ChartNormalizeSchedule {
        /// <summary>
        ///     Внутренний экземпляр представления нормализованного чарта
        /// </summary>
        private readonly IChartNormalized _normalized;
        /// <summary>
        ///     Внутренний список нормализаторов, расположенных в порядке выполнения
        /// </summary>
        private readonly IList<IChartNormalizer> _normalizers;
        /// <summary>
        ///     Чарт, над которым производится нормализация
        /// </summary>
        private readonly IChart _chart;
        /// <summary>
        ///     План нормализации чарта
        /// </summary>
        /// <param name="chart">Чарт, над которым производится нормализация</param>
        public ChartNormalizeSchedule(IChart chart) {
            _chart = chart;
            _normalized = new ChartNormalized();
            _normalizers = new List<IChartNormalizer>();
        }
        /// <summary>
        ///     Добавление настроенного нормалайзера в план нормализации
        /// </summary>
        /// <param name="normalizer">Настроенный экземпляр класса, реализующего <see cref="IChartNormalizer"/></param>
        public void AddNormalizer(IChartNormalizer normalizer) {
            _normalizers.Add(normalizer);
        }
        /// <summary>
        ///     Применение результата выполнения к исходному чарту
        /// </summary>
        /// <returns>Замыкание на исходный чарт, к которому применйн результат нормализации</returns>
        public IChart Apply() {
            return _normalized.Apply(_chart);
        }
        /// <summary>
        ///     Выполнение плана нормализации
        /// </summary>
        public void Execute() {
            _normalizers.DoForEach(_ => _.Normalize(_chart, _normalized));
        }
        /// <summary>
        ///     Возвращает экземпляр класса, представляющее нормализованное представление чарта
        /// </summary>
        /// <returns>Экземпляр класса, представляющее нормализованное представление чарта</returns>
        public IChartNormalized GetNormalized() {
            return _normalized;
        }
        /// <summary>
        ///     Возвращает перечисление настроенных нормализаторов
        /// </summary>
        /// <returns>Перечисление настроенных нормализаторов</returns>
        public IEnumerable<IChartNormalizer> GetNormalizers() {
            return _normalizers.AsEnumerable();
        }
    }
}
