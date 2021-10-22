using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

#nullable enable

namespace GlanceScreen
{
    /// <summary>
    /// The generic base implementation of a value converter.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TTarget">The target type.</typeparam>
    public abstract class ValueConverter<TSource, TTarget>
        : IValueConverter
    {
        /// <summary>
        /// Converts a source value to the target type.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TTarget? Convert(TSource? value)
        {
            return Convert(value, null, null);
        }

        /// <summary>
        /// Converts a target value back to the source type.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TSource? ConvertBack(TTarget? value)
        {
            return ConvertBack(value, null, null);
        }

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object? Convert(object? value, Type? targetType, object? parameter, string? language)
        {
            // CastExceptions will occur when invalid value, or target type provided.
            return Convert((TSource?)value, parameter, language);
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object? ConvertBack(object? value, Type? targetType, object? parameter, string? language)
        {
            // CastExceptions will occur when invalid value, or target type provided.
            return ConvertBack((TTarget?)value, parameter, language);
        }

        /// <summary>
        /// Converts a source value to the target type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        protected virtual TTarget? Convert(TSource? value, object? parameter, string? language)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Converts a target value back to the source type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        protected virtual TSource? ConvertBack(TTarget? value, object? parameter, string? language)
        {
            throw new NotSupportedException();
        }
    }

    public class IntegerHigherThanNilToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return false;
            }
            return (int)value > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class IntegerIsFourToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return false;
            }
            return (int)value == 4;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class IntegerIsNotFourToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return false;
            }
            return (int)value != 4;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }


    public class StringNullOrEmptyToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; } = Visibility.Collapsed;
        public Visibility FalseValue { get; set; } = Visibility.Visible;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return TrueValue;
            }
            return string.IsNullOrEmpty(value.ToString()) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class BooleanToOpacityConverter : IValueConverter
    {
        public BooleanToOpacityConverter() { }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return 0.3;

            if (value is bool && (bool)value)
            {
                return 1.0;
            }
            return 0.3;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((double)value == 1.0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Converts between Boolean and Visibility values. 
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts true values to Visibility.Visible and false values to
        /// Visibility.Collapsed, or the reverse if the parameter is "Reverse".
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language) =>
            (bool)value ^ (parameter as string ?? string.Empty).Equals("Reverse") ?
                Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Converts Visibility.Visible values to true and Visibility.Collapsed 
        /// values to false, or the reverse if the parameter is "Reverse".
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            (Visibility)value == Visibility.Visible ^ (parameter as string ?? string.Empty).Equals("Reverse");
    }

    public class NullToVisibilityConverter : IValueConverter
    {
        public Visibility NullValue { get; set; } = Visibility.Collapsed;
        public Visibility NonNullValue { get; set; } = Visibility.Visible;


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value == null) ? NullValue : NonNullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
