using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.codegen.model;

/** A serialized ATN for the java target, which requires we use strings and 16-bit unicode values */
public class SerializedJavaATN : SerializedATN {
	private readonly String[] serializedAsString;
	private readonly String[][] segments;

	public SerializedJavaATN(OutputModelFactory factory, ATN atn):base(factory) {
		IntegerList data = ATNSerializer.getSerialized(atn);
		data = ATNDeserializer.encodeIntsWith16BitWords(data);

		int size = data.size();
		Target target = factory.getGenerator().getTarget();
		int segmentLimit = target.getSerializedATNSegmentLimit();
		segments = new String[(int)(((long)size + segmentLimit - 1) / segmentLimit)][];
		int segmentIndex = 0;

		for (int i = 0; i < size; i += segmentLimit) {
			int segmentSize = Math.Min(i + segmentLimit, size) - i;
			String[] segment = new String[segmentSize];
			segments[segmentIndex++] = segment;
			for (int j = 0; j < segmentSize; j++) {
				segment[j] = target.encodeInt16AsCharEscape(data.get(i + j));
			}
		}

		serializedAsString = segments[0]; // serializedAsString is valid if only one segment
	}

	public Object getSerialized() { return serializedAsString; }
	public String[][] getSegments() { return segments; }
}
