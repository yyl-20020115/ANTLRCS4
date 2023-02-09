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

    public override string ToString() => name + "; isParser:" + isParser;
}
