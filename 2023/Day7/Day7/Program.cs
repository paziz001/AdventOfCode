// Go through the file and find all the types of cards and add
// them in a list. Along with their bid. a list of tuples
// Once you are done sort them by either type or if the type is the same
// by each of their starting cards
// have a case where you get the amount of points for a specific card
// have a case where you get the strength of a card type
// have methods to find each card type
// Joker will get also the same number as the one it presents when comparing 
using System.Data;
using System.Text.RegularExpressions;

using var fileReader = new StreamReader(new FileStream("input", FileMode.Open));

var handRegex = new Regex(@"(?<hand>\w+) (?<bid>\d+)", RegexOptions.Compiled);
var sortedList = new SortedList<string, int>(new HandComparer());
var sortedListWithJokers = new SortedList<string, int>(new HandComparer(playWithJokers: true));
while(!fileReader.EndOfStream)
{
    var line = await fileReader.ReadLineAsync();
    var capturedGroups = handRegex.Matches(line!)[0].Groups;
    var hand = capturedGroups["hand"].Value;
    var bid = int.Parse(capturedGroups["bid"].Value);
    sortedList.Add(hand, bid);
    sortedListWithJokers.Add(hand, bid);
}

var sum1 = 0;
var sum2 = 0;
for (var i = sortedList.Count; i > 0; i--)
{
    sum1 += i * sortedList.GetValueAtIndex(i - 1);
    sum2 += i * sortedListWithJokers.GetValueAtIndex(i - 1);
}
Console.WriteLine(sum1);
Console.WriteLine(sum2);

public class HandComparer(bool playWithJokers) : IComparer<string>
{
    private readonly bool _playWithJokers = playWithJokers;

    public HandComparer() : this(false)
    {
    }

    public int Compare(string? handA, string? handB)
    {
        var handAStrength = _playWithJokers ? GetHandStrengthWhenUsingJokers(handA) : GetHandStrength(handA!);
        var handBStrength = _playWithJokers ? GetHandStrengthWhenUsingJokers(handB) : GetHandStrength(handB!);
        if(handAStrength > handBStrength)
        {
            return 1;
        }
        
        if(handAStrength < handBStrength)
        {
            return -1;
        }

        for (var i = 0; i < handA!.Length; i++)
        {
            var cardAStrength = GetCardStrength(handA[i]);
            var cardBStrength = GetCardStrength(handB![i]);
            if(cardAStrength > cardBStrength)
            {
                return 1;
            }
            if(cardAStrength < cardBStrength)
            {
                return -1;
            }
        } 

        return 0;
    }

    int GetCardStrength(char card)
    {
        if(int.TryParse($"{card}", out var value))
        {
            return value;
        }

        return card switch
        {
            'T' => 10,
            'J' => _playWithJokers ? 1 : 11,
            'Q' => 12,
            'K' => 13,
            'A' => 14,
            _ => 1
        };
    }

    int GetHandStrengthWhenUsingJokers(string hand)
    {
        return GetHandStrength(hand, true);
    }

    int GetHandStrength(string hand, bool playWithJokers = false)
    {
        var cardTotals = new Dictionary<char, int>();
        foreach (var card in hand)
        {
            if(!cardTotals.TryGetValue(card, out int value))
            {
                value = 1;
                cardTotals.Add(card, value); 
                continue;
            }
            cardTotals[card]++;
        }
        var orderedDict = cardTotals.OrderByDescending((pair) => pair.Value).ToList();

        var fiveOfAKindWithJoker = playWithJokers && orderedDict.Count() == 2 && orderedDict.Any(pair => pair.Key == 'J');
        var fiveOfAKind = orderedDict[0].Value == 5 || fiveOfAKindWithJoker;
        if(fiveOfAKind)
        {
            return 7;
        }
        var fourOfAKindWithJoker = playWithJokers &&
            ((orderedDict.Any(p => p.Key == 'J' && (p.Value == 1 || p.Value == 3)) && orderedDict[0].Value == 3 && orderedDict[1].Value == 1) ||
            (orderedDict.Any(p => p.Key == 'J' && p.Value == 2) && orderedDict[0].Value == 2 && orderedDict[1].Value == 2));
            
        var fourOfAKind = orderedDict[0].Value == 4 || fourOfAKindWithJoker; 
        if(fourOfAKind)
        {
            return 6;
        }
        var fullHouseWithJoker = playWithJokers && 
            orderedDict[0].Value == 2 && orderedDict[1].Value == 2 &&
            orderedDict.Any(p => p.Key == 'J');
        var fullHouse = (orderedDict[0].Value == 3 && orderedDict[1].Value == 2) || fullHouseWithJoker;
        if(fullHouse)
        {
            return 5;
        }

        var threeOfAKindWithJoker = playWithJokers && 
            orderedDict[0].Value == 2 && orderedDict[1].Value == 1 &&
            orderedDict.Any(p => p.Key == 'J');
        var threeOfAKind = orderedDict[0].Value == 3 && orderedDict[1].Value == 1 || threeOfAKindWithJoker;
        if(threeOfAKind)
        {
            return 4;
        }
        var twoPair = orderedDict[0].Value == 2 && orderedDict[1].Value == 2;
        if(twoPair)
        {
            return 3;
        }
        var onePairWithJoker = playWithJokers && 
            orderedDict.Any(p => p.Key == 'J');
        var onePair = orderedDict[0].Value == 2 && orderedDict[1].Value == 1 || onePairWithJoker;
        if(onePair)
        {
            return 2;
        }

        return 1;
    }
}