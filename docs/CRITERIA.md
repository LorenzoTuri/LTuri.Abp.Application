# Criterias

The package extends the normal repositories, by providing some usedful features
like Criteria and in general dynamic filters.

Criteria (Class LTuri.Abp.Application.Repositories.Criteria.Criteria) are build in different parts:
- CriteriaFilters: rappresents the filters on the entities
- CriteriaPagination: rappresents the pagination
- CriteriaSorting: allows to build different sortings
- CriteriaAggregation: build additional aggregation for the query
- string Query: executes a custom query

## CriteriaFilter

Filters can be nested and can be used to build very complex filters. A single element is composed by:

Keep in mind that, depending on the FilterType, some properties are ignored (see below)

Because of the class construction, Value is always a string. Depending on the filter, the system tries
to cast it to the property type. Some of the filters ([String]) instead cast the property to string, in order to
try matching it in any case.
Also keep in mind that if an operation is not supported by the filter, an Exception is triggered
(ex: Greater filter on Guid property -> Greater and Lower are not implemented on Guid)

- FilterType Type: type of filter (see below), default FilterType.Equals;
- FilterBehaviour Behaviour: behaviour of filter (see below), default FilterBehaviour.None;
- string Field: property on which execute the filter, default "Id";
- string Value: value used for the filter, default "";
- IEnumerable<CriteriaFilter> Filters: list of additional filters

### FilterType

- And: allows multiple queries. Behaviour, Field and Value are ignored
- Or: allows multiple queries. Behaviour, Field and Value are ignored
- Not: allows multiple queries. Behaviour, Field and Value are ignored. Under the hood executes a AND, negated.

- Equals: equals filter. Filters are ignored.
- Contains: [String] contains filter. Filters are ignored.
- StartsWith: [String] starts with filter. Filters are ignored.
- EndsWith: [String] ends with filter. Filters are ignored.
- Greater: greater then filter. Filters are ignored.
- Lower: lower then filter. Filters are ignored.
- GreaterEquals: greater or equals then filter. Filters are ignored.
- LowerEquals: lower or equals then filter. Filters are ignored.
- FullText: Apply Contains on all the properties, using or. Filters are ignored.
- Any: Equals on the value comma separated. Use to build "property is one of" filter. Filters are ignored.

### FilterBehaviour

- None: No behaviour, default
- IgnoreCase: Tries to execute filters on the lowercase version of property and value. 

## CriteriaPagination

Pagination is built on only 2 parameters, the page and the size of the page.
There are no constraint of the size, so in case of large database, it's best to check the parameters provided here
in order to prevent returning too big datasets.

- int Page = 0;
- int Size = 100;

## CriteriaSorting

Sorting in Criteria is provided by a list, because many sortings can be nested.
They are executed in order.

- string By: property used to sort, default "Id"
- SortingDirection Direction: direction of the sorting, default SortingDirection.Desc

Allowed directions are SortingDirection.Asc and SortingDirection.Desc

## CriteriaAggregation

Is it possible to execute Aggregations on the query, to provide useful statistic data.
Aggregations are made on the whole set of results, no on the paged one.
Because of this is useful for computing values like Sum, otherwhis you'll need to get the whole, unpaginated
result set and manually compute the sum on it.

The aggregation is composed like this

- AggregationType Type: type of aggregation to execute, default AggregationType.Avg;
- AggregationBehaviour Behaviour: when to execute it, default AggregationBehaviour.Post;
- string Name: name of the aggregation, used as index in return
- string Field: field on which execute the aggregation

### AggregationType
field
- Avg: computes the average on the specified field
// Number of records for the specified field (where is not null)
- Count: computes the number of record, in which the property is not 
- Max: computes the max value of the property
- Min: computes the min value of the property
- Sum: computes the sum of the values of the property
- Stats: computes stats overall numeric values for the specified field (Avg, Count, Max, Min, Sum)
- Group: groups the results for each value of the provided field, returns a list of list
- Histogram: groups the results for each value of the provided field, by using an histogram aggregation 
  (grouped by minute, hour, day, week, month, quarter, year, day)

### AggregationBehaviour

The behaviour can be of Pre or Post. Using Pre means that the results are computed before the filters are 
applied, Post after the filter application.

## Query

The query is a custom stringified version to query on the results set. A query might be like

(prop1 eq "123") and ((prop1 eq '234') or (prop2 gt "1"))

Where the field is a simple string, and values are encapsulated by '' or "".

Valid concatenators are
and ->      'expression and expression'
or  ->      'expression or expression'
not ->      'not expression'
brackets -> '(expression)'

Valid operators are
eq, ieq             -> FilterType.Equals
contains, icontains -> FilterType.Contains
starts, istarts     -> FilterType.StartsWith
ends, iends         -> FilterType.EndsWith
gt, igt             -> FilterType.Greater
lt, ilt             -> FilterType.Lower
gte, igte           -> FilterType.GreaterEquals
lte, ilte           -> FilterType.LowerEquals
full, ifull         -> FilterType.FullText
any, iany           -> FilterType.Any

Operators starting with 'i' ignores the case. This custom query is then converted to Criteria.