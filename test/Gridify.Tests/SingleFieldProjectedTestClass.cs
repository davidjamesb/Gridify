using System;

namespace Gridify.Tests;

public class SingleFieldProjectedTestClass : ICloneable
{
   public SingleFieldProjectedTestClass()
   {
   }

   public SingleFieldProjectedTestClass(string name)
   {
      Name = name;
   }

   public string? Name { get; set; } = string.Empty;

   public object Clone()
   {
      return new SingleFieldProjectedTestClass
      {
         Name = Name
      };
   }
}
