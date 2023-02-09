namespace org.antlr.v4.codegen.model;

public class TokenInfo
{
    public readonly int type;
    public readonly string name;

    public TokenInfo(int type, string name)
    {
        this.type = type;
        this.name = name;
    }

    public override string ToString() => "TokenInfo{" +
                "type=" + type +
                ", name='" + name + '\'' +
                '}';
}
