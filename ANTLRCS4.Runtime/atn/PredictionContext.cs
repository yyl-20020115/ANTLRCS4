/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using System.Text;

namespace org.antlr.v4.runtime.atn;

public abstract class PredictionContext
{
    /**
	 * Represents {@code $} in an array in full context mode, when {@code $}
	 * doesn't mean wildcard: {@code $ + x = [$,x]}. Here,
	 * {@code $} = {@link #EMPTY_RETURN_STATE}.
	 */
    public static readonly int EMPTY_RETURN_STATE = int.MaxValue;

    private static readonly int INITIAL_HASH = 1;

    private static readonly AtomicInteger globalNodeCount = new AtomicInteger();
    public readonly int id = globalNodeCount.GetAndIncrement();

    /**
	 * Stores the computed hash code of this {@link PredictionContext}. The hash
	 * code is computed in parts to match the following reference algorithm.
	 *
	 * <pre>
	 *  private int referenceHashCode() {
	 *      int hash = {@link MurmurHash#initialize MurmurHash.initialize}({@link #INITIAL_HASH});
	 *
	 *      for (int i = 0; i &lt; {@link #size()}; i++) {
	 *          hash = {@link MurmurHash#update MurmurHash.update}(hash, {@link #getParent getParent}(i));
	 *      }
	 *
	 *      for (int i = 0; i &lt; {@link #size()}; i++) {
	 *          hash = {@link MurmurHash#update MurmurHash.update}(hash, {@link #getReturnState getReturnState}(i));
	 *      }
	 *
	 *      hash = {@link MurmurHash#finish MurmurHash.finish}(hash, 2 * {@link #size()});
	 *      return hash;
	 *  }
	 * </pre>
	 */
    public readonly int cachedHashCode;

    protected PredictionContext(int cachedHashCode) => this.cachedHashCode = cachedHashCode;

    /** Convert a {@link RuleContext} tree to a {@link PredictionContext} graph.
	 *  Return {@link EmptyPredictionContext#Instance} if {@code outerContext} is empty or null.
	 */
    public static PredictionContext FromRuleContext(ATN atn, RuleContext outerContext)
    {
        outerContext ??= ParserRuleContext.EMPTY;

        // if we are in RuleContext of start rule, s, then PredictionContext
        // is EMPTY. Nobody called us. (if we are empty, return empty)
        if (outerContext.parent == null || outerContext == ParserRuleContext.EMPTY)
        {
            return EmptyPredictionContext.Instance;
        }

        // If we have a parent, convert it to a PredictionContext graph
        PredictionContext parent = EmptyPredictionContext.Instance;
        parent = FromRuleContext(atn, outerContext.parent);

        var state = atn.states[(outerContext.invokingState)];
        var transition = state.Transition(0) as RuleTransition;
        return SingletonPredictionContext.Create(parent, transition.followState.stateNumber);
    }

    public abstract int Count { get; }

    public abstract PredictionContext GetParent(int index);

    public abstract int GetReturnState(int index);

    /** This means only the {@link EmptyPredictionContext#Instance} (wildcard? not sure) context is in set. */
    public virtual bool IsEmpty => this == EmptyPredictionContext.Instance;

    public bool HasEmptyPath =>
        // since EMPTY_RETURN_STATE can only appear in the last position, we check last one
        GetReturnState(Count - 1) == EMPTY_RETURN_STATE;

    public override int GetHashCode() => cachedHashCode;

    //public abstract bool Equals(object? obj);

    protected static int CalculateEmptyHashCode()
    {
        int hash = MurmurHash.Initialize(INITIAL_HASH);
        hash = MurmurHash.Finish(hash, 0);
        return hash;
    }

    protected static int CalculateHashCode(PredictionContext parent, int returnState)
    {
        var hash = MurmurHash.Initialize(INITIAL_HASH);
        hash = MurmurHash.Update(hash, parent);
        hash = MurmurHash.Update(hash, returnState);
        hash = MurmurHash.Finish(hash, 2);
        return hash;
    }

    protected static int CalculateHashCode(PredictionContext[] parents, int[] returnStates)
    {
        int hash = MurmurHash.Initialize(INITIAL_HASH);

        foreach (PredictionContext parent in parents)
        {
            hash = MurmurHash.Update(hash, parent);
        }

        foreach (int returnState in returnStates)
        {
            hash = MurmurHash.Update(hash, returnState);
        }

        hash = MurmurHash.Finish(hash, 2 * parents.Length);
        return hash;
    }

    // dispatch
    public static PredictionContext Merge(
        PredictionContext a,
        PredictionContext b,
        bool rootIsWildcard,
        DoubleKeyMap<PredictionContext, PredictionContext, PredictionContext> mergeCache)
    {
        //assert a!=null && b!=null; // must be empty context, never null

        // share same graph if both same
        if (a == b || a.Equals(b)) return a;

        if (a is SingletonPredictionContext context && b is SingletonPredictionContext context1)
        {
            return MergeSingletons(context,
                                   context1,
                                   rootIsWildcard, mergeCache);
        }

        // At least one of a or b is array
        // If one is $ and rootIsWildcard, return $ as * wildcard
        if (rootIsWildcard)
        {
            if (a is EmptyPredictionContext) return a;
            if (b is EmptyPredictionContext) return b;
        }

        // convert singleton so both are arrays to normalize
        if (a is SingletonPredictionContext context2)
        {
            a = new ArrayPredictionContext(context2);
        }
        if (b is SingletonPredictionContext context3)
        {
            b = new ArrayPredictionContext(context3);
        }
        return MergeArrays((ArrayPredictionContext)a, (ArrayPredictionContext)b,
                           rootIsWildcard, mergeCache);
    }

    /**
	 * Merge two {@link SingletonPredictionContext} instances.
	 *
	 * <p>Stack tops equal, parents merge is same; return left graph.<br>
	 * <embed src="images/SingletonMerge_SameRootSamePar.svg" type="image/svg+xml"/></p>
	 *
	 * <p>Same stack top, parents differ; merge parents giving array node, then
	 * remainders of those graphs. A new root node is created to point to the
	 * merged parents.<br>
	 * <embed src="images/SingletonMerge_SameRootDiffPar.svg" type="image/svg+xml"/></p>
	 *
	 * <p>Different stack tops pointing to same parent. Make array node for the
	 * root where both element in the root point to the same (original)
	 * parent.<br>
	 * <embed src="images/SingletonMerge_DiffRootSamePar.svg" type="image/svg+xml"/></p>
	 *
	 * <p>Different stack tops pointing to different parents. Make array node for
	 * the root where each element points to the corresponding original
	 * parent.<br>
	 * <embed src="images/SingletonMerge_DiffRootDiffPar.svg" type="image/svg+xml"/></p>
	 *
	 * @param a the first {@link SingletonPredictionContext}
	 * @param b the second {@link SingletonPredictionContext}
	 * @param rootIsWildcard {@code true} if this is a local-context merge,
	 * otherwise false to indicate a full-context merge
	 * @param mergeCache
	 */
    public static PredictionContext MergeSingletons(
        SingletonPredictionContext a,
        SingletonPredictionContext b,
        bool rootIsWildcard,
        DoubleKeyMap<PredictionContext, PredictionContext, PredictionContext> mergeCache)
    {
        if (mergeCache != null)
        {
            PredictionContext previous = mergeCache.Get(a, b);
            if (previous != null) return previous;
            previous = mergeCache.Get(b, a);
            if (previous != null) return previous;
        }

        PredictionContext rootMerge = MergeRoot(a, b, rootIsWildcard);
        if (rootMerge != null)
        {
            if (mergeCache != null) mergeCache.Put(a, b, rootMerge);
            return rootMerge;
        }

        if (a.returnState == b.returnState)
        { // a == b
            PredictionContext parent = Merge(a.parent, b.parent, rootIsWildcard, mergeCache);
            // if parent is same as existing a or b parent or reduced to a parent, return it
            if (parent == a.parent) return a; // ax + bx = ax, if a=b
            if (parent == b.parent) return b; // ax + bx = bx, if a=b
                                              // else: ax + ay = a'[x,y]
                                              // merge parents x and y, giving array node with x,y then remainders
                                              // of those graphs.  dup a, a' points at merged array
                                              // new joined parent so create new singleton pointing to it, a'
            PredictionContext a_ = SingletonPredictionContext.Create(parent, a.returnState);
            if (mergeCache != null) mergeCache.Put(a, b, a_);
            return a_;
        }
        else
        { // a != b payloads differ
          // see if we can collapse parents due to $+x parents if local ctx
            PredictionContext singleParent = null;
            if (a == b || (a.parent != null && a.parent.Equals(b.parent)))
            { // ax + bx = [a,b]x
                singleParent = a.parent;
            }
            if (singleParent != null)
            {   // parents are same
                // sort payloads and use same parent
                int[] _payloads = new int[] { a.returnState, b.returnState };
                if (a.returnState > b.returnState)
                {
                    _payloads[0] = b.returnState;
                    _payloads[1] = a.returnState;
                }
                PredictionContext[] _parents = { singleParent, singleParent };
                PredictionContext _a_ = new ArrayPredictionContext(_parents, _payloads);
                if (mergeCache != null) mergeCache.Put(a, b, _a_);
                return _a_;
            }
            // parents differ and can't merge them. Just pack together
            // into array; can't merge.
            // ax + by = [ax,by]
            int[] payloads = { a.returnState, b.returnState };
            PredictionContext[] parents = { a.parent, b.parent };
            if (a.returnState > b.returnState)
            { // sort by payload
                payloads[0] = b.returnState;
                payloads[1] = a.returnState;
                parents = new PredictionContext[] { b.parent, a.parent };
            }
            PredictionContext a_ = new ArrayPredictionContext(parents, payloads);
            if (mergeCache != null) mergeCache.Put(a, b, a_);
            return a_;
        }
    }

    /**
	 * Handle case where at least one of {@code a} or {@code b} is
	 * {@link EmptyPredictionContext#Instance}. In the following diagrams, the symbol {@code $} is used
	 * to represent {@link EmptyPredictionContext#Instance}.
	 *
	 * <h2>Local-Context Merges</h2>
	 *
	 * <p>These local-context merge operations are used when {@code rootIsWildcard}
	 * is true.</p>
	 *
	 * <p>{@link EmptyPredictionContext#Instance} is superset of any graph; return {@link EmptyPredictionContext#Instance}.<br>
	 * <embed src="images/LocalMerge_EmptyRoot.svg" type="image/svg+xml"/></p>
	 *
	 * <p>{@link EmptyPredictionContext#Instance} and anything is {@code #EMPTY}, so merged parent is
	 * {@code #EMPTY}; return left graph.<br>
	 * <embed src="images/LocalMerge_EmptyParent.svg" type="image/svg+xml"/></p>
	 *
	 * <p>Special case of last merge if local context.<br>
	 * <embed src="images/LocalMerge_DiffRoots.svg" type="image/svg+xml"/></p>
	 *
	 * <h2>Full-Context Merges</h2>
	 *
	 * <p>These full-context merge operations are used when {@code rootIsWildcard}
	 * is false.</p>
	 *
	 * <p><embed src="images/FullMerge_EmptyRoots.svg" type="image/svg+xml"/></p>
	 *
	 * <p>Must keep all contexts; {@link EmptyPredictionContext#Instance} in array is a special value (and
	 * null parent).<br>
	 * <embed src="images/FullMerge_EmptyRoot.svg" type="image/svg+xml"/></p>
	 *
	 * <p><embed src="images/FullMerge_SameRoot.svg" type="image/svg+xml"/></p>
	 *
	 * @param a the first {@link SingletonPredictionContext}
	 * @param b the second {@link SingletonPredictionContext}
	 * @param rootIsWildcard {@code true} if this is a local-context merge,
	 * otherwise false to indicate a full-context merge
	 */
    public static PredictionContext MergeRoot(SingletonPredictionContext a,
                                              SingletonPredictionContext b,
                                              bool rootIsWildcard)
    {
        if (rootIsWildcard)
        {
            if (a == EmptyPredictionContext.Instance) return EmptyPredictionContext.Instance;  // * + b = *
            if (b == EmptyPredictionContext.Instance) return EmptyPredictionContext.Instance;  // a + * = *
        }
        else
        {
            if (a == EmptyPredictionContext.Instance && b == EmptyPredictionContext.Instance) return EmptyPredictionContext.Instance; // $ + $ = $
            if (a == EmptyPredictionContext.Instance)
            { // $ + x = [x,$]
                int[] payloads = { b.returnState, EMPTY_RETURN_STATE };
                PredictionContext[] parents = { b.parent, null };
                PredictionContext joined =
                    new ArrayPredictionContext(parents, payloads);
                return joined;
            }
            if (b == EmptyPredictionContext.Instance)
            { // x + $ = [x,$] ($ is always last if present)
                int[] payloads = { a.returnState, EMPTY_RETURN_STATE };
                PredictionContext[] parents = { a.parent, null };
                PredictionContext joined =
                    new ArrayPredictionContext(parents, payloads);
                return joined;
            }
        }
        return null;
    }

    /**
	 * Merge two {@link ArrayPredictionContext} instances.
	 *
	 * <p>Different tops, different parents.<br>
	 * <embed src="images/ArrayMerge_DiffTopDiffPar.svg" type="image/svg+xml"/></p>
	 *
	 * <p>Shared top, same parents.<br>
	 * <embed src="images/ArrayMerge_ShareTopSamePar.svg" type="image/svg+xml"/></p>
	 *
	 * <p>Shared top, different parents.<br>
	 * <embed src="images/ArrayMerge_ShareTopDiffPar.svg" type="image/svg+xml"/></p>
	 *
	 * <p>Shared top, all shared parents.<br>
	 * <embed src="images/ArrayMerge_ShareTopSharePar.svg" type="image/svg+xml"/></p>
	 *
	 * <p>Equal tops, merge parents and reduce top to
	 * {@link SingletonPredictionContext}.<br>
	 * <embed src="images/ArrayMerge_EqualTop.svg" type="image/svg+xml"/></p>
	 */
    public static PredictionContext MergeArrays(
        ArrayPredictionContext a,
        ArrayPredictionContext b,
        bool rootIsWildcard,
        DoubleKeyMap<PredictionContext, PredictionContext, PredictionContext> mergeCache)
    {
        if (mergeCache != null)
        {
            var previous = mergeCache.Get(a, b);
            if (previous != null) return previous;
            previous = mergeCache.Get(b, a);
            if (previous != null) return previous;
        }

        // merge sorted payloads a + b => M
        int i = 0; // walks a
        int j = 0; // walks b
        int k = 0; // walks target M array

        var mergedReturnStates =
            new int[a.returnStates.Length + b.returnStates.Length];
        var mergedParents =
            new PredictionContext[a.returnStates.Length + b.returnStates.Length];
        // walk and merge to yield mergedParents, mergedReturnStates
        while (i < a.returnStates.Length && j < b.returnStates.Length)
        {
            var a_parent = a.parents[i];
            var b_parent = b.parents[j];
            if (a.returnStates[i] == b.returnStates[j])
            {
                // same payload (stack tops are equal), must yield merged singleton
                int payload = a.returnStates[i];
                // $+$ = $
                bool both = payload == EMPTY_RETURN_STATE &&
                                a_parent == null && b_parent == null;
                bool ax_ax = (a_parent != null && b_parent != null) &&
                                a_parent.Equals(b_parent); // ax+ax -> ax
                if (both || ax_ax)
                {
                    mergedParents[k] = a_parent; // choose left
                    mergedReturnStates[k] = payload;
                }
                else
                { // ax+ay -> a'[x,y]
                    var mergedParent =
                        Merge(a_parent, b_parent, rootIsWildcard, mergeCache);
                    mergedParents[k] = mergedParent;
                    mergedReturnStates[k] = payload;
                }
                i++; // hop over left one as usual
                j++; // but also skip one in right side since we merge
            }
            else if (a.returnStates[i] < b.returnStates[j])
            { // copy a[i] to M
                mergedParents[k] = a_parent;
                mergedReturnStates[k] = a.returnStates[i];
                i++;
            }
            else
            { // b > a, copy b[j] to M
                mergedParents[k] = b_parent;
                mergedReturnStates[k] = b.returnStates[j];
                j++;
            }
            k++;
        }

        // copy over any payloads remaining in either array
        if (i < a.returnStates.Length)
        {
            for (int p = i; p < a.returnStates.Length; p++)
            {
                mergedParents[k] = a.parents[p];
                mergedReturnStates[k] = a.returnStates[p];
                k++;
            }
        }
        else
        {
            for (int p = j; p < b.returnStates.Length; p++)
            {
                mergedParents[k] = b.parents[p];
                mergedReturnStates[k] = b.returnStates[p];
                k++;
            }
        }

        // trim merged if we combined a few that had same stack tops
        if (k < mergedParents.Length)
        { // write index < last position; trim
            if (k == 1)
            { // for just one merged element, return singleton top
                var a_ =
                    SingletonPredictionContext.Create(mergedParents[0],
                                                      mergedReturnStates[0]);
                mergeCache?.Put(a, b, a_);
                return a_;
            }
            Array.Resize(ref mergedParents, k);
            //mergedParents = Arrays.copyOf(mergedParents, k);
            Array.Resize(ref mergedReturnStates, k);
            //mergedReturnStates = Arrays.copyOf(mergedReturnStates, k);
        }

        var M =
            new ArrayPredictionContext(mergedParents, mergedReturnStates);

        // if we created same array as a or b, return that instead
        // TODO: track whether this is possible above during merge sort for speed
        if (M.Equals(a))
        {
            mergeCache?.Put(a, b, a);
            return a;
        }
        if (M.Equals(b))
        {
            mergeCache?.Put(a, b, b);
            return b;
        }

        CombineCommonParents(mergedParents);

        mergeCache?.Put(a, b, M);
        return M;
    }

    /**
	 * Make pass over all <em>M</em> {@code parents}; merge any {@code equals()}
	 * ones.
	 */
    protected static void CombineCommonParents(PredictionContext[] parents)
    {
        Dictionary<PredictionContext, PredictionContext> uniqueParents =
            new();

        for (int p = 0; p < parents.Length; p++)
        {
            PredictionContext parent = parents[p];
            if (!uniqueParents.ContainsKey(parent))
            { // don't replace
                uniqueParents[parent] = parent;
            }
        }

        for (int p = 0; p < parents.Length; p++)
        {
            if (uniqueParents.TryGetValue(parents[p], out var parent))
            {
                parents[p] = parent;// uniqueParents[(parents[p])];
            }
        }
    }

    public static string ToDOTString(PredictionContext context)
    {
        if (context == null) return "";
        var buffer = new StringBuilder();
        buffer.Append("digraph G {\n");
        buffer.Append("rankdir=LR;\n");

        var nodes = GetAllContextNodes(context);
        nodes.Sort((o1, o2) => o1.id - o2.id);
        //Collections.sort(nodes, new Comparator<PredictionContext>() {
        //	@Override
        //	public int compare(PredictionContext o1, PredictionContext o2) {
        //		return o1.id - o2.id;
        //	}
        //});

        foreach (var current in nodes)
        {
            if (current is SingletonPredictionContext)
            {
                var s = current.id.ToString();
                buffer.Append("  s").Append(s);
                var returnState = (current.GetReturnState(0).ToString());
                if (current is EmptyPredictionContext) returnState = "$";
                buffer.Append(" [label=\"").Append(returnState).Append("\"];\n");
                continue;
            }
            var arr = (ArrayPredictionContext)current;
            buffer.Append("  s").Append(arr.id);
            buffer.Append(" [shape=box, label=\"");
            buffer.Append('[');
            bool first = true;
            foreach (int inv in arr.returnStates)
            {
                if (!first) buffer.Append(", ");
                if (inv == EMPTY_RETURN_STATE) buffer.Append("$");
                else buffer.Append(inv);
                first = false;
            }
            buffer.Append("]");
            buffer.Append("\"];\n");
        }

        foreach (var current in nodes)
        {
            if (current == EmptyPredictionContext.Instance) continue;
            for (int i = 0; i < current.Count; i++)
            {
                if (current.GetParent(i) == null) continue;
                var s = (current.id.ToString());
                buffer.Append("  s").Append(s);
                buffer.Append("->");
                buffer.Append('s');
                buffer.Append(current.GetParent(i).id);
                if (current.Count > 1) buffer.Append(" [label=\"parent[" + i + "]\"];\n");
                else buffer.Append(";\n");
            }
        }

        buffer.Append("}\n");
        return buffer.ToString();
    }

    // From Sam
    public static PredictionContext GetCachedContext(
        PredictionContext context,
        PredictionContextCache contextCache,
        Dictionary<PredictionContext, PredictionContext> visited)
    {
        if (context.IsEmpty)
        {
            return context;
        }

        if (visited.TryGetValue(context, out var existing))
        {
            return existing;
        }

        existing = contextCache.Get(context);
        if (existing != null)
        {
            visited.Add(context, existing);
            return existing;
        }

        bool changed = false;
        var parents = new PredictionContext[context.Count];
        for (int i = 0; i < parents.Length; i++)
        {
            var parent = GetCachedContext(context.GetParent(i), contextCache, visited);
            if (changed || parent != context.GetParent(i))
            {
                if (!changed)
                {
                    parents = new PredictionContext[context.Count];
                    for (int j = 0; j < context.Count; j++)
                    {
                        parents[j] = context.GetParent(j);
                    }

                    changed = true;
                }

                parents[i] = parent;
            }
        }

        if (!changed)
        {
            contextCache.Add(context);
            visited[context] = context;
            return context;
        }

        PredictionContext updated;
        if (parents.Length == 0)
        {
            updated = EmptyPredictionContext.Instance;
        }
        else if (parents.Length == 1)
        {
            updated = SingletonPredictionContext.Create(parents[0], context.GetReturnState(0));
        }
        else
        {
            var arrayPredictionContext = (ArrayPredictionContext)context;
            updated = new ArrayPredictionContext(parents, arrayPredictionContext.returnStates);
        }

        contextCache.Add(updated);
        visited[updated] = updated;
        visited[context] = context;
        return updated;
    }

    //	// extra structures, but cut/paste/morphed works, so leave it.
    //	// seems to do a breadth-first walk
    //	public static List<PredictionContext> getAllNodes(PredictionContext context) {
    //		Dictionary<PredictionContext, PredictionContext> visited =
    //			new IdentityHashMap<PredictionContext, PredictionContext>();
    //		Deque<PredictionContext> workList = new ArrayDeque<PredictionContext>();
    //		workList.add(context);
    //		visited.put(context, context);
    //		List<PredictionContext> nodes = new ArrayList<PredictionContext>();
    //		while (!workList.isEmpty()) {
    //			PredictionContext current = workList.pop();
    //			nodes.add(current);
    //			for (int i = 0; i < current.size(); i++) {
    //				PredictionContext parent = current.getParent(i);
    //				if ( parent!=null && visited.put(parent, parent) == null) {
    //					workList.push(parent);
    //				}
    //			}
    //		}
    //		return nodes;
    //	}

    // ter's recursive version of Sam's getAllNodes()
    public static List<PredictionContext> GetAllContextNodes(PredictionContext context)
    {
        List<PredictionContext> nodes = new();
        Dictionary<PredictionContext, PredictionContext> visited =
            new();
        GetAllContextNodes(context, nodes, visited);
        return nodes;
    }

    public static void GetAllContextNodes(PredictionContext context,
                                           List<PredictionContext> nodes,
                                           Dictionary<PredictionContext, PredictionContext> visited)
    {
        if (context == null || visited.ContainsKey(context)) return;
        visited[context] = context;
        nodes.Add(context);
        for (int i = 0; i < context.Count; i++)
        {
            GetAllContextNodes(context.GetParent(i), nodes, visited);
        }
    }

    public string ToString(Recognizer<Token, ATNSimulator> recog) 
        => ToString();//		return toString(recog, ParserRuleContext.EMPTY);

    public string[] ToStrings(Recognizer<Token, ATNSimulator> recognizer, int currentState)
        => ToStrings(recognizer, EmptyPredictionContext.Instance, currentState);

    // FROM SAM
    public string[] ToStrings(Recognizer<Token, ATNSimulator> recognizer, PredictionContext stop, int currentState)
    {
        List<string> result = new();

        int perm = 0;

    outer:
        for (; ; perm++)
        {
            int offset = 0;
            bool last = true;
            PredictionContext p = this;
            int stateNumber = currentState;
            var localBuffer = new StringBuilder();
            localBuffer.Append('[');
            while (!p.IsEmpty && p != stop)
            {
                int index = 0;
                if (p.Count > 0)
                {
                    int bits = 1;
                    while ((1 << bits) < p.Count)
                    {
                        bits++;
                    }

                    int mask = (1 << bits) - 1;
                    index = (perm >> offset) & mask;
                    last &= index >= p.Count - 1;
                    if (index >= p.Count)
                    {
                        goto outer;
                    }
                    offset += bits;
                }

                if (recognizer != null)
                {
                    if (localBuffer.Length > 1)
                    {
                        // first char is '[', if more than that this isn't the first rule
                        localBuffer.Append(' ');
                    }

                    var atn = recognizer.getATN();
                    var s = atn.states[(stateNumber)];
                    var ruleName = recognizer.getRuleNames()[s.ruleIndex];
                    localBuffer.Append(ruleName);
                }
                else if (p.GetReturnState(index) != EMPTY_RETURN_STATE)
                {
                    if (!p.IsEmpty)
                    {
                        if (localBuffer.Length > 1)
                        {
                            // first char is '[', if more than that this isn't the first rule
                            localBuffer.Append(' ');
                        }

                        localBuffer.Append(p.GetReturnState(index));
                    }
                }
                stateNumber = p.GetReturnState(index);
                p = p.GetParent(index);
            }
            localBuffer.Append(']');
            result.Add(localBuffer.ToString());

            if (last)
            {
                break;
            }
        }

        return result.ToArray();
    }
}
