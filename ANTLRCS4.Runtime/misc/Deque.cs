using org.antlr.v4.runtime.atn;

namespace org.antlr.v4.runtime.tree;
public interface Deque<T>
{
    void AddFirst(T t);

    void AddLast(T t);

    bool OfferFirst(T t);

    bool OfferLast(T t);

    T RemoveFirst();

    T RemoveLast();

    T PollFirst();

    T PollLast();

    T GetFirst();

    T GetLast();

    T PeekFirst();

    T PeekLast();

    bool RemoveFirstOccurrence(object o);

    bool RemoveLastOccurrence(object o);

    bool Add(T t);

    bool Offer(T t);

    T Remove();

    T Poll();

    T Element();

    T Peek();

    bool AddAll(ICollection<T> c);

    void Push(T t);

    T Pop();


    bool Remove(Object o);

    bool Contains(Object o);

    int Count { get; }

    IEnumerator<T> Iterator();

    IEnumerator<T> DescendingIterator();

}

