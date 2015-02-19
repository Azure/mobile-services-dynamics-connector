using AutoMapper;
using Microsoft.Data.OData.Query;
using Microsoft.Data.OData.Query.SemanticAst;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData.Query;

namespace Microsoft.Windows.Azure.Service.DynamicsCrm
{
    internal class QueryExpressionBuilder<TTableData,TEntity>
    {
        protected string EntityLogicalName { get; set; }
        protected ODataQueryOptions ODataQueryOptions { get; set; }
        protected Dictionary<String, IMemberAccessor> PropertyMap { get; set; }

        public QueryExpressionBuilder(string entityLogicalName, ODataQueryOptions query)
        {
            EntityLogicalName = entityLogicalName;
            ODataQueryOptions = query;
            var map = Mapper.FindTypeMapFor<TTableData, TEntity>();
            if (map == null) throw new InvalidOperationException(String.Format("Could not find a map from {0} to {1}.", typeof(TTableData), typeof(TEntity)));

            this.PropertyMap = map.GetPropertyMaps().ToDictionary(m => m.SourceMember.Name, m => m.DestinationProperty, StringComparer.OrdinalIgnoreCase);
                               
        }

        public QueryExpression GetQueryExpression()
        {
            QueryExpression crmQuery = new QueryExpression("account");

            var map = Mapper.FindTypeMapFor<TTableData, TEntity>();
            var propertyMaps = map.GetPropertyMaps();

            UpdateCriteriaFromFilter(crmQuery.Criteria, ODataQueryOptions.Filter, propertyMaps);
            UpdateColumnSetFromSelectExpand(crmQuery.ColumnSet, ODataQueryOptions.SelectExpand, propertyMaps);
            UpdatePagingFromSkipAndTop(crmQuery.PageInfo, ODataQueryOptions.Skip, ODataQueryOptions.Top);
            UpdateOrdersFromOrderBy(crmQuery.Orders, ODataQueryOptions.OrderBy, propertyMaps);

            return crmQuery;
        }

        private static void UpdateCriteriaFromFilter(FilterExpression criteria, FilterQueryOption filter, IEnumerable<PropertyMap> propertyMaps)
        {
            if (filter != null)
            {
                if (filter.Context != null)
                {
                    if (filter.Context.ElementType.TypeKind != Microsoft.Data.Edm.EdmTypeKind.Entity)
                    {
                        throw new NotImplementedException(String.Format("Unsupported OData element type kind: {0}", filter.Context.ElementType.TypeKind));
                    }

                    if (filter.Context.ElementClrType != typeof(TTableData))
                    {
                        throw new InvalidOperationException(String.Format("Unexpected OData element type: {0}", filter.Context.ElementType));
                    }
                }

                UpdateCriteriaFromExpression(criteria, filter.FilterClause.Expression, propertyMaps);
            }
        }

        private static void UpdateCriteriaFromExpression(FilterExpression criteria, SingleValueNode expression, IEnumerable<PropertyMap> propertyMaps)
        {
            switch (expression.Kind)
            {
                case QueryNodeKind.BinaryOperator:
                    UpdateCriteriaFromBinaryExpression(criteria, (BinaryOperatorNode)expression, propertyMaps);
                    break;

                case QueryNodeKind.Convert:
                    UpdateCriteriaFromExpression(criteria, ((ConvertNode)expression).Source, propertyMaps);
                    break;

                case QueryNodeKind.SingleValueFunctionCall:
                    UpdateCriteriaFromSingleValueFunctionCall(criteria, (SingleValueFunctionCallNode)expression, propertyMaps);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private static void UpdateCriteriaFromBinaryExpression(FilterExpression criteria, BinaryOperatorNode expression, IEnumerable<PropertyMap> propertyMaps)
        {
            ConditionOperator crmOperator;
            switch (expression.OperatorKind)
            {
                case BinaryOperatorKind.And:
                case BinaryOperatorKind.Or:
                    var childCriteria = new FilterExpression(expression.OperatorKind == BinaryOperatorKind.And ? LogicalOperator.And : LogicalOperator.Or);
                    criteria.AddFilter(childCriteria);
                    UpdateCriteriaFromExpression(childCriteria, expression.Left, propertyMaps);
                    UpdateCriteriaFromExpression(childCriteria, expression.Right, propertyMaps);
                    return;

                case BinaryOperatorKind.Equal:
                    crmOperator = ConditionOperator.Equal;
                    break;

                default:
                    throw new NotImplementedException();
            }

            var right = (ConstantNode)expression.Right;

            criteria.AddCondition(GetAttributeName(expression.Left, propertyMaps), crmOperator, GetValue(expression.Right, propertyMaps));
        }

        private static void UpdateCriteriaFromSingleValueFunctionCall(FilterExpression criteria, SingleValueFunctionCallNode expression, IEnumerable<PropertyMap> propertyMaps)
        {
            QueryNode[] arguments = expression.Arguments.ToArray();

            switch (expression.Name.ToLowerInvariant())
            {
                case "startswith":
                    if (arguments.Length != 2) throw new InvalidOperationException("\'startswith\' expects 2 arguments.");

                    criteria.AddCondition(GetAttributeName(arguments[0], propertyMaps), ConditionOperator.BeginsWith, GetValue(arguments[1], propertyMaps));
                    break;
            }
        }

        private static object GetValue(QueryNode queryNode, IEnumerable<PropertyMap> propertyMaps)
        {
            switch (queryNode.Kind)
            {
                case QueryNodeKind.Constant:
                    return ((ConstantNode)queryNode).Value;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private static string GetAttributeName(QueryNode queryNode, IEnumerable<PropertyMap> propertyMaps)
        {
            switch (queryNode.Kind)
            {
                case QueryNodeKind.Convert:
                    return GetAttributeName(((ConvertNode)queryNode).Source, propertyMaps);

                case QueryNodeKind.SingleValuePropertyAccess:
                    var prop = GetDestinationProperty(((SingleValuePropertyAccessNode)queryNode).Property.Name, propertyMaps);
                    return prop.Name.ToLowerInvariant();

                default:
                    throw new NotImplementedException();
            }
        }

        private static IMemberAccessor GetDestinationProperty(string sourcePropertyName, IEnumerable<PropertyMap> propertyMaps)
        {
            return propertyMaps.First(m => m.SourceMember.Name == sourcePropertyName).DestinationProperty;
        }

        private static void UpdateColumnSetFromSelectExpand(ColumnSet columnSet, SelectExpandQueryOption selectExpand, IEnumerable<PropertyMap> propertyMaps)
        {
            if (selectExpand == null || selectExpand.SelectExpandClause == null || selectExpand.SelectExpandClause.AllSelected)
            {
                foreach (var propMap in propertyMaps)
                {
                    columnSet.AddColumn(propMap.DestinationProperty.Name.ToLowerInvariant());
                }
            }
            else
            {
                foreach (var item in selectExpand.SelectExpandClause.SelectedItems.OfType<PathSelectItem>())
                {
                    var pathItem = item.SelectedPath.OfType<PropertySegment>().Single();
                    var propMap = propertyMaps.First(m => m.SourceMember.Name == pathItem.Property.Name);
                    columnSet.AddColumn(propMap.DestinationProperty.Name.ToLowerInvariant());
                }
            }

            columnSet.Columns.Remove("id");
        }

        private static void UpdatePagingFromSkipAndTop(PagingInfo pagingInfo, SkipQueryOption skip, TopQueryOption top)
        {
            if (top == null)
            {
                pagingInfo.Count = 50;
            }
            else
            {
                pagingInfo.Count = top.Value;
            }

            if (skip == null)
            {
                pagingInfo.PageNumber = 1;
            }
            else
            {
                pagingInfo.PageNumber = skip.Value / pagingInfo.Count + 1;
            }
        }

        private static void UpdateOrdersFromOrderBy(DataCollection<OrderExpression> orders, OrderByQueryOption orderBy, IEnumerable<PropertyMap> propertyMaps)
        {
            if (orderBy != null)
            {
                foreach (var node in orderBy.OrderByNodes.OfType<OrderByPropertyNode>())
                {
                    var propMap = propertyMaps.First(m => m.SourceMember.Name == node.Property.Name);
                    var direction = node.Direction == OrderByDirection.Ascending ? OrderType.Ascending : OrderType.Descending;
                    orders.Add(new OrderExpression(propMap.DestinationProperty.Name.ToLowerInvariant(), direction));
                }
            }
        }
    }
}
