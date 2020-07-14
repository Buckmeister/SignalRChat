using Microsoft.AspNetCore.SignalR.Client;
using System;
using ChatClient.Models;
using Microsoft.AspNetCore.Http.Connections;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatClient.Input;
using ChatClient.ErrorHandling;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;

namespace ChatClient.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {   
            Output = new ObservableCollection<string>();
            Output.CollectionChanged += (o,e) => OnPropertyChanged(nameof(Output));

            CurrentMessage = new MessageModel { Username = "Chat Client", Content = "Welcome!" };
            CurrentMoodMessage = new MoodMessageModel { Caption = "Upload Your Mood Image" };

            IsServerUrlEnabled = true;
            IsUsernameEnabled = true;
            IsLoginDefault = true;
        }

        private HubConnection hubConnection { get; set; }

        private string _serverUrl;
        public string ServerUrl
        {
            get => _serverUrl; 
            set 
            { 
                _serverUrl = value;
                OnPropertyChanged(nameof(ServerUrl));
            }
        }

        private string _username;
        public string Username 
        {
            get => _username;
            set
            { 
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public ObservableCollection<string> Output { get; private set; }

        private string _newMessageContent;
        public string NewMessageContent
        {
            get => _newMessageContent;
            set
            {
                _newMessageContent = value;
                OnPropertyChanged(nameof(NewMessageContent));
            }
        }

        private MessageModel _currentMessage;
        public MessageModel CurrentMessage
        {
            get => _currentMessage;
            set
            {
                _currentMessage = value;
                OnPropertyChanged(nameof(CurrentMessage));
                Output.Add($"{_currentMessage.Username} says: {_currentMessage.Content}");
            }
        }


        // This is the object that is going to be sent to, and received from the server
        private MoodMessageModel _currentMoodMessage;
        public MoodMessageModel CurrentMoodMessage
        {
            get { return _currentMoodMessage; }
            set
            {
                _currentMoodMessage = value;
                OnPropertyChanged(nameof(CurrentMoodMessage));
                SetCurrentImageFromMoodMessage();
            }
        }

        // This is the binding source of the view's image control
        private BitmapImage  _currentImage;
        public BitmapImage CurrentImage
        {
            get { return _currentImage; }
            set
            {
                _currentImage = value;
                OnPropertyChanged(nameof(CurrentImage));
            }
        }


        private string _newCaption;
        public string NewCaption
        {
            get { return _newCaption; }
            set
            {
                _newCaption = value;
                OnPropertyChanged(nameof(NewCaption));
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
            }
        }


        private bool _isLoginEnabled;
        public bool IsLoginEnabled
        {
            get => _isLoginEnabled;
            set
            {
                _isLoginEnabled = value;
                OnPropertyChanged(nameof(IsLoginEnabled));
            }
        }

        private bool _isLoginDefault;
        public bool IsLoginDefault
        {
            get => _isLoginDefault;
            set
            {
                _isLoginDefault = value;
                OnPropertyChanged(nameof(IsLoginDefault));
            }
        }

        private bool _isSendMessageEnabled;
        public bool IsSendMessageEnabled
        {
            get => _isSendMessageEnabled;
            set
            {
                _isSendMessageEnabled = value;
                OnPropertyChanged(nameof(IsSendMessageEnabled));
            }
        }

        private bool _isSendMessageDefault;
        public bool IsSendMessageDefault
        {
            get => _isSendMessageDefault;
            set
            {
                _isSendMessageDefault = value;
                OnPropertyChanged(nameof(IsSendMessageDefault));
            }
        }

        private bool _isSendMoodMessageEnabled;
        public bool IsSetMoodMessageEnabled
        {
            get { return _isSendMoodMessageEnabled; }
            set
            {
                _isSendMoodMessageEnabled = value;
                OnPropertyChanged(nameof(IsSetMoodMessageEnabled));
            }
        }


        private bool _loggedIn;
        public bool LoggedIn
        {
            get => _loggedIn;
            set
            {
                _loggedIn = value;
                OnPropertyChanged(nameof(LoggedIn));
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));

                CommandManager.InvalidateRequerySuggested();
            }
        }

        private bool _isServerUrlEnabled;
        public bool IsServerUrlEnabled
        {
            get => _isServerUrlEnabled;
            set
            {
                _isServerUrlEnabled = value;
                OnPropertyChanged(nameof(IsServerUrlEnabled));
            }
        }

        private bool _isUsernameEnabled;
        public bool IsUsernameEnabled
        {
            get => _isUsernameEnabled;
            set
            {
                _isUsernameEnabled = value;
                OnPropertyChanged(nameof(IsUsernameEnabled));
            }
        }

        private IAsyncCommand _loginCommand;
        public IAsyncCommand LoginCommand
        {
            get
            {
                return _loginCommand ??
                    (
                    _loginCommand = new AsyncCommand(LoginExecuteAsync, LoginCanExecute, new ConsoleErrorHandler())
                    );
            }
        }

        private bool LoginCanExecute()
        {
            if (string.IsNullOrEmpty(ServerUrl) || string.IsNullOrEmpty(Username))
            {
                IsLoginEnabled = false;
                return false;
            }

            if (LoggedIn)
            {
                IsLoginEnabled = false;
                return false;
            }

            if (IsBusy)
            {
                IsLoginEnabled = false;
                return false;
            }

            IsLoginEnabled = true;
            return true;
        }

        private async Task LoginExecuteAsync()
        {
            try
            {
                IsBusy = true;

                if (await StartConnectionAsync())
                {
                    Output.Add("Login successful.");
                    LoggedIn = true; 
                    IsSendMessageDefault = true;
                    IsServerUrlEnabled = false; 
                    IsUsernameEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Output.Add("Login not successful.");
                Console.WriteLine("Error: " + ex.Message);
                Output.Add(ex.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }

        private IAsyncCommand _sendMessageCommand;
        public IAsyncCommand SendMessageCommand
        {
            get
            {
                return _sendMessageCommand ??
                    (
                    _sendMessageCommand = new AsyncCommand(SendMessageExecuteAsync, SendMessageCanExecute, new ConsoleErrorHandler())
                    );
            }
        }

        private bool SendMessageCanExecute()
        {
            if (string.IsNullOrEmpty(NewMessageContent))
            {
                IsSendMessageEnabled = false;
                return false;
            }

            if (!LoggedIn)
            {
                IsSendMessageEnabled = false;
                return false;
            }

            if (IsBusy)
            {
                IsSendMessageEnabled = false;
                return false;
            }

            IsSendMessageEnabled = true;
            return true;
        }

        private Task SendMessageExecuteAsync()
        {
            string user = Username;
            string message = NewMessageContent;
            NewMessageContent = string.Empty;
            return SendMessageAsync(user, message);
        }

        private IAsyncCommand _sendMoodMessageCommand;
        public IAsyncCommand SendMoodMessageCommand
        {
            get
            {
                return _sendMoodMessageCommand ??
                    (
                    _sendMoodMessageCommand = new AsyncCommand(SendMoodMessageExecuteAsync, SendMoodMessageCanExecute)
                    );
            }
        }

        public bool SendMoodMessageCanExecute()
        {
            if (!LoggedIn)
            {
                IsSetMoodMessageEnabled = false;
                return false;
            }

            if (IsBusy)
            {
                IsSetMoodMessageEnabled = false;
                return false;
            }

            if (string.IsNullOrEmpty(NewCaption))
            {
                IsSetMoodMessageEnabled = false;
                return false;
            }

            if (!File.Exists(ImagePath))
            {
                IsSetMoodMessageEnabled = false;
                return false;
            }

            IsSetMoodMessageEnabled = true;
            return true;
        }

        public async Task SendMoodMessageExecuteAsync()
        {
            string user = Username;
            string caption = NewCaption;
            byte[] imageData = File.ReadAllBytes(ImagePath);
            
            NewCaption = string.Empty;
            ImagePath = string.Empty;

            await SendMoodMessageAsync(user, caption, imageData);
        }

        
        private void SetCurrentImageFromMoodMessage()
        {
            if (CurrentMoodMessage.ImageData == null) return;

            try
            {
                using var imageStream = new MemoryStream(CurrentMoodMessage.ImageData);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource= imageStream;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
                bi.Freeze();
                CurrentImage = bi;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while decoding Mood Image: {ex.Message}");
                Output.Add($"Error while decoding Mood Image: {ex.Message}");
            }
            
        }

        private async Task<bool> StartConnectionAsync()
        {
            try
            {
                hubConnection = new HubConnectionBuilder()
                .WithUrl(ServerUrl, HttpTransportType.WebSockets)
                .AddMessagePackProtocol()
                .WithAutomaticReconnect()
                .Build();

                await hubConnection.StartAsync();

                hubConnection.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await StartConnectionAsync();
                };

                hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
                {
                    CurrentMessage = new MessageModel
                    {
                        Username = user,
                        Content = message
                    };
                });

                hubConnection.On<string, string, byte[]>("ReceiveMoodMessage", (user, caption, imageData) =>
                {
                    CurrentMoodMessage = new MoodMessageModel { Caption=caption, ImageData=imageData};
                    CurrentMessage = new MessageModel{ Username=user, Content="I've set a new Mood Image."};
                });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Output.Add(ex.Message);
                return false;
            }
        }

        private async Task SendMessageAsync(string username, string content)
        {
            try
            {
                await hubConnection.InvokeAsync("SendMessage", username, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Output.Add(ex.Message);
            }
        }

        private async Task SendMoodMessageAsync(string username, string caption, byte[] imageData)
        {
            try
            {
                await hubConnection.InvokeAsync("SendMoodMessage", username, caption, imageData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Output.Add(ex.Message);
            }
        }
    }
}
