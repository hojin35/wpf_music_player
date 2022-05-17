using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using WPFMvvMTest.Model;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO;
using WPFMvvMTest.Service;

namespace WPFMvvMTest.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private MediaPlayer mediaPlayer1 = new MediaPlayer();
        private readonly MP3FileList fileList = new MP3FileList();
        int thisSong = 0;
        public MediaPlayer MediaPlayer1
        {
            set
            {
                mediaPlayer1 = value;
            }
            get
            {
                return mediaPlayer1;
            }
        }

        private double volume;
        public double Volume
        {
            get
            {
                return volume;
            }
            set
            {
                volume = value;
                OnPropertyChanged("Volume");
                MediaPlayer1.Volume = volume / 100.0;
            }
        }
        private bool isChecked;

        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
        private double progress;
        public double Progress
        {
            get
            {
                if (MediaPlayer1.NaturalDuration.HasTimeSpan == false)
                    return 0;
                else
                    return MediaPlayer1.Position.TotalMilliseconds / MediaPlayer1.NaturalDuration.TimeSpan.TotalMilliseconds * 100.0;
            }
            set
            {
                progress = value;
                OnPropertyChanged("Progress");
                MediaPlayer1.Position = MediaPlayer1.NaturalDuration.TimeSpan / 100.0 * progress;

            }
        }

        private ICommand playCommand;
        public ICommand PlayCommand
        {
            get { return (this.playCommand) ?? (this.playCommand = new DelegateCommand(Play)); }
        }
        DispatcherTimer timer = new DispatcherTimer();

        private void Play()
        {
            MediaPlayer1.Play();
        }
        private void TimerTick(object sender,EventArgs e)
        {
            if (MediaPlayer1.NaturalDuration.HasTimeSpan == false)
                return;

            this.Item.Timer = MediaPlayer1.NaturalDuration.TimeSpan / 100.0 * progress;

            // POSITION 갱신 만들기 
            progress = MediaPlayer1.Position.TotalMilliseconds / MediaPlayer1.NaturalDuration.TimeSpan.TotalMilliseconds * 100.0;
           
            //if (MediaPlayer1.Position >= this.item.EndTime)
            if(progress == 100)
            {
                Stop();

                // CHECK Continuous 
                if(isChecked)
                {
                    MediaPlayer1.Close();
                    if (fileList.Count() - 1 <= thisSong)
                    {
                        thisSong = -1;
                    }

                    MediaPlayer1.Open(new Uri(fileList[++thisSong].Url,UriKind.Absolute));
                    Items = fileList;
                    progress = 0;
                    Play();
                }
            }
            OnPropertyChanged("Progress");
            OnPropertyChanged("Item");
        }
        private ICommand pauseCommand;
        public ICommand PauseCommand
        {
            get { return (this.pauseCommand) ?? (this.pauseCommand = new DelegateCommand(Pause)); }
        }
        private void Pause()
        {
            MediaPlayer1.Pause();
        }
        private ICommand stopCommand;
        public ICommand StopCommand
        {
            get { return (this.stopCommand) ?? (this.stopCommand = new DelegateCommand(Stop)); }
        }
        private void Stop()
        {
            MediaPlayer1.Stop();
        }
        private readonly MediaPlayerModel item;

        public MediaPlayerModel Item
        {
            get { return this.item; }
        }
        public MainViewModel()
        {
            this.item = new MediaPlayerModel();
            Volume = 100;
            fileList = Mp3Service.Mp3Init();

            MediaPlayer1.Open(new Uri(fileList[thisSong].Url, UriKind.Absolute));
            Items = fileList;
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += new EventHandler(TimerTick);
            // if (MediaPlayer1.NaturalDuration.HasTimeSpan)
                // this.Item.EndTime = MediaPlayer1.NaturalDuration.TimeSpan;
            // Remove : 연속 재생시 무한 오류때문에 넣어둠
            //this.Item.EndTime = TimeSpan.FromSeconds(10);
            timer.Start();
        }

        MP3FileList showList = new MP3FileList();

        public MP3FileList Items
        {
            get 
            {
                return showList;
            }
            set
            {
                showList.Clear();
                int startPoint = thisSong - 2 > 0 ? thisSong - 2 : 0;
                for (int i = startPoint; i <= thisSong + 2; i++)
                {
                    if (fileList.Count <= i)
                        break;
                    showList.Add(fileList[i]);
                }
                OnPropertyChanged("Items");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
    }
    #region DelegateCommand Class
    public class DelegateCommand : ICommand
    {
        private readonly Func<bool> canExecute;
        private readonly Action execute;

        public DelegateCommand(Action execute) : this(execute, null)
        {

        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object o)
        {
            if (this.canExecute == null)
            {
                return true;
            }
            return this.canExecute();
        }

        public void Execute(object o)
        {
            this.execute();
        }

        public void RaiseCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
    #endregion
}
