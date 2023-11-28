using var streamReader = new StreamReader(new FileStream("TestInput", FileMode.Open));
var line = await streamReader.ReadLineAsync(); 
var elfCalories = 0;
var firstPlace = int.MinValue;
var secondPlace = int.MinValue;
var thirdPlace = int.MinValue;
while(line is not null)
{
    if(string.IsNullOrWhiteSpace(line))
    {
        if(elfCalories >= firstPlace)
        {
            thirdPlace = secondPlace > thirdPlace ? secondPlace : thirdPlace;
            secondPlace = firstPlace > secondPlace ? firstPlace : secondPlace;
            firstPlace = elfCalories;
        }
        else if(elfCalories >= secondPlace)
        {
            thirdPlace = secondPlace > thirdPlace ? secondPlace : thirdPlace;
            secondPlace = elfCalories;
        }
        else if(elfCalories >= thirdPlace)
        {
            thirdPlace = elfCalories;
        }
        
        elfCalories = 0;
    }
    else if(int.TryParse(line, out var foodCalories))
    {
        elfCalories += foodCalories;
    } 
    line = await streamReader.ReadLineAsync(); 
}
Console.WriteLine($"The top elf has a total of {firstPlace} calories");
Console.WriteLine($"The top 3 elfs have a sum of {firstPlace + secondPlace + thirdPlace} calories");