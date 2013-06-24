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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using NRPlanes.Server;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;
using System.Configuration;
using NRPlanes.ServerData.EventsLog;

namespace NRPlanes.WpfServerHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServiceHost m_host;
        private GameService m_service;

        public ObservableCollection<Parameter> Parameters { get; private set; }

        private Parameter m_statusParameter = new Parameter("Status", "Not started");
        private Parameter m_gameObjectsAmountParameter = new Parameter("Game object amount");
        private Parameter m_serverFPSParameter = new Parameter("Server FPS");

        public MainWindow()
        {
            Parameters = new ObservableCollection<Parameter>();

            InitializeComponent();

            Parameters.Add(m_statusParameter);
            Parameters.Add(m_gameObjectsAmountParameter);
            Parameters.Add(m_serverFPSParameter);
        }

        private void Invoke(Action action)
        {
            Dispatcher.Invoke((Delegate)action);            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        m_service = new GameService(logString => { Invoke(() =>
                                {
                                    Log.Items.Add(logString);
                                    LogScrollView.ScrollToEnd();
                                }); });

                        m_host = new ServiceHost(m_service);
                        
                        m_host.Open();

                        int port = m_host.Description.Endpoints.Single().ListenUri.Port;

                        string info = string.Format("Started on {0} port", port);

                        Invoke(() => m_statusParameter.Value = info);

                        Task.Factory.StartNew(() => UpdateParamters());
                    }
                    catch (Exception exc)
                    {
                        Invoke(() => m_statusParameter.Value = string.Format("Error ({0})", exc.Message));
                    }
                });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (m_host != null)
                m_host.Close();
        }

        private void UpdateParamters()
        {
            while (true)
            {
                Invoke(() => m_gameObjectsAmountParameter.Value = m_service.World.GameObjectsCount);
                Invoke(() => m_serverFPSParameter.Value = m_service.FPS.ToString("F2"));

                Thread.Sleep(100);
            }
        }

        private void SnapshotButton_Click(object sender, RoutedEventArgs e)
        {
            List<object> dump = new List<object>();

            using (var handle = m_service.World.GameObjectsSafeReadHandle)
            {
                foreach (var item in handle.Items)
                {
                    dump.Add(item.Clone());
                }
            }

            SnapshotViewerWindow snapshotViewer = new SnapshotViewerWindow(dump);
            snapshotViewer.Title = string.Format("Snapshot by {0:H:mm:ss}", DateTime.Now);
            snapshotViewer.Show();
        }

        private void EventsLogButton_Click(object sender, RoutedEventArgs e)
        {
            List<GameEventsLogItem> log = m_service.WorldEventsLog.GetAll().ToList();

            EventsLogViewerWindow eventsLogViewer = new EventsLogViewerWindow(log);
            eventsLogViewer.Show();
        }
    }
}
