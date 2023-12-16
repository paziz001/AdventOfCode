using System.ComponentModel;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

var fileReader = new StreamReader(new FileStream("input", FileMode.Open));
List<string> contraptionTiles = []; 

while(!fileReader.EndOfStream)
{
    string line = await fileReader.ReadLineAsync() ?? "";
    contraptionTiles.Add(line);
}

var size = contraptionTiles.Count; 
var columnSize = contraptionTiles[0].Length;
for (var i = 0; i < size; i++)
{
     
}
HashSet<Beam> configs = []; 
Direction[] directions = [Direction.LEFT, Direction.RIGHT, Direction.DOWN, Direction.UP];

for(var row = 0; row < size; row+= size - 1)
{
    for (var col = 0; col < size; col++)
    {
        foreach (var direction in directions)
        {
            if(row == 0 && direction == Direction.UP)
            {
                continue;
            }
            if(row == size - 1 && direction == Direction.DOWN)
            {
                continue;
            }
            if(col == 0 && direction == Direction.LEFT)
            {
                continue;
            }
            if(col == size - 1 && direction == Direction.RIGHT)
            {
                continue;
            }
            configs.Add(new Beam(new Position(row, col), direction));
        }
    }
}

for(var col = 0; col < size; col+= size - 1)
{
    for (var row = 0; row < size - 1; row++)
    {
        foreach (var direction in directions)
        {
            if(row == 0 && direction == Direction.UP)
            {
                continue;
            }
            if(row == size - 1 && direction == Direction.DOWN)
            {
                continue;
            }
            if(col == 0 && direction == Direction.LEFT)
            {
                continue;
            }
            if(col == size - 1 && direction == Direction.RIGHT)
            {
                continue;
            }
            configs.Add(new Beam(new Position(row, col), direction));
        }
    }
}

List<int> sums = []; 

foreach (var config in configs)
{
    var sum = 0;
    Queue<Beam> beams = [];
    (char tile, HashSet<Beam> beamsPassed)[,] energizedTiles = new (char tile, HashSet<Beam> beamPassed)[size, columnSize];

    for (var i = 0; i < contraptionTiles.Count; i++)
    {
        for (var j = 0; j < contraptionTiles[0].Length; j++)
        {
            energizedTiles[i, j] = ('.', []);
        }
    }
    beams.Enqueue(config);

    while(beams.Count > 0)
    {
        var beam = beams.Dequeue();
        var (currPosition, currDirection) = beam;
        if(energizedTiles[currPosition.Row, currPosition.Col].tile is '#' && 
        energizedTiles[currPosition.Row, currPosition.Col].beamsPassed.Contains(beam))
        {
            continue;
        }

        energizedTiles[currPosition.Row, currPosition.Col].tile = '#';
        energizedTiles[currPosition.Row, currPosition.Col].beamsPassed.Add(beam);

        List<Beam> beamsToEnqueue = [];
        switch(contraptionTiles[currPosition.Row][currPosition.Col])
        {
            case TileType.SLASH_MIRROR:
                switch(currDirection)
                {
                    case Direction.RIGHT:
                        beamsToEnqueue.Add(new Beam(new Position(MoveUp(currPosition.Row), currPosition.Col), Direction.UP));
                        break;
                    case Direction.LEFT:
                        beamsToEnqueue.Add(new Beam(new Position(MoveDown(currPosition.Row), currPosition.Col), Direction.DOWN));
                        break;
                    case Direction.UP:
                        beamsToEnqueue.Add(new Beam(new Position(currPosition.Row, MoveRight(currPosition.Col)), Direction.RIGHT));
                        break;
                    case Direction.DOWN:
                        beamsToEnqueue.Add(new Beam(new Position(currPosition.Row, MoveLeft(currPosition.Col)), Direction.LEFT));
                        break;
                }
                break;

            case TileType.BACKSLASH_MIRROR:
                switch(currDirection)
                {
                    case Direction.RIGHT:
                        beamsToEnqueue.Add(new Beam(new Position(MoveDown(currPosition.Row), currPosition.Col), Direction.DOWN));                
                        break;
                    case Direction.LEFT:
                        beamsToEnqueue.Add(new Beam(new Position(MoveUp(currPosition.Row), currPosition.Col), Direction.UP));
                        break;
                    case Direction.UP:
                        beamsToEnqueue.Add(new Beam(new Position(currPosition.Row, MoveLeft(currPosition.Col)), Direction.LEFT));
                        break;
                    case Direction.DOWN:
                        beamsToEnqueue.Add(new Beam(new Position(currPosition.Row, MoveRight(currPosition.Col)), Direction.RIGHT));
                        break;
                }
                break;
            case TileType.VERTICAL_SPLITTER:
                switch(currDirection)
                {
                    case Direction.RIGHT:
                    case Direction.LEFT:
                        beamsToEnqueue.Add(new Beam(new Position(MoveUp(currPosition.Row), currPosition.Col), Direction.UP));                
                        beamsToEnqueue.Add(new Beam(new Position(MoveDown(currPosition.Row), currPosition.Col), Direction.DOWN));                
                        break;
                    case Direction.UP:
                        beamsToEnqueue.Add(new Beam(new Position(MoveUp(currPosition.Row), currPosition.Col), Direction.UP));                
                        break;
                    case Direction.DOWN:
                        beamsToEnqueue.Add(new Beam(new Position(MoveDown(currPosition.Row), currPosition.Col), Direction.DOWN));                
                        break;
                }
                break;

            case TileType.HORIZONTAL_SPLITTER:
                switch(currDirection)
                {
                    case Direction.UP:
                    case Direction.DOWN:
                        beamsToEnqueue.Add(new Beam(new Position(currPosition.Row, MoveRight(currPosition.Col)), Direction.RIGHT));
                        beamsToEnqueue.Add(new Beam(new Position(currPosition.Row, MoveLeft(currPosition.Col)), Direction.LEFT));
                        break;
                    case Direction.RIGHT:
                        beamsToEnqueue.Add(new Beam(new Position(currPosition.Row, MoveRight(currPosition.Col)), Direction.RIGHT));
                        break;
                    case Direction.LEFT:
                        beamsToEnqueue.Add(new Beam(new Position(currPosition.Row, MoveLeft(currPosition.Col)), Direction.LEFT));
                        break;
                }
                break;

            case TileType.EMPTY:
            default:
                switch(currDirection)
                {
                    case Direction.RIGHT:
                        beamsToEnqueue.Add(new Beam(new Position(currPosition.Row, MoveRight(currPosition.Col)), Direction.RIGHT));
                        break;
                    case Direction.LEFT:
                        beamsToEnqueue.Add(new Beam(new Position(currPosition.Row, MoveLeft(currPosition.Col)), Direction.LEFT));
                        break;
                    case Direction.UP:
                        beamsToEnqueue.Add(new Beam(new Position(MoveUp(currPosition.Row), currPosition.Col), Direction.UP));
                        break;
                    case Direction.DOWN:
                        beamsToEnqueue.Add(new Beam(new Position(MoveDown(currPosition.Row), currPosition.Col), Direction.DOWN));                
                        break;
                }
                break;
        }

        foreach (var beamToEnqueue in beamsToEnqueue)
        {
            var (next, _) = beamToEnqueue; 
            if(next.Row == currPosition.Row && next.Col == currPosition.Col)
            {
                continue;
            }
            beams.Enqueue(beamToEnqueue);
        }
    }

    for (var i = 0; i < contraptionTiles.Count; i++)
    {
        for (var j = 0; j < contraptionTiles[0].Length; j++)
        {
            if(energizedTiles[i, j].tile == '#')
            {
                sum += 1;
            }
        }
    }

    sums.Add(sum);
}


Console.WriteLine(sums[0]);
Console.WriteLine(sums.Max());

int MoveRight(int col)
{
    var newPosition = col + 1;
    return newPosition >= columnSize ? columnSize - 1 : newPosition;
}

int MoveLeft(int col)
{
    var newPosition = col - 1;
    return newPosition < 0 ? 0 : newPosition;
}

int MoveUp(int row)
{
    var newPosition = row - 1;
    return newPosition < 0 ? 0 : newPosition;
}

int MoveDown(int row)
{
    var newPosition = row + 1;
    return newPosition >= size ? size - 1 : newPosition;
}

record Beam(Position Position, Direction Direction);

record Position(int Row, int Col);

enum Direction
{
    LEFT = 0,
    DOWN = 1,
    UP = 2,
    RIGHT = 3
}

static class TileType
{
    public const char SLASH_MIRROR = '/';
    public const char BACKSLASH_MIRROR = '\\';
    public const char VERTICAL_SPLITTER = '|';
    public const char HORIZONTAL_SPLITTER = '-';
    public const char EMPTY = '.';
}