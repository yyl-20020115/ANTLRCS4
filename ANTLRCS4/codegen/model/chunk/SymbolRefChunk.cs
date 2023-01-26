using org.antlr.v4.codegen.model.decl;

namespace org.antlr.v4.codegen.model.chunk;


public abstract class SymbolRefChunk : ActionChunk {
	public readonly String name;
	public readonly String escapedName;

	public SymbolRefChunk(StructDecl ctx, String name, String escapedName) :base(ctx){
		this.name = name;
		this.escapedName = escapedName;
	}
}
