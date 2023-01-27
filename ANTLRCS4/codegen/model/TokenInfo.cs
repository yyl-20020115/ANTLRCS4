namespace org.antlr.v4.codegen.model;

public class TokenInfo {
	public readonly int type;
	public readonly String name;

	public TokenInfo(int type, String name) {
		this.type = type;
		this.name = name;
	}

	//@Override
	public String toString() {
		return "TokenInfo{" +
				"type=" + type +
				", name='" + name + '\'' +
				'}';
	}
}
