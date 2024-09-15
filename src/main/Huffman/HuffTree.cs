using System.Diagnostics.CodeAnalysis;

namespace Huffman;

/// <summary>
///     Class to represent the Huffman tree Node
/// </summary>
[SuppressMessage("Design", "CA1051:Do not declare visible instance fields")]
public sealed class Node
{
    public int freq;
    public NodeType nt;
    public int order;
    public Node right, left, parent;
    public int sym;

    public Node(NodeType nt, int sym, int freq, int order)
    {
        this.nt = nt;
        this.sym = sym;
        this.freq = freq;
        this.order = order;
    }

    private Node()
    {
    }
}
