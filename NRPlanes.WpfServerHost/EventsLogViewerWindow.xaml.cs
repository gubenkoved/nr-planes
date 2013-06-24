using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NRPlanes.WpfServerHost
{
    /// <summary>
    /// Interaction logic for EventsLogViewerWindow.xaml
    /// </summary>
    public partial class EventsLogViewerWindow : Window
    {
        private IEnumerable<object> m_objects;

        public EventsLogViewerWindow(IEnumerable<object> objects)
        {
            m_objects = objects;

            InitializeComponent();

            ObjectsList.ItemsSource = objects;
        }
    }
}
