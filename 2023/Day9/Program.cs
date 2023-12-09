var fileReader = new StreamReader(new FileStream("input", FileMode.Open));
long sum1 = 0;
long sum2 = 0;
while(!fileReader.EndOfStream)
{
    var line = await fileReader.ReadLineAsync();
    var sequences = new List<List<long>>
    {
        line.Split(' ').Select(long.Parse).ToList()
    };
    var currSequence = sequences[0];
    List<long> nextSequence = [];
    while(!currSequence.All(x => x == 0))
    {
        for(var i = 0; i < currSequence.Count - 1; i++)
        {
           nextSequence.Add(currSequence[i + 1] - currSequence[i]);
        }
        sequences.Add(nextSequence);
        currSequence = nextSequence; 
        nextSequence = [];
    }
    long nextInHistory = 0;
    long firstInHistory = 0;
    for (var i = sequences.Count - 1; i >= 0; i--)
    {
        var sequenceCount = sequences[i].Count;
        nextInHistory += sequences[i][sequenceCount - 1];
    }
    for (var i = sequences.Count - 1; i >= 0; i--)
    {
        firstInHistory = sequences[i][0] - firstInHistory;
    }
    sequences = null;
    sum1 += nextInHistory;
    sum2 += firstInHistory;
}

Console.WriteLine(sum1);
Console.WriteLine(sum2);
