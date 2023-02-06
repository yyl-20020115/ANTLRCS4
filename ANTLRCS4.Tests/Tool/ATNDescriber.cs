using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using System.Text;

namespace org.antlr.v4.test.tool;



/** Make human readable set of ints from serialized ATN like this (for debugging / testing):
 *
 * max type 1
 * 0:TOKEN_START -1
 * 1:RULE_START 0
 * 2:RULE_STOP 0
 * 3:BASIC 0
 * 4:BASIC 0
 * rule 0:1 1
 * mode 0:0
 * 0:'a'..128169
 * 0->1 EPSILON 0,0,0
 * 1->3 EPSILON 0,0,0
 * 3->4 SET 0,0,0
 * 4->2 EPSILON 0,0,0
 * 0:0
 */
public class ATNDescriber
{
    public ATN atn;
    private readonly List<string> tokenNames;

    public ATNDescriber(ATN atn, List<string> tokenNames)
    {
        //assert atn.grammarType != null;
        this.atn = atn;
        this.tokenNames = tokenNames;
    }

    /** For testing really; gives a human readable version of the ATN */
    public string Decode(int[] data)
    {
        var buffer = new StringBuilder();
        int p = 0;
        int version = data[p++];
        if (version != ATNDeserializer.SERIALIZED_VERSION)
        {
            var reason = $"Could not deserialize ATN with version {version} (expected {ATNDeserializer.SERIALIZED_VERSION}).";
            throw new UnsupportedOperationException(reason, new InvalidOperationException(typeof(ATN).Name));
        }

        p++; // skip grammarType
        int maxType = data[p++];
        buffer.Append("max type ").Append(maxType).Append('\n');
        int nstates = data[p++];
        for (int i = 0; i < nstates; i++)
        {
            int stype = data[p++];
            if (stype == ATNState.INVALID_TYPE) continue; // ignore bad type of states
            int ruleIndex = data[p++];
            if (ruleIndex == char.MaxValue)
            {
                ruleIndex = -1;
            }

            var arg = "";
            if (stype == ATNState.LOOP_END)
            {
                int loopBackStateNumber = data[p++];
                arg = " " + loopBackStateNumber;
            }
            else if (stype == ATNState.PLUS_BLOCK_START || stype == ATNState.STAR_BLOCK_START || stype == ATNState.BLOCK_START)
            {
                int endStateNumber = data[p++];
                arg = " " + endStateNumber;
            }
            buffer.Append(i).Append(':')
                    .Append(ATNState.serializationNames[(stype)]).Append(' ')
                    .Append(ruleIndex).Append(arg).Append('\n');
        }
        // this code is meant to model the form of ATNDeserializer.deserialize,
        // since both need to be updated together whenever a change is made to
        // the serialization format. The "dead" code is only used in debugging
        // and testing scenarios, so the form you see here was kept for
        // improved maintainability.
        // start
        int numNonGreedyStates = data[p++];
        for (int i = 0; i < numNonGreedyStates; i++)
        {
            int stateNumber = data[p++];
        }
        int numPrecedenceStates = data[p++];
        for (int i = 0; i < numPrecedenceStates; i++)
        {
            int stateNumber = data[p++];
        }
        // finish
        int nrules = data[p++];
        for (int i = 0; i < nrules; i++)
        {
            int s = data[p++];
            if (atn.grammarType == ATNType.LEXER)
            {
                int arg1 = data[p++];
                buffer.Append("rule ").Append(i).Append(':').Append(s).Append(' ').Append(arg1).Append('\n');
            }
            else
            {
                buffer.Append("rule ").Append(i).Append(':').Append(s).Append('\n');
            }
        }
        int nmodes = data[p++];
        for (int i = 0; i < nmodes; i++)
        {
            int s = data[p++];
            buffer.Append("mode ").Append(i).Append(':').Append(s).Append('\n');
        }
        int numBMPSets = data[p++];
        p = AppendSets(buffer, data, p, numBMPSets);
        int nedges = data[p++];
        for (int i = 0; i < nedges; i++)
        {
            int src = data[p];
            int trg = data[p + 1];
            int ttype = data[p + 2];
            int arg1 = data[p + 3];
            int arg2 = data[p + 4];
            int arg3 = data[p + 5];
            buffer.Append(src).Append("->").Append(trg)
                    .Append(' ').Append(Transition.serializationNames[(ttype)])
                    .Append(' ').Append(arg1).Append(',').Append(arg2).Append(',').Append(arg3)
                    .Append('\n');
            p += 6;
        }
        int ndecisions = data[p++];
        for (int i = 0; i < ndecisions; i++)
        {
            int s = data[p++];
            buffer.Append(i).Append(':').Append(s).Append('\n');
        }
        if (atn.grammarType == ATNType.LEXER)
        {
            // this code is meant to model the form of ATNDeserializer.deserialize,
            // since both need to be updated together whenever a change is made to
            // the serialization format. The "dead" code is only used in debugging
            // and testing scenarios, so the form you see here was kept for
            // improved maintainability.
            int lexerActionCount = data[p++];
            for (int i = 0; i < lexerActionCount; i++)
            {
                var actionType = (LexerActionType)data[p++];
                int data1 = data[p++];
                int data2 = data[p++];
            }
        }
        return buffer.ToString();
    }

    private int AppendSets(StringBuilder buffer, int[] data, int p, int nsets)
    {
        for (int i = 0; i < nsets; i++)
        {
            int nintervals = data[p++];
            buffer.Append(i).Append(':');
            bool containsEof = data[p++] != 0;
            if (containsEof)
            {
                buffer.Append(GetTokenName(Token.EOF));
            }

            for (int j = 0; j < nintervals; j++)
            {
                if (containsEof || j > 0)
                {
                    buffer.Append(", ");
                }

                int a = data[p++];
                int b = data[p++];
                buffer.Append(GetTokenName(a)).Append("..").Append(GetTokenName(b));
            }
            buffer.Append('\n');
        }
        return p;
    }

    public string GetTokenName(int t)
    {
        if (t == -1) return "EOF";

        if (atn.grammarType == ATNType.LEXER &&
                t >= char.MinValue && t <= char.MaxValue)
        {
            switch (t)
            {
                case '\n':
                    return "'\\n'";
                case '\r':
                    return "'\\r'";
                case '\t':
                    return "'\\t'";
                case '\b':
                    return "'\\b'";
                case '\f':
                    return "'\\f'";
                case '\\':
                    return "'\\\\'";
                case '\'':
                    return "'\\''";
                default:
                    if ((t >= 0x20 && t <= 0x7f) &&
                            !char.IsControl((char)t))
                    {
                        return '\'' + ((char)t).ToString() + '\'';
                    }
                    // turn on the bit above max "\uFFFF" value so that we pad with zeros
                    // then only take last 4 digits
                    //String hex = Integer.toHexString(t|0x10000).ToUpper().substring(1,5-1);
                    String unicodeStr = $"'\\u{t:X4}'";
                    return unicodeStr;
            }
        }

        if (tokenNames != null && t >= 0 && t < tokenNames.Count)
        {
            return tokenNames[(t)];
        }

        return t.ToString();
    }
}
