using System.ComponentModel;
using System.Drawing;
using System.Reflection.Metadata;
using System.Text;
using Microsoft.VisualBasic;

var fileReader = new StreamReader(new FileStream("input", FileMode.Open));
const int floorSize = 140;
char[][] floor = new char[floorSize][];


 
var counter = 0;
Tile startingPosition = null!; 
while(!fileReader.EndOfStream)
{
    var line = await fileReader.ReadLineAsync();
    var sPipeLocation = line.IndexOf('S');
    if(sPipeLocation >= 0)
    {
        startingPosition = new(counter, sPipeLocation);
    }
    floor[counter++] = line.ToCharArray(); 
}

var stack = new Stack<Tile>();
stack.Push(startingPosition);

var(startX, startY, _) = startingPosition;
var dir = new StringBuilder();
if (IsValidPipeAbove(floor, startX, startY))
{
   dir.Append('N'); 
}
if(IsValidPipeDown(floor, startX, startY))
{
    dir.Append('S');
}
if (IsValidPipeLeft(floor, startX, startY))
{
   dir.Append('W'); 
}
if (IsValidPipeRight(floor, startX, startY))
{
   dir.Append('E'); 
}

var startingPipe = dir.ToString() switch
{
    "NS" =>  '|',
    "WE" => '-',
    "NE" => 'L',
    "NW" => 'J',
    "SW" => '7',
    "SE" => 'F',
    _ => throw new ArgumentOutOfRangeException()
};

floor[startX][startY] = startingPipe;

var steps = 0;
while(stack.Count > 0)
{
    var currPos = stack.Pop();
    var (currX, currY, prevPos) = currPos;

    // check left
    if (prevPos?.Y != currY - 1 && PipePointsWest(floor[currX][currY]) && IsValidPipeLeft(floor, currX, currY))
    {
        var tile = new Tile(currX, currY - 1, currPos);
        if (tile.X == startX && tile.Y == startY )
        {
            steps = tile.StepsFromStart;
            break;
        }
        stack.Push(tile);
    }

    // check up
    if (prevPos?.X != currX - 1 && PipePointsNorth(floor[currX][currY]) && IsValidPipeAbove(floor, currX, currY))
    {
        var tile = new Tile(currX - 1, currY, currPos);
        if (tile.X == startX && tile.Y == startY )
        {
            steps = tile.StepsFromStart;
            break;
        }
        stack.Push(tile);
    }

    // check right
    if (prevPos?.Y != currY + 1 && PipePointsEast(floor[currX][currY]) && IsValidPipeRight(floor, currX, currY))
    {
        var tile = new Tile(currX, currY + 1, currPos);
        if (tile.X == startX && tile.Y == startY )
        {
            steps = tile.StepsFromStart;
            break;
        }
        stack.Push(tile);
    }

    // check down
    if (prevPos?.X != currX + 1 && PipePointsSouth(floor[currX][currY]) && IsValidPipeDown(floor, currX, currY))
    {
        var tile = new Tile(currX + 1, currY, currPos);
        if (tile.X == startX && tile.Y == startY )
        {
            steps = tile.StepsFromStart;
            break;
        }
        stack.Push(tile);
    }
}

Console.WriteLine(steps / 2);
static bool IsValidPipeLeft(char[][] floor, int currX, int currY)
{
    return currY - 1 >= 0 && PipePointsEast(floor[currX][currY - 1]);
}
static bool IsValidPipeAbove(char[][] floor, int currX, int currY)
{
    return currX - 1 >= 0 && PipePointsSouth(floor[currX - 1][currY]);
}

static bool IsValidPipeRight(char[][] floor, int currX, int currY)
{
    return currY + 1 < floorSize && PipePointsWest(floor[currX][currY + 1]);
}

static bool IsValidPipeDown(char[][] floor, int currX, int currY)
{
    return currX + 1 < floorSize && PipePointsNorth(floor[currX + 1][currY]);
}

static bool PipePointsEast(char pipe)
{
    return pipe is 'F' or 'L' or '-';
}
static bool PipePointsWest(char pipe)
{
    return pipe is '-' or 'J' or '7';
}
static bool PipePointsSouth(char pipe)
{
    return pipe is '|' or '7' or 'F';
}
static bool PipePointsNorth(char pipe)
{
    return pipe is '|' or 'J' or 'L';
}

record Tile (int X, int Y, Tile? Previous = null)
{
    public int StepsFromStart = (Previous?.StepsFromStart ?? 0) + 1;
    public void Deconstruct(out int X, out int Y, out Tile? Previous)
    {
        X = this.X;
        Y = this.Y;
        Previous = this.Previous;
    }
}
