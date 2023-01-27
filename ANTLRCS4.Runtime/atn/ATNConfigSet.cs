/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using ANTLRCS4.Runtime;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using System.Text;

namespace org.antlr.v4.runtime.atn;
/**
 * Specialized {@link Set}{@code <}{@link ATNConfig}{@code >} that can track
 * info about the set, with support for combining similar configurations using a
 * graph-structured stack.
 */
public class ATNConfigSet : HashSet<ATNConfig> {
	/**
	 * The reason that we need this is because we don't want the hash map to use
	 * the standard hash code and equals. We need all configurations with the same
	 * {@code (s,i,_,semctx)} to be equal. Unfortunately, this key effectively doubles
	 * the number of objects associated with ATNConfigs. The other solution is to
	 * use a hash table that lets us specify the equals/hashcode operation.
	 */
	public class ConfigHashSet : AbstractConfigHashSet {
		public ConfigHashSet(): base(ConfigEqualityComparator.INSTANCE) {
		}
	}

	public class ConfigEqualityComparator : AbstractEqualityComparator<ATNConfig> {
		public static readonly ConfigEqualityComparator INSTANCE = new ConfigEqualityComparator();

		private ConfigEqualityComparator() {
		}

		//@Override
		public int hashCode(ATNConfig o) {
			int hashCode = 7;
			hashCode = 31 * hashCode + o.state.stateNumber;
			hashCode = 31 * hashCode + o.alt;
			hashCode = 31 * hashCode + o.semanticContext.GetHashCode();
	        return hashCode;
		}

		//@Override
		public bool equals(ATNConfig a, ATNConfig b) {
			if ( a==b ) return true;
			if ( a==null || b==null ) return false;
			return a.state.stateNumber==b.state.stateNumber
				&& a.alt==b.alt
				&& a.semanticContext.Equals(b.semanticContext);
		}
	}

	/** Indicates that the set of configurations is read-only. Do not
	 *  allow any code to manipulate the set; DFA states will point at
	 *  the sets and they must not change. This does not protect the other
	 *  fields; in particular, conflictingAlts is set after
	 *  we've made this readonly.
 	 */
	protected bool @readonly = false;

	/**
	 * All configs but hashed by (s, i, _, pi) not including context. Wiped out
	 * when we go readonly as this set becomes a DFA state.
	 */
	public AbstractConfigHashSet configLookup;

	/** Track the elements as they are added to the set; supports get(i) */
	public readonly List<ATNConfig> configs = new (7);

	// TODO: these fields make me pretty uncomfortable but nice to pack up info together, saves recomputation
	// TODO: can we track conflicts as they are added to save scanning configs later?
	public int uniqueAlt;
	/** Currently this is only used when we detect SLL conflict; this does
	 *  not necessarily represent the ambiguous alternatives. In fact,
	 *  I should also point out that this seems to include predicated alternatives
	 *  that have predicates that evaluate to false. Computed in computeTargetState().
 	 */
	public BitSet conflictingAlts;

	// Used in parser and lexer. In lexer, it indicates we hit a pred
	// while computing a closure operation.  Don't make a DFA state from this.
	public bool hasSemanticContext;
	public bool dipsIntoOuterContext;

	/** Indicates that this configuration set is part of a full context
	 *  LL prediction. It will be used to determine how to merge $. With SLL
	 *  it's a wildcard whereas it is not for LL context merge.
	 */
	public readonly bool fullCtx;

	private int cachedHashCode = -1;

	public ATNConfigSet(bool fullCtx = true) {
		configLookup = new ConfigHashSet();
		this.fullCtx = fullCtx;
	}
	
	public ATNConfigSet(ATNConfigSet old): this(old.fullCtx)
    {
		addAll(old);
		this.uniqueAlt = old.uniqueAlt;
		this.conflictingAlts = old.conflictingAlts;
		this.hasSemanticContext = old.hasSemanticContext;
		this.dipsIntoOuterContext = old.dipsIntoOuterContext;
	}

	//@Override
	public bool add(ATNConfig config) {
		return add(config, null);
	}

	/**
	 * Adding a new config means merging contexts with existing configs for
	 * {@code (s, i, pi, _)}, where {@code s} is the
	 * {@link ATNConfig#state}, {@code i} is the {@link ATNConfig#alt}, and
	 * {@code pi} is the {@link ATNConfig#semanticContext}. We use
	 * {@code (s,i,pi)} as key.
	 *
	 * <p>This method updates {@link #dipsIntoOuterContext} and
	 * {@link #hasSemanticContext} when necessary.</p>
	 */
	public bool add(
		ATNConfig config,
		DoubleKeyMap<PredictionContext,PredictionContext,PredictionContext> mergeCache)
	{
		if ( @readonly ) throw new IllegalStateException("This set is readonly");
		if ( config.semanticContext != SemanticContext.Empty.Instance ) {
			hasSemanticContext = true;
		}
		if (config.getOuterContextDepth() > 0) {
			dipsIntoOuterContext = true;
		}
		ATNConfig existing = configLookup.getOrAdd(config);
		if ( existing==config ) { // we added this new one
			cachedHashCode = -1;
			configs.Add(config);  // track order here
			return true;
		}
		// a previous (s,i,pi,_), merge with it and save result
		bool rootIsWildcard = !fullCtx;
		PredictionContext merged =
			PredictionContext.merge(existing.context, config.context, rootIsWildcard, mergeCache);
		// no need to check for existing.context, config.context in cache
		// since only way to create new graphs is "call rule" and here. We
		// cache at both places.
		existing.reachesIntoOuterContext =
			Math.Max(existing.reachesIntoOuterContext, config.reachesIntoOuterContext);

		// make sure to preserve the precedence filter suppression during the merge
		if (config.isPrecedenceFilterSuppressed()) {
			existing.setPrecedenceFilterSuppressed(true);
		}

		existing.context = merged; // replace context; no need to alt mapping
		return true;
	}

	/** Return a List holding list of configs */
    public List<ATNConfig> elements() { return configs; }

	public HashSet<ATNState> getStates() {
        HashSet<ATNState> states = new HashSet<ATNState>();
		foreach (ATNConfig c in configs) {
			states.Add(c.state);
		}
		return states;
	}

	/**
	 * Gets the complete set of represented alternatives for the configuration
	 * set.
	 *
	 * @return the set of represented alternatives in this configuration set
	 *
	 * @since 4.3
	 */

	public BitSet getAlts() {
		BitSet alts = new BitSet();
		foreach (ATNConfig config in configs) {
			alts.set(config.alt);
		}
		return alts;
	}

	public List<SemanticContext> getPredicates() {
		List<SemanticContext> preds = new ();
		foreach (ATNConfig c in configs) {
			if ( c.semanticContext!=SemanticContext.Empty.Instance ) {
				preds.Add(c.semanticContext);
			}
		}
		return preds;
	}

	public ATNConfig get(int i) { return configs[i]; }

	public void optimizeConfigs(ATNSimulator interpreter) {
		if ( @readonly ) throw new IllegalStateException("This set is readonly");
		if ( configLookup.isEmpty() ) return;

		foreach (ATNConfig config in configs) {
//			int before = PredictionContext.getAllContextNodes(config.context).size();
			config.context = interpreter.getCachedContext(config.context);
//			int after = PredictionContext.getAllContextNodes(config.context).size();
//			System.out.println("configs "+before+"->"+after);
		}
	}

	//@Override
	public bool addAll(ICollection<ATNConfig> coll) {
		foreach (ATNConfig c in coll) add(c);
		return false;
	}

	//@Override
	public override bool Equals(Object? o) {
		if (o == this) {
			return true;
		}
		else if (!(o is ATNConfigSet)) {
			return false;
		}

//		System.out.print("equals " + this + ", " + o+" = ");
		ATNConfigSet other = (ATNConfigSet)o;
		bool same = configs!=null &&
			configs.Equals(other.configs) &&  // includes stack context
			this.fullCtx == other.fullCtx &&
			this.uniqueAlt == other.uniqueAlt &&
			this.conflictingAlts == other.conflictingAlts &&
			this.hasSemanticContext == other.hasSemanticContext &&
			this.dipsIntoOuterContext == other.dipsIntoOuterContext;

//		System.out.println(same);
		return same;
	}

	//@Override
	public override int GetHashCode() {
		if (isReadonly()) {
			if (cachedHashCode == -1) {
				cachedHashCode = configs.GetHashCode();
			}

			return cachedHashCode;
		}

		return configs.GetHashCode();
	}

	//@Override
	public int size() {
		return configs.Count;
	}

	//@Override
	public bool isEmpty() {
		return configs.Count==0;
	}

	//@Override
	public bool contains(Object o) {
		if (configLookup == null) {
			throw new UnsupportedOperationException("This method is not implemented for readonly sets.");
		}

		return configLookup.contains(o);
	}

	public bool containsFast(ATNConfig obj) {
		if (configLookup == null) {
			throw new UnsupportedOperationException("This method is not implemented for readonly sets.");
		}

		return configLookup.containsFast(obj);
	}

	//@Override
	public new IEnumerator<ATNConfig> GetEnumerator() {
		return configs.GetEnumerator();
	}

	//@Override
	public void clear() {
		if ( @readonly ) throw new IllegalStateException("This set is readonly");
		configs.Clear();
		cachedHashCode = -1;
		configLookup.clear();
	}

	public bool isReadonly() {
		return @readonly;
	}

	public void setReadonly(bool @readonly) {
		this.@readonly = @readonly;
		configLookup = null; // can't mod, no need for lookup cache
	}

	//@Override
	public String toString() {
		StringBuilder buf = new StringBuilder();
		buf.Append(elements().ToString());
		if ( hasSemanticContext ) buf.Append(",hasSemanticContext=").Append(hasSemanticContext);
		if ( uniqueAlt!=ATN.INVALID_ALT_NUMBER ) buf.Append(",uniqueAlt=").Append(uniqueAlt);
		if ( conflictingAlts!=null ) buf.Append(",conflictingAlts=").Append(conflictingAlts);
		if ( dipsIntoOuterContext ) buf.Append(",dipsIntoOuterContext");
		return buf.ToString();
	}

	// satisfy interface

	//@Override
	public ATNConfig[] toArray() {
		return configLookup.toArray();
	}

	//@Override
	public ATNConfig[] ToArray<T>(ATNConfig[] a) {
		return configLookup.toArray();
	}

	//@Override
	public bool remove(Object o) {
		throw new UnsupportedOperationException();
	}

	//@Override
	public bool containsAll(ICollection<ATNConfig> c) {
		throw new UnsupportedOperationException();
	}

	//@Override
	public bool retainAll(ICollection<ATNConfig> c) {
		throw new UnsupportedOperationException();
	}

	//@Override
	public bool removeAll(ICollection<ATNConfig> c) {
		throw new UnsupportedOperationException();
	}

	public abstract class AbstractConfigHashSet : Array2DHashSet<ATNConfig> {

		public AbstractConfigHashSet(AbstractEqualityComparator<ATNConfig> comparator, 
			int initialCapacity = 16, int initialBucketCapacity =2):base(comparator, initialCapacity, initialBucketCapacity) {
		}

		//@Override
		protected ATNConfig asElementType(Object o) {
			if (!(o is ATNConfig)) {
				return null;
			}

			return (ATNConfig)o;
		}

		//@Override
		protected ATNConfig[][] createBuckets(int capacity) {
			return new ATNConfig[capacity][];
		}

		//@Override
		protected ATNConfig[] createBucket(int capacity) {
			return new ATNConfig[capacity];
		}

	}
}
