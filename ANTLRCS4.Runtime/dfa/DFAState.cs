/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using System.Text;

namespace org.antlr.v4.runtime.dfa;


/** A DFA state represents a set of possible ATN configurations.
 *  As Aho, Sethi, Ullman p. 117 says "The DFA uses its state
 *  to keep track of all possible states the ATN can be in after
 *  reading each input symbol.  That is to say, after reading
 *  input a1a2..an, the DFA is in a state that represents the
 *  subset T of the states of the ATN that are reachable from the
 *  ATN's start state along some path labeled a1a2..an."
 *  In conventional NFA&rarr;DFA conversion, therefore, the subset T
 *  would be a bitset representing the set of states the
 *  ATN could be in.  We need to track the alt predicted by each
 *  state as well, however.  More importantly, we need to maintain
 *  a stack of states, tracking the closure operations as they
 *  jump from rule to rule, emulating rule invocations (method calls).
 *  I have to add a stack to simulate the proper lookahead sequences for
 *  the underlying LL grammar from which the ATN was derived.
 *
 *  <p>I use a set of ATNConfig objects not simple states.  An ATNConfig
 *  is both a state (ala normal conversion) and a RuleContext describing
 *  the chain of rules (if any) followed to arrive at that state.</p>
 *
 *  <p>A DFA state may have multiple references to a particular state,
 *  but with different ATN contexts (with same or different alts)
 *  meaning that state was reached via a different set of rule invocations.</p>
 */
public class DFAState
{
    public int stateNumber = -1;


    public ATNConfigSet configs = new ();

    /** {@code edges[symbol]} points to target of symbol. Shift up by 1 so (-1)
	 *  {@link Token#EOF} maps to {@code edges[0]}.
	 */

    public DFAState[] edges;

    public bool isAcceptState = false;

    /** if accept state, what ttype do we match or alt do we predict?
	 *  This is set to {@link ATN#INVALID_ALT_NUMBER} when {@link #predicates}{@code !=null} or
	 *  {@link #requiresFullContext}.
	 */
    public int prediction;

    public LexerActionExecutor lexerActionExecutor;

    /**
	 * Indicates that this state was created during SLL prediction that
	 * discovered a conflict between the configurations in the state. Future
	 * {@link ParserATNSimulator#execATN} invocations immediately jumped doing
	 * full context prediction if this field is true.
	 */
    public bool requiresFullContext;

    /** During SLL parsing, this is a list of predicates associated with the
	 *  ATN configurations of the DFA state. When we have predicates,
	 *  {@link #requiresFullContext} is {@code false} since full context prediction evaluates predicates
	 *  on-the-fly. If this is not null, then {@link #prediction} is
	 *  {@link ATN#INVALID_ALT_NUMBER}.
	 *
	 *  <p>We only use these for non-{@link #requiresFullContext} but conflicting states. That
	 *  means we know from the context (it's $ or we don't dip into outer
	 *  context) that it's an ambiguity not a conflict.</p>
	 *
	 *  <p>This list is computed by {@link ParserATNSimulator#predicateDFAState}.</p>
	 */

    public PredPrediction[] predicates;

    /** Map a predicate to a predicted alternative. */
    public class PredPrediction
    {
        public SemanticContext pred; // never null; at least SemanticContext.NONE
        public int alt;
        public PredPrediction(SemanticContext pred, int alt)
        {
            this.alt = alt;
            this.pred = pred;
        }
        public override string ToString() => "(" + pred + ", " + alt + ")";
    }

    public DFAState() { }

    public DFAState(int stateNumber) { this.stateNumber = stateNumber; }

    public DFAState(ATNConfigSet configs) { this.configs = configs; }

    /** Get the set of all alts mentioned by all ATN configurations in this
	 *  DFA state.
	 */
    public HashSet<int> GetAltSet()
    {
        var alts = new HashSet<int>();
        if (configs != null)
            foreach (var c in configs)
                alts.Add(c.alt);
        if (alts.Count == 0) return null;
        return alts;
    }

    public override int GetHashCode()
    {
        int hash = MurmurHash.Initialize(7);
        hash = MurmurHash.Update(hash, configs.GetHashCode());
        hash = MurmurHash.Finish(hash, 1);
        return hash;
    }

    /**
	 * Two {@link DFAState} instances are equal if their ATN configuration sets
	 * are the same. This method is used to see if a state already exists.
	 *
	 * <p>Because the number of alternatives and number of ATN configurations are
	 * finite, there is a finite number of DFA states that can be processed.
	 * This is necessary to show that the algorithm terminates.</p>
	 *
	 * <p>Cannot test the DFA state numbers here because in
	 * {@link ParserATNSimulator#addDFAState} we need to know if any other state
	 * exists that has this exact set of ATN configurations. The
	 * {@link #stateNumber} is irrelevant.</p>
	 */
    public override bool Equals(object? o)
    {
        // compare set of ATN configurations in this set with other
        if (this == o) return true;

        if (o is DFAState other)
        {
            // TODO (sam): what to do when configs==null?
            bool sameSet = this.configs.Equals(other.configs);
            //		Console.Out.WriteLine("DFAState.equals: "+configs+(sameSet?"==":"!=")+other.configs);
            return sameSet;
        }
        return false;

    }

    public override string ToString()
    {
        var buffer = new StringBuilder();
        buffer.Append(stateNumber).Append(':').Append(configs);
        if (isAcceptState)
        {
            buffer.Append("=>");
            if (predicates != null)
            {
                buffer.Append(string.Join<DFAState.PredPrediction>(',', predicates));
            }
            else
            {
                buffer.Append(prediction);
            }
        }
        return buffer.ToString();
    }
}
