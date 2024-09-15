using System.Diagnostics.CodeAnalysis;

namespace BurrowsWheelerTransform;

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Approved")]
internal sealed class HuffmanNode : IComparable<HuffmanNode>
{
    public HuffmanNode Left;
    public HuffmanNode Right;
    public int Size = 1;
    public byte Value;

    private readonly int _frequency;

    public HuffmanNode()
    {
    }

    public HuffmanNode(byte value, int frequency)
    {
        Value = value;
        _frequency = frequency;
        Left = null;
        Right = null;
    }

    public HuffmanNode(HuffmanNode left, HuffmanNode right)
    {
        _frequency = left._frequency + right._frequency;
        Size = left.Size + right.Size + 1;
        Left = left;
        Right = right;
    }

    public int CompareTo(HuffmanNode other)
    {
        return _frequency != other._frequency
            ? _frequency.CompareTo(other._frequency)
            : Value.CompareTo(other.Value);
    }

    public bool IsLeaf()
    {
        return Left is null && Right is null;
    }

    public override string ToString()
    {
        return $"{Value},{_frequency}";
    }
}
