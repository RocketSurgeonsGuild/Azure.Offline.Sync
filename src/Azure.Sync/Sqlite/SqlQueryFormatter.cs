using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Query;
using Newtonsoft.Json.Linq;

namespace Rocket.Surgery.Azure.Sync.SQLite
{
    public class SqlQueryFormatter : QueryNodeVisitor<QueryNode>
    {
        private readonly MobileServiceTableQueryDescription _query;
        private StringBuilder _sql;

        public IDictionary<string, object> Parameters { get; private set; }

        public SqlQueryFormatter(MobileServiceTableQueryDescription query)
        {
            _query = query;
        }

        public string FormatDelete()
        {
            var delQuery = _query.Clone(); // create a copy to avoid modifying the original

            delQuery.Selection.Clear();
            delQuery.Selection.Add(MobileServiceSystemColumns.Id);
            delQuery.IncludeTotalCount = false;

            var formatter = new SqlQueryFormatter(delQuery);
            string selectIdQuery = formatter.FormatSelect();
            string idMemberName = SqlHelpers.FormatMember(MobileServiceSystemColumns.Id);
            string tableName = SqlHelpers.FormatTableName(delQuery.TableName);
            string command = $"DELETE FROM {tableName} WHERE {idMemberName} IN ({selectIdQuery})";
            Parameters = formatter.Parameters;

            return command;
        }

        public string FormatSelect()
        {
            var command = new StringBuilder("SELECT ");

            if (_query.Selection.Any())
            {
                string columnNames = string.Join(", ", _query.Selection.Select(c => SqlHelpers.FormatMember(c)));
                command.Append(columnNames);
            }
            else
            {
                command.Append("*");
            }

            return FormatQuery(command.ToString());
        }

        public string FormatSelectCount()
        {
            Reset();

            if (_query.IncludeTotalCount)
            {
                FormatCountQuery();
            }

            return GetSql();
        }

        public override QueryNode Visit(BinaryOperatorNode nodeIn)
        {
            _sql.Append("(");

            QueryNode left = nodeIn.LeftOperand;
            QueryNode right = nodeIn.RightOperand;

            if (left != null)
            {
                // modulo requires the dividend to be an integer, monetary or numeric
                // rewrite the expression to convert to numeric, allowing the DB to apply
                // rounding if needed. our default data type for number is float which
                // is incompatible with modulo.
                if (nodeIn.OperatorKind == BinaryOperatorKind.Modulo)
                {
                    left = new ConvertNode(left, typeof(int));
                }

                left = left.Accept(this);
            }

            var rightConstant = right as ConstantNode;
            if (rightConstant != null && rightConstant.Value == null)
            {
                // inequality expressions against a null literal have a special
                // translation in SQL
                if (nodeIn.OperatorKind == BinaryOperatorKind.Equal)
                {
                    _sql.Append(" IS NULL");
                }
                else if (nodeIn.OperatorKind == BinaryOperatorKind.NotEqual)
                {
                    _sql.Append(" IS NOT NULL");
                }
            }
            else
            {
                switch (nodeIn.OperatorKind)
                {
                    case BinaryOperatorKind.Equal:
                        _sql.Append(" = ");
                        break;

                    case BinaryOperatorKind.NotEqual:
                        _sql.Append(" != ");
                        break;

                    case BinaryOperatorKind.LessThan:
                        _sql.Append(" < ");
                        break;

                    case BinaryOperatorKind.LessThanOrEqual:
                        _sql.Append(" <= ");
                        break;

                    case BinaryOperatorKind.GreaterThan:
                        _sql.Append(" > ");
                        break;

                    case BinaryOperatorKind.GreaterThanOrEqual:
                        _sql.Append(" >= ");
                        break;

                    case BinaryOperatorKind.And:
                        _sql.Append(" AND ");
                        break;

                    case BinaryOperatorKind.Or:
                        _sql.Append(" OR ");
                        break;

                    case BinaryOperatorKind.Add:
                        _sql.Append(" + ");
                        break;

                    case BinaryOperatorKind.Subtract:
                        _sql.Append(" - ");
                        break;

                    case BinaryOperatorKind.Multiply:
                        _sql.Append(" * ");
                        break;

                    case BinaryOperatorKind.Divide:
                        _sql.Append(" / ");
                        break;

                    case BinaryOperatorKind.Modulo:
                        _sql.Append(" % ");
                        break;
                }

                if (right != null)
                {
                    right = right.Accept(this);
                }
            }

            _sql.Append(")");

            if (left != nodeIn.LeftOperand || right != nodeIn.RightOperand)
            {
                return new BinaryOperatorNode(nodeIn.OperatorKind, left, right);
            }

            return nodeIn;
        }

        public override QueryNode Visit(ConstantNode nodeIn)
        {
            if (nodeIn.Value == null)
            {
                _sql.Append("NULL");
            }
            else
            {
                _sql.Append(CreateParameter(nodeIn.Value));
            }

            return nodeIn;
        }

        public override QueryNode Visit(MemberAccessNode nodeIn)
        {
            string memberName = SqlHelpers.FormatMember(nodeIn.MemberName);
            _sql.Append(memberName);

            return nodeIn;
        }

        public override QueryNode Visit(FunctionCallNode nodeIn)
        {
            switch (nodeIn.Name)
            {
                case "day":
                    return FormatDateFunction("%d", nodeIn);

                case "month":
                    return FormatDateFunction("%m", nodeIn);

                case "year":
                    return FormatDateFunction("%Y", nodeIn);

                case "hour":
                    return FormatDateFunction("%H", nodeIn);

                case "minute":
                    return FormatDateFunction("%M", nodeIn);

                case "second":
                    return FormatDateFunction("%S", nodeIn);

                case "floor":
                    return FormatFloorFunction(nodeIn);

                case "ceiling":
                    return FormatCeilingFunction(nodeIn);

                case "round":
                    return FormatRoundFunction(nodeIn);

                case "tolower":
                    return FormatStringFunction("LOWER", nodeIn);

                case "toupper":
                    return FormatStringFunction("UPPER", nodeIn);

                case "length":
                    return FormatStringFunction("LENGTH", nodeIn);

                case "trim":
                    return FormatStringFunction("TRIM", nodeIn);

                case "substringof":
                    return FormatLikeFunction(true, nodeIn, 0, 1, true);

                case "startswith":
                    return FormatLikeFunction(false, nodeIn, 1, 0, true);

                case "endswith":
                    return FormatLikeFunction(true, nodeIn, 1, 0, false);

                case "concat":
                    return FormatConcatFunction(nodeIn);

                case "indexof":
                    return FormatIndexOfFunction(nodeIn);

                case "replace":
                    return FormatStringFunction("REPLACE", nodeIn);

                case "substring":
                    return FormatSubstringFunction(nodeIn);
            }

            throw new NotImplementedException();
        }

        public override QueryNode Visit(UnaryOperatorNode nodeIn)
        {
            if (nodeIn.OperatorKind == UnaryOperatorKind.Negate)
            {
                _sql.Append("-(");
            }
            else if (nodeIn.OperatorKind == UnaryOperatorKind.Not)
            {
                _sql.Append("NOT(");
            }
            QueryNode operand = nodeIn.Operand.Accept(this);
            _sql.Append(")");

            if (operand != nodeIn.Operand)
            {
                return new UnaryOperatorNode(nodeIn.OperatorKind, operand);
            }

            return nodeIn;
        }

        public override QueryNode Visit(ConvertNode nodeIn)
        {
            _sql.Append("CAST(");

            QueryNode source = nodeIn.Source.Accept(this);

            _sql.Append(" AS ");

            string sqlType = SqlHelpers.GetStoreCastType(nodeIn.TargetType);
            _sql.Append(sqlType);

            _sql.Append(")");

            if (source != nodeIn.Source)
            {
                return new ConvertNode(source, nodeIn.TargetType);
            }

            return nodeIn;
        }

        private string CreateParameter(object value)
        {
            int paramNumber = Parameters.Count + 1;
            string paramName = "@p" + paramNumber;
            Parameters.Add(paramName, SqlHelpers.SerializeValue(new JValue(value), allowNull: true));
            return paramName;
        }

        private QueryNode FormatCeilingFunction(FunctionCallNode nodeIn)
        {
            // floor(x) + (x == floor(x) ? 0 : 1)
            FormatFloorFunction(nodeIn);
            _sql.Append(" + (CASE WHEN ");
            nodeIn.Arguments[0].Accept(this);
            _sql.Append(" = ");
            FormatFloorFunction(nodeIn);
            _sql.Append(" THEN 0 ELSE 1 END)");

            return nodeIn;
        }

        private QueryNode FormatConcatFunction(FunctionCallNode nodeIn)
        {
            string separator = string.Empty;
            foreach (QueryNode arg in nodeIn.Arguments)
            {
                _sql.Append(separator);
                arg.Accept(this);
                separator = " || ";
            }
            return nodeIn;
        }

        private void FormatCountQuery()
        {
            string tableName = SqlHelpers.FormatTableName(_query.TableName);
            _sql.AppendFormat("SELECT COUNT(1) AS [count] FROM {0}", tableName);

            if (_query.Filter != null)
            {
                FormatWhereClause(_query.Filter);
            }
        }

        private QueryNode FormatDateFunction(string formatSting, FunctionCallNode nodeIn)
        {
            // CAST(strftime('%d', datetime([__createdAt], 'unixepoch')) AS INTEGER)
            _sql.AppendFormat("CAST(strftime('{0}', datetime(", formatSting);
            nodeIn.Arguments[0].Accept(this);
            _sql.Append(", 'unixepoch')) AS INTEGER)");

            return nodeIn;
        }

        private QueryNode FormatFloorFunction(FunctionCallNode nodeIn)
        {
            // CASE WHEN x >= 0 THEN CAST(id AS INTEGER) // for +ve values cast to integer to drop the decimal places
            //      WHEN CAST(id AS INTEGER) = id THEN id // for integers just return them as they are
            //      ELSE CAST(id - 1.0 AS INTEGER) // for -ve values cast to integer rounds up close to zero
            // END

            var whenXisPositive = new BinaryOperatorNode(BinaryOperatorKind.GreaterThanOrEqual, nodeIn.Arguments[0], new ConstantNode(0));
            var castToInt = new ConvertNode(nodeIn.Arguments[0], typeof(int));
            var whenXIsInteger = new BinaryOperatorNode(BinaryOperatorKind.Equal, castToInt, nodeIn.Arguments[0]);
            var subtractOne = new BinaryOperatorNode(BinaryOperatorKind.Subtract, nodeIn.Arguments[0], new ConstantNode(1));
            var subtractOneThenCast = new ConvertNode(subtractOne, typeof(int));

            _sql.Append("(CASE WHEN ");
            whenXisPositive.Accept(this);
            _sql.Append(" THEN ");
            castToInt.Accept(this);
            _sql.Append(" WHEN ");
            whenXIsInteger.Accept(this);
            _sql.Append(" THEN ");
            nodeIn.Arguments[0].Accept(this);
            _sql.Append(" ELSE ");
            subtractOneThenCast.Accept(this);
            _sql.Append(" END)");
            return nodeIn;
        }

        private QueryNode FormatIndexOfFunction(FunctionCallNode nodeIn)
        {
            QueryNode result = FormatStringFunction("INSTR", nodeIn);
            _sql.Append(" - 1");
            return result;
        }

        private QueryNode FormatLikeFunction(bool startAny, FunctionCallNode nodeIn, int patternIndex, int valueIndex, bool endAny)
        {
            // like('%pattern%', value)
            _sql.Append("LIKE(");
            if (startAny)
            {
                _sql.Append("'%' || ");
            }
            nodeIn.Arguments[patternIndex].Accept(this);
            if (endAny)
            {
                _sql.Append(" || '%'");
            }
            _sql.Append(", ");
            nodeIn.Arguments[valueIndex].Accept(this);
            _sql.Append(")");

            return nodeIn;
        }

        private void FormatLimitClause(int? limit, int? offset)
        {
            _sql.AppendFormat(" LIMIT {0}", limit.GetValueOrDefault(int.MaxValue));

            if (offset.HasValue)
            {
                _sql.AppendFormat(" OFFSET {0}", offset.Value);
            }
        }

        private void FormatOrderByClause(IList<OrderByNode> orderings)
        {
            _sql.Append(" ORDER BY ");
            string separator = string.Empty;

            foreach (OrderByNode node in orderings)
            {
                _sql.Append(separator);
                node.Expression.Accept(this);
                if (node.Direction == OrderByDirection.Descending)
                {
                    _sql.Append(" DESC");
                }
                separator = ", ";
            }
        }

        private string FormatQuery(string command)
        {
            Reset();

            _sql.Append(command);

            string tableName = SqlHelpers.FormatTableName(_query.TableName);
            _sql.AppendFormat(" FROM {0}", tableName);

            if (_query.Filter != null)
            {
                FormatWhereClause(_query.Filter);
            }

            if (_query.Ordering.Count > 0)
            {
                FormatOrderByClause(_query.Ordering);
            }

            if (_query.Skip.HasValue || _query.Top.HasValue)
            {
                FormatLimitClause(_query.Top, _query.Skip);
            }

            return GetSql();
        }

        private QueryNode FormatRoundFunction(FunctionCallNode nodeIn)
        {
            _sql.Append("ROUND(");

            nodeIn.Arguments[0].Accept(this);

            _sql.Append(", 0)");

            return nodeIn;
        }

        private QueryNode FormatStringFunction(string fn, FunctionCallNode nodeIn)
        {
            _sql.AppendFormat("{0}(", fn);
            string separator = string.Empty;
            foreach (QueryNode arg in nodeIn.Arguments)
            {
                _sql.Append(separator);
                arg.Accept(this);
                separator = ", ";
            }
            _sql.Append(")");

            return nodeIn;
        }

        private QueryNode FormatSubstringFunction(FunctionCallNode nodeIn)
        {
            _sql.Append("SUBSTR(");
            nodeIn.Arguments[0].Accept(this);
            if (nodeIn.Arguments.Count > 1)
            {
                _sql.Append(", ");
                nodeIn.Arguments[1].Accept(this);
                _sql.Append(" + 1");  // need to add 1 since SQL is 1 based, but OData is zero based

                if (nodeIn.Arguments.Count > 2)
                {
                    _sql.Append(", ");
                    nodeIn.Arguments[2].Accept(this);
                }
            }
            _sql.Append(")");
            return nodeIn;
        }

        private void FormatWhereClause(QueryNode expression)
        {
            _sql.Append(" WHERE ");
            expression.Accept(this);
        }

        private string GetSql()
        {
            return _sql.ToString().TrimEnd();
        }

        private void Reset()
        {
            _sql = new StringBuilder();
            Parameters = new Dictionary<string, object>();
        }
    }
}