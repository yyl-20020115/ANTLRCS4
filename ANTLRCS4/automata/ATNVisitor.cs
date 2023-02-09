/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;

namespace org.antlr.v4.automata;



/** A simple visitor that walks everywhere it can go starting from s,
 *  without going into an infinite cycle. Override and implement
 *  visitState() to provide functionality.
 */
public class ATNVisitor
{
    public void Visit(ATNState s) => Visit(s, new());

    public void Visit(ATNState s, HashSet<int> visited)
    {
        if (!visited.Add(s.stateNumber)) return;
        visited.Add(s.stateNumber);

        VisitState(s);
        int n = s.NumberOfTransitions;
        for (int i = 0; i < n; i++)
        {
            var t = s.Transition(i);
            Visit(t.target, visited);
        }
    }

    public virtual void VisitState(ATNState s) { }
}
