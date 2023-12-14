var fileReader = new StreamReader(new FileStream("input", FileMode.Open));
List<char[]> tiles = []; 
List<char[]> verticalTiles = [];
while(!fileReader.EndOfStream)
{
    string line = await fileReader.ReadLineAsync() ?? "";
    tiles.Add(line.ToCharArray());
}

var sum1 = 0;
for (var i = 0; i < tiles[0].Length; i++)
{
    var verticalTile = new char[tiles.Count];
    var counter = 0;
    for (var j = tiles.Count - 1; j >= 0; j--)
    {
        verticalTile[counter++] = tiles[j][i];
    }
    verticalTiles.Add(verticalTile);
}

TiltDisk(verticalTiles);

Console.WriteLine(CalculateSum(verticalTiles));

static void TiltDisk(List<char[]> verticalTiles)
{
    for (var i = 0; i < verticalTiles.Count; i++)
    {
        var tile = new Span<char>(verticalTiles[i]);
        var prevRockIndex = tile.Length - 1;        
        while (true)
        {
            var tileToCheck = tile[..(prevRockIndex + 1)];
            var lastIndexOfRoundedRock = tileToCheck.LastIndexOf('O');
            if(lastIndexOfRoundedRock < 0)
            {
                break;
            }
            var emptyTileExistsNearRoundRock = tileToCheck[lastIndexOfRoundedRock..].IndexOf('.') == 1;
            if(emptyTileExistsNearRoundRock && 
                lastIndexOfRoundedRock != prevRockIndex)
           
            {
                var lastIndexOfRock = tile[(lastIndexOfRoundedRock+1)..].IndexOfAny('#', 'O') + lastIndexOfRoundedRock + 1;
                var edge = lastIndexOfRock == lastIndexOfRoundedRock ? prevRockIndex : lastIndexOfRock;
                var indexOfEmptyTile = tile[lastIndexOfRoundedRock..(edge + 1)].LastIndexOf('.') + lastIndexOfRoundedRock;
                tile[lastIndexOfRoundedRock] = '.';
                tile[indexOfEmptyTile] = 'O';
                prevRockIndex = indexOfEmptyTile;
                continue;
            }
            prevRockIndex = lastIndexOfRoundedRock - 1;
        }
        Console.WriteLine(new string(tile));
    }
}

static int CalculateSum(List<char[]> verticalTiles)
{
    var sum = 0;
    foreach (var tile in verticalTiles)
    {

        var tileSpan = new Span<char>(tile);
        List<int> rockWeights = [];
        var firstIndexOfRoundedRock = tileSpan.LastIndexOf('O');
        var rockSum = 0;
        
        while(firstIndexOfRoundedRock > -1)
        {
            rockSum += firstIndexOfRoundedRock + 1;
            firstIndexOfRoundedRock = tileSpan[..firstIndexOfRoundedRock].LastIndexOf('O');
        }

        sum += rockSum;
    }
     return sum;
}