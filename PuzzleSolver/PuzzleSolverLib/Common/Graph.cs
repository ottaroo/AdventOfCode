namespace PuzzleSolverLib.Common;

public class Edge<T>
{
    public required Node<T> From { get; set; }
    public required Node<T> To { get; set; }
    public int Weight { get; set; }
}

public class Subset<T>
{
    public required Node<T> Parent { get; set; }
    public int Rank { get; set; }
}

public class Node<T>
{
    public int Index { get; set; }
    public required T Data { get; set; }
    public List<Node<T>> Neighbours { get; set; } = [];
    public List<int> Weights { get; set; } = [];

    public static implicit operator Node<T>(int i) => new Node<T>() {Data = (T)(object)i};
    public static implicit operator Node<T>((int x, int y) mapPoint) => new Node<T>() {Data = (T)(object)(mapPoint.x,mapPoint.y)};

}

public class Graph<T>
{
    public required bool IsDirected { get; init; }
    public required bool IsWeighted { get; init; }
    public List<Node<T>> Nodes { get; } = [];

    public Edge<T>? this[int from, int to]
    {
        get
        {
            var nodeFrom = Nodes[from];
            var nodeTo = Nodes[to];
            int i = nodeFrom.Neighbours.IndexOf(nodeTo);
            if (i < 0) { return null; }
            Edge<T> edge = new()
            {
                From = nodeFrom,
                To = nodeTo,
                Weight = i < nodeFrom.Weights.Count ? nodeFrom.Weights[i] : 0
            };
            return edge;
        }
    }

    public virtual List<Edge<T>> GetEdges()
    {
        List<Edge<T>> edges = [];
        foreach (Node<T> from in Nodes)
        {
            for (int i = 0; i < from.Neighbours.Count; i++)
            {
                int weight = i < from.Weights.Count ? from.Weights[i] : 0;
                Edge<T> edge = new()
                {
                    From = from,
                    To = from.Neighbours[i],
                    Weight = weight
                };
                edges.Add(edge);
            }
        }
        return edges;
    }

    public virtual void AddEdge(Node<T> from, Node<T> to, int w = 0)
    {
        from.Neighbours.Add(to);
        if (IsWeighted) { from.Weights.Add(w); }

        if (!IsDirected)
        {
            to.Neighbours.Add(from);
            if (IsWeighted) { to.Weights.Add(w); }
        }
    }

    public virtual void RemoveEdge(Node<T> from, Node<T> to)
    {
        int index = from.Neighbours.FindIndex(n => n == to);
        if (index < 0) { return; }
        from.Neighbours.RemoveAt(index);
        if (IsWeighted) { from.Weights.RemoveAt(index); }

        if (!IsDirected)
        {
            index = to.Neighbours.FindIndex(n => n == from);
            if (index < 0) { return; }
            to.Neighbours.RemoveAt(index);
            if (IsWeighted) { to.Weights.RemoveAt(index); }
        }
    }

    public virtual Node<T> AddNode(T value)
    {
        Node<T> node = new() { Data = value };
        Nodes.Add(node);
        UpdateIndices();
        return node;
    }

    public virtual void RemoveNode(Node<T> nodeToRemove)
    {
        Nodes.Remove(nodeToRemove);
        UpdateIndices();
        Nodes.ForEach(n => RemoveEdge(n, nodeToRemove));
    }

    private void UpdateIndices()
    {
        int i = 0;
        Nodes.ForEach(n => n.Index = i++);
    }

    public List<Edge<T>> GetShortestPath(Node<T> source, Node<T> target)
    {
        int[] previous = new int[Nodes.Count];
        Array.Fill(previous, -1);

        int[] distances = new int[Nodes.Count];
        Array.Fill(distances, int.MaxValue);
        distances[source.Index] = 0;

        PriorityQueue<Node<T>, int> nodes = new();
        for (int i = 0; i < Nodes.Count; i++)
        {
            nodes.Enqueue(Nodes[i], distances[i]);
        }

        while (nodes.Count != 0)
        {
            Node<T> node = nodes.Dequeue();
            for (int i = 0; i < node.Neighbours.Count; i++)
            {
                Node<T> neighbor = node.Neighbours[i];
                int weight = i < node.Weights.Count ? node.Weights[i] : 0;

                int wTotal = distances[node.Index] + weight;

                if (distances[neighbor.Index] > wTotal)
                {
                    distances[neighbor.Index] = wTotal;
                    previous[neighbor.Index] = node.Index;
                    nodes.Enqueue(neighbor, distances[neighbor.Index]);
                }
            }
        }

        List<int> indices = [];
        int index = target.Index;
        while (index >= 0)
        {
            indices.Add(index);
            index = previous[index];
        }

        indices.Reverse();
        List<Edge<T>> result = [];
        for (int i = 0; i < indices.Count - 1; i++)
        {
            Edge<T> edge = this[indices[i], indices[i + 1]]!;
            result.Add(edge);
        }

        return result;
    }

    public List<Node<T>> DepthFirstSearch()
    {
        bool[] isVisited = new bool[Nodes.Count];
        List<Node<T>> result = [];
        DepthFirstSearch(isVisited, Nodes[0], result);
        return result;
    }

    private void DepthFirstSearch(bool[] isVisited, Node<T> node, List<Node<T>> result)
    {
        result.Add(node);
        isVisited[node.Index] = true;

        foreach (Node<T> neighbor in node.Neighbours)
        {
            if (!isVisited[neighbor.Index])
            {
                DepthFirstSearch(isVisited, neighbor, result);
            }
        }
    }

    public List<Node<T>> BreadthFirstSearch() => BreadthFirstSearch(Nodes[0]);

    private List<Node<T>> BreadthFirstSearch(Node<T> node)
    {
        bool[] isVisited = new bool[Nodes.Count];
        isVisited[node.Index] = true;

        List<Node<T>> result = [];
        Queue<Node<T>> queue = [];
        queue.Enqueue(node);
        while (queue.Count > 0)
        {
            Node<T> next = queue.Dequeue();
            result.Add(next);

            foreach (Node<T> neighbor in next.Neighbours)
            {
                if (!isVisited[neighbor.Index])
                {
                    isVisited[neighbor.Index] = true;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return result;
    }

    #region Minimum Spanning Tree - Kruskal's algorithm
    public List<Edge<T>> MSTKruskal()
    {
        List<Edge<T>> edges = GetEdges();
        edges.Sort((a, b) => a.Weight.CompareTo(b.Weight));
        Queue<Edge<T>> queue = new(edges);

        Subset<T>[] subsets = new Subset<T>[Nodes.Count];
        for (int i = 0; i < Nodes.Count; i++)
        {
            subsets[i] = new() { Parent = Nodes[i] };
        }

        List<Edge<T>> result = [];
        while (result.Count < Nodes.Count - 1)
        {
            Edge<T> edge = queue.Dequeue();
            Node<T> from = GetRoot(subsets, edge.From);
            Node<T> to = GetRoot(subsets, edge.To);
            if (from == to) { continue; }
            result.Add(edge);
            Union(subsets, from, to);
        }

        return result;
    }

    private Node<T> GetRoot(Subset<T>[] ss, Node<T> node)
    {
        int i = node.Index;
        ss[i].Parent = ss[i].Parent != node
            ? GetRoot(ss, ss[i].Parent)
            : ss[i].Parent;
        return ss[i].Parent;
    }

    private void Union(Subset<T>[] ss, Node<T> a, Node<T> b)
    {
        ss[b.Index].Parent = ss[a.Index].Rank >= ss[b.Index].Rank
            ? a : ss[b.Index].Parent;
        ss[a.Index].Parent = ss[a.Index].Rank < ss[b.Index].Rank
            ? b : ss[a.Index].Parent;
        if (ss[a.Index].Rank == ss[b.Index].Rank) { ss[a.Index].Rank++; }
    }
    #endregion


}