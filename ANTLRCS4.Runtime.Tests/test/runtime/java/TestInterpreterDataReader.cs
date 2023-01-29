/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using System.Reflection;
using System.Text;

namespace org.antlr.v4.test.runtime.java;

/** This file represents a simple sanity checks on the parsing of the .interp file
 *  available to the Java runtime for interpreting rather than compiling and executing parsers.
 */
public class TestInterpreterDataReader {
    [TestMethod]
    public void testParseFile() {
		Grammar g = new Grammar(
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
		String interpString = Tool.generateInterpreterData(g);
        string interpFile = Environment.CurrentDirectory + "test-" + Random.Shared.Next() + ".txt";// File.createTempFile(null, null);
		File.WriteAllBytes(interpFile, Encoding.UTF8.GetBytes(interpString));

        InterpreterDataReader.InterpreterData interpreterData = InterpreterDataReader.parseFile(interpFile.ToString());
        FieldInfo atnField = interpreterData.GetType().GetField("atn");
        FieldInfo vocabularyField = interpreterData.GetType().GetField("vocabulary");
        FieldInfo ruleNamesField = interpreterData.GetType().GetField("ruleNames");
        FieldInfo channelsField = interpreterData.GetType().GetField("channels");
        FieldInfo modesField = interpreterData.GetType().GetField("modes");

        //atnField.setAccessible(true);
        //vocabularyField.setAccessible(true);
        //ruleNamesField.setAccessible(true);
        //channelsField.setAccessible(true);
        //modesField.setAccessible(true);

        ATN atn = (ATN) atnField.GetValue(interpreterData);
        Vocabulary vocabulary = (Vocabulary) vocabularyField.GetValue(interpreterData);
		String[] literalNames = ((VocabularyImpl) vocabulary).getLiteralNames();
		String[] symbolicNames = ((VocabularyImpl) vocabulary).getSymbolicNames();
		List<String> ruleNames = castList<String>(ruleNamesField.GetValue(interpreterData), typeof(String));
        List<String> channels = castList<String>(channelsField.GetValue(interpreterData), typeof(String));
        List<String> modes = castList<String>(modesField.GetValue(interpreterData),typeof( String));

		Assert.AreEqual(6, vocabulary.getMaxTokenType());
		Assert.IsTrue(Enumerable.SequenceEqual(new String[]{"s","expr"}, ruleNames.ToArray()));
        Assert.IsTrue(Enumerable.SequenceEqual(new String[]{"", "", "'*'", "'/'", "'+'", "'-'", ""}, literalNames));
        Assert.IsTrue(Enumerable.SequenceEqual(new String[]{"", "INT", "MUL", "DIV", "ADD", "SUB", "WS"}, symbolicNames));
		Assert.IsNull(channels);
		Assert.IsNull(modes);

		IntegerList serialized = ATNSerializer.getSerialized(atn);
		Assert.AreEqual(ATNDeserializer.SERIALIZED_VERSION, serialized.get(0));
    }

    private List<T> castList<T>(Object obj, Type clazz) {
        List<T> result = new ();
        if (obj is List<T> list) {
            foreach (var o in list) {
                result.Add((T)o);
            }
            return result;
        }
        return null;
    }
}
