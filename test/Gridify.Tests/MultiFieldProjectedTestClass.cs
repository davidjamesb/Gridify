using System;

namespace Gridify.Tests;

public class MultiFieldProjectedTestClass : ICloneable
{
   public MultiFieldProjectedTestClass()
   {
   }

   public MultiFieldProjectedTestClass(int id, string name)
   {
      Id = id;
      Name = name;
   }

   public int Id { get; set; }
   public string? Name { get; set; } = string.Empty;

   public object Clone()
   {
      return new MultiFieldProjectedTestClass
      {
         Id = Id,
         Name = Name
      };
   }
}
