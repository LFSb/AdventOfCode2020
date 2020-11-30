using System;
using System.Diagnostics;

public class Program
{
  static void Main(string[] args)
  {
    var sw = new Stopwatch();
    sw.Start();
    
    Console.WriteLine(Days.Day1());

    System.Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms.");

    sw.Stop();
  }
}