﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.BrickScaleNormalizer {
    /// <summary>
    ///     Набор расширений для <see cref="BrickDataSet"/>
    /// </summary>
    public static class BrickDataSetExtensions {
        /// <summary>
        ///     Возвращает упорядоченное перечисление серий из <see cref="BrickDataSet"/>
        /// </summary>
        /// <param name="dataSet">Датасет</param>
        /// <returns>Упорядоченное перечисление серий из <see cref="BrickDataSet"/></returns>
        public static IEnumerable<BrickDataSetSeria> GetSeries(this BrickDataSet dataSet) {
            return dataSet.Series;
        }
        /// <summary>
        ///     Собирает упорядоченное перечисление колонок данных графика в виде <see cref="DataItemColon"/>
        /// </summary>
        /// <param name="dataSet">Датасет</param>
        /// <returns>Упорядоченное перечисление колонок данных графика в виде <see cref="DataItemColon"/></returns>
        public static IEnumerable<DataItemColon> GetColons(this BrickDataSet dataSet) {
            var subresult = dataSet.DataItems.GroupBy(_ => _.Idx).Select(_ => _.Select(__ => __).ToArray());
	        foreach (var ds in subresult){
		        yield return new DataItemColon(ds);
	        }
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataset"></param>
		public static void PrintDataset(this BrickDataSet dataset) {
			foreach (var seria in dataset.Series) {
				Console.WriteLine("Seria: " + seria.SeriaNumber);
				foreach (var row in seria.Rows) {
					Console.WriteLine("\tRow: {0}, Sum: {1}, Scale: {2}", row.RowNumber, row.Sum(_ => _.Value), row.ScaleType.ToString());
					foreach (var item in row) {
						Console.WriteLine("\t\tItem: {0}, Value: {1}", item.Idx, item.Value);
					}
				}
			}
		}
		/// <summary>
		///		Безопасная печать <see cref="BrickDataSet.Preferences"/> с проверкой на NOT NULL
		/// </summary>
		/// <param name="dataSet">Исходный датасет</param>
		public static void PrintPreferences(this BrickDataSet dataSet) {
			if (dataSet != null && dataSet.Preferences != null) {
				dataSet.Preferences.PrintPreferences();
			}
		}
		/// <summary>
		///		Печатает на экран предпочтения пользователя
		/// </summary>
		/// <param name="preferences">Исходный экземпляр предпочтений пользователя</param>
		public static void PrintPreferences(this UserPreferences preferences) {
			Console.WriteLine(preferences.AsString());
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="preferences"></param>
		/// <returns></returns>
		public static string AsString(this UserPreferences preferences) {
			var result = string.Empty;
			foreach (var property in preferences.GetType().GetProperties()) {
				var value = property.GetValue(preferences);
				if (value == null) {
					continue;
				}
				result += string.Format("{0}: {1},\n", property.Name, value.ToString());
			}
			return result;
		}
    }
}