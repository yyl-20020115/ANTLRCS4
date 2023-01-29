/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using System.Reflection;

namespace org.antlr.v4.test.runtime.states;

public class JavaCompiledState : CompiledState
{
    public readonly Assembly assembly;
    public readonly Type lexerType;
    public readonly Type parserType;

    public JavaCompiledState(GeneratedState previousState,
                             Assembly assembly,
                             Type lexer,
                             Type parser,
                             Exception exception
    )
        : base(previousState, exception)
    {
        ;
        this.assembly = assembly;
        this.lexerType = lexer;
        this.parserType = parser;
    }

    public Pair<Lexer, Parser> initializeLexerAndParser(String input)
    {
        ANTLRInputStream @in = new ANTLRInputStream(new StringReader(input));

        ConstructorInfo lexerConstructor = this.lexerType.GetConstructor(
            new Type[] { typeof(CharStream) });
        Lexer lexer = lexerConstructor.Invoke(new object[] { @in }) as Lexer;

        CommonTokenStream tokens = new CommonTokenStream(lexer);

        ConstructorInfo parserConstructor = parserType.GetConstructor(
            new Type[] { typeof(TokenStream) });
        Parser parser = parserConstructor.Invoke(new object[] { tokens }) as Parser;
        return new (lexer, parser);
    }
}
