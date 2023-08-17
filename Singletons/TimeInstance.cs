using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;

namespace counterstrikeWarTeamMaker.Singletons
{
    public class TimeInstance
    {
        private static int elapsedTime = 0;
        private  Timer _timer;
        private  TextBlock _textBlock;
        private static TimeInstance instance;
        private static object _deadLock = new object();
        public static TimeInstance Instance 
        { 
            get
            {
                lock (_deadLock)
                {
                    if (instance == null)
                    {
                        instance = new TimeInstance();
                    }
                    return instance;
                }
            } 
        }

        public  void StartTimer()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += TimerElapsed;
        
            _timer.Start();
        }
        public void GetElapsedTime(TextBlock txt)
        {
            _textBlock = txt;
        }
        public  void StopTimer()
        {
            _timer.Stop();
        }

        private  void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            elapsedTime++;
            if (_textBlock != null)
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);
                _textBlock.Dispatcher.Invoke(() =>
                {
                    _textBlock.Text = timeSpan.ToString("hh\\:mm\\:ss");
                });
            }
        }
    }
}
