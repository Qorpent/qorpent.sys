using System;
using System.Linq;
using Qorpent.Utils.BrickScaleNormalizer;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.Scaling;

namespace Qorpent.Charts.FusionCharts {
    /// <summary>
    /// 
    /// </summary>
    public class FusionChartsScaleNormalizer : FusionChartsAbstractNormalizer {
        /// <summary>
        /// 
        /// </summary>
        public FusionChartsScaleNormalizer() {
            Code = FusionChartsNormalizerCodes.ScaleNormalizer;
            Area = ChartNormalizerArea.YScale;
            AddDependency(FusionChartsNormalizerCodes.FusionChartsValuesNormalizer);
        }
        /// <summary>
        ///     Нормализация чарта
        /// </summary>
        /// <param name="chart">Представление исходного чарта</param>
        /// <param name="normalized">абстрактное представление нормализованного чарта</param>
        /// <returns>Замыкание на абстрактное представление нормализованного чарта</returns>
        public override IChartNormalized Normalize(IChart chart, IChartNormalized normalized) {
            normalized.AddScale(NormalizeYAxis(chart, normalized));
            return normalized;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="normalized"></param>
        /// <returns></returns>
        private ChartAbstractScale NormalizeYAxis(IChart chart, IChartNormalized normalized) {
	        var values = normalized.GetFixedAttributes<decimal>(FusionChartApi.Set_Value).Select(Convert.ToDouble).ToArray();
	        var brickRequest = new BrickRequest();
	        brickRequest.SourceMaxValue = Convert.ToDecimal(values.Max());
	        brickRequest.SourceMinValue = Convert.ToDecimal(values.Min());
	        brickRequest.Size = chart.EnsureConfig().Height.ToInt();
			var keephead = chart.EnsureConfig().KeepHead;
	        if (keephead) {
		        brickRequest.MinPixelTop = BrickRequest.DefaultPixelTopHat;
	        }
	        var requestedMinValue  = chart.EnsureConfig().MinValue.ToInt();
			if (requestedMinValue == -1) {
				brickRequest.MinimalScaleBehavior  = MiniamlScaleBehavior.FitMin;
			}else if (requestedMinValue != 0) {
				if (requestedMinValue > brickRequest.SourceMinValue) {
					brickRequest.MinimalScaleBehavior = MiniamlScaleBehavior.FitMin;
				}
				else {
					brickRequest.SourceMinValue = requestedMinValue;
					brickRequest.MinimalScaleBehavior = MiniamlScaleBehavior.MatchMin;
				}
			}
	        var bcatalog = new BrickCatalog();
	        var variant = bcatalog.GetBestVariant(brickRequest);
            
            return new ChartAbstractScale {
                NumDivLines = variant.ResultDivCount,
                MaxValue = Convert.ToDouble(variant.ResultMaxValue),
                MinValue = Convert.ToDouble(variant.ResultMinValue)
            };
        }
    }
}