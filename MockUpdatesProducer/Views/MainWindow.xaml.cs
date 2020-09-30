using System.Collections.Generic;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Extensions;
using Kafka.Public;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using UpdatesConsumer;

namespace MockUpdatesProducer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IKafkaProducer<string, Update> _producer;
        private Dictionary<UpdateType, Update> _updates;

        public MainWindow()
        {
            InitializeComponent();
            
            
        }
    }
}