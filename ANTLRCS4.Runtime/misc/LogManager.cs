/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.runtime.misc;

public class LogManager
{
    public class Record
    {
        public long timestamp;
        public StackTraceElement location;
        public string component;
        public string msg;
        public Record()
        {
            timestamp = DateTime.Now.Millisecond;
            location = null;// new Exception().StackTrace;//.getStackTrace()[0];
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            //SimpleDateFormat("yyyy-MM-dd HH:mm:ss:SSS").format(new Date(timestamp))
            buffer.Append(DateTime.Now.ToLongTimeString());
            buffer.Append(' ');
            buffer.Append(component);
            buffer.Append(' ');
            buffer.Append(location.GetFileName());
            buffer.Append(':');
            buffer.Append(location.GetLineNumber());
            buffer.Append(' ');
            buffer.Append(msg);
            return buffer.ToString();
        }
    }

    protected List<Record> records;

    public void Log(string component, string msg)
    {
        var r = new Record
        {
            component = component,
            msg = msg
        };
        if (records == null)
        {
            records = new();
        }
        records.Add(r);
    }

    public void Log(string msg) { Log(null, msg); }

    public void Save(string filename)
    {
        File.WriteAllText(filename, ToString());
    }

    public string Save()
    {
        //string dir = System.getProperty("java.io.tmpdir");
        var dir = ".";
        var defaultFilename =
            dir + "/antlr-" +
            DateTime.Now.ToString("yyyy-MM-dd-HH.mm.ss") + ".log";
        // new SimpleDateFormat("yyyy-MM-dd-HH.mm.ss").format(new DateTime())
        Save(defaultFilename);
        return defaultFilename;
    }

    public override string ToString()
    {
        if (records == null) return "";
        var nl = Environment.NewLine;
        var buffer = new StringBuilder();
        foreach (var r in records)
        {
            buffer.Append(r);
            buffer.Append(nl);
        }
        return buffer.ToString();
    }

    public static void TestMain(string[] args)
    {
        var mgr = new LogManager();
        mgr.Log("atn", "test msg");
        mgr.Log("dfa", "test msg 2");
        Console.WriteLine(mgr);
        mgr.Save();
    }
}
