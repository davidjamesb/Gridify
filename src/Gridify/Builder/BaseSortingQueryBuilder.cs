using Gridify.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gridify.Builder;

public abstract class BaseSortingQueryBuilder<TSortingQuery, T>(IGridifyMapper<T>? mapper = null)
{
   protected IGridifyMapper<T>? Mapper = mapper;

   protected abstract TSortingQuery ApplyProjection(TSortingQuery query, IEnumerable<ParsedProjection> projections);

   // TODO: We lose polymorphism here due to forcing result type to be IQueryable
   protected abstract IQueryable<TResult> ApplyProjection<TResult>(TSortingQuery query, IEnumerable<ParsedProjection> projections) where TResult : new();

   protected abstract TSortingQuery ApplySorting(TSortingQuery query, ParsedOrdering ordering);

   protected abstract TSortingQuery ApplyAnotherSorting(TSortingQuery query, ParsedOrdering ordering);

   public TSortingQuery ProcessOrdering(TSortingQuery query, string orderings, bool startWithThenBy)
   {
      var isFirst = !startWithThenBy;
      var orders = orderings.ParseOrderings().ToList();
      Mapper ??= BuildMapper(orders);

      foreach (var order in orders)
      {
         if (!Mapper.HasMap(order.MemberName))
         {
            // skip if there is no mappings available
            if (Mapper.Configuration.IgnoreNotMappedFields)
               continue;

            throw new GridifyMapperException($"Mapping '{order.MemberName}' not found");
         }

         if (isFirst)
         {
            query = ApplySorting(query, order);
            isFirst = false;
         }
         else
         {
            query = ApplyAnotherSorting(query, order);
         }
      }

      return query;
   }

   public TSortingQuery ProcessProjection(TSortingQuery query, string projection)
   {
      var parsed = projection.ParseProjection().ToList();
      Mapper ??= BuildMapper(parsed);

      foreach (var select in parsed)
      {
         if (!Mapper.HasMap(select.MemberName))
         {
            // skip if there is no mappings available
            if (Mapper.Configuration.IgnoreNotMappedFields)
               continue;

            throw new GridifyMapperException($"Mapping '{select.MemberName}' not found");
         }
      }

      return ApplyProjection(query, parsed);
   }

   public IQueryable<TResult> ProcessProjection<TResult>(TSortingQuery query, string projection) where TResult : new()
   {
      var parsed = projection.ParseProjection().ToList();
      Mapper ??= BuildMapper(parsed);

      foreach (var select in parsed)
      {
         if (!Mapper.HasMap(select.MemberName))
         {
            // skip if there is no mappings available
            if (Mapper.Configuration.IgnoreNotMappedFields)
               continue;

            throw new GridifyMapperException($"Mapping '{select.MemberName}' not found");
         }
      }

      return ApplyProjection<TResult>(query, parsed);
   }

   private static GridifyMapper<T> BuildMapper(List<ParsedOrdering> orderings)
   {
      var mapper = new GridifyMapper<T>();
      foreach (var order in orderings)
      {
         try
         {
            mapper.AddMap(order.MemberName);
         }
         catch (Exception)
         {
            if (!mapper.Configuration.IgnoreNotMappedFields)
               throw new GridifyMapperException($"Mapping '{order.MemberName}' not found");
         }
      }

      return mapper;
   }

   private static GridifyMapper<T> BuildMapper(List<ParsedProjection> projections)
   {
      var mapper = new GridifyMapper<T>();
      foreach (var projection in projections)
      {
         try
         {
            mapper.AddMap(projection.MemberName);
         }
         catch (Exception)
         {
            if (!mapper.Configuration.IgnoreNotMappedFields)
               throw new GridifyMapperException($"Mapping '{projection.MemberName}' not found");
         }
      }

      return mapper;
   }
}
