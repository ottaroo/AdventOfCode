using System.Text;

namespace PuzzleSolverLib.Puzzles.Y2024;

public class Day23b : PuzzleBaseClass
{
    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        // Read all and make sure connection is sorted alphabetically
        var lines = File.ReadAllLines(inputFile.ToString()).Select(x => string.Join("-", x.Split("-").OrderBy(l => l))).ToList();

        var computers = new HashSet<string>();
        lines.ForEach(l => l.Split("-").ToList().ForEach(i => computers.Add(i)));

        var graph = new Graph(computers);
        foreach (var line in lines)
        {
            var c = line.Split("-").OrderBy(x => x).ToArray();
            graph.AddEdge(c[0], c[1]);
        }

        var biggestLan = graph.FindSCCs();
        var code = string.Join(",", biggestLan.Split(",").OrderBy(x => x));

        return code;
    }


    public class Graph
    {
        private readonly List<int>[] _adjacencyList;
        private readonly Dictionary<int, string> _intToString;
        private readonly List<int>[] _reverseAdjacencyList;
        private readonly Dictionary<string, int> _stringToInt;
        private readonly int _vertices;

        public Graph(IEnumerable<string> nodes)
        {
            _stringToInt = new Dictionary<string, int>();
            _intToString = new Dictionary<int, string>();

            var index = 0;
            foreach (var node in nodes)
            {
                _stringToInt[node] = index;
                _intToString[index] = node;
                index++;
            }

            _vertices = _stringToInt.Count;
            _adjacencyList = new List<int>[_vertices];
            _reverseAdjacencyList = new List<int>[_vertices];
            for (var i = 0; i < _vertices; i++)
            {
                _adjacencyList[i] = [];
                _reverseAdjacencyList[i] = [];
            }
        }

        public void AddEdge(string v, string w)
        {
            var vIndex = _stringToInt[v];
            var wIndex = _stringToInt[w];
            _adjacencyList[vIndex].Add(wIndex);
            _reverseAdjacencyList[wIndex].Add(vIndex); // Reverse graph for Kosaraju's algorithm
        }

        private string DFSUtil(int v, bool[] visited)
        {
            visited[v] = true;
            var sb = new StringBuilder();
            sb.Append(_intToString[v]);
            sb.Append(",");

            foreach (var adjacent in _reverseAdjacencyList[v])
                if (!visited[adjacent])
                    sb.Append(DFSUtil(adjacent, visited));

            return sb.ToString();
        }

        private void FillOrder(int v, bool[] visited, Stack<int> stack)
        {
            visited[v] = true;

            foreach (var adjacent in _adjacencyList[v])
                if (!visited[adjacent])
                    FillOrder(adjacent, visited, stack);

            stack.Push(v);
        }

        public string FindSCCs()
        {
            var stack = new Stack<int>();
            var visited = new bool[_vertices];

            // Step 1: Fill vertices in stack according to their finishing times
            for (var i = 0; i < _vertices; i++)
                if (!visited[i])
                    FillOrder(i, visited, stack);

            // Step 2: Reverse the graph (already done during edge addition)

            // Step 3: Process all vertices in order defined by the stack
            visited = new bool[_vertices];
            var sb = new StringBuilder();
            while (stack.Count > 0)
            {
                var v = stack.Pop();

                if (!visited[v]) sb.Append(DFSUtil(v, visited));
            }

            return sb.ToString().TrimEnd(',');
        }
    }
}