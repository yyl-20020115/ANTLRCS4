/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime.tree;
using org.antlr.v4.misc;
using org.antlr.v4.parse;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;


public class ElementFrequenciesVisitor : GrammarTreeVisitor
{
    /**
	 * This special value means "no set", and is used by {@link #minFrequencies}
	 * to ensure that {@link #combineMin} doesn't merge an empty set (all zeros)
	 * with the results of the first alternative.
	 */
    private static readonly FrequencySet<string> SENTINEL = new ();

    public readonly ArrayDeque<FrequencySet<string>> frequencies = new();
    private readonly ArrayDeque<FrequencySet<string>> minFrequencies = new();

    public ElementFrequenciesVisitor(TreeNodeStream input) : base(input)
    {
        frequencies.Push(new());
        minFrequencies.Push(SENTINEL);
    }

    public FrequencySet<string> GetMinFrequencies()
    {
        //assert minFrequencies.size() == 1;
        //assert minFrequencies.peek() != SENTINEL;
        //assert SENTINEL.isEmpty();

        return minFrequencies.Peek();
    }

    /** During code gen, we can assume tree is in good shape */
    //@Override
    public override ErrorManager GetErrorManager() => base.GetErrorManager();

    /*
	 * Common
	 */

    /**
	 * Generate a frequency set as the union of two input sets. If an
	 * element is contained in both sets, the value for the output will be
	 * the maximum of the two input values.
	 *
	 * @param a The first set.
	 * @param b The second set.
	 * @return The union of the two sets, with the maximum value chosen
	 * whenever both sets contain the same key.
	 */
    protected static FrequencySet<string> CombineMax(FrequencySet<String> a, FrequencySet<String> b)
    {
        var result = CombineAndClip(a, b, 1);
        foreach (var entry in a)
        {
            result[(entry.Key)].v = entry.Value.v;
        }

        foreach (var entry in b)
        {
            var slot = result[(entry.Key)];
            slot.v = Math.Max(slot.v, entry.Value.v);
        }

        return result;
    }

    /**
	 * Generate a frequency set as the union of two input sets. If an
	 * element is contained in both sets, the value for the output will be
	 * the minimum of the two input values.
	 *
	 * @param a The first set.
	 * @param b The second set. If this set is {@link #SENTINEL}, it is treated
	 * as though no second set were provided.
	 * @return The union of the two sets, with the minimum value chosen
	 * whenever both sets contain the same key.
	 */
    protected static FrequencySet<string> CombineMin(FrequencySet<string> a, FrequencySet<string> b)
    {
        if (b == SENTINEL)
        {
            return a;
        }

        //assert a != SENTINEL;
        var result = CombineAndClip(a, b, int.MaxValue);
        foreach (var entry in result)
        {
            entry.Value.v = Math.Min(a.CountFor(entry.Key), b.CountFor(entry.Key));
        }

        return result;
    }

    /**
	 * Generate a frequency set as the union of two input sets, with the
	 * values clipped to a specified maximum value. If an element is
	 * contained in both sets, the value for the output, prior to clipping,
	 * will be the sum of the two input values.
	 *
	 * @param a The first set.
	 * @param b The second set.
	 * @param clip The maximum value to allow for any output.
	 * @return The sum of the two sets, with the individual elements clipped
	 * to the maximum value given by {@code clip}.
	 */
    protected static FrequencySet<string> CombineAndClip(FrequencySet<string> a, FrequencySet<string> b, int clip)
    {
        var result = new FrequencySet<string>();
        foreach (var entry in a)
        {
            for (int i = 0; i < entry.Value.v; i++)
            {
                result.Add(entry.Key);
            }
        }

        foreach (var entry in b)
        {
            for (int i = 0; i < entry.Value.v; i++)
            {
                result.Add(entry.Key);
            }
        }

        foreach (var entry in result)
        {
            entry.Value.v = Math.Min(entry.Value.v, clip);
        }

        return result;
    }

    //@Override
    public void tokenRef(TerminalAST @ref)
    {
        frequencies.Peek().Add(@ref.getText());
        minFrequencies.Peek().Add(@ref.getText());
    }

    //@Override
    public void ruleRef(GrammarAST @ref, ActionAST arg)
    {
        frequencies.Peek().Add(@ref.getText());
        minFrequencies.Peek().Add(@ref.getText());
    }

    //@Override
    public void stringRef(TerminalAST @ref)
    {
        var tokenName = @ref.g.getTokenName(@ref.getText());

        if (tokenName != null && !tokenName.StartsWith("T__"))
        {
            frequencies.Peek().Add(tokenName);
            minFrequencies.Peek().Add(tokenName);
        }
    }

    /*
	 * Parser rules
	 */

    //@Override
    protected void enterAlternative(AltAST tree)
    {
        frequencies.Push(new FrequencySet<string>());
        minFrequencies.Push(new FrequencySet<string>());
    }

    //@Override
    protected void exitAlternative(AltAST tree)
    {
        frequencies.Push(CombineMax(frequencies.Pop(), frequencies.Pop()));
        minFrequencies.Push(CombineMin(minFrequencies.Pop(), minFrequencies.Pop()));
    }

    //@Override
    protected void enterElement(GrammarAST tree)
    {
        frequencies.Push(new FrequencySet<String>());
        minFrequencies.Push(new FrequencySet<String>());
    }

    //@Override
    protected void exitElement(GrammarAST tree)
    {
        frequencies.Push(CombineAndClip(frequencies.Pop(), frequencies.Pop(), 2));
        minFrequencies.Push(CombineAndClip(minFrequencies.Pop(), minFrequencies.Pop(), 2));
    }

    //@Override
    protected void enterBlockSet(GrammarAST tree)
    {
        frequencies.Push(new FrequencySet<String>());
        minFrequencies.Push(new FrequencySet<String>());
    }

    //@Override
    protected void exitBlockSet(GrammarAST tree)
    {
        foreach (var entry in frequencies.Peek())
        {
            // This visitor counts a block set as a sequence of elements, not a
            // sequence of alternatives of elements. Reset the count back to 1
            // for all items when leaving the set to ensure duplicate entries in
            // the set are treated as a maximum of one item.
            entry.Value.v = 1;
        }

        if (minFrequencies.Peek().Count > 1)
        {
            // Everything is optional
            minFrequencies.Peek().Clear();
        }

        frequencies.Push(CombineAndClip(frequencies.Pop(), frequencies.Pop(), 2));
        minFrequencies.Push(CombineAndClip(minFrequencies.Pop(), minFrequencies.Pop(), 2));
    }

    //@Override
    protected void exitSubrule(GrammarAST tree)
    {
        if (tree.getType() == CLOSURE || tree.getType() == POSITIVE_CLOSURE)
        {
            foreach (var entry in frequencies.Peek())
            {
                entry.Value.v = 2;
            }
        }

        if (tree.getType() == CLOSURE || tree.getType() == OPTIONAL)
        {
            // Everything inside a closure is optional, so the minimum
            // number of occurrences for all elements is 0.
            minFrequencies.Peek().Clear();
        }
    }

    /*
	 * Lexer rules
	 */

    //@Override
    protected void enterLexerAlternative(GrammarAST tree)
    {
        frequencies.Push(new FrequencySet<String>());
        minFrequencies.Push(new FrequencySet<String>());
    }

    //@Override
    protected void exitLexerAlternative(GrammarAST tree)
    {
        frequencies.Push(CombineMax(frequencies.Pop(), frequencies.Pop()));
        minFrequencies.Push(CombineMin(minFrequencies.Pop(), minFrequencies.Pop()));
    }

    //@Override
    protected void enterLexerElement(GrammarAST tree)
    {
        frequencies.Push(new FrequencySet<String>());
        minFrequencies.Push(new FrequencySet<String>());
    }

    //@Override
    protected void exitLexerElement(GrammarAST tree)
    {
        frequencies.Push(CombineAndClip(frequencies.Pop(), frequencies.Pop(), 2));
        minFrequencies.Push(CombineAndClip(minFrequencies.Pop(), minFrequencies.Pop(), 2));
    }

    //@Override
    protected void exitLexerSubrule(GrammarAST tree)
    {
        if (tree.getType() == CLOSURE || tree.getType() == POSITIVE_CLOSURE)
        {
            foreach (var entry in frequencies.Peek())
            {
                entry.Value.v = 2;
            }
        }

        if (tree.getType() == CLOSURE)
        {
            // Everything inside a closure is optional, so the minimum
            // number of occurrences for all elements is 0.
            minFrequencies.Peek().Clear();
        }
    }
}
