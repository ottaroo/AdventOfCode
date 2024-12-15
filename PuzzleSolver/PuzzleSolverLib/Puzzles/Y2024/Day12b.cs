using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using PuzzleSolverLib.Common;
using PuzzleSolverLib.Services;

namespace PuzzleSolverLib.Puzzles.Y2024;


public class Day12b : PuzzleBaseClass
{

    public class Box
    {
        public int Top { get; set; }    
        public int Left { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int NumberOfLeftEdges { get; set; } = 1;
        public int NumberOfRightEdges { get; set; } = 1;
        public int NumberOfTopEdges { get; set; } = 1;
        public int NumberOfBottomEdges { get; set; } = 1;

        public int EdgesTotal => NumberOfBottomEdges+ NumberOfLeftEdges + NumberOfRightEdges + NumberOfTopEdges;

        public void Merge(Box boxFromBelow)
        {
            //  ####
            //  ####
            if (Left == boxFromBelow.Left && Width == boxFromBelow.Width)
            {
                NumberOfLeftEdges = 0;
                NumberOfRightEdges = 0;
                NumberOfBottomEdges = 0;
                boxFromBelow.NumberOfTopEdges = 0;
                return;
            }


            //  ####
            //  #
            if (Left == boxFromBelow.Left && boxFromBelow.Width < Width)
            {
                NumberOfLeftEdges = 0;
                boxFromBelow.NumberOfTopEdges = 0;
                return;
            }

            //  # 
            //  ####
            if (Left == boxFromBelow.Left && Width < boxFromBelow.Width)
            {
                NumberOfLeftEdges = 0;
                NumberOfBottomEdges = 0;
                return;
            }

            //    #  
            //  ###
            if (boxFromBelow.Left < Left && boxFromBelow.Left + boxFromBelow.Width == Left+Width)
            {
                NumberOfRightEdges = 0;
                NumberOfBottomEdges = 0;
                return;
            }

            //  ###
            //    #
            if (boxFromBelow.Left + boxFromBelow.Width == Left + Width && boxFromBelow.Left > Left)
            {
                NumberOfRightEdges = 0;
                boxFromBelow.NumberOfTopEdges = 0;
                return;
            }

            // #####
            //   #
            if (Left < boxFromBelow.Left && Left+Width > boxFromBelow.Left +  boxFromBelow.Width)
            {
                NumberOfBottomEdges += 1;
                boxFromBelow.NumberOfTopEdges = 0;
                return;
            }

            //   #
            // #####
            if (Left > boxFromBelow.Left && Left + Width < boxFromBelow.Left + boxFromBelow.Width)
            {
                NumberOfBottomEdges = 0;
                boxFromBelow.NumberOfTopEdges += 1;
                return;
            }

        }

    }

    public class Region
    {
        public char SeedType { get; set; }
        public List<(int x, int y)> LandPlots { get; set; } = new();

        public int GetFencePrice(char[][] mapReference)
        {
            return GetRequiredFenceSize(mapReference) * LandPlots.Count;
        }

        public virtual int GetRequiredFenceSize(char[][] mapReference)
        {
            // Split solution into horizontal boxes
            var listOfBoxes = new List<Box>();
            var map = MapFunctions.CreateMap(LandPlots);
            var mapAsStringArray = MapFunctions.MapToStringArray(map);
            for (var y = 0; y < mapAsStringArray.Length; y++)
            {
                var matches = RegularExpressions.Word().Matches(mapAsStringArray[y]);
                foreach (Match match in matches)
                {
                    listOfBoxes.Add(new Box {Height = 1, Left = match.Index, Width = match.Length, Top = y});
                }
            }

            // merge all boxes to find number of edges
            for (var y = 1; y < mapAsStringArray.Length; y++)
            {
                var mergeBoxes = listOfBoxes.Where(m => m.Top == y - 1).OrderBy(m => m.Left);
                var currentBoxes = listOfBoxes.Where(b=>b.Top == y).OrderBy(b=>b.Left);
                foreach (var box in currentBoxes)
                {
                    foreach (var mergeBox in mergeBoxes)
                    {
                        mergeBox.Merge(box);
                    }
                }
            }


            return listOfBoxes.Sum(b => b.EdgesTotal);
        }
    }



    private char[][] _map;

    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var seeds = new HashSet<(char seed, int x, int y)>();
        var txt = File.ReadAllLines(inputFile.ToString()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        _map = MapFunctions.CreateMap(txt, (c, mapPoint) => { seeds.Add((c, mapPoint.X, mapPoint.Y));});


        var listOfRegions = new List<Region>();
        var seedTypes = seeds.Select(x => x.seed).Distinct().ToList();

        foreach (var seedType in seedTypes)
        {
            var seedList = seeds.Where(s => s.seed.Equals(seedType)).Select(s => (s.x, s.y)).ToList();
            var connected = MapFunctions.FindAllConnectedPoints(seedList);
            foreach (var group in connected)
            {
                var region = new Region {LandPlots = group, SeedType = seedType};
                listOfRegions.Add(region);
            }
        }

        return listOfRegions.Sum(x=>x.GetFencePrice(_map)).ToString();
    }
}