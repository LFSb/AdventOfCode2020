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

  #region Day2: Todo

  public static string Day2()
  {
    return OutputResult(string.Empty, string.Empty);
  }

  #endregion
}