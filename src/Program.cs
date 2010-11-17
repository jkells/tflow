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

namespace TFlow
{
    class Program
    {
        static void Main(string[] args)
        {
            // Parse Command Line.
            var options = new CommandLine();
            if(!options.Parse(args))
                return;

            // Parse link file.
            var links = new LinkFile();
            if(!links.Load(options.LinkFile))
                return;

            MainLoop(options, links);
        }

        /// <summary>
        /// Run all the tests and then sleep for a while.
        /// </summary>
        private static void MainLoop(CommandLine options, LinkFile links)
        {
            using (var outputFile = new OutputFile(links))
            {
                if(!outputFile.Open(options.OutputFile))
                    return;

                for(int i = 0; i < options.SampleCount; i++)
                {
                    var result = RunTests(links, options);
                    outputFile.OutputResult(result);
                    CountDown.Wait(options.SampleSleep);
                }
            }
        }

        /// <summary>
        ///  Run a test for each link in the link file.
        /// </summary>
        private static Dictionary<Uri, int> RunTests(ILinkFile links, CommandLine options)
        {
            var results = new Dictionary<Uri, int>();
            foreach (var link in links.Uris)
            {
                var downloadTester = new DownloadTester(options.MaxWait, options.MaxDownload, link);
                if(!downloadTester.RunTest())
                {
                    Console.WriteLine("Error downloading url: {0}", link);
                    continue;
                }

                var result = downloadTester.AverageSpeed;
                results.Add(link, (int)result);

                Console.WriteLine("{0} {1, 35}: {2,5} kB/s", DateTime.Now, link.Host, (int)downloadTester.AverageSpeed / 1000);
            }

            return results;
        }
    }
}
