﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Storage
{
    public interface IInstrumentStore<T> : IDisposable where T : class 
    {
        IQueryable<T> Query();
        Instrument Get(Guid id);
        Task<Instrument> UpsertAsync(T entity);
        Task Delete(T entity);
    }
}
