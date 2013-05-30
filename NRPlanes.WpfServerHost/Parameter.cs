using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NRPlanes.WpfServerHost
{
    public class Parameter : DependencyObject
    {
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Parameter));

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(Parameter));

        public Parameter(string name)
            :this(name, "undefinded")
        {
        }

        public Parameter(string name, object value)
        {
            Name = name;

            Value = value;
        }
    }
}
