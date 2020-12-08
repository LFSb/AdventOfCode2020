using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

public static partial class Days
{
  private const string InputBasePath = @"Days/Input/";

  private static string OutputResult(string part1 = "", string part2 = "")
  {
    return $"{Environment.NewLine}- Part 1: {part1}{Environment.NewLine}- Part 2: {part2}";
  }

  #region Day1: Solved!

  public static string Day1()
  {
    var target = 2020;
    var input = File.ReadAllLines(Path.Combine(InputBasePath, "Day1.txt")).Select(int.Parse);
    var inputStack = new Stack<int>(input);

    var part1 = string.Empty; //For part 1, we need to find a pair of numbers that together sum to 2020.

    while (part1 == string.Empty)
    {
      var current = inputStack.Pop();

      if (current.CanSumTo(target, inputStack, out var candidate))
      {
        part1 = (current * candidate).ToString();
      }
    }

    var part2 = string.Empty; //For part 2, we need to find three numbers that together sum to 2020.

    inputStack = new Stack<int>(input);

    while (part2 == string.Empty)
    {
      var current = inputStack.Pop();

      foreach (var next in inputStack.ToArray())
      {
        if ((current + next).CanSumTo(target, new Stack<int>(inputStack.Except(new[] { next })), out var candidate))
        {
          part2 = (current * next * candidate).ToString();
        }
      }
    }

    return OutputResult(part1, part2);
  }

  private static bool CanSumTo(this int source, int target, Stack<int> numbers, out int candidate)
  {
    candidate = numbers.FirstOrDefault(x => x == (target - source));
    return candidate != 0;
  }

  #endregion

  #region Day2: Done!

  public static string Day2()
  {
    var inputs = File.ReadAllLines(Path.Combine(InputBasePath, "Day2.txt"));

    var rules = inputs.Select(input => new Day2Rule(input));

    var part1 = rules.Count(x => x.IsValidP1());

    var part2 = rules.Count(x => x.IsValidP2());

    return OutputResult(part1.ToString(), part2.ToString());
  }

  private class Day2Rule
  {
    public int Min { get; set; }

    public int Max { get; set; }

    public char Rule { get; set; }

    public string PassWord { get; set; }

    public Day2Rule(string input)
    {
      var minMax = input.Substring(0, input.IndexOf(' ')).Split('-');

      Min = int.Parse(minMax[0]);
      Max = int.Parse(minMax[1]);
      Rule = input[input.IndexOf(' ') + 1];
      PassWord = input.Substring(input.IndexOf(':') + 2);
    }

    public bool IsValidP1()
    {
      var count = PassWord.Count(x => x == Rule);

      return count >= Min && count <= Max;
    }

    public bool IsValidP2()
    {
      var min = PassWord[Min - 1] == Rule;
      var max = PassWord[Max - 1] == Rule;
      return min ^ max;
    }
  }

  #endregion

  #region Day3: Solved!

  public static string Day3()
  {
    var input = File.ReadAllLines(Path.Combine(InputBasePath, "Day3.txt"));

    var steps = new[]
    {
      new Tuple<int, int>(1, 1),
      new Tuple<int, int>(3, 1),
      new Tuple<int, int>(5, 1),
      new Tuple<int, int>(7, 1),
      new Tuple<int, int>(1, 2)
    };

    var d3grid = new Day3Grid(input, input.Length * steps.Max(x => x.Item1));

    var yStep = 1;
    var xStep = 3;

    return OutputResult(d3grid.P1(yStep, xStep).ToString(), d3grid.P2(steps).ToString());
  }

  private class Day3Grid
  {
    public bool[][] Grid { get; private set; }

    private int CurrentX { get; set; }

    private int CurrentY { get; set; }

    public Day3Grid(string[] inputs, int maxWidth)
    {
      Grid = new bool[inputs.Length][];

      var xdepth = inputs[0].Length;

      for (var y = 0; y < inputs.Length; y++)
      {
        for (var x = 0; x < maxWidth; x++)
        {
          if (Grid[y] == null)
          {
            Grid[y] = new bool[maxWidth];
          }

          Grid[y][x] = inputs[y][x % xdepth] == '#';
        }
      }
    }

    public int P1(int ystep, int xstep)
    {
      var count = 0;

      while (CurrentY < Grid.Length - 1)
      {
        CurrentY += ystep;
        CurrentX += xstep;

        if (Grid[CurrentY][CurrentX])
        {
          count++;
        }
      }

      //Reset the X/Y values for the next part.

      CurrentY = 0;
      CurrentX = 0;

      return count;
    }

    public int P2(Tuple<int, int>[] inputs)
    {
      var result = 1; //To avoid multiplying by 0..

      foreach (var input in inputs)
      {
        result *= P1(input.Item2, input.Item1); ;
      }

      return result;
    }
  }

  #endregion

  #region Day4: Solved!
  public static string Day4()
  {
    var input = File.ReadAllText(Path.Combine(InputBasePath, "Day4.txt"));

    var passports = input.Split(new[] { $"{Environment.NewLine}{Environment.NewLine}" }, StringSplitOptions.RemoveEmptyEntries);

    var parsedPassports = new List<Day4Passport>();

    foreach (var passport in passports)
    {
      parsedPassports.Add(new Day4Passport(passport));
    }

    return OutputResult(parsedPassports.Count(x => x.P1()).ToString(), parsedPassports.Count(x => x.P2()).ToString());
  }

  public class Day4Passport
  {
    public string byr { get; private set; }

    public string iyr { get; private set; }

    public string eyr { get; private set; }

    public string hgt { get; private set; }

    public string hcl { get; private set; }

    public string ecl { get; private set; }

    public string pid { get; private set; }

    public string cid { get; private set; }

    public Day4Passport(string passport)
    {
      var split = passport.Replace(Environment.NewLine, " ").Split(' ').ToDictionary(x => x.Split(':')[0], x => x.Split(':')[1]);

      var props = this.GetType().GetProperties().ToArray();

      foreach (var prop in props)
      {
        if (split.ContainsKey(prop.Name))
        {
          prop.SetValue(this, split[prop.Name]);
        }
      }
    }

    public bool P1()
    {
      var validCount = this.GetType().GetProperties().Select(x => x.GetValue(this)).Count(x => x != null);

      return (validCount == 8 || (validCount == 7 && this.cid == null));
    }

    private string[] ValidEyeColor = new string[]
    {
      "amb","blu","brn","gry","grn","hzl","oth"
    };

    public bool P2()
    {
      var isValid = true;

      isValid &= byr != null && byr.Length == 4 && int.TryParse(byr, out var parsedByr) && (parsedByr >= 1920 && parsedByr <= 2002);

      isValid &= iyr != null && iyr.Length == 4 && int.TryParse(iyr, out var parsedIyr) && (parsedIyr >= 2010 && parsedIyr <= 2020);

      isValid &= eyr != null && eyr.Length == 4 && int.TryParse(eyr, out var parsedEyr) && (parsedEyr >= 2020 && parsedEyr <= 2030);

      if (hgt != null && hgt.EndsWith("in"))
      {
        isValid &= int.TryParse(hgt.Substring(0, hgt.IndexOf('i')), out var parsedHeight) && parsedHeight >= 59 && parsedHeight <= 76;
      }
      else if (hgt != null && hgt.EndsWith("cm"))
      {
        isValid &= int.TryParse(hgt.Substring(0, hgt.IndexOf('c')), out var parsedHeight) && parsedHeight >= 150 && parsedHeight <= 193;
      }
      else
      {
        return false;
      }

      isValid &= hcl != null && hcl.StartsWith("#") && hcl.Length == 7 && hcl.Substring(1).All(x => char.IsLetterOrDigit(x));

      isValid &= ecl != null && ValidEyeColor.Contains(ecl);

      isValid &= pid != null && pid.Length == 9 && pid.All(x => char.IsDigit(x));

      return isValid;
    }
  }
  #endregion

  #region Day5: Solved!

  public static string Day5()
  {
    var inputs = File.ReadAllLines(Path.Combine(InputBasePath, "Day5.txt")); //new[] { "FBFBBFFRLR", "BFFFBBFRRR", "FFFBBBFRRR", "BBFFBBFRLL" };

    var seatAmount = 128;

    var columnAmount = 8;

    var p1 = 0;

    var seats = new Dictionary<int, int[]>();

    foreach (var input in inputs)
    {
      var test = new Day5Seats(input, seatAmount, columnAmount, ref seats);

      p1 = Math.Max(p1, test.SeatID);
    }

    //For P2, we need to find the SeatID of the Seat that is missing. Both the first and the last rows have seats missing, so disregard those.
    seats.Remove(seats.Keys.Min());
    seats.Remove(seats.Keys.Max());

    //Next, we flatten the list of seat ID's
    var flattenedSeats = seats.SelectMany(x => x.Value);

    //And P2 is the missing seat, aka the first seat not present in the list + 1, with + 1 added to its seat id.
    var p2 = flattenedSeats.First(x => !flattenedSeats.Contains(x + 1)) + 1;

    return OutputResult(p1.ToString(), p2.ToString());
  }

  public class Day5Seats
  {
    //F = front, B = Back, L = Left, R = right

    private int Row { get; set; }

    private int Column { get; set; }

    public int SeatID => (Row * 8) + Column;

    public Day5Seats(string input, int seatAmount, int columns, ref Dictionary<int, int[]> seats)
    {
      Row = PartitionByInput(new Queue<char>(input.Substring(0, 7)), seatAmount);

      Column = PartitionByInput(new Queue<char>(input.Substring(7)), columns);

      if (!seats.ContainsKey(Row))
      {
        seats.Add(Row, new int[columns]);
      }

      seats[Row][Column] = SeatID;
    }

    public int PartitionByInput(Queue<char> input, int limit)
    {
      var min = 0;
      var max = limit - 1;

      while (input.Any())
      {
        switch (input.Dequeue())
        {
          case 'L':
          case 'F':
            {
              max -= Partition(min, max);
            }
            break;
          case 'B':
          case 'R':
            {
              min += Partition(min, max);
            }
            break;
          default:
            {
              throw new Exception("Someone really fucked up.");
            }
        }
      }

      return max;
    }

    private int Partition(int min, int max)
    {
      return (int)Math.Ceiling(((decimal)max - min) / 2);
    }

  }
  #endregion

  #region Day6: Solved!

  public static string Day6()
  {
    var parsedInput = new Day6Groups(new Queue<string>(File.ReadAllLines(Path.Combine(InputBasePath, "Day6.txt"))));

    var p1 = parsedInput.Groups.Sum(x => x.SelectMany(y => y.ToCharArray()).Distinct().Count()); //We want to know the sum of the amount of people that answered yes to any question.

    var p2 = 0; //For p2, we want to know the sum of the amount of answers that were answered as "yes" by everyone in the group.

    foreach (var group in parsedInput.Groups)
    {
      foreach (var answer in group.SelectMany(x => x.ToCharArray()).Distinct().ToList())
      {
        if (group.All(x => x.Contains(answer)))
        {
          p2++;
        }
      }
    }

    return OutputResult(p1.ToString(), p2.ToString());
  }

  public class Day6Groups
  {
    public List<List<string>> Groups { get; set; }

    public Day6Groups(Queue<string> inputs)
    {
      Groups = new List<List<string>> { new List<string>() };

      while (inputs.Any())
      {
        var current = inputs.Dequeue();

        if (string.IsNullOrEmpty(current))
        {
          Groups.Add(new List<string>());
        }
        else
        {
          Groups.Last().Add(current);
        }
      }
    }
  }

  #endregion

  #region Day7: Todo

  public static string Day7()
  {
    var inputs = new[]
    {
      "light red bags contain 1 bright white bag, 2 muted yellow bags.",
      "dark orange bags contain 3 bright white bags, 4 muted yellow bags.",
      "bright white bags contain 1 shiny gold bag.",
      "muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.",
      "shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.",
      "dark olive bags contain 3 faded blue bags, 4 dotted black bags.",
      "vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.",
      "faded blue bags contain no other bags.",
      "dotted black bags contain no other bags."
    }; //We should parse this input in two stages. First, we need all the different colors of bags. We do this by only parsing the part before "bags" and then building a list of all possible bags.

    //var inputs = File.ReadAllLines(Path.Combine(InputBasePath, "Day7.txt")); //Doesn't work yet AARRGGHH

    var bags = new List<Day7Bag>();

    foreach(var input in inputs)
    {
      var bagName = input.Substring(0, input.IndexOf("bags")).Trim();

      bags.Add(new Day7Bag(bagName, 1));
    }

    //Afterwards, we go through the inputs again but this time, we parse the part where we build our tree.

    foreach(var input in inputs.Where(x => !x.Contains("no other bags")))
    {
      var bagName = input.Substring(0, input.IndexOf("bags")).Trim();
      var current = bags.First(x => x.Name == bagName);
      var contents = input.Substring(input.IndexOf("contain") + "contain".Length).Split(',');

      foreach(var content in contents)
      {
        var split = content.Trim().Split(' ');
        var amount = int.Parse(split[0]);
        var qualifier = split[1];
        var color = split[2];
        var bagToMove = bags.First(x => x.Qualifier == qualifier && x.Color == color); //We first look top level, then through the contents

        if(bagToMove != null)
        {
          for(var idx = 0; idx < amount; idx++)
          {
            current.Contents.Add(bagToMove);
          }          
        }
      }
    }

    var p1 = 0; //We need to know what bags contain the shiny gold bag.

    foreach(var bag in bags)
    {
      var localBag = bag;

      while(localBag.Contents.Any(x => !x.Searched))
      {
        localBag = localBag.Contents.FirstOrDefault(x => !x.Searched);

        if(localBag == null)
        {
          break;
        }
        else if(localBag.Name == "shiny gold")
        {
          p1++;
          localBag.Contents.ForEach(b => b.Searched = true);
          break;
        }
      }
    }

    return OutputResult(p1.ToString());
  }

  public class Day7Bag
  {
    public string Qualifier { get; set; }

    public string Color { get; set; }

    public string Name => $"{Qualifier} {Color}";

    public bool Searched { get; set; }

    public List<Day7Bag> Contents { get; private set; }

    public Day7Bag(string name, int amount)
    {
      var split = name.Split(' ');
      Qualifier = split[0];
      Color = split[1];

      Contents = new List<Day7Bag>();
    }
  }

  #endregion
}