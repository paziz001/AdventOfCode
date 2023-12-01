using var inputReader = new StreamReader(new FileStream("input", FileMode.Open));
var sum = 0;
var sumWithWordNumbers = 0;

while(!inputReader.EndOfStream)
{
    var line = await inputReader.ReadLineAsync() ?? "";
    short calibrationValue = 0;

    var numbers = line.Where(c => short.TryParse($"{c}", out var number) ? number > 0 : false)
        .Select(c => short.Parse($"{c}"))
        .ToList();
    if (numbers.Count > 0)
    {
        short.TryParse($"{numbers.First()}{numbers.Last()}", out calibrationValue);
        sum += calibrationValue; 
    }

    numbers = GetNumbersForPart2(line);
    if (numbers.Count > 0)
    {
        short.TryParse($"{numbers.First()}{numbers.Last()}", out calibrationValue);
        sumWithWordNumbers += calibrationValue;
    }
}

Console.WriteLine($"Part 1: {sum}");
Console.WriteLine($"Part 2: {sumWithWordNumbers}");

List<short> GetNumbersForPart2(string line)
{
    var wordNumbers = new Dictionary<string, short>(9){{"one", 1}, { "two", 2 }, {"three", 3}, {"four", 4}, {"five",5}, {"six", 6}, {"seven", 7}, {"eight", 8}, {"nine", 9}};
    var numbers = new List<short>(); 
    
    for (var i = 0; i < line.Length; i++)
    {
        short.TryParse($"{line[i]}", out var number);
        if (number > 0)
        {
            numbers.Add(number);
        }
        for (var j = 3; j + i <= line.Length; j++) 
        {
            var wordNumber = line.Substring(i, j);
            if(wordNumbers.TryGetValue(wordNumber, out number))
            {
               numbers.Add(number); 
               break;
            }
        }
    }
    return numbers;
}