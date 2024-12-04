namespace PuzzleSolverLib.Puzzles.Y2024
{

    public class Day04a : PuzzleBaseClass
    {

        [Flags]
        public enum Connected
        {
            None = 0,
            North = 1 << 0,
            NorthEast = 1 << 1,
            East = 1 << 2,
            SouthEast = 1 << 3,
            South = 1 << 4,
            SouthWest = 1 << 5,
            West = 1 << 6,
            NorthWest = 1 << 7
        }


        public struct Map
        {
            public Position Position { get; set; }
            public char Character { get; set; }

            public Connected Connected { get; set; }
        }


        public struct Position
        {
            public int X { get; set; }  
            public int Y { get; set; }  
        }

        public override string? OnSolve(ReadOnlySpan<char> inputFile)
        {
            var map = new List<Map>();
            var text = File.ReadAllLines(inputFile.ToString());
            for (var y = 0; y < text.Length; y++)
            {
                for (var x = 0; x < text[y].Length; x++)
                {
                    if (text[y][x] == 'X')
                    {
                        var mapPoint = new Map
                        {
                            Position = new Position { X = x, Y = y },
                            Character = text[y][x]
                        };

                        // Look east
                        if (x + 3 < text[y].Length)
                        {
                            if (text[y][x + 1] == 'M' && text[y][x + 2] == 'A' && text[y][x + 3] == 'S')
                                mapPoint.Connected |= Connected.East;
                        }
                        // look south-east
                        if (x + 3 < text[y].Length && y + 3 < text.Length)
                        {
                            if (text[y + 1][x + 1] == 'M' && text[y + 2][x + 2] == 'A' && text[y + 3][x + 3] == 'S')
                                mapPoint.Connected |= Connected.SouthEast;
                        }
                        // look south
                        if (y + 3 < text.Length)
                        {
                            if (text[y + 1][x] == 'M' && text[y + 2][x] == 'A' && text[y + 3][x] == 'S')
                                mapPoint.Connected |= Connected.South;
                        }
                        // look south-west
                        if (x - 3 >= 0 && y + 3 < text.Length)
                        {
                            if (text[y + 1][x - 1] == 'M' && text[y + 2][x - 2] == 'A' && text[y + 3][x - 3] == 'S')
                                mapPoint.Connected |= Connected.SouthWest;
                        }
                        // look west
                        if (x - 3 >= 0)
                        {
                            if (text[y][x - 1] == 'M' && text[y][x - 2] == 'A' && text[y][x - 3] == 'S')
                                mapPoint.Connected |= Connected.West;
                        }
                        // look north-west
                        if (x - 3 >= 0 && y - 3 >= 0)
                        {
                            if (text[y - 1][x - 1] == 'M' && text[y - 2][x - 2] == 'A' && text[y - 3][x - 3] == 'S')
                                mapPoint.Connected |= Connected.NorthWest;
                        }
                        // look north
                        if (y - 3 >= 0)
                        {
                            if (text[y - 1][x] == 'M' && text[y - 2][x] == 'A' && text[y - 3][x] == 'S')
                                mapPoint.Connected |= Connected.North;
                        }
                        // look north-east
                        if (x + 3 < text[y].Length && y - 3 >= 0)
                        {
                            if (text[y - 1][x + 1] == 'M' && text[y - 2][x + 2] == 'A' && text[y - 3][x + 3] == 'S')
                                mapPoint.Connected |= Connected.NorthEast;
                        }

                        // keep map point if it's connected
                        if (mapPoint.Connected != Connected.None)
                            map.Add(mapPoint);

                    }

                }
            }

            var xmas = 0;
            foreach (var mapPoint in map)
            {
                if (mapPoint.Connected.HasFlag(Connected.North))
                    xmas++;
                if (mapPoint.Connected.HasFlag(Connected.NorthEast))
                    xmas++;
                if (mapPoint.Connected.HasFlag(Connected.East))
                    xmas++;
                if (mapPoint.Connected.HasFlag(Connected.SouthEast))
                    xmas++;
                if (mapPoint.Connected.HasFlag(Connected.South))
                    xmas++;
                if (mapPoint.Connected.HasFlag(Connected.SouthWest))
                    xmas++;
                if (mapPoint.Connected.HasFlag(Connected.West))
                    xmas++;
                if (mapPoint.Connected.HasFlag(Connected.NorthWest))
                    xmas++;
            }


            return xmas.ToString();
        }
    }
}
