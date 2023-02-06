namespace org.antlr.v4.runtime.tree;

public class ArrayDeque<T> : List<T>, Deque<T>
{
    public ArrayDeque() : base() { }
    public ArrayDeque(int capacity) : base(capacity) { }
    public ArrayDeque(IEnumerable<T> collection) : base(collection) { }

    public new bool Add(T t)
    {
        this.AddLast(t);
        return true;
    }

    public bool AddAll(ICollection<T> c)
    {
        int s = this.Count;
        this.AddRange(c);
        return this.Count > s;
    }

    public void AddFirst(T t) => this.Insert(0, t);

    public void AddLast(T t) => this.Add(t);

    public bool Contains(object o) => this.Contains(o);

    public IEnumerator<T> DescendingIterator() => this.Reverse<T>().GetEnumerator();
    public IEnumerator<T> Iterator() => this.GetEnumerator();
    public T Element() => this.GetFirst();

    public T GetFirst() => this[0];

    public T GetLast() => this[^1];

    public bool Offer(T t) => this.OfferLast(t);

    public bool OfferFirst(T t)
    {
        this.AddFirst(t);
        return true;
    }

    public bool OfferLast(T t)
    {
        this.AddLast(t);
        return true;
    }

    public T Peek() => this.PeekFirst();

    public T PeekFirst() => this[0];

    public T PeekLast() => this[^1];

    public T Poll() => this.PollFirst();

    public T PollFirst() => this.RemoveFirst();

    public T PollLast() => this.RemoveLast();

    public T Pop() => this.RemoveFirst();

    public void Push(T t) => this.AddFirst(t);

    public T Remove() => throw new NotImplementedException();

    public bool Remove(object o) => this.RemoveFirstOccurrence(o);

    public T RemoveFirst()
    {
        T t = this[0];
        this.RemoveAt(0);
        return t;
    }

    public bool RemoveFirstOccurrence(object o)
    {
        if(o is T t)
        {
            var i = this.IndexOf(t);
            if (i >= 0)
            {
                this.Remove(i);
                return true;
            }
        }
        return false;

    }

    public T RemoveLast()
    {
        T last = this[^1];
        this.RemoveAt(this.Count - 1);
        return last;
    }

    public bool RemoveLastOccurrence(object o)
    {
        if(o is T t)
        {
            int i = this.LastIndexOf(t);
            if (i >= 0)
            {
                this.Remove(i);
                return true;
            }
        }
        return false;
    }

    public int Size() => this.Count;
}
