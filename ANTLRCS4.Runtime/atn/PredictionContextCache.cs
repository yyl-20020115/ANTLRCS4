/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.atn;


/** Used to cache {@link PredictionContext} objects. Its used for the shared
 *  context cash associated with contexts in DFA states. This cache
 *  can be used for both lexers and parsers.
 */
public class PredictionContextCache
{
    protected readonly Dictionary<PredictionContext, PredictionContext> cache = new();

    /** Add a context to the cache and return it. If the context already exists,
	 *  return that one instead and do not add a new context to the cache.
	 *  Protect shared cache from unsafe thread access.
	 */
    public PredictionContext Add(PredictionContext ctx)
    {
        if (ctx == EmptyPredictionContext.Instance) return EmptyPredictionContext.Instance;
        if (cache.TryGetValue(ctx, out var existing))
        {
            //			Console.Out.WriteLine(name+" reuses "+existing);
            return existing;
        }
        cache[ctx] = ctx;
        return ctx;
    }

    public PredictionContext Get(PredictionContext ctx) 
        => cache.TryGetValue(ctx, out var prediction) ? prediction : null;

    public int Size => cache.Count;
}
