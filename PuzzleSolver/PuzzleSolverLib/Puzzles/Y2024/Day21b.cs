
using System.Text;
using PuzzleSolverLib.Common;

namespace PuzzleSolverLib.Puzzles.Y2024;

using MapPoint = (int x, int y);

public class Day21b : PuzzleBaseClass
{
    // NOTE: do some caching perhaps and maybe just return lengths instead of entire sequences
    

    private readonly DirectionalKeyPad _directionalKeyPad = new(c => { });

    public string[] GetShortestSequences(string sequence)
    {
        var sequences = _directionalKeyPad.GetMoveSequenceFor(sequence);
        var minLength = sequences.Min(x => x.Length);
        return sequences.Distinct().Where(x => x.Length == minLength).ToArray();
    }


    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {

        var lines = File.ReadAllLines(inputFile.ToString());

        var numPad = new NumericalKeyPad(c => { });
        var dirPad = new DirectionalKeyPad(c => { });

        var puzzleSum = 0;

        // Get all codes
        foreach (var line in lines)
        {

            var moves = numPad.GetMoveSequenceFor(line.Trim()).Distinct();
            var shortestSequence = moves.Min(x => x.Length);
            var shortestSequences = moves.Where(x => x.Length == shortestSequence).ToList();


            for (var n = 0; n < 1; n++)
            {
                var manyShortSequences = new List<string>();
                foreach(var sequence in shortestSequences)
                    manyShortSequences.AddRange( GetShortestSequences(sequence));

                var minLength = manyShortSequences.Min(x => x.Length);
                shortestSequences = manyShortSequences.Where(x=>x.Length == minLength).ToList();
            }


            var sequenceLength = int.MaxValue;
            var shortSequence = string.Empty;
            var prevShortSequence = string.Empty;
            var shortSequenceLock = new Lock();

            Parallel.ForEach(shortestSequences, move =>
            {
                var directionalPad = new DirectionalKeyPad(c => { });
                var sequences = directionalPad.GetMoveSequenceFor(move);

                Parallel.ForEach(sequences,  sequence =>
                {
                    var l = sequence.Length;
                    if (l < sequenceLength)
                    {
                        lock (shortSequenceLock)
                        {
                            if (l < sequenceLength)
                            {
                                sequenceLength = l;
                                shortSequence = sequence;
                                prevShortSequence = move;
                            }
                        }
                    }
                });
            });



            Console.WriteLine($"code: {line}");
            Console.WriteLine($"\tsource: {prevShortSequence}");
            Console.WriteLine($"\tsequence: {shortSequence} length: {sequenceLength}");
            Console.WriteLine($"\tDiff: {shortSequence.Length - prevShortSequence.Length}");
            var num = RegularExpressions.Numbers().Match(line);

            puzzleSum += (sequenceLength * int.Parse(num.Value));

        }

        return puzzleSum.ToString();
    }


    public abstract class KeyPadBaseClass
    {
        private readonly Action<char> _buttonPressedCallback;

        protected KeyPadBaseClass(Action<char> buttonPressedCallback)
        {
            _buttonPressedCallback = buttonPressedCallback;
            StartPosition = GetPosition('A');
            CurrentPosition = StartPosition;
        }

        public MapPoint GetPosition(char c)
        {
            for (var y = 0; y < KeyPad.Length; y++)
                for (var x = 0; x < KeyPad[y].Length; x++)
                    if (KeyPad[y][x] == c)
                        return (x, y);
            return (-1, -1);
        }

        public char GetCharAtPosition(MapPoint point) => KeyPad[point.y][point.x];


        public abstract char[][] KeyPad { get; }
        public MapPoint StartPosition { get; }
        public MapPoint CurrentPosition { get; set; }

        protected void SetPosition(MapPoint change)
        {
            var tmp = new MapPoint(CurrentPosition.x + change.x, CurrentPosition.y + change.y);
            var ch = GetCharAtPosition(tmp);
            if (ch == ' ')
                throw new InvalidOperationException("PANIC!");
            CurrentPosition = tmp;
        }

        public virtual void MoveDown() => SetPosition(MapFunctions.GetDirection(MapFunctions.Direction.Down));
        public virtual void MoveUp() => SetPosition(MapFunctions.GetDirection(MapFunctions.Direction.Up));
        public virtual void MoveLeft() => SetPosition(MapFunctions.GetDirection(MapFunctions.Direction.Left));
        public virtual void MoveRight() => SetPosition(MapFunctions.GetDirection(MapFunctions.Direction.Right));

        public virtual void Push()
        {
            _buttonPressedCallback.Invoke(GetCharAtPosition(CurrentPosition));
        }

        public void PressButton(ConsoleKeyInfo key) => PressButton(key.Key);

        public void PressButton(ConsoleKey key)
        {
            if (ParseButtonPressed(key, out var ch))
                _buttonPressedCallback.Invoke(ch);
        }

        public abstract bool ParseButtonPressed(ConsoleKey key, out char buttonPressed);

        public void ExecuteMoves(string moves)
        {
            Reset();

            foreach (var ch in moves)
            {
                switch (ch)
                {
                    case '^':
                        MoveUp();
                        break;
                    case '<':
                        MoveLeft();
                        break;
                    case '>':
                        MoveRight();
                        break;
                    case 'v':
                        MoveDown();
                        break;
                    case 'A' or 'a':
                        Push();
                        break;
                }
            }
        }

        

        public string[] GetMoveSequenceFor(string input, MapPoint? currentPosition = null)
        {
            if (currentPosition == null)
                Reset();
            else
                CurrentPosition = currentPosition.Value;

            var sequences = new Stack<string>();

            for (var n = 0; n < input.Length; n++)
            {
                var to = GetPosition(input[n]);
                if (to.x == -1)
                    throw new InvalidOperationException($"Invalid character: {input[n]}");

                // find both paths to the target
                var paths = FindAllShortestPaths(CurrentPosition, to);
                if (paths.Count == 0)
                    throw new InvalidOperationException($"No path from {CurrentPosition} to {to}");

                var listOfMoves = new HashSet<string>();

                if (paths is [{Count: 1}])
                {
                    listOfMoves.Add("A");
                }
                else
                {
                    foreach (var path in paths)
                    {
                        var moves = new StringBuilder();
                        for (var i = 1; i < path.Count; i++)
                        {
                            var dir = (path[i].X - path[i - 1].X, path[i].Y - path[i - 1].Y);
                            var direction = MapFunctions.GetDirection(dir);
                            switch (direction)
                            {
                                case MapFunctions.Direction.Up:
                                    moves.Append('^');
                                    break;
                                case MapFunctions.Direction.Down:
                                    moves.Append('v');
                                    break;
                                case MapFunctions.Direction.Left:
                                    moves.Append('<');
                                    break;
                                case MapFunctions.Direction.Right:
                                    moves.Append('>');
                                    break;
                            }
                        }

                        moves.Append('A');
                        listOfMoves.Add(moves.ToString());
                    }
                }


                if (sequences.Count > 0)
                {
                    var newListOfSequences = new List<string>();
                    while(sequences.TryPop(out var sequence))
                        foreach(var moves in listOfMoves)
                            newListOfSequences.Add($"{sequence}{moves}");
                    foreach (var newSequence in newListOfSequences)
                        sequences.Push(newSequence);
                }
                else
                {
                    foreach(var moves in listOfMoves)
                        sequences.Push(moves);
                }


                CurrentPosition = paths.Last().Last();
            }



            return sequences.Distinct().ToArray();
        }


        public void Reset()
        {
            CurrentPosition = StartPosition;
        }

        public List<MapPoints> FindAllShortestPaths(MapPoint start, MapPoint end)
        {
            List<MapPoints> allPaths = new();
            Queue<MapPoints> queue = new();
            queue.Enqueue([start]);
            int pathLength = Math.Abs(start.x - end.x) + Math.Abs(start.y - end.y) + 1;
            while (queue.Count > 0)
            {
                var path = queue.Dequeue();
                var current = path[^1];
                if (current == end)
                {
                    if (path.Count <= pathLength)
                    {
                        allPaths.Add(path);
                    }

                    continue;
                }

                foreach (var direction in GetNeighbors(current))
                {
                    if (!path.Contains(direction))
                    {
                        var mp = new MapPoints();
                        mp.AddRange(path.ToArray());
                        mp.Add(direction);
                        queue.Enqueue(mp);
                    }
                }
            }

            return allPaths;
        }

        private List<MapPoint> GetNeighbors(MapPoint point)
        {
            int[] dx = [-1, 1, 0, 0];
            int[] dy = [0, 0, -1, 1];

            var neighbors = new List<MapPoint>();
            var rows = KeyPad.Length;
            var cols = KeyPad[0].Length;
            for (var i = 0; i < dy.Length; i++)
            {
                var newRow = point.y + dy[i];
                var newCol = point.x + dx[i];
                if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols && KeyPad[newRow][newCol] != ' ')
                {
                    neighbors.Add((newCol, newRow));
                }
            }

            return neighbors;
        }



  
    }

    public class NumericalKeyPad(Action<char> buttonPressedCallback) : KeyPadBaseClass(buttonPressedCallback)
    {
        public override char[][] KeyPad { get; } =
        [
            ['7', '8', '9'],
            ['4', '5', '6'],
            ['1', '2', '3'],
            [' ', '0', 'A']
        ];
        public override bool ParseButtonPressed(ConsoleKey key, out char buttonPressed)
        {
            switch (key)
            {
                case ConsoleKey.NumPad0:
                case ConsoleKey.NumPad1:
                case ConsoleKey.NumPad2:
                case ConsoleKey.NumPad3:
                case ConsoleKey.NumPad4:
                case ConsoleKey.NumPad5:
                case ConsoleKey.NumPad6:
                case ConsoleKey.NumPad7:
                case ConsoleKey.NumPad8:
                case ConsoleKey.NumPad9:
                    buttonPressed = (char)('0' + key - ConsoleKey.NumPad0);
                    return true;
                case ConsoleKey.D0:
                case ConsoleKey.D1:
                case ConsoleKey.D2:
                case ConsoleKey.D3:
                case ConsoleKey.D4:
                case ConsoleKey.D5:
                case ConsoleKey.D6:
                case ConsoleKey.D7:
                case ConsoleKey.D8:
                case ConsoleKey.D9:
                    buttonPressed = (char)('0' + key - ConsoleKey.D0);
                    return true;
                case ConsoleKey.Enter:
                    buttonPressed = 'A';
                    return true;
                default:
                    buttonPressed = ' ';
                    return false;
            }
        }
    }

    public class DirectionalKeyPad(Action<char> buttonPressedCallback) : KeyPadBaseClass(buttonPressedCallback)
    {
        public override char[][] KeyPad { get; } = [[' ', '^', 'A'], ['<', 'v', '>']];
        public override bool ParseButtonPressed(ConsoleKey key, out char buttonPressed)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    buttonPressed = '^';
                    return true;
                case ConsoleKey.LeftArrow:
                    buttonPressed = '<';
                    return true;
                case ConsoleKey.RightArrow:
                    buttonPressed = '>';
                    return true;
                case ConsoleKey.DownArrow:
                    buttonPressed = 'v';
                    return true;
                case ConsoleKey.Enter:
                    buttonPressed = 'A';
                    return true;
                default:
                    buttonPressed = ' ';
                    return false;
            }
        }
    }
}