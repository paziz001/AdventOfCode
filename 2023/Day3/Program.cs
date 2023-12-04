using System.Text;

//
// Solution without using 2d array which fails for some edge cases 
// 
// var fileReader = new StreamReader(new FileStream("inputExample", FileMode.Open));
// var sum1 = 0;
// List<(int startIndex, string number)> prevUnmatchedNumbers = 
//     [];
// List<(int startIndex, string number)> currUnmatchedNumbers = 
//     [];
// HashSet<int> prevSymbolIndexes = [];
// HashSet<int> currSymbolIndexes = [];
// while(!fileReader.EndOfStream)
// {
//     var line = await fileReader.ReadLineAsync();
//     var currNumber = new StringBuilder();
    
    
//     for (var i = 0; i < line?.Length; i++)
//     {
//         // found part of the number
//         if(int.TryParse($"{line[i]}", out var n)) 
//         {
//             currNumber.Append(n);
//             continue;
//         }
//         // Found a character other than '.'
//         if(line[i] != '.')
//         {
//             sum1 += currNumber.Length == 0 ? 0 : int.Parse(currNumber.ToString());     
//             var listToIterate = 
//                 new List<(int startIndex, string number)>(prevUnmatchedNumbers);
//             // check if the symbol is adjacent to the previous numbers
//             for (var j = listToIterate.Count - 1; j >= 0; j--) 
//             {
//                 if(i >= listToIterate[j].startIndex - 1 && i <= listToIterate[j].startIndex + listToIterate[j].number.Length)
//                 {
//                     sum1 += int.Parse(listToIterate[j].number);
//                     prevUnmatchedNumbers.RemoveAt(j);
//                 }
//             }
//             currNumber.Clear();
//             currSymbolIndexes.Add(i);
//             continue;
//         }

//         // Found '.' but there was a non dot character at the start of
//         // a number we started building
//         if(currNumber.Length > 0 && currSymbolIndexes.Contains(i - (currNumber.Length + 1)))
//         {
//             sum1 += int.Parse(currNumber.ToString());
//             currNumber.Clear();
//             continue;
//         }
        
//         // We were building a number but we couldnt find a character at the boundaries
//         // we should store it to see if it matches diagonally in the next line
//         if(currNumber.Length > 0)
//         {
//             var startIndex = i - currNumber.Length;
//             var startDiagBoundary = startIndex - 1;
//             var endDiagBoundary = i;
//             if (prevSymbolIndexes.Any(index => index >= startDiagBoundary  && index <= endDiagBoundary))
//             {
//                 sum1 += int.Parse(currNumber.ToString());
//             }
//             else
//             {
//                 currUnmatchedNumbers.Add((startIndex, currNumber.ToString()));
//             }
//             currNumber.Clear();
//         }
//     }
//     prevUnmatchedNumbers = currUnmatchedNumbers;
//     currUnmatchedNumbers = [];
//     prevSymbolIndexes = currSymbolIndexes;
//     currSymbolIndexes = [];
// }

// Console.WriteLine(sum1);


var fileReader = new StreamReader(new FileStream("input", FileMode.Open));

char[][] lines = new char[200][];


 
var counter = 0;
while(!fileReader.EndOfStream)
{
    var line = await fileReader.ReadLineAsync();
    lines[counter++] = line.ToCharArray(); 
}
var sum1 = 0;
var gearSymbols = new Dictionary<(int y, int x), List<int>>();
var currNumber = new StringBuilder();
for (var i = 0; i < lines.GetLength(0) && lines[i] != null; i++)
{
    for (var j = 0; j < lines[i].Length; j++)
    {
        // Skip until you find a number
        var numberAdded = false;
        if(!int.TryParse($"{lines[i][j]}", out var _)) continue;
        currNumber.Append(lines[i][j]);
        // if there is anything other than anumber it means that we need to check if there any non . symbols around
        // or if at the end of line
        if(j + 1 < lines[i].Length && int.TryParse($"{lines[i][j+1]}", out var _)) continue;
        for (var k = i - 1 >= 0 ? i - 1 : 0; k <= (i + 1 >= lines.GetLength(0) ? lines.GetLength(0) - 1 : i + 1) && lines[k] != null; k++)
        {
            var hStartBoundary = j - currNumber.Length;
            var hEndBoundary = j + 1;
            for (var l = hStartBoundary >= 0 ? hStartBoundary : 0; l <= (hEndBoundary >= lines[i].Length ? lines[i].Length - 1 : hEndBoundary); l++)
            {
                if(lines[k][l] == '*')
                {
                    // store the coordinates of the symbol and count the amount of times you have found it
                    var parsedNumber = int.Parse(currNumber.ToString());
                    if(!gearSymbols.ContainsKey((k, l)))
                    {
                        gearSymbols.Add((k, l), []);
                    }
                    gearSymbols[(k, l)].Add(parsedNumber);
                }
                // Skip if the actual number is being checked since we care only for the symbols
                if(!numberAdded && lines[k][l] != '.' && !int.TryParse($"{lines[k][l]}", out var _))
                {
                    sum1 += int.Parse(currNumber.ToString() == "" ? "0" : currNumber.ToString());
                    numberAdded = true;
                }
            }
        }
        currNumber.Clear();
    }
}

Console.WriteLine(sum1);
var sum2 = gearSymbols.Where((kv) => kv.Value.Count == 2).Sum((kv) => kv.Value.Aggregate((a, b) => a * b));
Console.WriteLine(sum2);