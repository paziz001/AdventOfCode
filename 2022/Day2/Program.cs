using var streamReader = new StreamReader(new FileStream("input", FileMode.Open));
var line = await streamReader.ReadLineAsync();
var playerScoreWithRandomStrategy = 0;
var playerScoreWithAnotherStategy = 0;
while(line is not null)
{
    var splitLine = line.Split(" ");
    var opponentResponse = splitLine[0];

    var playerResponse = splitLine[1];
    playerScoreWithRandomStrategy += GetPlayerOverallScore(opponentResponse, playerResponse);

    var expectedEndResult = splitLine[1];
    playerScoreWithAnotherStategy += GetPlayerOverallScore(
        opponentResponse,
        GetExpectedPlayerResponse(opponentResponse, expectedEndResult));
    line = await streamReader.ReadLineAsync();
}

Console.WriteLine($"The total score with random strategy is {playerScoreWithRandomStrategy}");
Console.WriteLine($"The total score with another strategy is {playerScoreWithAnotherStategy}");

int GetPlayerOverallScore(string opponentResponse, string playerResponse)
{
    return PlayerSymbol.GetScoreBasedOnSymbol(playerResponse) +
        GetPlayerRoundScore(opponentResponse, playerResponse);

}

int GetPlayerRoundScore(string opponentChoice, string playerChoice)
{
    var roundScore = (opponentChoice, playerChoice);
    var lost = roundScore is   
        (OpponentSymbol.Rock, PlayerSymbol.Scissors) or
        (OpponentSymbol.Paper, PlayerSymbol.Rock) or
        (OpponentSymbol.Scissors, PlayerSymbol.Paper);

    var draw = roundScore is   
        (OpponentSymbol.Rock, PlayerSymbol.Rock) or
        (OpponentSymbol.Paper, PlayerSymbol.Paper) or
        (OpponentSymbol.Scissors, PlayerSymbol.Scissors); 
     
    var won = roundScore is   
        (OpponentSymbol.Rock, PlayerSymbol.Paper) or
        (OpponentSymbol.Paper, PlayerSymbol.Scissors) or
        (OpponentSymbol.Scissors, PlayerSymbol.Rock);

    if (lost) return 0;
    if (draw) return 3;
    if (won) return 6;

    return 0;
}

string GetExpectedPlayerResponse(string opponentChoice, string expectedResultSymbol) => expectedResultSymbol switch
{
    ExpextedRoundResult.Lost => FindLosingResponse(opponentChoice),
    ExpextedRoundResult.Draw => FindDrawResponse(opponentChoice),
    ExpextedRoundResult.Win => FindWinningResponse(opponentChoice),
    _ => throw new ArgumentOutOfRangeException()
};

string FindWinningResponse(string opponentResponse) => opponentResponse switch
{
    OpponentSymbol.Paper => PlayerSymbol.Scissors,
    OpponentSymbol.Rock => PlayerSymbol.Paper,
    OpponentSymbol.Scissors => PlayerSymbol.Rock
};

string FindLosingResponse(string opponentResponse) => opponentResponse switch
{
    OpponentSymbol.Paper => PlayerSymbol.Rock,
    OpponentSymbol.Rock => PlayerSymbol.Scissors,
    OpponentSymbol.Scissors => PlayerSymbol.Paper
};

string FindDrawResponse(string opponentResponse) => opponentResponse switch
{
    OpponentSymbol.Paper => PlayerSymbol.Paper,
    OpponentSymbol.Rock => PlayerSymbol.Rock,
    OpponentSymbol.Scissors => PlayerSymbol.Scissors
};

file static class OpponentSymbol
{
    public const string Rock = "A";
    public const string Paper = "B";
    public const string Scissors = "C";
}

file static class PlayerSymbol
{
    public const string Rock = "X";
    public const string Paper = "Y";
    public const string Scissors = "Z";

    public static int GetScoreBasedOnSymbol(string choice) => choice switch
    {
        Rock => 1,
        Paper => 2,
        Scissors => 3,
        _ => throw new ArgumentOutOfRangeException()
    };
}

file static class ExpextedRoundResult
{
    public const string Lost = "X";
    public const string Draw = "Y";
    public const string Win = "Z";
}