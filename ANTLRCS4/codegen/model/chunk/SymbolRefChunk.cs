using org.antlr.v4.codegen.model.decl;

namespace org.antlr.v4.codegen.model.chunk;


public abstract class SymbolRefChunk : ActionChunk
{
    public readonly string name;
    public readonly string escapedName;

    public SymbolRefChunk(StructDecl ctx, string name, string escapedName) : base(ctx)
    {
        this.name = name;
        this.escapedName = escapedName;
    }
}
