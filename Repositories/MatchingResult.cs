﻿using Volo.Abp.Domain.Entities;

namespace LTuri.Abp.Application.Repositories
{
    public class MatchingResult<TEntity> : List<TEntity>
    {
        public long TotalCount { get; set; }

        public long UnfilteredCount { get; set; }

        public Dictionary<string, object> Aggregations { get; set; } = new Dictionary<string, object>();

        public MatchingResult(
            IEnumerable<TEntity> entities
        ) : base(entities)
        {}
    }
}
