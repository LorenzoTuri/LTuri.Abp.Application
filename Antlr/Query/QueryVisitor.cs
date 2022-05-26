using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using LTuri.Abp.Application.Repositories.Criteria;

namespace LTuri.Abp.Application.Antlr.Query
{

    public class QueryVisitor : QueryBaseVisitor<IQueryPart>
    {

        public Criteria BuildCriteria(string queryString)
        {
            var criteria = new Criteria();

            var inputStream = new AntlrInputStream(queryString);
            var lexer = new QueryLexer(inputStream);
            
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new QueryParser(tokenStream);

            parser.RemoveErrorListeners();
            parser.AddErrorListener(new ThrowingErrorListener());

            var query = parser.query();
            var queryPart = (QueryPartQuery)Visit(query);

            if (queryPart.Expression != null)
            {
                var filters = new List<CriteriaFilter>();
                var criteriaFilter = queryPart.Expression.ToCriteriaFilter();
                filters.Add(criteriaFilter);
                criteria.Filters = filters;
            }

            return criteria;
        }

        public override IQueryPart VisitQuery([NotNull] QueryParser.QueryContext context)
        {
            return new QueryPartQuery()
            {
                Expression = (QueryPartExpression)Visit(context.expression())
            };
        }

        public override IQueryPart VisitExpressionFilter([NotNull] QueryParser.ExpressionFilterContext context) => 
            new QueryPartExpression() {
                Type = QueryPartExpressionType.Filter,
                Filter = (QueryPartFilter)Visit(context.filter())
            };
        public override IQueryPart VisitExpressionNot([NotNull] QueryParser.ExpressionNotContext context) =>
            new QueryPartExpression()
            {
                Type = QueryPartExpressionType.Not,
                Expressions = new List<QueryPartExpression>() {
                    (QueryPartExpression)Visit(context.expression())
                }
            };
        public override IQueryPart VisitExpressionParenthesis([NotNull] QueryParser.ExpressionParenthesisContext context) =>
            new QueryPartExpression()
            {
                Type = QueryPartExpressionType.Parentheses,
                Expressions = new List<QueryPartExpression>() {
                    (QueryPartExpression)Visit(context.expression())
                }
            };
        public override IQueryPart VisitExpressionAnd([NotNull] QueryParser.ExpressionAndContext context) =>
            new QueryPartExpression()
            {
                Type = QueryPartExpressionType.And,
                Expressions = new List<QueryPartExpression>() {
                    (QueryPartExpression)Visit(context.expression1),
                    (QueryPartExpression)Visit(context.expression2)
                }
            };
        public override IQueryPart VisitExpressionOr([NotNull] QueryParser.ExpressionOrContext context) =>
            new QueryPartExpression()
            {
                Type = QueryPartExpressionType.Or,
                Expressions = new List<QueryPartExpression>() {
                    (QueryPartExpression)Visit(context.expression1),
                    (QueryPartExpression)Visit(context.expression2)
                }
            };

        protected IQueryPart VisitFilterBase(
            QueryPartFilterType type,
            IToken prop,
            IToken str
        )
        {
            return new QueryPartFilter()
            {
                Type = type,
                Property = prop.Text, 
                Value = str.Text,
            };
        }

        public override IQueryPart VisitFilterEq([NotNull] QueryParser.FilterEqContext context) => 
            VisitFilterBase(QueryPartFilterType.Eq, context.prop, context.str);
        public override IQueryPart VisitFilterIEq([NotNull] QueryParser.FilterIEqContext context) =>
            VisitFilterBase(QueryPartFilterType.IEq, context.prop, context.str);
        public override IQueryPart VisitFilterContains([NotNull] QueryParser.FilterContainsContext context) =>
            VisitFilterBase(QueryPartFilterType.Contains, context.prop, context.str);
        public override IQueryPart VisitFilterIContains([NotNull] QueryParser.FilterIContainsContext context) =>
            VisitFilterBase(QueryPartFilterType.IContains, context.prop, context.str);
        public override IQueryPart VisitFilterStarts([NotNull] QueryParser.FilterStartsContext context) =>
            VisitFilterBase(QueryPartFilterType.Starts, context.prop, context.str);
        public override IQueryPart VisitFilterIStarts([NotNull] QueryParser.FilterIStartsContext context) =>
            VisitFilterBase(QueryPartFilterType.IStarts, context.prop, context.str);
        public override IQueryPart VisitFilterEnds([NotNull] QueryParser.FilterEndsContext context) =>
            VisitFilterBase(QueryPartFilterType.Ends, context.prop, context.str);
        public override IQueryPart VisitFilterIEnds([NotNull] QueryParser.FilterIEndsContext context) =>
            VisitFilterBase(QueryPartFilterType.IEnds, context.prop, context.str);
        public override IQueryPart VisitFilterGt([NotNull] QueryParser.FilterGtContext context) =>
            VisitFilterBase(QueryPartFilterType.Gt, context.prop, context.str);
        public override IQueryPart VisitFilterIGt([NotNull] QueryParser.FilterIGtContext context) =>
            VisitFilterBase(QueryPartFilterType.IGt, context.prop, context.str);
        public override IQueryPart VisitFilterLt([NotNull] QueryParser.FilterLtContext context) =>
            VisitFilterBase(QueryPartFilterType.Lt, context.prop, context.str);
        public override IQueryPart VisitFilterILt([NotNull] QueryParser.FilterILtContext context) =>
            VisitFilterBase(QueryPartFilterType.ILt, context.prop, context.str);
        public override IQueryPart VisitFilterGte([NotNull] QueryParser.FilterGteContext context) =>
            VisitFilterBase(QueryPartFilterType.Gte, context.prop, context.str);
        public override IQueryPart VisitFilterIGte([NotNull] QueryParser.FilterIGteContext context) =>
            VisitFilterBase(QueryPartFilterType.IGte, context.prop, context.str);
        public override IQueryPart VisitFilterLte([NotNull] QueryParser.FilterLteContext context) =>
            VisitFilterBase(QueryPartFilterType.Lte, context.prop, context.str);
        public override IQueryPart VisitFilterILte([NotNull] QueryParser.FilterILteContext context) =>
            VisitFilterBase(QueryPartFilterType.ILte, context.prop, context.str);
        public override IQueryPart VisitFilterFull([NotNull] QueryParser.FilterFullContext context) =>
            VisitFilterBase(QueryPartFilterType.Full, context.prop, context.str);
        public override IQueryPart VisitFilterIFull([NotNull] QueryParser.FilterIFullContext context) =>
            VisitFilterBase(QueryPartFilterType.IFull, context.prop, context.str);
        public override IQueryPart VisitFilterAny([NotNull] QueryParser.FilterAnyContext context) =>
            VisitFilterBase(QueryPartFilterType.Any, context.prop, context.str);
        public override IQueryPart VisitFilterIAny([NotNull] QueryParser.FilterIAnyContext context) =>
            VisitFilterBase(QueryPartFilterType.IAny, context.prop, context.str);
    }
}
