namespace org.antlr.v4.automata;

public class CharactersDataCheckStatus {
	public readonly bool collision;
	public readonly bool notImpliedCharacters;

	public CharactersDataCheckStatus(bool collision, bool notImpliedCharacters) {
		this.collision = collision;
		this.notImpliedCharacters = notImpliedCharacters;
	}
}
