using System.Text.RegularExpressions;

var regex = new Regex(@"((?:\d+ *)+)");
var numberMatch = new Regex(@"(\d+)");
var fileReader = new StreamReader(new FileStream("input", FileMode.Open));
var sum1 = 0;
var cardsWithMatches = new Dictionary<int, int>();
var cardsToProcess = new Stack<int>();
while(!fileReader.EndOfStream)
{
    var line = await fileReader.ReadLineAsync();
    var matches = regex.Matches(line);
    var cardNumbers = numberMatch.Matches(matches[1].Value!).Select(n => n.Value);
    var winningNumbersMatch = numberMatch.Matches(matches[2].Value!).Select(n => n.Value);
    var wonNumbers = cardNumbers.Intersect(winningNumbersMatch).ToList();
    sum1 += wonNumbers 
        .Aggregate(0, (acc, x) => acc == 0 ? 1 : acc * 2 );
    var cardNumber = int.Parse(matches[0].Value);
    cardsWithMatches.Add(cardNumber, wonNumbers.Count);
    cardsToProcess.Push(cardNumber);
}

Console.WriteLine(sum1);
var sum2 = 0;
while(cardsToProcess.Count != 0)
{
    var cardToProcess = cardsToProcess.Pop();

    var lastWinningCard = cardToProcess + cardsWithMatches[cardToProcess];
    var lastScratchCard = cardsWithMatches.Last().Key;
    var numberOfWinningCards = lastWinningCard <= lastScratchCard
        ? cardsWithMatches[cardToProcess]
        : cardsWithMatches[cardToProcess] - (lastWinningCard - lastScratchCard);  
    foreach(var match in Enumerable.Range(cardToProcess + 1, numberOfWinningCards))
    {
        if(match == cardToProcess) continue;
        cardsToProcess.Push(match);
    }
    sum2++;
}

Console.WriteLine(sum2);