namespace LTuri.Abp.Application.Repositories.Criteria.Enum
{
    public enum FilterType
    {
        // Multiple queries, and behaviour
        And,
        // Multiple queries, or behaviour
        Or,
        // Multiple queries, negate behaviour. 
        Not,
        // Equals, by comparing string value
        Equals,
        // Contains, by comparing string value (string1 is substring of string2)
        Contains,
        // StartsWith, by comparing string value
        StartsWith,
        // EndsWith, by comparing string value
        EndsWith,
        // Greater comparisong, by comparing strings
        Greater,
        // Lower comparisong, by comparing strings
        Lower,
        // Greater equals comparisong, by comparing strings
        GreaterEquals,
        // Lower equals comparisong, by comparing strings
        LowerEquals,
        // Fulltext comparisong, use contains on all supported properties
        FullText,
        // Value is one of (value must a comma separated string)
        Any
    }
}
