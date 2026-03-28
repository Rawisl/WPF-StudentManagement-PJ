using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace WPF_StudentManagement_Project.Services
{
    class TextBoxHelper
    {
        // --- 1. THUỘC TÍNH CHỈ NHẬP SỐ NGUYÊN ---
        public static readonly DependencyProperty IsNumericOnlyProperty =
            DependencyProperty.RegisterAttached("IsNumericOnly", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(false, OnIsNumericOnlyChanged));

        public static void SetIsNumericOnly(UIElement element, bool value) => element.SetValue(IsNumericOnlyProperty, value);
        public static bool GetIsNumericOnly(UIElement element) => (bool)element.GetValue(IsNumericOnlyProperty);

        private static void OnIsNumericOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue) textBox.PreviewTextInput += BlockNonNumeric;
                else textBox.PreviewTextInput -= BlockNonNumeric;
            }
        }

        private static void BlockNonNumeric(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        // --- 2. THUỘC TÍNH NHẬP SỐ THẬP PHÂN ---
        public static readonly DependencyProperty IsDecimalOnlyProperty =
            DependencyProperty.RegisterAttached("IsDecimalOnly", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(false, OnIsDecimalOnlyChanged));

        public static void SetIsDecimalOnly(UIElement element, bool value) => element.SetValue(IsDecimalOnlyProperty, value);
        public static bool GetIsDecimalOnly(UIElement element) => (bool)element.GetValue(IsDecimalOnlyProperty);

        private static void OnIsDecimalOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue) textBox.PreviewTextInput += BlockNonDecimal;
                else textBox.PreviewTextInput -= BlockNonDecimal;
            }
        }

        private static void BlockNonDecimal(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");
            if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
                return;
            }

            if (sender is TextBox textBox && (e.Text == "." || e.Text == ","))
            {
                if (textBox.Text.Contains(".") || textBox.Text.Contains(","))
                    e.Handled = true;
            }
        }
    }
}
