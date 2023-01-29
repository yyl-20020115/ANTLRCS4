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
    public readonly Type lexer;
    public readonly Type parser;

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
        this.lexer = lexer;
        this.parser = parser;
    }

    public Pair<Lexer, Parser> initializeLexerAndParser(String input)
    {
        ANTLRInputStream @in = new ANTLRInputStream(new StringReader(input));

        ConstructorInfo lexerConstructor = lexer.getConstructor(CharStream);
        Lexer lexer = lexerConstructor.newInstance(@in);

        CommonTokenStream tokens = new CommonTokenStream(lexer);

        ConstructorInfo parserConstructor = parser.getConstructor(TokenStream);
        Parser parser = parserConstructor.newInstance(tokens);
        return new (lexer, parser);
    }
}
