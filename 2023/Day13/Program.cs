using System.Data;
using System.Text;
using Pattern = (System.Collections.Generic.List<string> value, int mirrorPos, bool hasSmudge);

var fileReader = new StreamReader(new FileStream("input", FileMode.Open));
List<List<string>> allHorizontalPatterns = []; 
allHorizontalPatterns.Add([]);
var counter = 0;
while(!fileReader.EndOfStream)
{
    string line = await fileReader.ReadLineAsync() ?? "";
    if (string.IsNullOrEmpty(line.Trim()))
    {
        counter++;
        allHorizontalPatterns.Add([]);
    }
    else
    {
        allHorizontalPatterns[counter].Add(line!);
    }
}

var sum1 = 0;
var sum2 = 0;
foreach(var horizontalPattern in allHorizontalPatterns)
{
    var verticalPattern = new List<string>();
    for (var i = 0; i < horizontalPattern[0].Length; i++)
    {
        var verticalLine = new StringBuilder();
        for (var j = 0; j < horizontalPattern.Count; j++)
        {
            verticalLine.Append(horizontalPattern[j][i]);
        }
        verticalPattern.Add(verticalLine.ToString());
    }

    sum1 += FindSumForPattern(verticalPattern) + (FindSumForPattern(horizontalPattern) * 100);
    sum2 += FindSumForPattern(verticalPattern, considerSmudges: true) + (FindSumForPattern(horizontalPattern, considerSmudges: true) * 100);
}

Console.WriteLine(sum1);
Console.WriteLine(sum2);

static List<Pattern> FindPossibleMirrorPatterns(List<string> patterns, bool considerSmudges = false)
{
    List<Pattern> mirrorPositions = [];
    for (var i = 1; i < patterns.Count; i++)
    {
        var currLine = patterns[i];
        var prevLine = patterns[i - 1];
        if (currLine == prevLine)
        {
            mirrorPositions.Add((patterns, i, false));
        }
        else if (considerSmudges && TryFindSingleSmudge(currLine, prevLine, out var value))
        {
            List<char[]> corrected = [.. patterns.Select(x => x.ToCharArray())];
            corrected[i][value] = patterns[i - 1][value];
            mirrorPositions.Add((corrected.Select(x => new string(x)).ToList(), i, true));
        }
    }

    return mirrorPositions;

}

static bool TryFindSingleSmudge(string currLine, string prevLine, out int smudgePosition)
{
    var smudges = 0;
    smudgePosition = 0;
    for (var i = 0; i < currLine.Length; i++)
    {
        if (smudges > 1)
        {
            break;
        }
        if (currLine[i] != prevLine[i])
        {
            smudgePosition = i;
            smudges++;
        }
    }
    
    if(smudges == 1)
    {
        return true;
    }

    smudgePosition = 0;
    return false;
}

static bool HasPerfectReflection(Pattern pattern, bool considerSmudges = false)
{
    var (value, mirrorPos, hasSmudge) = pattern;
    var origin = mirrorPos - 1;
    var reflection = mirrorPos;

    var reflectionsCount = Math.Min(origin, value.Count - (mirrorPos + 1));

    var foundAllReflections = true;
    var smudgeCount = 0;
    for (var i = 0; i < reflectionsCount; i++)
    {
        origin -= 1;
        reflection += 1;
        if (value[origin] != value[reflection])
        {
            if(!considerSmudges || (considerSmudges && (smudgeCount > 0 || !TryFindSingleSmudge(value[origin], value[reflection], out var _))))
            {
                foundAllReflections = false;
            }
            else
            {
                smudgeCount++;
            }
        }
    }

    return (foundAllReflections && !considerSmudges) || foundAllReflections && (smudgeCount == 1 || pattern.hasSmudge);
}

static int FindSumForPattern(List<string> pattern, bool considerSmudges = false)
{
    var sum = 0;
    foreach (var mirrorPattern in FindPossibleMirrorPatterns(pattern, considerSmudges))
    {
        if (HasPerfectReflection(mirrorPattern, considerSmudges))
        {
            sum += mirrorPattern.mirrorPos;
        }
    }

    return sum;
}