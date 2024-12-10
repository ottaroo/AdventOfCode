using System.Text;
using PuzzleSolverLib.Puzzles.Y2023;

namespace PuzzleSolverLib.Puzzles.Y2024;

public class DiskMapV2 
{
    public LinkedList<Block> Blocks { get; set; } = new();

    public virtual void Unzip(string compressed)
    {
        var id = 0;
        var i = 0;
        foreach (var ch in compressed.AsSpan())
        {
            var size = ch - '0';
            if ((i++ & 1) == 0)
            {
                Blocks.AddLast(new FileBlock(id) {Size = size});

                id++;
            }
            else
            {
                Blocks.AddLast(new FreeSpace() {Size = size});
            }
        }
    }

    public string Compress()
    {
        var sb = new StringBuilder();
        foreach (var block in Blocks)
        {
            if (block is FileBlock fb)
                sb.Append("".PadLeft(fb.Size, (char)('0' + fb.Id)));
            else
                sb.Append("".PadLeft(block.Size, '.'));
        }

        return sb.ToString();
    }

    public long Checksum()
    {
        var chk = 0L;

        var pos = 0;
        var node = Blocks.First;
        while (node != null)
        {
            if (node.Value is FileBlock fb)
            {
                for(var i = 0; i < fb.Size; i++)
                    chk += ((pos + i) * fb.Id);
                pos += fb.Size - 1;
            }

            node = node.Next;
            pos++;
        }

        return chk;
    }

    public void DeFragment()
    {
        var first = Blocks.First;
        var node = Blocks.Last;

        while (node != null && node != first)
        {
            if (node.Value is not FileBlock fb)
            {
                node = node.Previous;
                continue;
            }

            var next = node.Previous;

            var freeSpaceNode = Blocks.First;
            while (freeSpaceNode != null && freeSpaceNode != node)
            {
                if (freeSpaceNode.Value is FreeSpace fs && fs.Size >= fb.Size)
                {
                    Blocks.AddBefore(node, new FreeSpace() {Size = fb.Size});
                    Blocks.Remove(node);
                    Blocks.AddBefore(freeSpaceNode, node);
                    freeSpaceNode.Value.Size -= fb.Size;

                    break;
                }

                freeSpaceNode = freeSpaceNode.Next;
            }

            node = next;
        }

    }


}


public class Day09b : PuzzleBaseClass
{
    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var allText = File.ReadAllText(inputFile.ToString());
        var diskMap = new DiskMapV2();
        diskMap.Unzip(allText);
        diskMap.DeFragment();

        diskMap.Compress();


        Log.WriteInfo(diskMap.Compress());

        return diskMap.Checksum().ToString();


    }
}