var itemPriorities = new Dictionary<char, short>();

short priorityCounter = 1;
for(var i = Convert.ToInt16('a'); i <= Convert.ToInt16('z'); i++)
{
    itemPriorities.Add(Convert.ToChar(i), priorityCounter);
    priorityCounter++;
}
for(var i = Convert.ToInt16('A'); i <= Convert.ToInt16('Z'); i++)
{
    itemPriorities.Add(Convert.ToChar(i), priorityCounter);
    priorityCounter++;
}

var inputFile = new FileStream("input", FileMode.Open);

using var streamReader = new StreamReader(inputFile);

var prioritySumOfCommonItems = 0;
var prioritySumOfBudges = 0;
var rucksackCounter = 0;
var groupRucksacks = new string[3]; 
var line = await streamReader.ReadLineAsync();
while(line is not null)
{
    prioritySumOfCommonItems += CalculatePrioritySumOfCommonItemsInCompartments(line, itemPriorities);

    groupRucksacks[rucksackCounter] = line;
    var groupFound = rucksackCounter == 2;
    if(groupFound)
    {
        prioritySumOfBudges += CalculatePrioritySumOfCommonItems(itemPriorities, groupRucksacks);
        rucksackCounter = 0;
    }
    else
    {
        rucksackCounter++;
    }

    line = await streamReader.ReadLineAsync();
}

Console.WriteLine($"The sum of common item priorities in ruck compartments is {prioritySumOfCommonItems}");
Console.WriteLine($"The sum of badge priorities of all elf groups is {prioritySumOfBudges}");

int CalculatePrioritySumOfCommonItemsInCompartments(string racksack, Dictionary<char, short> itemPriorities)
{
    string firstCompartment = racksack[..(racksack.Length / 2)];
    string secondCompartment = racksack[(racksack.Length / 2)..];

    return CalculatePrioritySumOfCommonItems(itemPriorities, firstCompartment, secondCompartment);
}

int CalculatePrioritySumOfCommonItems(Dictionary<char, short> itemPriorities, params string[] itemGroups)
{
    IEnumerable<char> commonItems = itemGroups.Aggregate((itemGroupA, itemGroupB) => new string(itemGroupA.Intersect(itemGroupB).ToArray())); 

    return commonItems.Sum(item => itemPriorities[item]); 
}