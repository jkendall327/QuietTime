using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QuietTime.Other
{
    class ScaleValueHelper<T>
    {
        /*
         * Source for scaling UI: https://www.inchoatethoughts.com/scaling-your-user-interface-in-a-wpf-application
         * I don't fully understand this, I'm not sure I want to fully understand this, but it seems to work OK.
         */

        public DependencyProperty Get()
        {
            UIPropertyMetadata metadata = new UIPropertyMetadata(
                            1.0,
                            new PropertyChangedCallback(OnScaleValueChanged),
                            new CoerceValueCallback(OnCoerceScaleValue));

            return DependencyProperty.Register("ScaleValue", typeof(double), typeof(T), metadata);
        }

        private void OnScaleValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

        }

        public double CalculateScale(double actualHeight, double actualWidth, DependencyObject obj)
        {
            double yScale = actualHeight / 400.0d;
            double xScale = actualWidth / 500.0d;
            double value = Math.Min(xScale, yScale);
            return (double)OnCoerceScaleValue(obj, value);
        }

        private object OnCoerceScaleValue(DependencyObject obj, object value)
        {
            if (obj is T window)
            {
                return OnCoerceScaleValue((double)value);
            }

            return value;
        }

        protected virtual double OnCoerceScaleValue(double value)
        {
            if (double.IsNaN(value))
                return 1.0d;

            return Math.Max(0.1, value);
        }
    }
}
