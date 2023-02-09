/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime;

namespace org.antlr.v4.tool;

/** An {@link InterpreterRuleContext} that knows which alternative
 *  for a rule was matched.
 *
 *  @see GrammarParserInterpreter
 *  @since 4.5.1
 */
public class GrammarInterpreterRuleContext : InterpreterRuleContext
{
    public int outerAltNum = 1;

    public GrammarInterpreterRuleContext(ParserRuleContext parent, int invokingStateNumber, int ruleIndex)
    : base(parent, invokingStateNumber, ruleIndex)
    {

    }

    /** The predicted outermost alternative for the rule associated
	 *  with this context object.  If this node left recursive, the true original
	 *  outermost alternative is returned.
	 */
    public int OuterAltNum { get => outerAltNum; set => this.outerAltNum = value; }

    public override int AltNumber { get =>
        // override here and called old functionality; makes it backward compatible vs changing names
        OuterAltNum; set => OuterAltNum = value; }
}
