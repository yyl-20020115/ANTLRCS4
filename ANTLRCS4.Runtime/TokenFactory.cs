/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime;

/** The default mechanism for creating tokens. It's used by default in Lexer and
 *  the error handling strategy (to create missing tokens).  Notifying the parser
 *  of a new factory means that it notifies its token source and error strategy.
 */
public interface TokenFactory
{

}
public interface TokenFactory<Symbol>: TokenFactory where Symbol : Token {
    /** This is the method used to create tokens in the lexer and in the
	 *  error handling strategy. If text!=null, than the start and stop positions
	 *  are wiped to -1 in the text override is set in the CommonToken.
	 */
    Symbol Create(Pair<TokenSource, CharStream> source, int type, String text,
				  int channel, int start, int stop,
				  int line, int charPositionInLine);

    /** Generically useful */
    Symbol Create(int type, String text);
}
