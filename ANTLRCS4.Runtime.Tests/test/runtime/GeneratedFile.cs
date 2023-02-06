namespace org.antlr.v4.test.runtime;

public class GeneratedFile
{
    public readonly string name;
    public readonly bool isParser;

    public GeneratedFile(string name, bool isParser)
    {
        this.name = name;
        this.isParser = isParser;
    }

    //@Override
    public override string ToString() => name + "; isParser:" + isParser;
}
