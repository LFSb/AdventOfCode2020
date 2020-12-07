using System;
using System.Diagnostics;

public class Program
{
  static void Main(string[] args)
  {
    var sw = new Stopwatch();
    sw.Start();
    
    Console.WriteLine(Days.Day6());

    System.Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms.");

    sw.Stop();
  }
}