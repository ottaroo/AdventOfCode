﻿using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using PuzzleSolverLib.Services;

namespace PuzzleSolverLib.Puzzles.Y2023;

public enum CardCombination
{
    HighCard = 1,
    OnePair = 2,
    TwoPairs = 3,
    ThreeOfAKind = 4,
    FullHouse = 5,
    FourOfAKind = 6,
    FiveOfAKind = 7
}



public class Day07a : PuzzleBaseClass
{
    public class Player : IComparable<Player>
{
    protected readonly Func<ReadOnlySpan<int>, CardCombination> EvalCards;

    public Player(ReadOnlySpan<char> cards, int bid, int playerNumber, Func<ReadOnlySpan<int>, CardCombination> evalCards)
    {
        EvalCards = evalCards;
        for (var n = 0; n < cards.Length; n++)
        {
            if (char.IsDigit(cards[n]))
                Cards[n] = cards[n] - '0';
            else
                switch (cards[n] &~ 0x20)
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
                    default:
                        continue;
                }
            Score |= Cards[n] << (16 - (n*4));
        }

        Score |=  ((int)evalCards(Cards) << 20);

        Bid = bid;
        PlayerNumber = playerNumber;

    }

    public int PlayerNumber { get; protected set; }
    public int[] Cards { get; } = new int[5];
    public int Bid { get; protected set; }

    public int Score { get;protected set; }

    public int AssignedRank { get; set; }
    

    public virtual int CompareTo(Player? other)
    {
        if (other == null)
            return 1;

        var playersHand = EvalCards(Cards);
        var otherPlayersHand = EvalCards(other.Cards);

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

        return 0;
    }

    public virtual string CardsToString()
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



    public virtual CardCombination GetTypeOfHand(ReadOnlySpan<int> cards)
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


    public virtual Player CreatePlayer(ReadOnlySpan<char> cards, int bid, int playerNumber)
        => new Player(cards, bid, playerNumber, GetTypeOfHand);

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
                players.Add(CreatePlayer(line.AsSpan()[..5], int.Parse(line.AsSpan()[6..]), players.Count + 1));
            }

            players.Sort();
            var maxRank = players.Select(x => x.Score).Distinct().Count();
            var sumOfAll = 0L;


            // assign rank
            var lastScore = 0;
            foreach (var player in players.OrderByDescending(x=>x.Score))
            {
                if (player.Score != lastScore && lastScore != 0)
                    maxRank--;
                player.AssignedRank = maxRank;
                lastScore = player.Score;

                Log.WriteDebug($"Player: {player.PlayerNumber} Rank: {player.AssignedRank} [{player.CardsToString()} = {GetTypeOfHand(player.Cards)}]");

            }

            sumOfAll = players.Sum(x => x.AssignedRank * x.Bid);

            return sumOfAll.ToString();
        }
        catch (Exception ex)
        {
            LastError = ex;
            return null;
        }
    }

    public override string Description => @"Too long, didn't read... see http://adventToCode.com/2023/day/7";
}