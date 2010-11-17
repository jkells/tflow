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
using System.Threading;

namespace TFlow
{
    /// <summary>
    /// Little class to count down on the console.
    /// </summary>
    public static class CountDown
    {
        public static void Wait(int seconds)
        {
            var endTime = DateTime.Now.AddSeconds(seconds);

            while(DateTime.Now < endTime)
            {
                var remaining = endTime - DateTime.Now;
                if(remaining.Seconds > 1)
                {
                    Console.Write(new string('\b', 60));
                    if (remaining.Minutes > 0)
                    {
                        Console.Write("{0} minutes and {1} seconds until next test.", remaining.Minutes,
                                      remaining.Seconds);
                    }
                    else
                    {
                        Console.Write("{0} seconds until next test.", remaining.Seconds);
                    }
                }
                Thread.Sleep(500);
            }
            Console.Write(new string('\b', 60));
        }
    }
}
