using System.Text;

namespace PuzzleSolverLib.Puzzles.Y2024;


public class DiskMap()
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
                for (var n = 0; n < size; n++)
                    Blocks.AddLast(new FileBlock(id));

                id++;
            }
            else
            {
                for (var n = 0; n < size; n++)
                    Blocks.AddLast(new FreeSpace());
            }
        }
    }

    public string Compress()
    {
        var sb = new StringBuilder();
        foreach (var block in Blocks)
        {
            if (block is FileBlock fb)
                sb.Append(fb.Id.ToString());
            else
                sb.Append(".");
        }

        return sb.ToString();
    }

    public long Checksum()
    {
        var chk = 0L;

        var pos = 0;
        var node = Blocks.First;
        while (node?.Next != null)
        {
            if (node.Value is FileBlock fb)
                chk += pos * fb.Id;
            node = node.Next;
            pos++;
        }

        return chk;
    }

    public void DeFragment()
    {
        var freeSpace = new Stack<Block>();

        while (true)
        {
            var fileNode = Blocks.Last;
            while (fileNode != null)
            {
                if (fileNode.Value is FileBlock)
                    break;
                fileNode = fileNode.Previous;
            }

            var spaceNode = Blocks.First;
            while (spaceNode != null)
            {
                if (spaceNode.Value is FreeSpace)
                    break;

                spaceNode = spaceNode.Next;
            }

            if (spaceNode == null || fileNode == null)
                break;


            var nextNode = spaceNode.Next;
            if (nextNode == null)
                break;


            Blocks.Remove(spaceNode);
            Blocks.Remove(fileNode);
            Blocks.AddBefore(nextNode, fileNode);
            freeSpace.Push(spaceNode.Value);
        }

        while (freeSpace.TryPop(out var block))
            Blocks.AddLast(block);
    }

}


public abstract class Block
{
    public int Size { get; set; }
}

public class FileBlock(int id) : Block
{
    public int Id { get; } = id;
}

public class FreeSpace : Block
{

}


public class Day09a : PuzzleBaseClass
{
    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        var allText = File.ReadAllText(inputFile.ToString());
        var diskMap = new DiskMap();
        diskMap.Unzip(allText);
        diskMap.DeFragment();

        diskMap.Compress();


        Log.WriteInfo(diskMap.Compress());

        return diskMap.Checksum().ToString();


    }
}