/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestLexerActions
{
    // ----- ACTIONS --------------------------------------------------------

    [TestMethod]
    public void TestActionExecutedInDFA()
    {
        var grammar =
            "lexer grammar L;\n" +
            "I : '0'..'9'+ {outStream.println(\"I\");} ;\n" +
            "WS : (' '|'\\n') -> skip ;";
        var executedState = ToolTestUtils.execLexer("L.g4", grammar, "L", "34 34");
        var expecting =
            "I\n" +
            "I\n" +
            "[@0,0:1='34',<1>,1:0]\n" +
            "[@1,3:4='34',<1>,1:3]\n" +
            "[@2,5:4='<EOF>',<-1>,1:5]\n";
        Assert.AreEqual(expecting, executedState.output);
    }

    [TestMethod]
    public void TestActionEvalsAtCorrectIndex()
    {
        var grammar =
            "lexer grammar L;\n" +
            "I : [0-9] {outStream.println(\"2nd char: \"+(char)_input.LA(1));} [0-9]+ ;\n" +
            "WS : (' '|'\\n') -> skip ;";
        var executedState = ToolTestUtils.execLexer("L.g4", grammar, "L", "123 45");
        var expecting =
            "2nd char: 2\n" +
            "2nd char: 5\n" +
            "[@0,0:2='123',<1>,1:0]\n" +
            "[@1,4:5='45',<1>,1:4]\n" +
            "[@2,6:5='<EOF>',<-1>,1:6]\n";
        Assert.AreEqual(expecting, executedState.output);
    }

    /**
	 * This is a regressing test for antlr/antlr4#469 "Not all internal lexer
	 * rule actions are executed".
	 * https://github.com/antlr/antlr4/issues/469
	 */
    [TestMethod]
    public void TestEvalMultipleActions()
    {
        var grammar =
            "lexer grammar L;\n" +
            "\n" +
            "@lexer::members\n" +
            "{\n" +
            "class Marker\n" +
            "{\n" +
            "   Marker (Lexer lexer) { this.lexer = lexer; }\n" +
            "\n" +
            "   public String getText ()\n" +
            "   {\n" +
            "      return lexer._input.getText (new Interval (start_index, stop_index));\n" +
            "   }\n" +
            "\n" +
            "   public void start ()  { start_index = lexer._input.index (); outStream.println (\"Start:\" + start_index);}\n" +
            "   public void stop () { stop_index = lexer._input.index (); outStream.println (\"Stop:\" + stop_index);}\n" +
            "\n" +
            "   private int start_index = 0;\n" +
            "   private int stop_index = 0;\n" +
            "   private Lexer lexer;\n" +
            "}\n" +
            "\n" +
            "Marker m_name = new Marker (this);\n" +
            "}\n" +
            "\n" +
            "HELLO: 'hello' WS { m_name.start (); } NAME { m_name.stop (); } '\\n' { outStream.println (\"Hello: \" + m_name.getText ()); };\n" +
            "NAME: ('a'..'z' | 'A'..'Z')+ ('\\n')?;\n" +
            "\n" +
            "fragment WS: [ \\r\\t\\n]+ ;\n";
        var executedState = ToolTestUtils.execLexer("L.g4", grammar, "L", "hello Steve\n");
        var expecting =
            "Start:6\n" +
            "Stop:11\n" +
            "Hello: Steve\n" +
            "\n" +
            "[@0,0:11='hello Steve\\n',<1>,1:0]\n" +
            "[@1,12:11='<EOF>',<-1>,2:0]\n";
        Assert.AreEqual(expecting, executedState.output);
    }

    [TestMethod]
    public void Test2ActionsIn1Rule()
    {
        var grammar =
            "lexer grammar L;\n" +
            "I : [0-9] {outStream.println(\"x\");} [0-9]+ {outStream.println(\"y\");} ;\n" +
            "WS : (' '|'\\n') -> skip ;";
        var executedState = ToolTestUtils.execLexer("L.g4", grammar, "L", "123 45");
        var expecting =
            "x\n" +
            "y\n" +
            "x\n" +
            "y\n" +
            "[@0,0:2='123',<1>,1:0]\n" +
            "[@1,4:5='45',<1>,1:4]\n" +
            "[@2,6:5='<EOF>',<-1>,1:6]\n";
        Assert.AreEqual(expecting, executedState.output);
    }

    [TestMethod]
    public void TestAltActionsIn1Rule()
    {
        var grammar =
            "lexer grammar L;\n" +
            "I : ( [0-9]+ {outStream.print(\"int\");}\n" +
            "    | [a-z]+ {outStream.print(\"id\");}\n" +
            "    )\n" +
            "    {outStream.println(\" last\");}\n" +
            "    ;\n" +
            "WS : (' '|'\\n') -> skip ;";
        var executedState = ToolTestUtils.execLexer("L.g4", grammar, "L", "123 ab");
        var expecting =
            "int last\n" +
            "id last\n" +
            "[@0,0:2='123',<1>,1:0]\n" +
            "[@1,4:5='ab',<1>,1:4]\n" +
            "[@2,6:5='<EOF>',<-1>,1:6]\n";
        Assert.AreEqual(expecting, executedState.output);
    }

    [TestMethod]
    public void TestActionPlusCommand()
    {
        var grammar =
            "lexer grammar L;\n" +
            "I : '0'..'9'+ {outStream.println(\"I\");} -> skip ;\n" +
            "WS : (' '|'\\n') -> skip ;";
        var executedState = ToolTestUtils.execLexer("L.g4", grammar, "L", "34 34");
        var expecting =
            "I\n" +
            "I\n" +
            "[@0,5:4='<EOF>',<-1>,1:5]\n";
        Assert.AreEqual(expecting, executedState.output);
    }

    // ----- COMMANDS --------------------------------------------------------

    [TestMethod]
    public void TestSkipCommand()
    {
        var grammar =
            "lexer grammar L;\n" +
            "I : '0'..'9'+ {outStream.println(\"I\");} ;\n" +
            "WS : (' '|'\\n') -> skip ;";
        var executedState = ToolTestUtils.execLexer("L.g4", grammar, "L", "34 34");
        var expecting =
            "I\n" +
            "I\n" +
            "[@0,0:1='34',<1>,1:0]\n" +
            "[@1,3:4='34',<1>,1:3]\n" +
            "[@2,5:4='<EOF>',<-1>,1:5]\n";
        Assert.AreEqual(expecting, executedState.output);
    }

    [TestMethod]
    public void TestMoreCommand()
    {
        var grammar =
            "lexer grammar L;\n" +
            "I : '0'..'9'+ {outStream.println(\"I\");} ;\n" +
            "WS : '#' -> more ;";
        var executedState = ToolTestUtils.execLexer("L.g4", grammar, "L", "34#10");
        var expecting =
            "I\n" +
            "I\n" +
            "[@0,0:1='34',<1>,1:0]\n" +
            "[@1,2:4='#10',<1>,1:2]\n" +
            "[@2,5:4='<EOF>',<-1>,1:5]\n";
        Assert.AreEqual(expecting, executedState.output);
    }

    [TestMethod]
    public void TestTypeCommand()
    {
        var grammar =
            "lexer grammar L;\n" +
            "I : '0'..'9'+ {outStream.println(\"I\");} ;\n" +
            "HASH : '#' -> type(HASH) ;";
        var executedState = ToolTestUtils.execLexer("L.g4", grammar, "L", "34#");
        var expecting =
            "I\n" +
            "[@0,0:1='34',<1>,1:0]\n" +
            "[@1,2:2='#',<2>,1:2]\n" +
            "[@2,3:2='<EOF>',<-1>,1:3]\n";
        Assert.AreEqual(expecting, executedState.output);
    }

    [TestMethod]
    public void TestCombinedCommand()
    {
        var grammar =
            "lexer grammar L;\n" +
            "I : '0'..'9'+ {outStream.println(\"I\");} ;\n" +
            "HASH : '#' -> type(100), skip, more  ;";
        var executedState = ToolTestUtils.execLexer("L.g4", grammar, "L", "34#11");
        var expecting =
            "I\n" +
            "I\n" +
            "[@0,0:1='34',<1>,1:0]\n" +
            "[@1,2:4='#11',<1>,1:2]\n" +
            "[@2,5:4='<EOF>',<-1>,1:5]\n";
        Assert.AreEqual(expecting, executedState.output);
    }

    [TestMethod]
    public void TestLexerMode()
    {
        var grammar =
            "lexer grammar L;\n" +
            "STRING_START : '\"' -> pushMode(STRING_MODE), more;\n" +
            "WS : (' '|'\\n') -> skip ;\n" +
            "mode STRING_MODE;\n" +
            "STRING : '\"' -> popMode;\n" +
            "ANY : . -> more;\n";
        var executedState = ToolTestUtils.execLexer("L.g4", grammar, "L", "\"abc\" \"ab\"");
        var expecting =
            "[@0,0:4='\"abc\"',<2>,1:0]\n" +
            "[@1,6:9='\"ab\"',<2>,1:6]\n" +
            "[@2,10:9='<EOF>',<-1>,1:10]\n";
        Assert.AreEqual(expecting, executedState.output);
    }

    [TestMethod]
    public void TestLexerPushPopModeAction()
    {
        var grammar =
            "lexer grammar L;\n" +
            "STRING_START : '\"' -> pushMode(STRING_MODE), more ;\n" +
            "WS : (' '|'\\n') -> skip ;\n" +
            "mode STRING_MODE;\n" +
            "STRING : '\"' -> popMode ;\n" +  // token type 2
            "ANY : . -> more ;\n";
        var executedState = ToolTestUtils.execLexer("L.g4", grammar, "L", "\"abc\" \"ab\"");
        var expecting =
            "[@0,0:4='\"abc\"',<2>,1:0]\n" +
            "[@1,6:9='\"ab\"',<2>,1:6]\n" +
            "[@2,10:9='<EOF>',<-1>,1:10]\n";
        Assert.AreEqual(expecting, executedState.output);
    }

    [TestMethod]
    public void TestLexerModeAction()
    {
        var grammar =
            "lexer grammar L;\n" +
            "STRING_START : '\"' -> mode(STRING_MODE), more ;\n" +
            "WS : (' '|'\\n') -> skip ;\n" +
            "mode STRING_MODE;\n" +
            "STRING : '\"' -> mode(DEFAULT_MODE) ;\n" + // ttype 2 since '"' ambiguity
            "ANY : . -> more ;\n";
        var executedState = ToolTestUtils.execLexer("L.g4", grammar, "L", "\"abc\" \"ab\"");
        var expecting =
            "[@0,0:4='\"abc\"',<2>,1:0]\n" +
            "[@1,6:9='\"ab\"',<2>,1:6]\n" +
            "[@2,10:9='<EOF>',<-1>,1:10]\n";
        Assert.AreEqual(expecting, executedState.output);
    }

    // ----- PREDICATES --------------------------------------------------------

    /**
	 * This is a regression test for antlr/antlr4#398 "Lexer: literal matches
	 * while negated char set fail to match"
	 * https://github.com/antlr/antlr4/issues/398
	 */
    [TestMethod]
    public void TestFailingPredicateEvalIsNotCached()
    {
        var grammar =
            "lexer grammar TestLexer;\n" +
            "\n" +
            "fragment WS: [ \\t]+;\n" +
            "fragment EOL: '\\r'? '\\n';\n" +
            "\n" +
            "LINE: WS? ~[\\r\\n]* EOL { !getText().trim().startsWith(\"Item:\") }?;\n" +
            "ITEM: WS? 'Item:' -> pushMode(ITEM_HEADING_MODE);\n" +
            "\n" +
            "mode ITEM_HEADING_MODE;\n" +
            "\n" +
            "NAME: ~[\\r\\n]+;\n" +
            "SECTION_HEADING_END: EOL -> popMode;\n";
        var input =
            "A line here.\n" +
            "Item: name of item\n" +
            "Another line.\n" +
            "More line.\n";
        var executedState = ToolTestUtils.execLexer("TestLexer.g4", grammar, "TestLexer", input);
        var expecting =
            "[@0,0:12='A line here.\\n',<1>,1:0]\n" +
            "[@1,13:17='Item:',<2>,2:0]\n" +
            "[@2,18:30=' name of item',<3>,2:5]\n" +
            "[@3,31:31='\\n',<4>,2:18]\n" +
            "[@4,32:45='Another line.\\n',<1>,3:0]\n" +
            "[@5,46:56='More line.\\n',<1>,4:0]\n" +
            "[@6,57:56='<EOF>',<-1>,5:0]\n";
        Assert.AreEqual(expecting, executedState.output);
    }

}
