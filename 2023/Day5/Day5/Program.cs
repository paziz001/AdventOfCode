using System.Data;
using System.Text.RegularExpressions;

var seedsHeaderReg = new Regex(@"((\d+ ?)+)");
var individualSeedsRegex = new Regex(@"(\d+)+");
var seedRangesRegex = new Regex(@"((\d+ \d+)+)");
var mapHeaderRegex = new Regex(@"^\w+-\w+-\w+ map:");
var mapRangesRegex = new Regex(@"(\d+)+");
var fileReader = new StreamReader(new FileStream("input", FileMode.Open));

var seedsLine = await fileReader.ReadLineAsync();
var seeds = individualSeedsRegex.Matches(
    seedsHeaderReg.Matches(seedsLine)[0].Value).Select(s => long.Parse(s.Value));

var seedRanges = seedRangesRegex.Matches(seedsLine).Select(s =>
{
    var range = s.Value.Split(" ");
    var rangeStart = long.Parse(range[0]);
    var rangeLength = long.Parse(range[1]);
    return new Range(rangeStart, rangeStart + rangeLength);
}).ToList(); 

var groupMappings = new List<List<(Range src, Range dest)>>();
List<(Range src, Range dest)>? mappings = null; 
while(!fileReader.EndOfStream)
{
    var line = await fileReader.ReadLineAsync();
    if(string.IsNullOrEmpty(line.Trim()))
    {
        if(mappings is not null)
        {
            groupMappings.Add(mappings);
        }
        continue;
    }

    if(mapHeaderRegex.IsMatch(line))
    {
        mappings = [];
        continue;
    }

    var map = mapRangesRegex.Matches(line).Select(x => long.Parse(x.Value)).ToArray();
    var sourceMapping = new Range(map[1], map[1] + map[2]);
    var destMapping = new Range(map[0], map[0] + map[2]);

    mappings?.Add((sourceMapping, destMapping));
}

var min = long.MaxValue;
long sourceId = -1;
foreach(var seed in seeds)
{
    sourceId = seed;
    foreach(var groupMapping in groupMappings)
    {
        foreach(var (src, dest) in groupMapping)
        {
            if(sourceId >= src.Start && sourceId < src.End)
            {
                sourceId = sourceId - src.Start + dest.Start; 
                break;
            }
        }
    }
    min = sourceId <= min ? sourceId : min;
}
Console.WriteLine(min);
min = long.MaxValue;


var sourceRangeWasMapped = false;
List<Range> nextSourceRanges = [];
for (var i = 0; i < seedRanges.Count; i++)
{
    List<Range> currSourceRanges = [seedRanges[i]];
    for (var k = 0; k < groupMappings.Count; k++)
    {
        for(var j = 0; j < currSourceRanges.Count; j++)
        {
            foreach (var (src, dest) in groupMappings[k])
            {
                long sourceRangeStart;
                long sourceRangeEnd; 
                if (currSourceRanges[j].Start >= src.Start && currSourceRanges[j].End <= src.End)
                {
                    sourceRangeStart = currSourceRanges[j].Start - src.Start + dest.Start;
                    sourceRangeEnd = dest.End - (src.End - currSourceRanges[j].End); 
                    nextSourceRanges.Add(new Range(sourceRangeStart, sourceRangeEnd));  
                    sourceRangeWasMapped = true;
                    break;
                }
                if (currSourceRanges[j].Start >= src.Start && currSourceRanges[j].Start < src.End
                    && currSourceRanges[j].End > src.End)
                {
                    sourceRangeStart = currSourceRanges[j].Start - src.Start + dest.Start;
                    sourceRangeEnd = dest.End;
                    nextSourceRanges.Add(new Range(sourceRangeStart, sourceRangeEnd));
                    currSourceRanges.Add(new Range(src.End, currSourceRanges[j].End));
                    sourceRangeWasMapped = true;
                    break;
                }

                if (currSourceRanges[j].End > src.Start && currSourceRanges[j].End <= src.End
                    && currSourceRanges[j].Start < src.Start)
                {
                    sourceRangeStart = dest.Start;
                    sourceRangeEnd = dest.End - (src.End - currSourceRanges[j].End);
                    nextSourceRanges.Add(new Range(sourceRangeStart, sourceRangeEnd));
                    currSourceRanges.Add(new Range(currSourceRanges[j].Start, src.Start));
                    sourceRangeWasMapped = true;
                    break;
                }
                if(currSourceRanges[j].Start < src.Start && currSourceRanges[j].End > src.End)
                {
                    nextSourceRanges.Add(new Range(dest.Start, dest.End));
                    currSourceRanges.Add(new Range(currSourceRanges[j].Start, src.Start));
                    currSourceRanges.Add(new Range(src.End, currSourceRanges[j].End));
                    sourceRangeWasMapped = true;
                    break;
                }
            }
            if(!sourceRangeWasMapped)
            {
                nextSourceRanges.Add(currSourceRanges[j]);
            }
            sourceRangeWasMapped = false;
        }

        currSourceRanges = nextSourceRanges;
        nextSourceRanges = [];
    }
    var sourceRangeMin = currSourceRanges.Min(x => x.Start); 
    min = sourceRangeMin <= min ? sourceRangeMin : min;
    nextSourceRanges = [];
}
Console.WriteLine(min);
static int SortByStartOfRange((Range src, Range dest) range1, (Range src, Range dest) range2)
{
    return range1.dest.Start.CompareTo(range2.dest.Start);
}

record Range(long Start, long End);
