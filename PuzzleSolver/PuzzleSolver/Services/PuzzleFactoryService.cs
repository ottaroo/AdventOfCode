using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuzzleSolver.Puzzles;
using PuzzleSolver.Puzzles.Y2023;

namespace PuzzleSolver.Services 
{
    public interface IPuzzleFactoryService
    {
        IPuzzle CreatePuzzle(int year, int day, int part);
    }

    //public partial class PuzzleFactoryService 
    //{
        
    //}
}
