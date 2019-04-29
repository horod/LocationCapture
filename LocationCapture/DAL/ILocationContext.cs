using LocationCapture.Models;
using System;
using System.Collections.Generic;

namespace LocationCapture.DAL
{
    public interface ILocationContext : IDisposable
    {
        IEnumerable<Location> Locations { get; }

        IEnumerable<LocationSnapshot> LocationSnapshots{ get; }

        TEntity Add<TEntity>(TEntity entityToAdd) where TEntity : class;

        TEntity Remove<TEntity>(TEntity entityToRemove) where TEntity : class;

        void SaveChanges();
    }
}
