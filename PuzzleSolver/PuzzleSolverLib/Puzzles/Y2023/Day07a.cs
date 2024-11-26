using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using PuzzleSolverLib.Services;

namespace PuzzleSolverLib.Puzzles.Y2023;

public enum CardCombination
{
    HighCard,
    OnePair,
    TwoPairs,
    ThreeOfAKind,
    FullHouse,
    FourOfAKind,
    FiveOfAKind
}

public class Player : IComparable<Player>
{
    public Player(ReadOnlySpan<char> cards, int bid, int playerNumber)
    {
        for (var n = 0; n < cards.Length; n++)
        {
            if (char.IsDigit(cards[n]))
                Cards[n] =  int.Parse(char.ToString(cards[n]), NumberStyles.Integer);
            else
                switch (cards[n] & ~ 20)
                {
                    case 'A':
                        Cards[n] = 14;
                        break;
                    case 'K':
                        Cards[n] = 13;
                        break;
                    case 'Q':
                        Cards[n] = 12;
                        break;
                    case 'J':
                        Cards[n] = 11;
                        break;
                    case 'T':
                        Cards[n] = 10;
                        break;
                }
        }

        Bid = bid;
        PlayerNumber = playerNumber;

    }

    public int PlayerNumber { get; }
    public int[] Cards { get; } = new int[5];
    public int Bid { get; }

    public int AssignedRank { get; set; }
    

    public static CardCombination GetTypeOfHand(ReadOnlySpan<int> cards)
    {
        Span<int> cardCount =
        [
            cards.Count(cards[0]),
            cards.Count(cards[1]),
            cards.Count(cards[2]),
            cards.Count(cards[3]),
            cards.Count(cards[4]),
        ];
        var threeOfAKind = 0;
        var pair = 0;
        foreach (var cnt in cardCount)
        {
            switch (cnt)
            {
                case 5:
                    return CardCombination.FiveOfAKind;
                case 4:
                    return CardCombination.FourOfAKind;
                case 3:
                    threeOfAKind++;
                    break;
                case 2:
                    pair++;
                    break;
            }
        }

        if (threeOfAKind > 0 && pair > 0)
            return CardCombination.FullHouse;

        if (threeOfAKind > 0)
            return CardCombination.ThreeOfAKind;

        if (pair > 2)
            return CardCombination.TwoPairs;

        if (pair > 0)
            return CardCombination.OnePair;

        return CardCombination.HighCard;
    }


    public int CompareTo(Player? other)
    {
        if (other == null)
            return 1;

        var playersHand = GetTypeOfHand(Cards);
        var otherPlayersHand = GetTypeOfHand(other.Cards);

        if (playersHand < otherPlayersHand)
            return -1;
        if (playersHand > otherPlayersHand)
            return 1;

        for (var n = 0; n < 5; n++)
        {
            if (Cards[n] < other.Cards[n])
                return -1;
            if (Cards[n] > other.Cards[n])
                return 1;
        }

        if (PlayerNumber < other.PlayerNumber)
            return -1;
        if (PlayerNumber > other.PlayerNumber)
            return 1;

        return 0;
    }

    public string CardsToString()
    {
        return string.Create(Cards.Length, Cards, (span, cards) =>
        {
            for (int i = 0; i < cards.Length; i++)
            {
                span[i] = cards[i] switch
                {
                    14 => 'A',
                    13 => 'K',
                    12 => 'Q',
                    11 => 'J',
                    10 => 'T',
                    _ => (char)('0' + cards[i])
                };
            }
        });
    }
}


public class Day07a : PuzzleBaseClass
{
    public override string? OnSolve(ReadOnlySpan<char> inputFile)
    {
        try
        {
            var players = new List<Player>();
            using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            using var sr = new StreamReader(fs, Encoding.UTF8, true, 4096, true);
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (line == null)
                    continue;
                players.Add(new Player(line.AsSpan()[0..5], int.Parse(line.AsSpan()[6..], NumberStyles.Integer), players.Count + 1));
            }

            players.Sort();
            var maxRank = players.Count;
            var sumOfAll = 0L;

            // How to rank those with the same hand? i.e all 5 cards are equal - share rank? or in order of appearance?
            for (var n = maxRank; n > 0; n--)
            {
                Log.WriteDebug($"Player: {players[n-1].PlayerNumber} Cards: {players[n-1].CardsToString()} Bid: {players[n-1].Bid} [{Player.GetTypeOfHand(players[n-1].Cards)}]");

                players[n-1].AssignedRank = n;
                sumOfAll += (players[n - 1].Bid * n);
            }


            return sumOfAll.ToString();
        }
        catch (Exception ex)
        {
            LastError = ex;
            return null;
        }
    }

    public override string Description => @"Too long, didn't read... see http://adventToCode.com/2023/day/6";
}