namespace BurrowsWheelerTransform;

/// <summary>Implements a priority queue of T, where T has an ordering.</summary>
/// Elements may be added to the queue in any order, but when we pull
/// elements out of the queue, they will be returned in 'ascending' order.
/// Adding new elements into the queue may be done at any time, so this is
/// useful to implement a dynamically growing and shrinking queue. Both adding
/// an element and removing the first element are log(N) operations.
///
/// The queue is implemented using a priority-heap data structure. For more
/// details on this elegant and simple data structure see "Programming Pearls"
/// in our library. The tree is implemented atop a list, where 2N and 2N+1 are
/// the child nodes of node N. The tree is balanced and left-aligned so there
/// are no 'holes' in this list.
/// http://stackoverflow.com/a/1937896/1499431
/// <typeparam name="T">Type T, should implement IComparable[T];</typeparam>
public sealed class PriorityQueue<T> where T : IComparable<T>
{
    /// <summary>The List we use for implementation.</summary>
    private readonly List<T> _mA = new();

    /// <summary>Returns the number of elements in the queue.</summary>
    public int Count => _mA.Count;

    /// <summary>Returns true if the queue is empty.</summary>
    /// Trying to call Peek() or Next() on an empty queue will throw an exception.
    /// Check using Empty first before calling these methods.
    public bool Empty => _mA.Count == 0;

    /// <summary>Clear all the elements from the priority queue</summary>
    public void Clear()
    {
        _mA.Clear();
    }

    /// <summary>Add an element to the priority queue - O(log(n)) time operation.</summary>
    /// <param name="item">The item to be added to the queue</param>
    public void Add(T item)
    {
        // We add the item to the end of the list (at the bottom of the
        // tree). Then, the heap-property could be violated between this element
        // and it's parent. If this is the case, we swap this element with the
        // parent (a safe operation to do since the element is known to be less
        // than it's parent). Now the element move one level up the tree. We repeat
        // this test with the element and it's new parent. The element, if lesser
        // than everybody else in the tree will eventually bubble all the way up
        // to the root of the tree (or the head of the list). It is easy to see
        // this will take log(N) time, since we are working with a balanced binary
        // tree.
        var n = _mA.Count;
        _mA.Add(item);
        while (n != 0)
        {
            var p = n / 2; // This is the 'parent' of this item
            if (_mA[n].CompareTo(_mA[p]) >= 0)
            {
                break; // Item >= parent
            }

            var tmp = _mA[n];
            _mA[n] = _mA[p];
            _mA[p] = tmp; // Swap item and parent
            n = p; // And continue
        }
    }

    /// <summary>Allows you to look at the first element waiting in the queue, without removing it.</summary>
    /// This element will be the one that will be returned if you subsequently call Next().
    public T Peek()
    {
        return _mA[0];
    }

    /// <summary>Removes and returns the first element from the queue (least element)</summary>
    /// <returns>The first element in the queue, in ascending order.</returns>
    public T Next()
    {
        // The element to return is of course the first element in the array,
        // or the root of the tree. However, this will leave a 'hole' there. We
        // fill up this hole with the last element from the array. This will
        // break the heap property. So we bubble the element downwards by swapping
        // it with it's lower child until it reaches it's correct level. The lower
        // child (one of the original elements with index 1 or 2) will now be at the
        // head of the queue (root of the tree).
        var val = _mA[0];
        var nMax = _mA.Count - 1;
        _mA[0] = _mA[nMax];
        _mA.RemoveAt(nMax); // Move the last element to the top

        var p = 0;
        while (true)
        {
            // c is the child we want to swap with. If there
            // is no child at all, then the heap is balanced
            var c = p * 2;
            if (c >= nMax)
            {
                break;
            }

            // If the second child is smaller than the first, that's the one
            // we want to swap with this parent.
            if (c + 1 < nMax && _mA[c + 1].CompareTo(_mA[c]) < 0)
            {
                c++;
            }

            // If the parent is already smaller than this smaller child, then
            // we are done
            if (_mA[p].CompareTo(_mA[c]) <= 0)
            {
                break;
            }

            // Otherwise, swap parent and child, and follow down the parent
            var tmp = _mA[p];
            _mA[p] = _mA[c];
            _mA[c] = tmp;
            p = c;
        }

        return val;
    }
}
