using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dapper.Linq
{
    public class SqlBuilder
    {
        private class Token
        {
            public TokenType Type { get; }

            public string Text { get; }

            public Token(TokenType type, string text)
            {
                Type = type;
                Text = text;
            }
        }

        private enum TokenType
        {
            From,
            Set,
            Where,
            Join,
            GroupBy,
            Having,
            OrderBy,
            Limit
        }

        private List<Token> _tokens = new List<Token>();

        private string View => (from a in _tokens
                                where a.Type == TokenType.From
                                select a into s
                                select s.Text).FirstOrDefault() ?? string.Empty;

        public DynamicParameters Parameters { get; }

        public SqlBuilder()
        {
            Parameters = new DynamicParameters();
        }

        public SqlBuilder(object parma)
        {
            Parameters = new DynamicParameters(parma);
        }

        public SqlBuilder Where(string sql, bool flag = true)
        {
            if (flag)
            {
                _tokens.Add(new Token(TokenType.Where, sql));
            }

            return this;
        }

        public SqlBuilder Join(string sql, bool flag = true)
        {
            if (flag)
            {
                _tokens.Add(new Token(TokenType.Join, sql));
            }

            return this;
        }

        public SqlBuilder Having(string sql, bool flag = true)
        {
            if (flag)
            {
                _tokens.Add(new Token(TokenType.Having, sql));
            }

            return this;
        }

        public SqlBuilder OrderBy(string sql, bool flag = true)
        {
            if (flag)
            {
                _tokens.Add(new Token(TokenType.OrderBy, sql));
            }

            return this;
        }

        public SqlBuilder GroupBy(string sql, bool flag = true)
        {
            if (flag)
            {
                _tokens.Add(new Token(TokenType.GroupBy, sql));
            }

            return this;
        }

        public SqlBuilder Limit(int row, int size)
        {
            _tokens.Add(new Token(TokenType.Limit, $"{row}, {size}"));
            return this;
        }

        public SqlBuilder Page(int row, int size)
        {
            return Limit((row - 1) * size, size);
        }

        public string Select(string columns = "*")
        {
            List<Token> tokens = _tokens;
            return string.Join(" ", "SELECT " + columns + " FROM " + View, Build(tokens));
        }

        public string Update()
        {
            IEnumerable<Token> tokens = from a in _tokens
                                        where a.Type != TokenType.OrderBy
                                        where a.Type != TokenType.Having
                                        where a.Type != TokenType.GroupBy
                                        where a.Type != TokenType.Limit
                                        select a;
            return string.Join(" ", "UPDATE " + View, Build(tokens));
        }

        public string Delete()
        {
            IEnumerable<Token> tokens = from a in _tokens
                                        where a.Type != TokenType.Set
                                        where a.Type != TokenType.OrderBy
                                        where a.Type != TokenType.Having
                                        where a.Type != TokenType.GroupBy
                                        where a.Type != TokenType.Limit
                                        select a;
            return string.Join(" ", "DELETE FROM " + View, Build(tokens));
        }

        public string Count()
        {
            IEnumerable<Token> tokens = from a in _tokens
                                        where a.Type != TokenType.Set
                                        where a.Type != TokenType.OrderBy
                                        where a.Type != TokenType.Limit
                                        select a;
            if (_tokens.Any((Token a) => a.Type == TokenType.GroupBy))
            {
                string text = string.Join(" ", "SELECT * FROM " + View, Build(tokens));
                return "SELECT COUNT(*) FROM (" + text + ") AS t";
            }

            return string.Join(" ", "SELECT COUNT(*) FROM " + View, Build(tokens));
        }

        public string Build(string format)
        {
            Dictionary<TokenType, string> tokens = GetGroupTokens(_tokens);
            return Regex.Replace(format, "/\\*\\*(?<token>\\w+)\\*\\*/", delegate (Match match)
            {
                string token = match.Groups["token"].Value.ToUpper();
                _ = match.Value;
                TokenType? tokenType = GetTokenType(token);
                return (tokenType.HasValue && tokens.ContainsKey(tokenType.Value)) ? tokens[tokenType.Value] : string.Empty;
            }, RegexOptions.IgnoreCase);
        }

        public SqlBuilder Clone()
        {
            SqlBuilder sqlBuilder = new SqlBuilder();
            foreach (Token token in _tokens)
            {
                sqlBuilder._tokens.Add(token);
            }
            return sqlBuilder;
        }

        private static Dictionary<TokenType, string> GetGroupTokens(IEnumerable<Token> tokens)
        {
            return (from a in tokens
                    where a.Type != TokenType.From
                    group a by a.Type into s
                    orderby s.Key
                    select s).Select(delegate (IGrouping<TokenType, Token> s)
                    {
                        string separator = ", ";
                        if (s.Key == TokenType.Where || s.Key == TokenType.GroupBy)
                        {
                            separator = " AND ";
                        }

                        if (s.Key == TokenType.Join)
                        {
                            separator = " ";
                        }

                        string text = string.Join(separator, s.Select((Token a) => a.Text));
                        string value = string.Empty;
                        switch (s.Key)
                        {
                            case TokenType.Set:
                                value = "SET " + text;
                                break;
                            case TokenType.Where:
                                value = "WHERE " + text;
                                break;
                            case TokenType.Join:
                                value = text ?? "";
                                break;
                            case TokenType.GroupBy:
                                value = "Group By " + text;
                                break;
                            case TokenType.Having:
                                value = "HAVING " + text;
                                break;
                            case TokenType.OrderBy:
                                value = "ORDER BY " + text;
                                break;
                            case TokenType.Limit:
                                value = "LIMIT " + text;
                                break;
                        }

                        return new KeyValuePair<TokenType, string>(s.Key, value);
                    }).ToDictionary((KeyValuePair<TokenType, string> s) => s.Key, (KeyValuePair<TokenType, string> s) => s.Value);
        }

        private static string Build(IEnumerable<Token> tokens)
        {
            return string.Join(" ", from s in GetGroupTokens(tokens)
                                    select s.Value);
        }

        private static TokenType? GetTokenType(string token)
        {
            if (token != null)
            {
                switch (token.Length)
                {
                    case 5:
                        switch (token[0])
                        {
                            case 'W':
                                if (!(token == "WHERE"))
                                {
                                    break;
                                }

                                return TokenType.Where;
                            case 'L':
                                if (!(token == "LIMIT"))
                                {
                                    break;
                                }

                                return TokenType.Limit;
                        }

                        break;
                    case 7:
                        switch (token[0])
                        {
                            case 'G':
                                if (!(token == "GROUPBY"))
                                {
                                    break;
                                }

                                return TokenType.GroupBy;
                            case 'O':
                                if (!(token == "ORDERBY"))
                                {
                                    break;
                                }

                                return TokenType.OrderBy;
                        }

                        break;
                    case 3:
                        if (!(token == "SET"))
                        {
                            break;
                        }

                        return TokenType.Set;
                    case 4:
                        if (!(token == "JOIN"))
                        {
                            break;
                        }

                        return TokenType.Join;
                    case 6:
                        if (!(token == "HAVING"))
                        {
                            break;
                        }

                        return TokenType.Having;
                }
            }

            return null;
        }
    }
}
