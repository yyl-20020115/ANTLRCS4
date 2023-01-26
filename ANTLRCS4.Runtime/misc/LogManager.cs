/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.runtime.misc;

public class LogManager {
    public class Record {
		public long timestamp;
        public StackTraceElement location;
        public String component;
        public String msg;
		public Record() {
            timestamp = DateTime.Now.Millisecond;
            location = null;// new Exception().StackTrace;//.getStackTrace()[0];
		}

		//@Override
		public override String ToString() {
            StringBuilder buf = new StringBuilder();
            //SimpleDateFormat("yyyy-MM-dd HH:mm:ss:SSS").format(new Date(timestamp))
            buf.Append(DateTime.Now.ToLongTimeString());
            buf.Append(" ");
            buf.Append(component);
            buf.Append(" ");
            buf.Append(location.getFileName());
            buf.Append(":");
            buf.Append(location.getLineNumber());
            buf.Append(" ");
            buf.Append(msg);
            return buf.ToString();
		}
	}

	protected List<Record> records;

	public void log(String component, String msg) {
		Record r = new Record();
		r.component = component;
		r.msg = msg;
		if ( records==null ) {
			records = new ();
		}
		records.Add(r);
	}

    public void log(String msg) { log(null, msg); }

    public void save(String filename){
        File.WriteAllText(filename, toString());
    }

    public String save(){
        //String dir = System.getProperty("java.io.tmpdir");
        String dir = ".";
        String defaultFilename =
            dir + "/antlr-" +
            DateTime.Now.ToString("yyyy-MM-dd-HH.mm.ss") + ".log";
        // new SimpleDateFormat("yyyy-MM-dd-HH.mm.ss").format(new DateTime())
        save(defaultFilename);
        return defaultFilename;
    }

    //@Override
    public String toString() {
        if ( records==null ) return "";
        String nl = Environment.NewLine;
        StringBuilder buf = new StringBuilder();
        foreach (Record r in records) {
            buf.Append(r);
            buf.Append(nl);
        }
        return buf.ToString();
    }

    public static void TestMain(String[] args){
        LogManager mgr = new LogManager();
        mgr.log("atn", "test msg");
        mgr.log("dfa", "test msg 2");
        Console.WriteLine(mgr);
        mgr.save();
    }
}
