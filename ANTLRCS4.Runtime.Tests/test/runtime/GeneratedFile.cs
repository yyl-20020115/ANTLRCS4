namespace org.antlr.v4.test.runtime;

public class GeneratedFile {
	public readonly String name;
	public readonly bool isParser;

	public GeneratedFile(String name, bool isParser) {
		this.name = name;
		this.isParser = isParser;
	}

	//@Override
	public String ToString() {
		return name + "; isParser:" + isParser;
	}
}
