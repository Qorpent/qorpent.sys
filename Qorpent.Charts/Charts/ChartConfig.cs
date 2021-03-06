﻿using System;

namespace Qorpent.Charts {
    /// <summary>
    ///     Имплементация конфига чарта
    /// </summary>
    public class ChartConfig : Scope, IChartConfig {
		
        /// <summary>
        ///     Тип чарта
        /// </summary>
        public string Type {
            get { return Get<string>(ChartDefaults.ChartTypeAttributeName); }
            set { Set(ChartDefaults.ChartTypeAttributeName, value); }
        }
        /// <summary>
        ///     Ширина чарта
        /// </summary>
        public string Width {
            get { return Get(ChartDefaults.ChartWidthAttributeName, "800"); }
            set { Set(ChartDefaults.ChartWidthAttributeName, value); }
        }
        /// <summary>
        ///     Высота чарта
        /// </summary>
        public string Height {
            get { return Get(ChartDefaults.ChartHeightAttributeName, "300"); }
            set { Set(ChartDefaults.ChartHeightAttributeName, value); }
        }
        /// <summary>
        ///     Максимальное значение чарта
        /// </summary>
        public string MaxValue {
            get { return Get<string>(ChartDefaults.ChartMaxValueAttributeName); }
            set { Set(ChartDefaults.ChartMaxValueAttributeName, value); }
        }

	    /// <summary>
	    ///     Минимальное значение чарта
	    /// </summary>
	    public object NativeResult {
            get { return Get<object>(ChartDefaults.NativeResult); }
            set { Set(ChartDefaults.NativeResult, value); }
        }
        /// <summary>
        ///     Минимальное значение чарта
        /// </summary>
        public string MinValue {
            get { return Get<string>(ChartDefaults.ChartMinValueAttributeName); }
            set { Set(ChartDefaults.ChartMinValueAttributeName, value); }
        }
        /// <summary>
        ///     Режим отладки чарта
        /// </summary>
        public string Debug {
            get { return Get<string>(ChartDefaults.ChartDebugAttributeName); }
            set { Set(ChartDefaults.ChartDebugAttributeName, value); }
        }
        /// <summary>
        /// Id чарта
        /// </summary>
        public string Id {
            get { return Get<string>(ChartDefaults.ChartIdAttributeName); }
            set { Set(ChartDefaults.ChartIdAttributeName, value); }
        }
        /// <summary>
        ///     Контейнер чарта
        /// </summary>
        public string Container {
            get { return Get<string>(ChartDefaults.ChartContainerAttributeName); }
            set { Set(ChartDefaults.ChartContainerAttributeName, value); }
        }
        /// <summary>
        ///     Рендер чарта средствами svg
        /// </summary>
        public bool JavaScriptRender {
            get { return Get<bool>(ChartDefaults.ChartRenderAttributeName); }
            set { Set(ChartDefaults.ChartRenderAttributeName, value); }
        }

        /// <summary>
        /// Тип данных графика
        /// </summary>
        public string DataType {
            get { return Get<string>(ChartDefaults.ChartDataTypeAttributeName); }
            set { Set(ChartDefaults.ChartDataTypeAttributeName, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool FitAxis {
            get { return Get<bool>(ChartDefaults.FitAxis); }
            set { Set(ChartDefaults.FitAxis, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Divlines {
            get { return Get<int>(ChartDefaults.Divlines); }
            set { Set(ChartDefaults.Divlines, value); }
        }
        /// <summary>
        ///     Признак того, нужно ли удерживать шапку
        /// </summary>
        public bool KeepHead {
            get { return Get<bool>(ChartDefaults.KeepHead, true); }
            set { Set(ChartDefaults.KeepHead, value); }
        }
	    /// <summary>
	    /// 
	    /// </summary>
	    public ChartState State {
			get { return this.Ensure(ChartDefaults.ChartState, new ChartState()); }
            set { Set(ChartDefaults.ChartState, value); }    
	    }

	    /// <summary>
        ///     Указывает как показывать число
        /// </summary>
        public ChartShowValuesAs ShowValuesAs {
            get { return Get<ChartShowValuesAs>(ChartDefaults.ShowValuesAs); }
            set { Set(ChartDefaults.ShowValuesAs, value); }
        }
        /// <summary>
        ///     Признак того, что нужно использовать скалинг значений по умолчанию.
        /// </summary>
        public bool UseDefaultScaling { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ChartConfig() {
            FitAxis = true;
            ShowValuesAs = ChartShowValuesAs.AsIs;
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="error"></param>
		public static IChartConfig Create(Exception error) {
			IChartConfig result = null;
			if (error is ChartException) {
				result = ((ChartException) error).Config ?? new ChartConfig();

			} else {
				result = new ChartConfig();
			}
			result.State.Level = ErrorLevel.Fatal;
			result.State.Exception = error;
			return result;
		}

    }
}
