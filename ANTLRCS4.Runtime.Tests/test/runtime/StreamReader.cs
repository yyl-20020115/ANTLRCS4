/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.test.runtime;

public class RunnableStreamReader : Runnable
{
    private readonly StringBuilder buffer = new ();
    private readonly TextReader reader;
    private readonly Thread worker;

    public RunnableStreamReader(TextReader reader)
    {
        this.reader = reader;
        worker = new Thread(this.Run);
    }

    public void Start()
    {
        worker.Start();
    }

    ////@Override
    public void Run()
    {
        try
        {
            while (true)
            {
                int c = reader.Read();
                if (c == -1)
                {
                    break;
                }
                if (c == '\r')
                {
                    continue;
                }
                buffer.Append((char)c);
            }
        }
        catch (IOException ioe)
        {
            Console.Error.WriteLine("can't read output from process");
        }
    }

    /** wait for the thread to finish */
    public void Join()
    {
        worker.Join();
    }

    ////@Override
    public override string ToString() => buffer.ToString();
}
