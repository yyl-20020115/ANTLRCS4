using org.antlr.v4.runtime.atn;

namespace org.antlr.v4.codegen.model;

/** A serialized ATN for the java target, which requires we use strings and 16-bit unicode values */
public class SerializedJavaATN : SerializedATN
{
    private readonly string[] serializedAsString;
    private readonly string[][] segments;

    public SerializedJavaATN(OutputModelFactory factory, ATN atn) : base(factory)
    {
        var data = ATNSerializer.GetSerialized(atn);
        data = ATNDeserializer.EncodeIntsWith16BitWords(data);

        int size = data.Size();
        var target = factory.GetGenerator().Target;
        int segmentLimit = target.GetSerializedATNSegmentLimit();
        segments = new string[(int)(((long)size + segmentLimit - 1) / segmentLimit)][];
        int segmentIndex = 0;

        for (int i = 0; i < size; i += segmentLimit)
        {
            var segmentSize = Math.Min(i + segmentLimit, size) - i;
            var segment = new string[segmentSize];
            segments[segmentIndex++] = segment;
            for (int j = 0; j < segmentSize; j++)
                segment[j] = target.EncodeInt16AsCharEscape(data.Get(i + j));
        }

        serializedAsString = segments[0]; // serializedAsString is valid if only one segment
    }

    public override object GetSerialized() => serializedAsString;
    public string[][] GetSegments() => segments;
}
