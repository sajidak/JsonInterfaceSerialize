using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JsonInterfaceSerialize.Utilities.Helpers;

namespace JsonInterfaceSerialize.Services
{
    #region Refactor to proper locations

    #region Common Definitions

    /// <summary>
    /// SQL match type, that comes between the column name on the left and the value on the right.
    /// </summary>
    public enum SQL_MATCH_TYPE
    {
        EQUALS = 0,
        LIKE = 1,
        IN = 2,
        IS_NULL = 3,
        GREATER_THAN = 4,
        LESS_THAN = 5,
        NOT_EQUALS = 20,
        NOT_LIKE = 21,
        NOT_IN = 22,
        IS_NOT_NULL = 23,

        // TBD
        // BETWEEN = 50,   // Not sure if this can be done without structure change
    }

    /// <summary>
    /// Columns definitions mirroring table structure, of columns that may be used in custom query.
    /// </summary>
    public class TableDefs
    {
        public static IDictionary<string, IDictionary<string, SqlDbType>> TableDefList = new Dictionary<string, IDictionary<string, SqlDbType>>
            {
                {"user", new Dictionary<string, SqlDbType>
                    {
                        {"user_id"          ,SqlDbType.Int      },
                        {"user_name"        ,SqlDbType.VarChar  },
                        {"email"            ,SqlDbType.VarChar  },
                        {"status"           ,SqlDbType.Bit      },
                        {"last_login"       ,SqlDbType.DateTime },
                        {"updated_by"       ,SqlDbType.VarChar  },
                        {"created_utc"      ,SqlDbType.DateTime },
                        {"last_updated_utc" ,SqlDbType.DateTime },
                        {"is_active"        ,SqlDbType.Bit      },
                    }
                },
            };
    }

    #endregion Common Definitions

    #region Data Models

    public interface ISortColumnDM
    {
        string Name { get; set; }
        string SortDirection { get; set; }
    }
    public class SortColumnDM : ISortColumnDM
    {
        public string Name { get; set; }
        public string SortDirection { get; set; }
    }

    public interface IFilterColumnDM
    {
        string Condition { get; set; }
        string Name { get; set; }
        SQL_MATCH_TYPE MatchType { get; set; }
        string Value { get; set; }
    }
    public class FilterColumnDM : IFilterColumnDM
    {
        public FilterColumnDM()
        {
            Name = string.Empty; Value = string.Empty; Condition = string.Empty; MatchType = SQL_MATCH_TYPE.EQUALS;
        }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Condition { get; set; }
        public SQL_MATCH_TYPE MatchType { get; set; }
    }

    public interface IFlexiQueryRequestDM
    {
        string TableName { get; set; }
        IList<string> DisplayColumns { get; set; }
        IList<IFilterColumnDM> FilterColumns { get; set; }
        IList<ISortColumnDM> SortColumns { get; set; }
        int PageNumber { get; set; }
        int PageSize { get; set; }
    }
    /// <summary>
    /// Container / Wrapper for a custom query
    /// </summary>
    public class FlexiQueryRequestDM : IFlexiQueryRequestDM
    {
        public FlexiQueryRequestDM()
        {
            TableName = string.Empty;
            DisplayColumns = new List<string>();
            FilterColumns = new List<IFilterColumnDM>();
            SortColumns = new List<ISortColumnDM>();
        }
        public string TableName { get; set; }
        public IList<string> DisplayColumns { get; set; }
        public IList<IFilterColumnDM> FilterColumns { get; set; }
        public IList<ISortColumnDM> SortColumns { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public interface IDynamicSqlIngredientsDM
    {
        string CommandText { get; set; }
        IList<DbParameter> Parameters { get; set; }
        string Comments { get; set; }
    }
    public class DynamicSqlIngredientsDM : IDynamicSqlIngredientsDM
    {
        public DynamicSqlIngredientsDM() { CommandText = string.Empty; Parameters = new List<DbParameter>(); }
        // TODO: Add wrapper candy for maintainence
        public string CommandText { get; set; }
        public IList<DbParameter> Parameters { get; set; }
        public string Comments { get; set; }
    }

    #endregion // Data Models

    #region Interface Converters
    /// <summary>
    /// Deserialize a Class, serialized as JSON string, to its Interface.
    /// Enhancement for deserializing to Interfaces from a JSON string.
    /// Overrides the default converter with specific Interface-Class mapping.
    /// </summary>
    /// <typeparam name="I">Type of Interface to deserialize the JSON text as</typeparam>
    /// <typeparam name="C">Type of Class represented by JSON text</typeparam>
    public class InterfaceConverter<I, C> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => (objectType == typeof(I));
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => serializer.Deserialize(reader, typeof(C));
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => serializer.Serialize(writer, value, typeof(C));
    }

    /// <summary>
    /// For deserializing a Class, serialized as JSON string, to its Interface.
    /// Version 1, using Constructor parameters.
    /// Superseded by version using Generic Type.
    /// </summary>
    public class InterfaceClassMap : JsonConverter
    {
        private readonly Type IType;
        private readonly Type CType;

        /// <summary>
        /// Initialize the instance of the Converter/Mapper
        /// </summary>
        /// <param name="typeInterface">Type of Interface to deserialize the JSON text as</param>
        /// <param name="typeClass">Type of Class represented by JSON text</param>
        public InterfaceClassMap(System.Type typeInterface, System.Type typeClass) { IType = typeInterface; CType = typeClass; }
        public override bool CanConvert(Type objectType)
            => (objectType == IType);
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => serializer.Deserialize(reader, CType);
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => serializer.Serialize(writer, value, CType);
    }

    public class JsonHelpers
    {
        /// <summary>
        /// Convert specific class
        /// Superseded by helper class using Generic Type <see cref="static T FromJson<T>(string data)"/>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IFlexiQueryRequestDM FlexiQueryRequestDM_FromJson(string data)
        {
            var js = new JsonSerializerSettings();
            js.Converters.Add(new InterfaceConverter<IFlexiQueryRequestDM, FlexiQueryRequestDM>());
            js.Converters.Add(new InterfaceConverter<IFilterColumnDM, FilterColumnDM>());
            js.Converters.Add(new InterfaceConverter<ISortColumnDM, SortColumnDM>());
            return JsonConvert.DeserializeObject<IFlexiQueryRequestDM>(data, js);
        }

        #region Generic Convertor
        /// <summary>
        /// Convert any Class. Requires mapping for all Classes that may be deserialized.
        /// </summary>
        /// <typeparam name="T">Type of the Interface to Deserialize to.</typeparam>
        /// <param name="data">JSON text, serialized from mapped Class.</param>
        /// <returns></returns>
        public static T FromJson<T>(string data)
        {
            if (JssMap is null) InitMap();
            return JsonConvert.DeserializeObject<T>(data, JssMap);
        }

        #region Support Code
        private static JsonSerializerSettings JssMap = null;
        /// <summary>
        /// Must contain Interface-Class mapping for all Classes that need to De-Serialize
        /// </summary>
        private static void InitMap()
        {
            if (JssMap != null) return; // Shortcircuit to avoid repeats
            try
            {
                JssMap = new JsonSerializerSettings();
                JssMap.Converters.Add(new InterfaceConverter<IFlexiQueryRequestDM, FlexiQueryRequestDM>());
                JssMap.Converters.Add(new InterfaceConverter<IFilterColumnDM, FilterColumnDM>());
                JssMap.Converters.Add(new InterfaceConverter<ISortColumnDM, SortColumnDM>());
                // Add mapping for all Classes that may be serialized.
            }
            catch (Exception)
            {
                JssMap = null;
                throw;
            }
        }
        #endregion // Support Code

        #endregion Generic Convertor
    }

    #endregion // Interface Converters

    #region SQL Utilities

    public class SqlHelpers
    {
        /// <summary>
        /// Remove or transform characters from the value that can pose a risk to SQL Server security.
        /// </summary>
        /// <param name="val">Text to scrub for potential risks.</param>
        /// <returns>SQL safe text</returns>
        public static string ScrubValueSQL(string val, ILogger log = null)
        {
            string lsClean = string.Empty;
            try
            {
                // TODO: Add logic, only indicative now for demo
                lsClean = val
                        .Trim()
                        .Replace("--", "")
                        .Replace("/*", "")
                        .Replace("*/", "")
                        .Replace("@", "")
                        ;
            }
            catch (Exception se)
            {
                if (log is null) throw se; // Should helper throw an exception?
                else log.LogError(
                        ExceptionHelpers.SerializeExceptionTxt(se, $"Failed to scrub SQL value. {val}")
                        );
            }
            return lsClean;
        }

        /// <summary>
        /// Create new parameter. 
        /// Abstracted to avoid dependency on System.Data.SqlClient in Service classes
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <param name="dbtype">DataType of the parameter</param>
        /// <returns>A usable SqlParameter</returns>
        public static SqlParameter NewParameter(string name, string value, SqlDbType dbtype)
        {
            object loVal = dbtype switch
            {
                SqlDbType.SmallInt => short.Parse(ScrubValueSQL(value)),
                SqlDbType.Int => int.Parse(ScrubValueSQL(value)),
                SqlDbType.BigInt => long.Parse(ScrubValueSQL(value)),
                _ => ScrubValueSQL(value),
            };
            return new SqlParameter(name, dbtype) { Value = loVal };
        }

        /// <summary>
        /// Create list of parameters for IN clauses. 
        /// Abstracted to avoid dependency on System.Data.SqlClient in Service classes
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <param name="dbtype">DataType of the parameter</param>
        /// <returns>List of SqlParameter, one for each value.</returns>
        public static List<SqlParameter> NewParameter(string name, string values, SqlDbType dbtype, SQL_MATCH_TYPE match_type)
        {
            List<SqlParameter> lstParms = new List<SqlParameter>();
            // TODO: Add all types used in system
            // https://docs.microsoft.com/en-us/dotnet/api/system.data.sqldbtype?view=netcore-3.1

            switch (match_type)
            {
                case SQL_MATCH_TYPE.IN:
                case SQL_MATCH_TYPE.NOT_IN:
                    // Build the list
                    int liIDX = 1;
                    values.Split(",", StringSplitOptions.None)
                        .ToList()
                        .ForEach(v => lstParms.Add(
                            NewParameter(name + liIDX++.ToString("_00"), v, dbtype))
                        );
                    break;
                case SQL_MATCH_TYPE.EQUALS:
                case SQL_MATCH_TYPE.LIKE:
                case SQL_MATCH_TYPE.GREATER_THAN:
                case SQL_MATCH_TYPE.LESS_THAN:
                case SQL_MATCH_TYPE.NOT_EQUALS:
                case SQL_MATCH_TYPE.NOT_LIKE:
                    lstParms.Add(NewParameter(name, values, dbtype));
                    break;
                case SQL_MATCH_TYPE.IS_NULL:
                case SQL_MATCH_TYPE.IS_NOT_NULL:
                    // Nothing to be done
                    break;
                default:
                    // Throw warning, we dont know what this is
                    break;
            }
            return lstParms;
        }

        /// <summary>
        /// generate list of parameter names for an IN clause
        /// </summary>
        /// <param name="name">Name template</param>
        /// <param name="value">Comma "," delimited set of values.</param>
        /// <returns></returns>
        public static string ParameterNames(string name, string value)
        {
            StringBuilder lsbParms = new StringBuilder();
            string[] lasVals = value.Split(",", StringSplitOptions.RemoveEmptyEntries);
            int liCounter = 1;
            foreach (string vVal in lasVals)
                lsbParms.Append(name + liCounter++.ToString("_00") + ", ");
            if (lsbParms.ToString().Length > 2)
                return lsbParms.ToString()[..^2];
            else return name;
        }

    }

    #endregion //SQL Utilities

    #endregion Refactor to proper locations


    public interface IDynamicSqlSVC : IDisposable
    {
        /// <summary>
        /// Prepares the ingredients needed to build a SQL Command object, excluding the DB Connection.
        /// NOTE: Assumes validation is done on `queryReq`
        /// </summary>
        /// <param name="queryReq"></param>
        /// <param name="_logger"></param>
        /// <returns></returns>
        IDynamicSqlIngredientsDM PrepareDynamicCommand(IFlexiQueryRequestDM queryReq, ILogger _logger);
    }

    public class DynamicSqlSVC : IDynamicSqlSVC
    {
        public DynamicSqlSVC() { }
        public DynamicSqlSVC(ILogger logger) : this() { Log = logger; }
        private ILogger Log { get; set; }

        public IDynamicSqlIngredientsDM PrepareDynamicCommand(IFlexiQueryRequestDM queryReq, ILogger _logger)
        {
            if (Log is null && _logger is null) throw new NullReferenceException("No usable logger availaible");
            ILogger log = (_logger is null) ? Log : _logger;

            IDynamicSqlIngredientsDM DSI = new DynamicSqlIngredientsDM();

            // Basic validation only
            // queryReq  is not null
            if (queryReq is null)
            {
                log.LogError("Request body is missing or is empty.");
                return null;
            }
            // tableName is not null
            if (string.IsNullOrWhiteSpace(queryReq.TableName))
            {
                log.LogError("Table Name is missing or is empty.");
                return null;
            }

            // Working variables
            IDictionary<string, SqlDbType> ldctTableDef;
            StringBuilder lsbSQL = new StringBuilder();
            StringBuilder lsbDisplay = new StringBuilder();
            StringBuilder lsbFilter = new StringBuilder();
            StringBuilder lsbSort = new StringBuilder();
            string lsColName;
            string lsParmName;
            string lsCondition;
            SqlDbType dbType;
            int liOffset;
            int liFetchCount;

            // Begin customization
            if (TableDefs.TableDefList.ContainsKey(queryReq.TableName.ToLowerInvariant()))
            {
                ldctTableDef = TableDefs.TableDefList[queryReq.TableName.ToLowerInvariant()];
            }
            else
            {
                ldctTableDef = null;
                log.LogWarning("Table not in whitelist. Simple select shall be used.");
            }

            // Build Display Columns
            foreach (string vCol in queryReq.DisplayColumns)
            {
                if (ldctTableDef is null) break; // there is no tabledef.
                lsColName = vCol.Trim().ToLowerInvariant();  // Assuming all column names in DB are lower case
                if (!ldctTableDef.ContainsKey(lsColName))
                {
                    log.LogWarning("Unknown column ({0}) asked for table {1} display.", queryReq.TableName);
                    continue;
                }
                lsbDisplay.Append(lsColName + ", ");
            }
            if (string.IsNullOrWhiteSpace(lsbDisplay.ToString())) lsbDisplay.Append("*  ");
            if (lsbDisplay.ToString().Length > 2)
            {
                lsbSQL
                    .Append("SELECT ")
                    .Append(lsbDisplay.ToString()[..^2])
                    .Append(" FROM ")
                    .Append(queryReq.TableName.ToLowerInvariant())
                    ;
            }

            // Build Filter Columns
            foreach (IFilterColumnDM vFC in queryReq.FilterColumns)
            {
                if (ldctTableDef is null) break; // there is no tabledef.
                lsColName = vFC.Name.Trim().ToLowerInvariant();  // Assuming all column names in DB are lower case
                lsParmName = "@PARM_" + lsColName.ToUpperInvariant();

                if (!ldctTableDef.ContainsKey(lsColName))
                {
                    log.LogWarning("Unknown column ({0}) asked for table {1} filter.", lsColName, queryReq.TableName);
                    continue;
                }

                if (
                         string.IsNullOrWhiteSpace(vFC.Condition)
                      || vFC.Condition.Trim().Equals("AND", StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    lsCondition = " AND ";
                }
                else
                {
                    lsCondition = "  OR ";
                }

                dbType = ldctTableDef[lsColName];
                switch (vFC.MatchType)
                {
                    case SQL_MATCH_TYPE.EQUALS:
                        lsbFilter.Append(lsCondition + lsColName + " = " + lsParmName);
                        break;
                    case SQL_MATCH_TYPE.LIKE:
                        lsbFilter.Append(lsCondition + lsColName + " LIKE " + lsParmName);
                        break;
                    case SQL_MATCH_TYPE.IN:
                        // TODO: Build list of Paramater placeholders
                        lsbFilter.Append(lsCondition + lsColName + " IN (" + SqlHelpers.ParameterNames(lsParmName, vFC.Value) + ")");
                        SqlHelpers
                            .NewParameter(lsParmName, vFC.Value, dbType, vFC.MatchType)
                            .ForEach(p => DSI.Parameters.Add(p));
                        lsColName = string.Empty;
                        break;
                    case SQL_MATCH_TYPE.IS_NULL:
                        lsbFilter.Append(lsCondition + lsColName + " IS NULL");
                        lsColName = String.Empty; // No parameter required for this
                        break;
                    case SQL_MATCH_TYPE.GREATER_THAN:
                        lsbFilter.Append(lsCondition + lsColName + " > " + lsParmName);
                        break;
                    case SQL_MATCH_TYPE.LESS_THAN:
                        lsbFilter.Append(lsCondition + lsColName + " < " + lsParmName);
                        break;
                    case SQL_MATCH_TYPE.NOT_EQUALS:
                        lsbFilter.Append(lsCondition + lsColName + " <> " + lsParmName);
                        break;
                    case SQL_MATCH_TYPE.NOT_LIKE:
                        lsbFilter.Append(lsCondition + lsColName + " NOT LIKE " + lsParmName);
                        break;
                    case SQL_MATCH_TYPE.NOT_IN:
                        lsbFilter.Append(lsCondition + lsColName + " NOT IN (" + SqlHelpers.ParameterNames(lsParmName, vFC.Value) + ")");
                        SqlHelpers
                            .NewParameter(lsParmName, vFC.Value, dbType, vFC.MatchType)
                            .ForEach(p => DSI.Parameters.Add(p));
                        lsColName = string.Empty;
                        break;
                    case SQL_MATCH_TYPE.IS_NOT_NULL:
                        lsbFilter.Append(lsCondition + lsColName + " IS NOT NULL");
                        lsColName = String.Empty; // No parameter required for this
                        break;
                    default:
                        lsColName = String.Empty;
                        break;
                }
                if (lsColName != String.Empty)
                    DSI.Parameters.Add(SqlHelpers.NewParameter(lsParmName, vFC.Value, dbType));

            }
            if (lsbFilter.ToString().Length > 5)
            {
                lsbSQL
                    .Append(" WHERE ")
                    .Append(lsbFilter.ToString()[5..])
                    ;
            }

            // Build Sort Columns
            foreach (ISortColumnDM vSC in queryReq.SortColumns)
            {
                if (ldctTableDef is null) break; // there is no tabledef.
                lsColName = vSC.Name.Trim().ToLowerInvariant();  // Assuming all column names in DB are upper case
                if (!ldctTableDef.ContainsKey(lsColName))
                {
                    log.LogWarning("Unknown column ({0}) asked for table {1} sort.", lsColName, queryReq.TableName);
                    continue;
                }
                if (
                       string.IsNullOrWhiteSpace(vSC.SortDirection)
                    || vSC.SortDirection.Trim().Equals("DESC", StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    lsbSort.Append(lsColName + " DESC, ");
                }
                else
                {
                    lsbSort.Append(lsColName + " ASC, ");
                }
            }
            if (lsbSort.ToString().Length > 2)
            {
                lsbSQL
                    .Append(" ORDER BY ")
                    .Append(lsbSort.ToString()[..^2])
                    ;
            }

            // Calculate Paging
            liFetchCount = queryReq.PageSize;
            if (liFetchCount == 0) liFetchCount = 1;
            liOffset = queryReq.PageNumber * liFetchCount;
            lsbSQL
                .Append(" OFFSET ")
                .Append(liOffset)
                .Append(" ROWS")
                .Append(" FETCH NEXT ")
                .Append(liFetchCount)
                .Append(" ROWS ONLY")
                .Append(";");
            ;

            // Test post conditions
            string lsDebug = "";
            log.LogInformation("TABLE:   {0}", queryReq.TableName.ToLowerInvariant());
            lsDebug = lsbDisplay.ToString();
            log.LogInformation("DISPLAY: {0}", lsDebug);
            lsDebug = lsbFilter.ToString();
            log.LogInformation("FILTER:  {0}", lsDebug);
            lsDebug = lsbSort.ToString();
            log.LogInformation("SORT:    {0}", lsDebug);
            lsDebug = liOffset.ToString();
            log.LogInformation("OFFSET:  {0}", lsDebug);
            lsDebug = liFetchCount.ToString();
            log.LogInformation("FETCH:   {0}", lsDebug);
            // SELECT {0} FROM {1} WHERE {2} ORDER BY {3} OFFSET {4} ROWS FETCH NEXT {5} ROWS ONLY;
            log.LogInformation(lsbSQL.ToString());

            // Build Command
            DSI.CommandText = lsbSQL.ToString();

            return DSI;
        }

        public void Dispose() { }

    }
}
