using org.antlr.v4.tool;
using System.Text;

namespace org.antlr.v4.automata;

public class RangeBorderCharactersData {
	public readonly int lowerFrom;
	public readonly int upperFrom;
	public readonly int lowerTo;
	public readonly int upperTo;
	public readonly bool mixOfLowerAndUpperCharCase;

	public RangeBorderCharactersData(int lowerFrom, int upperFrom, int lowerTo, int upperTo, bool mixOfLowerAndUpperCharCase) {
		this.lowerFrom = lowerFrom;
		this.upperFrom = upperFrom;
		this.lowerTo = lowerTo;
		this.upperTo = upperTo;
		this.mixOfLowerAndUpperCharCase = mixOfLowerAndUpperCharCase;
	}

	public static RangeBorderCharactersData getAndCheckCharactersData(int from, int to, Grammar grammar, CommonTree tree,
																	  bool reportRangeContainsNotImpliedCharacters
	) {
		int lowerFrom = Character.toLowerCase(from);
		int upperFrom = Character.toUpperCase(from);
		int lowerTo = Character.toLowerCase(to);
		int upperTo = Character.toUpperCase(to);

		bool isLowerFrom = lowerFrom == from;
		bool isLowerTo = lowerTo == to;
		bool mixOfLowerAndUpperCharCase = isLowerFrom && !isLowerTo || !isLowerFrom && isLowerTo;
		if (reportRangeContainsNotImpliedCharacters && mixOfLowerAndUpperCharCase && from <= 0x7F && to <= 0x7F) {
			StringBuilder notImpliedCharacters = new StringBuilder();
			for (int i = from; i < to; i++) {
				if (!Character.isAlphabetic(i)) {
					notImpliedCharacters.append((char)i);
				}
			}
			if (notImpliedCharacters.length() > 0) {
				grammar.tool.errMgr.grammarError(ErrorType.RANGE_PROBABLY_CONTAINS_NOT_IMPLIED_CHARACTERS, grammar.fileName, tree.getToken(),
						(char) from, (char) to, notImpliedCharacters.toString());
			}
		}
		return new RangeBorderCharactersData(lowerFrom, upperFrom, lowerTo, upperTo, mixOfLowerAndUpperCharCase);
	}

	public bool isSingleRange() {
		return lowerFrom == upperFrom && lowerTo == upperTo ||
				mixOfLowerAndUpperCharCase ||
				lowerTo - lowerFrom != upperTo - upperFrom;
	}
}
