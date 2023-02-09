using org.antlr.v4.runtime;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.test.tool;

public class MockIntTokenStream : TokenStream
{
    public IntegerList types;
    int p = 0;

    public MockIntTokenStream(IntegerList types) => this.types = types;

    
    public void Consume() => p++;

    
    public int LA(int i) => LT(i).Type;

    
    public int Mark() => Index;

    
    public int Index => p;

    
    public void Release(int marker) => Seek(marker);

    
    public void Seek(int index) => p = index;

    
    public int Count => types.Size;

    
    public string SourceName => IntStream.UNKNOWN_SOURCE_NAME;

    
    public virtual Token LT(int i)
    {
        CommonToken t;
        int rawIndex = p + i - 1;
        if (rawIndex >= types.Size) t = new CommonToken(Token.EOF);
        else t = new CommonToken(types.Get(rawIndex));
        t.        TokenIndex = rawIndex;
        return t;
    }

    
    public virtual Token Get(int i) => new CommonToken(types.Get(i));

    
    public virtual TokenSource TokenSource => null;


    
    public string Text => throw new UnsupportedOperationException("can't give strings");


    
    public string GetText(Interval interval)
    {
        throw new UnsupportedOperationException("can't give strings");
    }


    
    public string GetText(RuleContext ctx)
    {
        throw new UnsupportedOperationException("can't give strings");
    }


    
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
