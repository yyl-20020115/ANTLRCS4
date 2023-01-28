/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

package org.antlr.v4.test.runtime.states;

import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.Pair;

import java.io.IOException;
import java.io.StringReader;
import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;

public class JavaCompiledState : CompiledState {
	public final ClassLoader loader;
	public final Class<? : Lexer> lexer;
	public final Class<? : Parser> parser;

	public JavaCompiledState(GeneratedState previousState,
							 ClassLoader loader,
							 Class<? : Lexer> lexer,
							 Class<? : Parser> parser,
							 Exception exception
	) {
		super(previousState, exception);
		this.loader = loader;
		this.lexer = lexer;
		this.parser = parser;
	}

	public Pair<Lexer, Parser> initializeLexerAndParser(String input)
			{
		ANTLRInputStream in = new ANTLRInputStream(new StringReader(input));

		Constructor<? : Lexer> lexerConstructor = lexer.getConstructor(CharStream.class);
		Lexer lexer = lexerConstructor.newInstance(in);

		CommonTokenStream tokens = new CommonTokenStream(lexer);

		Constructor<? : Parser> parserConstructor = parser.getConstructor(TokenStream.class);
		Parser parser = parserConstructor.newInstance(tokens);
		return new Pair<>(lexer, parser);
	}
}
