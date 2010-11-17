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
    public interface ILinkFile
    {
        IList<Uri> Uris { get; }
        bool Load(string filename);
    }

    /// <summary>
    /// Represents the collection of links in the link file.
    /// </summary>
    public class LinkFile : ILinkFile
    {
        public IList<Uri> Uris { get; private set; }
        public LinkFile()
        {
            Uris = new List<Uri>();
        }


        public bool Load(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine("Link file does not exist.");
                return false;
            }

            try
            {
                var lines = ReadFile(filename);
                for (int i = 0; i < lines.Count; i++ )
                {
                    var line = lines[i];
                    if (Uri.IsWellFormedUriString(line, UriKind.Absolute))
                    {
                        Uris.Add(new Uri(line));
                    }
                    else
                    {
                        Console.WriteLine("Reading link file. Invalid URI line: {0} Uri: {1}", i, line);
                    }
                }
                
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error reading link file: " + exception.Message);
                return false;
            }

            if (Uris.Count == 0)
            {
                Console.WriteLine("No valid URI's in link file.");
                return false;
            }

            return true;
        }

        private static IList<string> ReadFile(string filename)
        {
            var links = new List<string>();
            using (var fs = new StreamReader(filename))
            {
                while (!fs.EndOfStream)
                {
                    links.Add(fs.ReadLine().Trim());
                }
            }
            return links;
        }
    }
}
