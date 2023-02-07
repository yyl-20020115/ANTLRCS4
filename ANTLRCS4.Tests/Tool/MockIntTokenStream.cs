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
    public void Consume() => p++;

    ////@Override
    public int LA(int i) => LT(i).Type;

    ////@Override
    public int Mark() => Index;

    ////@Override
    public int Index => p;

    ////@Override
    public void Release(int marker) => Seek(marker);

    ////@Override
    public void Seek(int index) => p = index;

    ////@Override
    public int Count => types.Size;

    ////@Override
    public string SourceName => IntStream.UNKNOWN_SOURCE_NAME;

    ////@Override
    public Token LT(int i)
    {
        CommonToken t;
        int rawIndex = p + i - 1;
        if (rawIndex >= types.Size) t = new CommonToken(Token.EOF);
        else t = new CommonToken(types.Get(rawIndex));
        t.        TokenIndex = rawIndex;
        return t;
    }

    ////@Override
    public Token Get(int i) => new CommonToken(types.Get(i));

    ////@Override
    public TokenSource TokenSource => null;


    ////@Override
    public string Text => throw new UnsupportedOperationException("can't give strings");


    ////@Override
    public string GetText(Interval interval)
    {
        throw new UnsupportedOperationException("can't give strings");
    }


    ////@Override
    public string GetText(RuleContext ctx)
    {
        throw new UnsupportedOperationException("can't give strings");
    }


    ////@Override
    public string GetText(Token start, Token stop)
    {
        throw new UnsupportedOperationException("can't give strings");
    }

    public int Range()
    {
        throw new NotImplementedException();
    }

    public string ToString(int start, int stop)
    {
        throw new NotImplementedException();
    }

    public string ToString(Token start, Token stop)
    {
        throw new NotImplementedException();
    }

    public void Rewind(int nvaeMark)
    {
        throw new NotImplementedException();
    }

    public void Rewind()
    {
        throw new NotImplementedException();
    }
}
