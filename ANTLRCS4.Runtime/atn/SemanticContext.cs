/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

/** A tree structure used to record the semantic context in which
 *  an ATN configuration is valid.  It's either a single predicate,
 *  a conjunction {@code p1&&p2}, or a sum of products {@code p1||p2}.
 *
 *  <p>I have scoped the {@link AND}, {@link OR}, and {@link Predicate} subclasses of
 *  {@link SemanticContext} within the scope of this outer class.</p>
 */
public abstract class SemanticContext
{
    /**
	 * For context independent predicates, we evaluate them without a local
	 * context (i.e., null context). That way, we can evaluate them without
	 * having to create proper rule-specific context during prediction (as
	 * opposed to the parser, which creates them naturally). In a practical
	 * sense, this avoids a cast exception from RuleContext to myruleContext.
	 *
	 * <p>For context dependent predicates, we must pass in a local context so that
	 * references such as $arg evaluate properly as _localctx.arg. We only
	 * capture context dependent predicates in the context in which we begin
	 * prediction, so we passed in the outer context here in case of context
	 * dependent predicate evaluation.</p>
	 */
    public abstract bool Eval(Recognizer parser, RuleContext parserCallStack);

    /**
	 * Evaluate the precedence predicates for the context and reduce the result.
	 *
	 * @param parser The parser instance.
	 * @param parserCallStack
	 * @return The simplified semantic context after precedence predicates are
	 * evaluated, which will be one of the following values.
	 * <ul>
	 * <li>{@link Empty#Instance}: if the predicate simplifies to {@code true} after
	 * precedence predicates are evaluated.</li>
	 * <li>{@code null}: if the predicate simplifies to {@code false} after
	 * precedence predicates are evaluated.</li>
	 * <li>{@code this}: if the semantic context is not changed as a result of
	 * precedence predicate evaluation.</li>
	 * <li>A non-{@code null} {@link SemanticContext}: the new simplified
	 * semantic context after precedence predicates are evaluated.</li>
	 * </ul>
	 */
    public virtual SemanticContext EvalPrecedence(Recognizer parser, RuleContext parserCallStack) => this;

    public class Empty : SemanticContext
    {
        /**
		 * The default {@link SemanticContext}, which is semantically equivalent to
		 * a predicate of the form {@code {true}?}.
		 */
        public static readonly Empty Instance = new ();

        public override bool Eval(Recognizer parser, RuleContext parserCallStack) => false;
    }

    public class Predicate : SemanticContext
    {
        public readonly int ruleIndex;
        public readonly int predIndex;
        public readonly bool isCtxDependent;  // e.g., $i ref in pred

        protected Predicate()
        {
            this.ruleIndex = -1;
            this.predIndex = -1;
            this.isCtxDependent = false;
        }

        public Predicate(int ruleIndex, int predIndex, bool isCtxDependent)
        {
            this.ruleIndex = ruleIndex;
            this.predIndex = predIndex;
            this.isCtxDependent = isCtxDependent;
        }

        public override bool Eval(Recognizer parser, RuleContext parserCallStack)
        {
            var localctx = isCtxDependent ? parserCallStack : null;
            return parser.Sempred(localctx, ruleIndex, predIndex);
        }

        public override int GetHashCode()
        {
            int hashCode = MurmurHash.Initialize();
            hashCode = MurmurHash.Update(hashCode, ruleIndex);
            hashCode = MurmurHash.Update(hashCode, predIndex);
            hashCode = MurmurHash.Update(hashCode, isCtxDependent ? 1 : 0);
            hashCode = MurmurHash.Finish(hashCode, 3);
            return hashCode;
        }

        public override bool Equals(object? obj) 
            => obj is Predicate p && (this == obj || this.ruleIndex == p.ruleIndex &&
                       this.predIndex == p.predIndex &&
                       this.isCtxDependent == p.isCtxDependent);

        public override string ToString()
            => "{" + ruleIndex + ":" + predIndex + "}?";
    }

    public class PrecedencePredicate : SemanticContext, IComparable<PrecedencePredicate>
    {
        public readonly int precedence;

        protected PrecedencePredicate()
        {
            this.precedence = 0;
        }

        public PrecedencePredicate(int precedence)
        {
            this.precedence = precedence;
        }

        public override bool Eval(Recognizer parser, RuleContext parserCallStack) 
            => parser.Precpred(parserCallStack, precedence);

        public override SemanticContext EvalPrecedence(Recognizer parser, RuleContext parserCallStack)
            => parser.Precpred(parserCallStack, precedence) ? Empty.Instance : (SemanticContext?)null;

        public int CompareTo(PrecedencePredicate? o) => precedence - o.precedence;

        public override int GetHashCode()
        {
            int hashCode = 1;
            hashCode = 31 * hashCode + precedence;
            return hashCode;
        }

        public override bool Equals(object? obj) 
            => obj is PrecedencePredicate other && (this == obj
                || this.precedence == other.precedence);

        // precedence >= _precedenceStack.peek()
        public override string ToString() 
            => "{" + precedence + ">=prec}?";
    }

    /**
	 * This is the base class for semantic context "operators", which operate on
	 * a collection of semantic context "operands".
	 *
	 * @since 4.3
	 */
    public abstract class Operator : SemanticContext
    {
        /**
		 * Gets the operands for the semantic context operator.
		 *
		 * @return a collection of {@link SemanticContext} operands for the
		 * operator.
		 *
		 * @since 4.3
		 */

        public abstract ICollection<SemanticContext> GetOperands();
    }

    /**
	 * A semantic context which is true whenever none of the contained contexts
	 * is false.
	 */
    public class AND : Operator
    {
        public readonly SemanticContext[] opnds;

        public AND(SemanticContext a, SemanticContext b)
        {
            var operands = new HashSet<SemanticContext>();
            if (a is AND ax) operands.UnionWith((ax.opnds));
            else operands.Add(a);
            if (b is AND bx) operands.UnionWith((bx.opnds));
            else operands.Add(b);

            var precedencePredicates = FilterPrecedencePredicates(operands);
            if (precedencePredicates.Count > 0)
            {
                // interested in the transition with the lowest precedence
                var reduced = precedencePredicates.Min();
                operands.Add(reduced);
            }

            opnds = operands.ToArray();
        }

        
        public override ICollection<SemanticContext> GetOperands() => opnds.ToList();

        
        public override bool Equals(object? obj)
            => this == obj || obj is AND other && Enumerable.SequenceEqual(this.opnds, other.opnds);

        
        public override int GetHashCode() => MurmurHash.GetHashCode(opnds, base.GetHashCode());

        /**
		 * {@inheritDoc}
		 *
		 * <p>
		 * The evaluation of predicates by this context is short-circuiting, but
		 * unordered.</p>
		 */
        
        public override bool Eval(Recognizer parser, RuleContext parserCallStack)
        {
            foreach (var opnd in opnds)
                if (!opnd.Eval(parser, parserCallStack)) return false;
            return true;
        }

        public virtual SemanticContext EvalPrecedence(Recognizer<Token, ATNSimulator> parser, RuleContext parserCallStack)
        {
            bool differs = false;
            List<SemanticContext> operands = new();
            foreach (var context in opnds)
            {
                var evaluated = context.EvalPrecedence(parser, parserCallStack);
                differs |= (evaluated != context);
                if (evaluated == null)
                {
                    // The AND context is false if any element is false
                    return null;
                }
                else if (evaluated != Empty.Instance)
                {
                    // Reduce the result by skipping true elements
                    operands.Add(evaluated);
                }
            }

            if (!differs)
            {
                return this;
            }

            if (operands.Count == 0)
            {
                // all elements were true, so the AND context is true
                return Empty.Instance;
            }

            var result = operands[(0)];
            for (int i = 1; i < operands.Count; i++)
            {
                result = SemanticContext.And(result, operands[(i)]);
            }

            return result;
        }

        
        public override string ToString() 
            => RuntimeUtils.Join(opnds, "&&");
    }

    /**
	 * A semantic context which is true whenever at least one of the contained
	 * contexts is true.
	 */
    public class OR : Operator
    {
        public readonly SemanticContext[] opnds;

        public OR(SemanticContext a, SemanticContext b)
        {
            var operands = new HashSet<SemanticContext>();
            if (a is OR ax) operands.UnionWith(ax.opnds);
            else operands.Add(a);
            if (b is OR bx) operands.UnionWith(bx.opnds);
            else operands.Add(b);

            var precedencePredicates = FilterPrecedencePredicates(operands);
            if (precedencePredicates.Count > 0)
            {
                // interested in the transition with the highest precedence
                var reduced = precedencePredicates.Max();
                operands.Add(reduced);
            }

            this.opnds = operands.ToArray();
        }

        
        public override ICollection<SemanticContext> GetOperands() => opnds.ToList();// Arrays.asList(opnds);

        
        public override bool Equals(object? obj) 
            => this == obj || (obj is OR other && Enumerable.SequenceEqual(this.opnds, other.opnds));

        
        public override int GetHashCode() 
            => MurmurHash.GetHashCode(opnds, base.GetHashCode());

        /**
		 * {@inheritDoc}
		 *
		 * <p>
		 * The evaluation of predicates by this context is short-circuiting, but
		 * unordered.</p>
		 */
        
        public override bool Eval(Recognizer parser, RuleContext parserCallStack)
        {
            foreach (var opnd in opnds)
                if (opnd.Eval(parser, parserCallStack)) return true;
            return false;
        }

        public override SemanticContext EvalPrecedence(Recognizer parser, RuleContext parserCallStack)
        {
            bool differs = false;
            List<SemanticContext> operands = new();
            foreach (var context in opnds)
            {
                var evaluated = context.EvalPrecedence(parser, parserCallStack);
                differs |= (evaluated != context);
                if (evaluated == Empty.Instance)
                {
                    // The OR context is true if any element is true
                    return Empty.Instance;
                }
                else if (evaluated != null)
                {
                    // Reduce the result by skipping false elements
                    operands.Add(evaluated);
                }
            }

            if (!differs)
            {
                return this;
            }

            if (operands.Count == 0)
            {
                // all elements were false, so the OR context is false
                return null;
            }

            var result = operands[(0)];
            for (int i = 1; i < operands.Count; i++)
            {
                result = SemanticContext.Or(result, operands[(i)]);
            }

            return result;
        }

        public override string ToString() => RuntimeUtils.Join((opnds), "||");
    }

    public static SemanticContext And(SemanticContext a, SemanticContext b)
    {
        if (a == null || a == Empty.Instance) return b;
        if (b == null || b == Empty.Instance) return a;
        var result = new AND(a, b);
        return result.opnds.Length == 1 ? result.opnds[0] : result;
    }

    /**
	 *
	 *  @see ParserATNSimulator#getPredsForAmbigAlts
	 */
    public static SemanticContext Or(SemanticContext a, SemanticContext b)
    {
        if (a == null) return b;
        if (b == null) return a;
        if (a == Empty.Instance || b == Empty.Instance) return Empty.Instance;
        var result = new OR(a, b);
        return result.opnds.Length == 1 ? result.opnds[0] : result;
    }

    private static List<PrecedencePredicate> FilterPrecedencePredicates(ICollection<SemanticContext> collection)
    {
        List<PrecedencePredicate> result = null;
        foreach (var context in collection.ToArray())
        {
            if (context is PrecedencePredicate predicate)
            {
                result ??= new();
                result.Add(predicate);
                collection.Remove(context);
            }
        }

        return result ?? new();
    }
}
