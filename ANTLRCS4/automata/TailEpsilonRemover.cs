/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;

namespace org.antlr.v4.automata;

/**
 *
 * @author Terence Parr
 */
public class TailEpsilonRemover : ATNVisitor
{

    private readonly ATN _atn;

    public TailEpsilonRemover(ATN atn)
    {
        this._atn = atn;
    }

    public override void VisitState(ATNState p)
    {
        if (p.getStateType() == ATNState.BASIC && p.getNumberOfTransitions() == 1)
        {
            var q = p.transition(0).target;
            if (p.transition(0) is RuleTransition transition)
            {
                q = transition.followState;
            }
            if (q.getStateType() == ATNState.BASIC)
            {
                // we have p-x->q for x in {rule, action, pred, token, ...}
                // if edge out of q is single epsilon to block end
                // we can strip epsilon p-x->q-eps->r
                var trans = q.transition(0);
                if (q.getNumberOfTransitions() == 1 && trans is EpsilonTransition)
                {
                    var r = trans.target;
                    if (r is BlockEndState || r is PlusLoopbackState || r is StarLoopbackState)
                    {
                        // skip over q
                        if (p.transition(0) is RuleTransition transition1)
                        {
                            transition1.followState = r;
                        }
                        else
                        {
                            p.transition(0).target = r;
                        }
                        _atn.removeState(q);
                    }
                }
            }
        }
    }
}
