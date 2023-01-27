// $ANTLR 3.5.3 org\\antlr\\v4\\parse\\ActionSplitter.g 2023-01-27 22:27:34

using org.antlr.v4.runtime;

namespace org.antlr.v4.parse;

public class MismatchedSetException :RecognitionException
{
    private object value;
    private IntStream input;

    public MismatchedSetException(object value, IntStream input)
        :base(null,input,null)
    {
        this.value = value;
        this.input = input;
    }
}