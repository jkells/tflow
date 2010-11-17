// Copyright (C) 2010 Jared Kells (jkells@gmail.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace TFlow
{
    public interface IDownloadTester
    {
        double AverageSpeed { get; }
        long BytesDownloaded { get; }
        bool RunTest();
    }

    /// <summary>
    /// Test an individual download URI
    /// </summary>
    public class DownloadTester : IDownloadTester
    {
        private readonly int _maxDownloadBytes;
        private readonly int _maxWait;
        private readonly Uri _uri;
        private readonly object _progressLock = new object();

        private readonly IList<KeyValuePair<DateTime, long>> _progressRecord = new List<KeyValuePair<DateTime, long>>();
        private readonly AutoResetEvent _wait = new AutoResetEvent(false);
        
        public double AverageSpeed { get; private set; }
        public long BytesDownloaded { get; private set; }

        public DownloadTester(int maxWait, int maxDownloadBytes, Uri uri)
        {
            _maxWait = maxWait > 1 ? maxWait : 1;
            _maxDownloadBytes = maxDownloadBytes > 1000 ? maxDownloadBytes : 1000;
            _uri = uri;
        }

        public bool RunTest()
        {
            var webClient = new WebClient();

            webClient.DownloadProgressChanged += DownloadProgressChanged;
            webClient.DownloadStringCompleted += DownloadStringCompleted;
            webClient.DownloadStringAsync(_uri);

            // Here we wait for the download to complete or timeout.
            _wait.WaitOne(_maxWait * 1000);

            webClient.CancelAsync();
            while(webClient.IsBusy)
            {
                Thread.Sleep(10);
            }

            // Clear junk we were spitting out while it was downloading
            ClearLine();

            if (_progressRecord.Count > 2)
            {
                AverageSpeed = CalculateAverageSpeed();
                BytesDownloaded = _progressRecord.Last().Value;
                return true;
            }

            return false;
        }

        private void DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            _wait.Set();
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Hit here from multiple threads.
            lock (_progressLock)
            {
                _progressRecord.Add(new KeyValuePair<DateTime, long>(DateTime.Now, e.BytesReceived));
                OutputRunningTotal();    
            }
            
            if (e.BytesReceived >= _maxDownloadBytes)
            {
                _wait.Set();
            }
        }

        // Helper to clear the line.
        private static void ClearLine()
        {
            Console.Write(new string('\b', 60));
            Console.Write(new string(' ', 60));
            Console.Write(new string('\b', 60));
        }

        private void OutputRunningTotal()
        {
            if(_progressRecord.Count > 2)
            {
                var speed = (int) CalculateAverageSpeed()/1000;
                ClearLine();
                Console.Write("{0}: {1} kB/s",_uri.Host, speed);
            }
        }
        
        private double CalculateAverageSpeed()
        {
            int totalRecords = _progressRecord.Count;

            var start = _progressRecord[0];
            var finish = _progressRecord[totalRecords - 1];

            var milliseconds = (finish.Key - start.Key).TotalMilliseconds;
            var bytes = finish.Value - start.Value;

            return bytes / (milliseconds / 1000.0);
        }
    }
}
