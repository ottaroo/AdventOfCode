namespace PuzzleSolverLib.Puzzles.Y2024
{

    public class Day04b : PuzzleBaseClass
    {

        public struct Map
        {
            public Position Position { get; set; }
            public char Character { get; set; }
            public int MasCount { get; set; }
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
            // Skip 1st and last line since A need rows above and below
            for (var y = 1; y < text.Length - 1; y++)
            {
                // Skip first and last column since A need columns to the left and right
                for (var x = 1; x < text[y].Length -1; x++)
                {
                    if (text[y][x] == 'A')
                    {
                        var mapPoint = new Map
                        {
                            Position = new Position { X = x, Y = y },
                            Character = text[y][x]
                        };

                        // Look for MAS & SAM \
                        if ((text[y - 1][x - 1] == 'M' && text[y + 1][x + 1] == 'S') || (text[y - 1][x - 1] == 'S' && text[y + 1][x + 1] == 'M'))
                            mapPoint.MasCount++;

                        // Look for MAS & SAM /
                        if ((text[y + 1][x - 1] == 'M' && text[y - 1][x + 1] == 'S') || (text[y + 1][x - 1] == 'S' && text[y - 1][x + 1] == 'M'))
                            mapPoint.MasCount++;



                        // keep map point if it's connected
                        if (mapPoint.MasCount == 2)
                            map.Add(mapPoint);

                    }

                }
            }



            return map.Count.ToString();
        }
    }
}
