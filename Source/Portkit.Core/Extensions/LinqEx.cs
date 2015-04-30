using System;
using System.Collections.Generic;
using System.Linq;

namespace Portkit.Core.Extensions
{
    public static class LinqEx
    {
        public static double WeightedAverage<T>(this IEnumerable<T> records, Func<T, double> value, Func<T, double> weight)
        {
            var dataPoints = records as T[] ?? records.ToArray();
            double weightedValueSum = dataPoints.Sum(x => value(x) * weight(x));
            double weightSum = dataPoints.Sum(weight);
            return weightedValueSum / weightSum;
        }
    }
}
