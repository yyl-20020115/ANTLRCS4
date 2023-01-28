/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;

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
		Path interpFile = Files.createTempFile(null, null);
		Files.write(interpFile, interpString.getBytes(StandardCharsets.UTF_8));

        InterpreterDataReader.InterpreterData interpreterData = InterpreterDataReader.parseFile(interpFile.ToString());
        Field atnField = interpreterData.getClass().getDeclaredField("atn");
        Field vocabularyField = interpreterData.getClass().getDeclaredField("vocabulary");
        Field ruleNamesField = interpreterData.getClass().getDeclaredField("ruleNames");
        Field channelsField = interpreterData.getClass().getDeclaredField("channels");
        Field modesField = interpreterData.getClass().getDeclaredField("modes");

        atnField.setAccessible(true);
        vocabularyField.setAccessible(true);
        ruleNamesField.setAccessible(true);
        channelsField.setAccessible(true);
        modesField.setAccessible(true);

        ATN atn = (ATN) atnField.get(interpreterData);
        Vocabulary vocabulary = (Vocabulary) vocabularyField.get(interpreterData);
		String[] literalNames = ((VocabularyImpl) vocabulary).getLiteralNames();
		String[] symbolicNames = ((VocabularyImpl) vocabulary).getSymbolicNames();
		List<String> ruleNames = castList(ruleNamesField.get(interpreterData), String);
        List<String> channels = castList(channelsField.get(interpreterData), String);
        List<String> modes = castList(modesField.get(interpreterData), String);

		Assert.AreEqual(6, vocabulary.getMaxTokenType());
		assertArrayEquals(new String[]{"s","expr"}, ruleNames.ToArray());
		assertArrayEquals(new String[]{"", "", "'*'", "'/'", "'+'", "'-'", ""}, literalNames);
		assertArrayEquals(new String[]{"", "INT", "MUL", "DIV", "ADD", "SUB", "WS"}, symbolicNames);
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
