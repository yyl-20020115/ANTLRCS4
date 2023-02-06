using org.antlr.v4.runtime;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.test.tool;

public class MockIntTokenStream : TokenStream
{
    public IntegerList types;
    int p = 0;

    public MockIntTokenStream(IntegerList types) => this.types = types;

    ////@Override
    public void consume() => p++;

    ////@Override
    public int LA(int i) => LT(i).getType();

    ////@Override
    public int mark() => index();

    ////@Override
    public int index() => p;

    ////@Override
    public void release(int marker) => seek(marker);

    ////@Override
    public void seek(int index) => p = index;

    ////@Override
    public int Count => types.Size();

    ////@Override
    public string getSourceName() => IntStream.UNKNOWN_SOURCE_NAME;

    ////@Override
    public Token LT(int i)
    {
        CommonToken t;
        int rawIndex = p + i - 1;
        if (rawIndex >= types.Size()) t = new CommonToken(Token.EOF);
        else t = new CommonToken(types.Get(rawIndex));
        t.setTokenIndex(rawIndex);
        return t;
    }

    ////@Override
    public Token get(int i) => new CommonToken(types.Get(i));

    ////@Override
    public TokenSource getTokenSource()
    {
        return null;
    }


    ////@Override
    public string getText()
    {
        throw new UnsupportedOperationException("can't give strings");
    }


    ////@Override
    public string getText(Interval interval)
    {
        throw new UnsupportedOperationException("can't give strings");
    }


    ////@Override
    public string getText(RuleContext ctx)
    {
        throw new UnsupportedOperationException("can't give strings");
    }


    ////@Override
    public string getText(Token start, Token stop)
    {
        throw new UnsupportedOperationException("can't give strings");
    }

    public int range()
    {
        throw new NotImplementedException();
    }

    public string toString(int start, int stop)
    {
        throw new NotImplementedException();
    }

    public string toString(Token start, Token stop)
    {
        throw new NotImplementedException();
    }

    public void rewind(int nvaeMark)
    {
        throw new NotImplementedException();
    }

    public void rewind()
    {
        throw new NotImplementedException();
    }
}
