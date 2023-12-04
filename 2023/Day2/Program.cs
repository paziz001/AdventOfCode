using var fileReader = new StreamReader(new FileStream("input", FileMode.Open));
const short redsTotal = 12;
const short greensTotal = 13;
const short bluesTotal = 14; 
int gameSum = 0;
var sumOfPowerSets = 0;
while(!fileReader.EndOfStream)
{
    var gameInfo = await fileReader.ReadLineAsync();
    int gameNumber = int.Parse(gameInfo!.Split(':')[0].Split(' ')[1]);
    var gameSets = gameInfo!.Split(':')[1].Split(';').Select(set => set.Trim());  
    var foundFalseGameSet = false;
    var maxRed = 0; 
    var maxBlue = 0;
    var maxGreen = 0;
    foreach (var set in gameSets)
    {
        var cubeTotals = set.Split(',').Select(total => total.Trim());
        foreach(var cubeTotal in cubeTotals)
        {
            var splitTotal = cubeTotal.Split(' ');
            var total = int.Parse(splitTotal[0]);
            switch(splitTotal[1])
            {
                case "green": 
                    foundFalseGameSet = foundFalseGameSet || total > greensTotal;
                    maxGreen = total > maxGreen ? total : maxGreen;
                    break;
                case "red": 
                    foundFalseGameSet = foundFalseGameSet || total > redsTotal;
                    maxRed = total > maxRed ? total : maxRed;
                    break;
                case "blue": 
                    foundFalseGameSet = foundFalseGameSet || total > bluesTotal;
                    maxBlue = total > maxBlue ? total : maxBlue;
                    break;
            }
        }
    }
    if(!foundFalseGameSet)
    {
        gameSum += gameNumber;
    }
    sumOfPowerSets += maxRed * maxGreen * maxBlue;

}
Console.WriteLine($"Part 1:{gameSum}");
Console.WriteLine($"Part 2:{sumOfPowerSets}");