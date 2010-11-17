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
using NDesk.Options;

namespace TFlow
{
    public class CommandLine
    {
        public string LinkFile { get; private set; }
        public string OutputFile { get; private set; }
        public int MaxWait { get; set; }
        public int MaxDownload { get; set; }
        public int SampleCount { get; set; }
        public int SampleSleep { get; set; }

        private bool _help;
        private readonly OptionSet _options;
        
        public CommandLine()
        {
            _options = new OptionSet
                           {
                               {"l|link-file=", "Link file containing a collection of download links", x => LinkFile = x},
                               {"o|output-file=", "File to write output. Output is in csv format", x => OutputFile = x},
                               {"w|max-wait=", "Maximum time to spend downloading each link in seconds. Default: 15", x => MaxWait = int.Parse(x)},
                               {"d|max-download=", "Maximum number of bytes to download from each link. Default 8000000(8MB)",x => MaxDownload = int.Parse(x)},
                               {"h|?|help", "Display this help message.", x => _help=true },
                               {"c|sample-count=", "Number of times to take a sample of all the urls.", x=> SampleCount = int.Parse(x)},
                               {"s|sample-sleep=", "How long to sleep between samples in seconds.", x=> SampleSleep = int.Parse(x)},
                           };

            MaxWait = 15;
            MaxDownload = 8000000;
            SampleCount = 1;
            SampleSleep = 0;
        }
    

        public bool Parse(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("TFlow is an application to help you monitor download speeds over time to a number of hosts.");
                Console.WriteLine("Run with --help for options.");
                return false;
            }

            try
            {
                _options.Parse(args);
            }
            catch(Exception)
            {
                Console.WriteLine("Error parsing command line arguments.");
                Console.WriteLine("--help for options.");
                return false;
            }

            if(_help)
            {
                _options.WriteOptionDescriptions(Console.Out);
                return false;
            }

            if (string.IsNullOrEmpty(LinkFile))
            {
                Console.WriteLine("You must specify a link file. See --help for more information.");
                return false;
            }

            if (string.IsNullOrEmpty(OutputFile))
            {
                Console.WriteLine("You must specify an output file. See --help for more information.");
                return false;
            }
            return true;
        }
    }
}
