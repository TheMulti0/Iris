using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
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

        private readonly IKafkaProducer<string, InternalUpdate> _producer;
        private readonly Dictionary<UpdateType, InternalUpdate> _updates;

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

        private static IKafkaProducer<string, InternalUpdate> CreateProducer()
        {
            var baseKafkaConfig = new BaseKafkaConfig
            {
                BrokersServers = "localhost:9092",
                DefaultTopic = "updates",
                KeySerializationType = SerializationType.String,
                ValueSerializationType = SerializationType.Json
            };

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new CustomConsoleLoggerProvider());

            return KafkaProducerFactory.Create<string, InternalUpdate>(
                baseKafkaConfig,
                loggerFactory,
                new JsonSerializerOptions());
        }

        private static Dictionary<UpdateType, InternalUpdate> CreateUpdates() => new Dictionary<UpdateType, InternalUpdate>
        {
            {
                UpdateType.Text,
                new InternalUpdate
                {
                    AuthorId = "MockUser",
                    Content = "Mock update"
                }
            },
            {
                UpdateType.TextWithUrl,
                new InternalUpdate
                {
                    AuthorId = "MockUser",
                    Content = "Mock update",
                    Url = "https://mock-url.com"
                }
            },
            {
                UpdateType.Audio,
                new InternalUpdate
                {
                    AuthorId = "MockUser",
                    Media = new List<InternalMedia>
                    {
                        new InternalMedia
                        {
                            _type = "Audio",
                            Url = "https://awaod01.streamgates.net/103fm_aw/nis1109206.mp3?aw_0_1st.collectionid=nis&aw_0_1st.episodeid=109206&aw_0_1st.skey=1599814244&listeningSessionID=5f159c950b71b138_191_254__54fddcd17821d4ada536bb55cbcd9a3084e57e35",
                            ThumbnailUrl = string.Empty,
                            DurationSeconds = 60,
                            Title = "Title",
                            Artist = "Artist"
                        }
                    }
                }
            },
            {
                UpdateType.AudioWithDetails,
                new InternalUpdate
                {
                    AuthorId = "MockUser",
                    Content = "Mock audio",
                    Url = "https://mock-url.com",
                    Media = new List<InternalMedia>
                    {
                        new InternalMedia
                        {
                            _type = "Audio",
                            Url = "https://awaod01.streamgates.net/103fm_aw/nis1109206.mp3?aw_0_1st.collectionid=nis&aw_0_1st.episodeid=109206&aw_0_1st.skey=1599814244&listeningSessionID=5f159c950b71b138_191_254__54fddcd17821d4ada536bb55cbcd9a3084e57e35",
                            DurationSeconds = 60,
                            ThumbnailUrl = string.Empty,
                            Title = "Title",
                            Artist = "Artist"
                        } 
                    }
                }
            },
            {
                UpdateType.Photo,
                new InternalUpdate
                {
                    AuthorId = "MockUser",
                    Media = new List<InternalMedia>
                    {
                        new InternalMedia
                        {
                            _type = "Photo",
                            Url = "https://www.creare.co.uk/wp-content/uploads/2016/02/google-1018443_1920.png",
                            ThumbnailUrl = string.Empty
                        } 
                    }
                }
            },
            {
                UpdateType.PhotoWithDetails,
                new InternalUpdate
                {
                    AuthorId = "MockUser",
                    Content = "Mock photo",
                    Url = "https://mock-url.com",
                    Media = new List<InternalMedia>
                    {
                        new InternalMedia
                        {
                            _type = "Photo",
                            Url = "https://www.creare.co.uk/wp-content/uploads/2016/02/google-1018443_1920.png",
                            ThumbnailUrl = string.Empty
                        } 
                    }
                }
            },
            {
                UpdateType.Video,
                new InternalUpdate
                {
                    AuthorId = "MockUser",
                    Media = new List<InternalMedia>
                    {
                        new InternalMedia
                        {
                            _type = "Photo",
                            Url = "https://www.creare.co.uk/wp-content/uploads/2016/02/google-1018443_1920.png",
                            ThumbnailUrl = string.Empty
                        } 
                    }
                }
            },
            {
                UpdateType.VideoWithDetails,
                new InternalUpdate
                {
                    AuthorId = "MockUser",
                    Content = "Mock video",
                    Url = "https://mock-url.com",
                    Media = new List<InternalMedia>
                    {
                        new InternalMedia
                        {
                            _type = "Photo",
                            Url = "https://www.creare.co.uk/wp-content/uploads/2016/02/google-1018443_1920.png",
                            ThumbnailUrl = string.Empty
                        } 
                    }
                }
            },
            {
                UpdateType.MultipleMedia,
                new InternalUpdate
                {
                    AuthorId = "MockUser",
                    Media = new List<InternalMedia>
                    {
                        new InternalMedia
                        {
                            _type = "Video",
                            Url = "http://www.sample-videos.com/video123/mp4/720/big_buck_bunny_720p_1mb.mp4",
                            ThumbnailUrl = string.Empty
                        }, 
                        new InternalMedia
                        {
                            _type = "Photo",
                            Url = "https://www.creare.co.uk/wp-content/uploads/2016/02/google-1018443_1920.png",
                            ThumbnailUrl = string.Empty
                        } 
                    }
                }
            },
            {
                UpdateType.MultipleMediaWithDetails,
                new InternalUpdate
                {
                    AuthorId = "MockUser",
                    Content = "Mock multiple media",
                    Url = "https://mock-url.com",
                    Media = new List<InternalMedia>
                    {
                        new InternalMedia
                        {
                            _type = "Video",
                            Url = "http://www.sample-videos.com/video123/mp4/720/big_buck_bunny_720p_1mb.mp4",
                            ThumbnailUrl = string.Empty
                        }, 
                        new InternalMedia
                        {
                            _type = "Photo",
                            Url = "https://www.creare.co.uk/wp-content/uploads/2016/02/google-1018443_1920.png",
                            ThumbnailUrl = string.Empty
                        } 
                    }
                }
            }
        };

        private void Produce(object o)
        {
            var type = (UpdateType) o;

            _producer.Produce("Mock", _updates[type]);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}