/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.atn;

/** TODO: this is old comment:
 *  A tree of semantic predicates from the grammar AST if label==SEMPRED.
 *  In the ATN, labels will always be exactly one predicate, but the DFA
 *  may have to combine a bunch of them as it collects predicates from
 *  multiple ATN configurations into a single DFA state.
 */
public class PredicateTransition : AbstractPredicateTransition
{
    public readonly int ruleIndex;
    public readonly int predIndex;
    public readonly bool isCtxDependent;  // e.g., $i ref in pred

    public PredicateTransition(ATNState target, int ruleIndex, int predIndex, bool isCtxDependent) : base(target)
    {
        this.ruleIndex = ruleIndex;
        this.predIndex = predIndex;
        this.isCtxDependent = isCtxDependent;
    }

    public override int SerializationType => PREDICATE;

    public override bool IsEpsilon => true;
    public override bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol) => false;

    public SemanticContext.Predicate GetPredicate()
        => new(ruleIndex, predIndex, isCtxDependent);

    public override string ToString() 
        => $"pred_{ruleIndex}:{predIndex}";

}
