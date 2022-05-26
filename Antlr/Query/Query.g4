grammar Query;
/**
This grammar is used to generate a CriteriaFilter by using a stringed query like
(prop1 eq "123") and ((prop1 eq '234') or (prop2 gt "1"))

Valid concatenators are
and -> FilterType.And
or -> FilterType.Or

Valid operators are
eq -> FilterType.Equals
contains -> FilterType.Contains
starts -> FilterType.StartsWith
ends -> FilterType.EndsWith
gt -> FilterType.Greater
lt -> FilterType.Lower
gte -> FilterType.GreaterEquals
lte -> FilterType.LowerEquals
full -> FilterType.FullText
any -> FilterType.Any

Properties are simple strings, values are '' or "" encapsulated strings
*/

query
    : expression EOF
    ;
 
expression
    : filter                                                    #ExpressionFilter
    | 'not' expression                                          #ExpressionNot
    | '(' expression ')'                                        #ExpressionParenthesis
    | expression1 = expression 'and' expression2 = expression   #ExpressionAnd
    | expression1 = expression 'or' expression2 = expression    #ExpressionOr
    ;
 
filter
    : prop = PROPERTY 'eq' str = STRING          #FilterEq
    | prop = PROPERTY 'ieq' str = STRING         #FilterIEq
    | prop = PROPERTY 'contains' str = STRING    #FilterContains
    | prop = PROPERTY 'icontains' str = STRING   #FilterIContains
    | prop = PROPERTY 'starts' str = STRING      #FilterStarts
    | prop = PROPERTY 'istarts' str = STRING     #FilterIStarts
    | prop = PROPERTY 'ends' str = STRING        #FilterEnds
    | prop = PROPERTY 'iends' str = STRING       #FilterIEnds
    | prop = PROPERTY 'gt' str = STRING          #FilterGt
    | prop = PROPERTY 'igt' str = STRING         #FilterIGt
    | prop = PROPERTY 'lt' str = STRING          #FilterLt
    | prop = PROPERTY 'ilt' str = STRING         #FilterILt
    | prop = PROPERTY 'gte' str = STRING         #FilterGte
    | prop = PROPERTY 'igte' str = STRING        #FilterIGte
    | prop = PROPERTY 'lte' str = STRING         #FilterLte
    | prop = PROPERTY 'ilte' str = STRING        #FilterILte
    | prop = PROPERTY 'full' str = STRING        #FilterFull
    | prop = PROPERTY 'ifull' str = STRING       #FilterIFull
    | prop = PROPERTY 'any' str = STRING         #FilterAny
    | prop = PROPERTY 'iany' str = STRING        #FilterIAny
    ;

STRING 
    : '"' ~('"') * '"'
    | '\'' ~('\'') * '\''
    ;

PROPERTY
    : ('a'..'z' | 'A'..'Z') ('a'..'z' | 'A'..'Z' | '0'..'9' | '_') *
    ;
    
WS : [ \t\r\n] -> skip;