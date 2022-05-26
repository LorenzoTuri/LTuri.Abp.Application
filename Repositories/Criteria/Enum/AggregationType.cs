namespace LTuri.Abp.Application.Repositories.Criteria.Enum
{
    public enum AggregationType
    {
        /// Normal aggregations, returns value
        // Average of all numeric values for the specified field
        Avg,
        // Number of records for the specified field (where is not null)
        Count,
        // Maximum value for the specified field
        Max,
        // Minimal value for the specified field
        Min,
        // Sum of all numeric values for the specified field
        Sum,
        // Stats overall numeric values for the specified field (Avg, Count, Max, Min, Sum)
        Stats,

        /// Grouping aggregation, returns list
        // Groups the results for each value of the provided field
        Group,
        // Groups the result for each value of the provided field, by using an histogram aggregation
        // (grouped by minute, hour, day, week, month, quarter, year, day)
        Histogram
    }
}
