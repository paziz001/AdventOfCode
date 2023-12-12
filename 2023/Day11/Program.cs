using System.Text.RegularExpressions;

var emptySpaceRegex = new Regex(@"^\.+$");
var rowCounter = 0;
var rowsToExpand = new List<int>();
var lines = new List<string>();
var fileReader = new StreamReader(new FileStream("input", FileMode.Open));
while(!fileReader.EndOfStream)
{
    var line = await fileReader.ReadLineAsync();
    if (emptySpaceRegex.IsMatch(line!))
    {
        rowsToExpand.Add(rowCounter);
    }
    lines.Add(line!);
    rowCounter++;
}

var columnsToExpand = new List<int>();
for (var i = 0; i < lines[0].Length; i++)
{
    var emptyColFound = true;
    for(var j = 0; j < lines.Count; j++)
    {
        if(lines[j][i] != '.')
        {
            emptyColFound = false;
        };
    }
    if(emptyColFound)
    {
        columnsToExpand.Add(i);
    }
}

var regex = new Regex(@"(#)"); 
var galaxyCount = 1;
Dictionary<int, Coordinates> galaxyCoordinates = []; 
for (var i = 0; i < lines.Count; i++)
{
    var prevIndex = 0;
    foreach (var match in regex.Matches(lines[i])) 
    {
        var indexOfGalaxy = lines[i].IndexOf('#', prevIndex);
        prevIndex = indexOfGalaxy + 1;
        galaxyCoordinates[galaxyCount] = new Coordinates(i, indexOfGalaxy); 
        galaxyCount++;
    }
}

Dictionary<int, List<Coordinates>> pairs = []; 
for (var i = 1; i <= galaxyCount; i++)
{
    for (var j = i + 1; j <= galaxyCount - 1; j++)
    {
        if(!pairs.TryGetValue(i, out var value))
        {
            pairs.Add(i, []);
        }
        pairs[i].Add(galaxyCoordinates[j]);
    }
}

var rowSize = lines.Count;
var colSize = lines[0].Length;
Dictionary<Coordinates, List<(Coordinates targetCoords, int distance)>> allDistances = [];
foreach (var pair in pairs)
{
    List<(Coordinates targetCoords, int distance)> currDistances = [];
    bool[,] visited = new bool[lines.Count, lines[0].Length];

    var startCoords = galaxyCoordinates[pair.Key];
    var startNode = new Node(startCoords, 0);
    Queue<Node> queue = [];
    queue.Enqueue(startNode);
    visited[startCoords.Row, startCoords.Col] = true;

    while(queue.Count > 0)
    {
        var node = queue.Dequeue();
        var colLeft = node.Coords.Col - 1;
        var colRight = node.Coords.Col + 1;

        var rowUp = node.Coords.Row - 1;
        var rowDown = node.Coords.Row + 1;

        for(var row = rowUp; row <= rowDown; row += 2)
        {
            if(row < 0 || row >= rowSize)
            {
                continue;
            }
            if(visited[row, node.Coords.Col])
            {
                continue;
            }
            visited[row, node.Coords.Col] = true;
            var adjacentNode = new Node(new Coordinates(row, node.Coords.Col), node.DistanceFromGoal + 1);
            var space = lines[row][node.Coords.Col]; 
            if(pair.Value.Contains(adjacentNode.Coords))
            {
                currDistances.Add((adjacentNode.Coords, adjacentNode.DistanceFromGoal));
            }
            queue.Enqueue(adjacentNode);
        }
        
        for(var col = colLeft; col <= colRight; col+=2)
        {
            if(col < 0 || col >= colSize)
            {
                continue;
            }
            if(visited[node.Coords.Row, col])
            {
                continue;
            }
            visited[node.Coords.Row, col] = true;
            var adjacentNode = new Node(new Coordinates(node.Coords.Row, col), node.DistanceFromGoal + 1);
            var space = lines[node.Coords.Row][col]; 
            if(pair.Value.Contains(adjacentNode.Coords))
            {
                currDistances.Add((adjacentNode.Coords, adjacentNode.DistanceFromGoal));
            }
            queue.Enqueue(adjacentNode);
        }
        if(currDistances.Count == pair.Value.Count)
        {
            break;
        }
    }
    allDistances.Add(startCoords,  currDistances);
}

long part1 = CalculateSum(1);
long part2 = CalculateSum(999999);

Console.WriteLine(part1);
Console.WriteLine(part2);

long CalculateSum(int spacesToAdd)
{
    long sum = 0;
    foreach (var nodeDistances in allDistances)
    {
        long nodeSum = 0;
        foreach (var distance in nodeDistances.Value)
        {
            var startCoords = nodeDistances.Key;
            long rowsToAdd = Enumerable
                .Range(
                    Math.Min(distance.targetCoords.Row, startCoords.Row),
                    Math.Abs(distance.targetCoords.Row - startCoords.Row))
                .Where(rowsToExpand.Contains)
                .Count() * spacesToAdd;
            long columnsToAdd = Enumerable
                .Range(
                    Math.Min(distance.targetCoords.Col, startCoords.Col),
                    Math.Abs(distance.targetCoords.Col - startCoords.Col))
                .Where(columnsToExpand.Contains)
                .Count() * spacesToAdd;
            nodeSum += distance.distance + rowsToAdd + columnsToAdd;
        }
        sum += nodeSum;
    }

    return sum;
}

record Node(Coordinates Coords, int DistanceFromGoal);

record Coordinates(int Row, int Col);


