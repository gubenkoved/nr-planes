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

namespace NRPlanes.WpfServerHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServiceHost _host;
        private GameService _service;

        public ObservableCollection<Parameter> Parameters { get; private set; }

        private Parameter _statusParameter = new Parameter("Status", "Not started");
        private Parameter _gameObjectsAmountParameter = new Parameter("Game object amount");
        private Parameter _serverFPSParameter = new Parameter("Server FPS");

        public MainWindow()
        {
            Parameters = new ObservableCollection<Parameter>();

            InitializeComponent();

            Parameters.Add(_statusParameter);
            Parameters.Add(_gameObjectsAmountParameter);
            Parameters.Add(_serverFPSParameter);
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
                        _service = new GameService(logString => { Invoke(() =>
                                {
                                    Log.Items.Add(logString);
                                    LogScrollView.ScrollToEnd();
                                }); });

                        _host = new ServiceHost(_service);
                        
                        _host.Open();

                        int port = _host.Description.Endpoints.Single().ListenUri.Port;

                        string info = string.Format("Started on {0} port", port);

                        Invoke(() => _statusParameter.Value = info);

                        Task.Factory.StartNew(() => UpdateParamters());
                    }
                    catch (Exception exc)
                    {
                        Invoke(() => _statusParameter.Value = string.Format("Error ({0})", exc.Message));
                    }
                });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_host != null)
                _host.Close();
        }

        private void UpdateParamters()
        {
            while (true)
            {
                Invoke(() => _gameObjectsAmountParameter.Value = _service.World.GameObjects.Count());
                Invoke(() => _serverFPSParameter.Value = _service.FPS.ToString("F2"));

                Thread.Sleep(100);
            }
        }
    }
}
