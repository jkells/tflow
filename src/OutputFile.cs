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
using System.IO;

namespace TFlow
{
    /// <summary>
    /// Code for writing the CSV file.
    /// </summary>
    public class OutputFile : IDisposable
    {
        private string _filename;
        private readonly ILinkFile _linkFile;
        private StreamWriter _writer;
        
        public OutputFile(ILinkFile linkFile)
        {
            _linkFile = linkFile;
        }

        public bool Open(string filename)
        {
            bool existingFile = File.Exists(filename);
            try
            {
                _filename = filename;
                _writer = new StreamWriter(_filename, true);

                if (existingFile)
                {
                    Console.WriteLine("Output file already exists, appending to existing file.");
                    return true;
                }

                WriteHeader();
                return true;
            }
            catch(Exception exception)
            {
                Console.WriteLine("Error opening file: {0}. {1}", _filename, exception.Message);
                return false;
            }
        }

        // Header of the file is the form "Timestampe,hostname1,hostname2 etc"
        public void WriteHeader()
        {
            _writer.Write("Timestamp");
            for (int i =0; i < _linkFile.Uris.Count; i++)
            {
                _writer.Write(",");

                var hostname = _linkFile.Uris[i].Host;
                _writer.Write(hostname);
            }
            _writer.WriteLine();
            _writer.Flush();
        }

        public void OutputResult(Dictionary<Uri, int> results)
        {
            _writer.Write(DateTime.Now);
            for (int i =0; i < _linkFile.Uris.Count; i++)
            {
                _writer.Write(",");
                
                var uri = _linkFile.Uris[i];
                if(results.ContainsKey(uri))
                {
                    _writer.Write(results[uri]);
                }
            }
            _writer.WriteLine();
            _writer.Flush();
        }
        public void Dispose()
        {
            if(_writer != null)
                _writer.Dispose();
        }
    }
}
