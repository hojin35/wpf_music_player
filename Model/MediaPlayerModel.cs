using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
namespace WPFMvvMTest.Model
{
    public class MediaPlayerModel
    {
        public TimeSpan EndTime
        {
            get;set;
        }
        public TimeSpan Timer
        {
            get;set;
        }
        public string TimerFormat
        {
            get { return Timer.ToString(@"m\:ss"); }
        }
    }

    public class MP3FileList : ObservableCollection<MP3File>
    {
        
    }
    public class MP3File
    {
        public string Url { get;set;}
        public string FileName { get;set;}
    }
}
