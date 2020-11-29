using System.Collections.Generic;
using System.Windows;
using Kafka.Public;
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