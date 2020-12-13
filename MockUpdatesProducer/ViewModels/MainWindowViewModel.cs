using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using Common;
using Extensions;
using Kafka.Public;
using Microsoft.Extensions.Logging;
using MockUpdatesProducer.Annotations;
using UpdatesConsumer;

namespace MockUpdatesProducer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ICommand ProduceCommand { get; }

        public int Rows { get; } = 4;

        public int Columns { get; } = 3;
        
        private ObservableCollection<UpdateTypeButton> _updateTypes;

        public ObservableCollection<UpdateTypeButton> UpdateTypes
        {
            get => _updateTypes;
            set
            {
                _updateTypes = value;
                OnPropertyChanged();
            } 
        }

        private string _name = "Mock";

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private readonly IKafkaProducer<string, Update> _producer;
        private readonly Dictionary<UpdateType, Update> _updates;

        public MainWindowViewModel()
        {
            ProduceCommand = new DelegateCommand(Produce);

            UpdateTypes = CreateUpdateTypes();

            _producer = CreateProducer();

            _updates = CreateUpdates();
        }

        private static ObservableCollection<UpdateTypeButton> CreateUpdateTypes()
        {
            var updateTypes = (UpdateType[]) (Enum.GetValues(typeof(UpdateType)));

            IEnumerable<UpdateTypeButton> updateTypeButtons = updateTypes
                .Select(type => new UpdateTypeButton(type));
            
            return new ObservableCollection<UpdateTypeButton>(updateTypeButtons);
        }

        private static IKafkaProducer<string, Update> CreateProducer()
        {
            var baseKafkaConfig = new BaseKafkaConfig
            {
                BrokersServers = "localhost:9092",
                Topic = "updates",
                KeySerializationType = SerializationType.String,
                ValueSerializationType = SerializationType.Json
            };

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new CustomConsoleLoggerProvider());

            return KafkaProducerFactory.Create<string, Update>(
                baseKafkaConfig,
                loggerFactory,
                new JsonSerializerOptions
                {
                    Converters = { new MediaJsonConverter() }
                });
        }

        private static Dictionary<UpdateType, Update> CreateUpdates()
        {
            var audio = new Audio(
                "https://awaod01.streamgates.net/103fm_aw/nis1109206.mp3?aw_0_1st.collectionid=nis&aw_0_1st.episodeid=109206&aw_0_1st.skey=1599814244&listeningSessionID=5f159c950b71b138_191_254__54fddcd17821d4ada536bb55cbcd9a3084e57e35",
                string.Empty,
                TimeSpan.FromMinutes(1),
                "Title",
                "Artist");
            
            var photo = new Photo(
                "https://www.creare.co.uk/wp-content/uploads/2016/02/google-1018443_1920.png");
            
            var video = new Video(
                "http://mirror.bigbuckbunny.de/peach/bigbuckbunny_movies/big_buck_bunny_720p_surround.avi",
                string.Empty);
            
            return new()
            {
                {
                    UpdateType.Text,
                    new Update
                    {
                        AuthorId = "MockUser",
                        Content = "Mock update"
                    }
                },
                {
                    UpdateType.TextWithUrl,
                    new Update
                    {
                        AuthorId = "MockUser",
                        Content = "Mock update",
                        Url = "https://mock-url.com"
                    }
                },
                {
                    UpdateType.Audio,
                    new Update
                    {
                        AuthorId = "MockUser",
                        Media = new List<IMedia>
                        {
                            audio
                        }
                    }
                },
                {
                    UpdateType.AudioWithDetails,
                    new Update
                    {
                        AuthorId = "MockUser",
                        Content = "Mock audio",
                        Url = "https://mock-url.com",
                        Media = new List<IMedia>
                        {
                            audio
                        }
                    }
                },
                {
                    UpdateType.Photo,
                    new Update
                    {
                        AuthorId = "MockUser",
                        Media = new List<IMedia>
                        {
                            photo
                        }
                    }
                },
                {
                    UpdateType.PhotoWithDetails,
                    new Update
                    {
                        AuthorId = "MockUser",
                        Content = "Mock photo",
                        Url = "https://mock-url.com",
                        Media = new List<IMedia>
                        {
                            photo
                        }
                    }
                },
                {
                    UpdateType.Video,
                    new Update
                    {
                        AuthorId = "MockUser",
                        Media = new List<IMedia>
                        {
                            video
                        }
                    }
                },
                {
                    UpdateType.VideoWithDetails,
                    new Update
                    {
                        AuthorId = "MockUser",
                        Content = "Mock video",
                        Url = "https://mock-url.com",
                        Media = new List<IMedia>
                        {
                            video
                        }
                    }
                },
                {
                    UpdateType.MultipleMedia,
                    new Update
                    {
                        AuthorId = "MockUser",
                        Media = new List<IMedia>
                        {
                            video,
                            photo,
                            audio
                        }
                    }
                },
                {
                    UpdateType.MultipleMediaWithDetails,
                    new Update
                    {
                        AuthorId = "MockUser",
                        Content = "Mock multiple media",
                        Url = "https://mock-url.com",
                        Media = new List<IMedia>
                        {
                            video,
                            photo,
                            audio
                        }
                    }
                }
            };
        }

        private void Produce(object o)
        {
            var type = (UpdateType) o;

            _producer.Produce(Name, _updates[type]);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}