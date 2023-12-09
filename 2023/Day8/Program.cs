using System.Text.RegularExpressions;

var useExampleInput = false;
using var fileReader = new StreamReader(new FileStream(useExampleInput ? "inputExample" : "input", FileMode.Open));
var regex = new Regex(@"^(?<node>\w{3}) = \(((?<left>\w{3}), (?<right>\w{3}))\)$");

var instructions = await fileReader.ReadLineAsync();
await fileReader.ReadLineAsync();

var map = new Dictionary<string, (string left, string right)>();
var startNodes = new HashSet<string>();
while (!fileReader.EndOfStream)
{
    var line = await fileReader.ReadLineAsync();
    var coordinates = regex.Matches(line)[0].Groups;
    var node = coordinates["node"].Value;
    map.Add(node, (coordinates["left"].Value, coordinates["right"].Value));
    if(node.EndsWith('A'))
    {
        startNodes.Add(coordinates["node"].Value);
    }
}

List<string> nextNodes = [];


var part1 = await CalculateStepsOnInstructions(instructions, map, startNode: "AAA", (node) => node == "ZZZ");

IEnumerable<Task<long>> CreateTasksForGhosts()
{
    foreach (var startNode in startNodes)
    {
        yield return CalculateStepsOnInstructions(instructions, map, startNode: startNode, (node) => node.EndsWith('Z'));
    }
}

Console.WriteLine(part1);
var initialSteps = await Task.WhenAll(CreateTasksForGhosts());

// Use of LCM formula to find the common amount of steps that lead to a Z location
// The use of LCM is only possible because each starting point leads to a z location with the same amount of steps each time
// for example the the point AAA leads to ZZZ every 11567 steps starting with the first 11567 steps
var part2 = CalculateLCM(initialSteps);
Console.WriteLine(part2);

// Solution where we are adding the total of steps for each starting point until we find a
// comoon amount of steps for all starting points leading to a Z location
// List<long> currSteps = [.. initialSteps];
// while (currSteps.Aggregate((a, b) => a == b ? b : -1) == -1)
// {
//     for (var i = 0; i < currSteps.Count; i++)
//     {
//         currSteps[i] += initialSteps[i]; 
//     }
// }

// Console.WriteLine(currSteps[0]);


async Task<long> CalculateStepsOnInstructions(string? instructions, Dictionary<string, (string left, string right)> map, string startNode, Func<string, bool> IsEndNode)
{
    return await Task.Run(() =>
    { 
        long sum = 0;
        var currNode = startNode;
        while (!IsEndNode(currNode))
        {
            for (var i = 0; i < instructions.Length; i++)
            {
                if (instructions[i] == 'R')
                {
                    currNode = map[currNode].right;
                }
                else if (instructions[i] == 'L')
                {
                    currNode = map[currNode].left;
                            
                }
                sum++;
                if (IsEndNode(currNode))
                {
                    break;
                }
            }
        }
        return sum;
    });
}

decimal CalculateLCM(params long[] numbers)
{
    if(numbers.Length <= 0)
    {
       return numbers?[0] ?? 0; 
    }
    var minNumber = numbers.Min();
    if(minNumber == 0)
    {
        throw new InvalidOperationException("LCM of 0 does not exist");
    }

    long highestCommonDevisor = 1;
    for (long i = minNumber; i > 0; i--)
    {
        var foundHighestCommonDevisor = true;
        foreach (long number in numbers)
        {
            if(number % i != 0)
            {
                foundHighestCommonDevisor = false;
                break;
            }
        }
        if(foundHighestCommonDevisor)
        {
            highestCommonDevisor =  i;
            break;
        }
    }

    return numbers.Aggregate((a, b) => a * b / highestCommonDevisor);
}

record Node(string Name, Node Left, Node Right);

