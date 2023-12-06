using System.Data;
using System.Text.RegularExpressions;
using var fileReader = new StreamReader(new FileStream("input", FileMode.Open));
var timesInput = await fileReader.ReadLineAsync();
var distancesInput = await fileReader.ReadLineAsync();
var regex = new Regex(@"(\d+)");

var times = regex.Matches(timesInput).Select(t => long.Parse(t.Value)).ToList();
var distances = regex.Matches(distancesInput).Select(d => long.Parse(d.Value)).ToList();

long part1 = 1;

for (var i = 0; i < times.Count; i++)
{
    part1 *= FindTotalWaysToBeat(times[i], distances[i]);
}

Console.WriteLine(part1);


var longTime = long.Parse(string.Join("", times));
var longDistance = long.Parse(string.Join("", distances));
var part2 = FindTotalWaysToBeat(longTime, longDistance);

Console.WriteLine(part2);

static long FindTotalWaysToBeat(long time, long distance)
{
    var beatTimes = 0;
    for (var j = 1; j < time; j++)
    {
        var holdTheButtonMS = j;
        var speed = j;
        var remainingTime = time - holdTheButtonMS;
        var distanceTravelled = remainingTime * speed;
        if (holdTheButtonMS >= time)
        {
            continue;
        }
        if (distanceTravelled > distance)
        {
            beatTimes++;
        }
    }

    return beatTimes;
}