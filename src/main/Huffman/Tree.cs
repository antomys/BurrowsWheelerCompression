namespace Huffman;

/// <summary>
///     The class representing the adaptive Huffman tree
///     providing the full range of needed operations
/// </summary>
public sealed class Tree
{
    public const int CharIsEof = -1;
    private int _tempcode, _count;
    private bool InNyt;

    private readonly int nodeCount;

    //Array of pointers to nodes to avoid expensive DFS
    private readonly Node[] nodes;

    private Node ptr;

    //Reference to the root node
    private readonly Node root;

    /// <summary>
    ///     The constructor for the Tree class
    /// </summary>
    public Tree()
    {
        root = new Node(NodeType.INT, 258, 0, 516);
        root.right = new Node(NodeType.NYT, 257, 0, root.order - 1);
        root.left = new Node(NodeType.EOF, 256, 0, root.order - 2)
        {
            parent = root.right.parent = root
        };
        nodes = new Node[259];
        nodes[256] = root.left;
        nodes[257] = root.right;
        nodeCount = 258;
    }

    /// <summary>
    ///     The method to perform tree reconfiguration
    /// </summary>
    /// <param name="n1">
    ///     A <see cref="Node" />
    ///     Reference to the first node
    /// </param>
    /// <param name="n2">
    ///     A <see cref="Node" />
    ///     Reference to the second node
    /// </param>
    private void SwapNodes(Node n1, Node n2)
    {
        (n1.freq, n2.freq) = (n2.freq, n1.freq);

        var t1 = n1.parent.left;
        var t2 = n2.parent.left;

        //Determine if n1 was the left or the right child
        if (t1 == n1)
        {
            n1.parent.left = n2;
        }
        else
        {
            n1.parent.right = n2;
        }

        //Same thing for the second node
        if (t2 == n2)
        {
            n2.parent.left = n1;
        }
        else
        {
            n2.parent.right = n1;
        }

        //Swap the nodes
        (n1.parent, n2.parent) = (n2.parent, n1.parent);
    }

    /// <summary>
    ///     Returns the reference to the node that has the same frequency
    ///     as the argument and highest order
    /// </summary>
    /// <param name="nd">
    ///     A <see cref="Node" />
    ///     Reference to the node
    /// </param>
    /// <returns>
    ///     A <see cref="Node" />
    /// </returns>
    private Node FindHighestWithSameFreq(Node nd)
    {
        var current = nd;
        if (nd.parent == null)
        {
            return current;
        }

        var nd2 = current.parent;
        if (nd2.left == current && nd2.right.freq == current.freq)
        {
            current = nd2.right;
        }

        if (nd2.parent == null)
        {
            return current;
        }

        var nd3 = nd2.parent;
        if (nd3.left == nd2 && nd3.right.freq == current.freq)
        {
            current = nd3.right;
        }
        else if (nd3.right == nd2 && nd3.left.freq == current.freq)
        {
            current = nd3.left;
        }

        return current;
    }

    /// <summary>
    ///     Returns the current Not-Yet-Transfered node
    /// </summary>
    /// <returns>
    ///     A <see cref="Node" />
    /// </returns>
    private Node GetNytNode()
    {
        return nodes[257];
    }

    /// <summary>
    ///     Adds a node to the tree and returns the reference to it
    /// </summary>
    /// <param name="sym">
    ///     A <see cref="System.Char" />
    ///     The code of added symbol
    /// </param>
    /// <param name="count">
    ///     A <see cref="System.Int32" />
    ///     The initial frequency (typically is 1)
    /// </param>
    /// <returns>
    ///     A <see cref="Node" />
    /// </returns>
    private Node AddToTree(int sym, int count)
    {
        var nyt = GetNytNode();
        nyt.nt = NodeType.INT;

        nyt.right = new Node(NodeType.NYT, 257, 0, nyt.order - 1);
        nyt.left = new Node(NodeType.SYM, sym, count, nyt.order - 2)
        {
            parent = nyt.right.parent = nyt
        };
        nyt.sym = 259;
        nodes[257] = nyt.right;
        nodes[sym] = nyt.left;
        return nyt.right;
    }

    /// <summary>
    ///     Updates the tree with the provided symbol and performs the rebuild
    ///     when necessary
    /// </summary>
    /// <param name="sym">
    ///     A <see cref="System.Char" />
    /// </param>
    public void UpdateTree(int sym)
    {
        if (sym > nodeCount)
        {
            return;
        }

        var temp = nodes[sym];
        if (temp == null)
        {
            temp = AddToTree(sym, 0);
        }

        do
        {
            var same = FindHighestWithSameFreq(temp);
            if (same != temp && temp.parent != same)
            {
                SwapNodes(temp, same);
            }

            temp.freq++;
            temp = temp.parent;
        } while (temp != null);
    }

    public bool contains(int sym)
    {
        return sym <= nodeCount && nodes[sym] != null;
    }

    //FIXME : DIRTY HACK
    public Node GetRootNode()
    {
        return root;
    }

    public int DecodeBinary(int bit)
    {
        try
        {
            ptr ??= root;

            if (InNyt)
            {
                _tempcode <<= 1;
                _tempcode |= bit;
                _count++;
                if (_count != 8)
                {
                    return CharIsEof;
                }

                UpdateTree(_tempcode);
                var sym = _tempcode;
                _tempcode = _count = 0;
                InNyt = false;
                return sym;

            }

            ptr = bit == 1 ? ptr.right : ptr.left;

            if (ptr is { nt: NodeType.NYT, sym: 257 })
            {
                ptr = root;
                InNyt = true;
                return CharIsEof;
            }

            if (ptr.nt != NodeType.SYM)
            {
                return CharIsEof;
            }

            {
                var sym = ptr.sym;
                UpdateTree(sym);
                ptr = root;
                return sym;
            }

        }
        catch (NullReferenceException)
        {
            throw new Exception("Corrupted Huffman sequence supplied for decoding");
        }
    }

    public Stack<int> GetCode(int sym)
    {
        var bits = new Stack<int>();
        var pointer = nodes[sym];
        while (pointer is { parent: not null })
        {
            bits.Push(pointer.parent.left == pointer ? 0 : 1);

            pointer = pointer.parent;
        }

        return bits;
    }
}
