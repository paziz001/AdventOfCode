using System.Text;
using System.Text.RegularExpressions;

var regexRecord = new Regex(@"^(?<conditions>.+) (?<groups>.+)$");
var fileReader = new StreamReader(new FileStream("input", FileMode.Open));
var sum = 0;
while(!fileReader.EndOfStream)
{
    var line = await fileReader.ReadLineAsync();
    var record = regexRecord.Matches(line);
    var condition = record[0].Groups["conditions"].Value;
    var groups = record[0].Groups["groups"].Value.Split(',').Select(int.Parse).ToList();

    sum += PrintPermutations(condition);
    int PrintPermutations(string condition)
    {
        var index = condition.IndexOf('?');
        var regex = new Regex(@"\?");
        if(regex.IsMatch(condition))
        {
            return PrintPermutations(regex.Replace(condition, ".", 1)) +
                PrintPermutations(regex.Replace(condition, "#", 1));
        }
        else if(IsValid(condition))
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    bool IsValid(string condition)
    {
        var regexBuild = new StringBuilder();
        regexBuild.Append(@"^\.*");
        for (var i = 0; i < groups.Count; i++) 
        {
            regexBuild.Append(@$"(#{{{groups[i]}}})");
            if (i < (groups.Count - 1)) 
            {
                regexBuild.Append(@"\.+");
            }
            else
            {
                regexBuild.Append(@"\.*$");
            }
        }

        var regexPattern = new Regex(regexBuild.ToString());

        return regexPattern.IsMatch(condition);
    }

}

Console.WriteLine(sum);
