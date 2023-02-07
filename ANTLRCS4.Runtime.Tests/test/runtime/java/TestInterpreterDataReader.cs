/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using System.Text;

namespace org.antlr.v4.test.runtime.java;

/** This file represents a simple sanity checks on the parsing of the .interp file
 *  available to the Java runtime for interpreting rather than compiling and executing parsers.
 */
[TestClass]
public class TestInterpreterDataReader
{
    [TestMethod]
    public void TestParseFile()
    {
        var g = new Grammar(
                "grammar Calc;\n" +
                "s :  expr EOF\n" +
                "  ;\n" +
                "expr\n" +
                "  :  INT            # number\n" +
                "  |  expr (MUL | DIV) expr  # multiply\n" +
                "  |  expr (ADD | SUB) expr  # add\n" +
                "  ;\n" +
                "\n" +
                "INT : [0-9]+;\n" +
                "MUL : '*';\n" +
                "DIV : '/';\n" +
                "ADD : '+';\n" +
                "SUB : '-';\n" +
                "WS : [ \\t]+ -> channel(HIDDEN);");
        var interpString = Tool.generateInterpreterData(g);
        var interpFile = Environment.CurrentDirectory + "test-" + Random.Shared.Next() + ".txt";// File.createTempFile(null, null);
        File.WriteAllBytes(interpFile, Encoding.UTF8.GetBytes(interpString));

        var interpreterData = InterpreterDataReader.ParseFile(interpFile.ToString());
        var t = interpreterData.GetType();
        var atnField = t.GetField("atn");
        var vocabularyField = t.GetField("vocabulary");
        var ruleNamesField = t.GetField("ruleNames");
        var channelsField = t.GetField("channels");
        var modesField = t.GetField("modes");

        //atnField.setAccessible(true);
        //vocabularyField.setAccessible(true);
        //ruleNamesField.setAccessible(true);
        //channelsField.setAccessible(true);
        //modesField.setAccessible(true);

        var atn = (ATN)atnField.GetValue(interpreterData);
        var vocabulary = (Vocabulary)vocabularyField.GetValue(interpreterData);
        var literalNames = ((VocabularyImpl)vocabulary).getLiteralNames();
        var symbolicNames = ((VocabularyImpl)vocabulary).getSymbolicNames();
        var ruleNames = CastList<string>(ruleNamesField.GetValue(interpreterData));
        var channels = CastList<string>(channelsField.GetValue(interpreterData));
        var modes = CastList<string>(modesField.GetValue(interpreterData));

        Assert.AreEqual(6, vocabulary.GetMaxTokenType());
        Assert.IsTrue(Enumerable.SequenceEqual(new string[] { "s", "expr" }, ruleNames.ToArray()));
        Assert.IsTrue(Enumerable.SequenceEqual(new string[] { "", "", "'*'", "'/'", "'+'", "'-'", "" }, literalNames));
        Assert.IsTrue(Enumerable.SequenceEqual(new string[] { "", "INT", "MUL", "DIV", "ADD", "SUB", "WS" }, symbolicNames));
        Assert.IsNull(channels);
        Assert.IsNull(modes);

        var serialized = ATNSerializer.GetSerialized(atn);
        Assert.AreEqual(ATNDeserializer.SERIALIZED_VERSION, serialized.Get(0));
    }

    private static List<T> CastList<T>(object obj)
    {
        List<T> result = new();
        if (obj is List<T> list)
        {
            foreach (var o in list)
                result.Add((T)o);
            return result;
        }
        return null;
    }
}
