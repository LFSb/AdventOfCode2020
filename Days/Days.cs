using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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

  #region Day3: Todo
  
  public static string Day3()
  {
    var input = File.ReadAllLines(Path.Combine(InputBasePath, "Day3.txt"));

    var steps = new []
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

      for(var y = 0; y < inputs.Length; y++)
      {
        for(var x = 0; x < maxWidth; x++)
        {
          if(Grid[y] == null)
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

      while(CurrentY < Grid.Length - 1)
      {
        CurrentY += ystep;
        CurrentX += xstep;

        if(Grid[CurrentY][CurrentX])
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

      foreach(var input in inputs)
      {
        result *= P1(input.Item2, input.Item1);;
      }

      return result;
    }
  }

  #endregion
}