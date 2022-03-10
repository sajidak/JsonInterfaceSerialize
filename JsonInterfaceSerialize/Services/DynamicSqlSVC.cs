using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
    }

    public class TableDefs
    {
        /// <summary>
        /// Whitelist of valid columns for TABLE NewReg
        /// </summary>
        public static IDictionary<string, SqlDbType> TableDef_NewReg = new Dictionary<string, SqlDbType>
            {
                {"ID"          ,SqlDbType.BigInt   },
                {"PHR_ID"      ,SqlDbType.BigInt   },
                {"PHR_TEXT"    ,SqlDbType.NVarChar },
                {"PHR_CODE"    ,SqlDbType.NVarChar },
                {"MATCH_NAME"  ,SqlDbType.NVarChar },
                {"LANG"        ,SqlDbType.NVarChar },
                {"SOURCE"      ,SqlDbType.NVarChar },
            };

        /// <summary>
        /// Whitelist of valid columns for TABLE [dbo].[user]
        /// </summary>
        public static IDictionary<string, SqlDbType> TableDef_Users = new Dictionary<string, SqlDbType>
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
            DisplayColumns = new List<string>();
            FilterColumns = new List<IFilterColumnDM>();
            SortColumns = new List<ISortColumnDM>();
        }
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
                if (log != null) log.LogError(
                    ExceptionHelpers.SerializeExceptionTxt(se, $"Failed to scrub SQL value.")
                    );
                else throw se; // Should helper throw an exception?
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
            return new SqlParameter("@" + name, dbtype) { Value = SqlHelpers.ScrubValueSQL(value) };
        }
    }

    #endregion //SQL Utilities

    #endregion Refactor to proper locations


    public interface IDynamicSqlSVC : IDisposable
    {
        // SqlCommand CreateDynamicCommand(IFlexiQueryRequestDM queryReq, string tableName, IDictionary<string, SqlDbType> tableDef, ILogger _logger);
        IDynamicSqlIngredientsDM PrepareDynamicCommand(IFlexiQueryRequestDM queryReq, string tableName, IDictionary<string, SqlDbType> tableDef, ILogger _logger);
    }

    public class DynamicSqlSVC : IDynamicSqlSVC
    {
        public DynamicSqlSVC() { }
        public DynamicSqlSVC(ILogger logger) : this() { Log = logger; }
        private ILogger Log { get; set; }

        /*
        // Superseded by  PrepareDynamicCommand
        public SqlCommand CreateDynamicCommand(IFlexiQueryRequestDM queryReq, string tableName, IDictionary<string, SqlDbType> tableDef, ILogger _logger)
        {
            if (Log is null && _logger is null) throw new NullReferenceException("No usable logger availaible");
            ILogger log = (_logger is null) ? Log : _logger;

            // TODO: Basic validation
            // queryReq  is not null
            // tableName is not null
            // tableDef  is not null

            // Working variables
            string lsColName = "";
            string lsCondition = "";
            string lsTypeQualifier = "";
            SqlDbType dbType = SqlDbType.Text;
            int liOffset = 0;
            int liFetchCount = 0;

            StringBuilder lsbDisplay = new StringBuilder();
            StringBuilder lsbFilter = new StringBuilder();
            StringBuilder lsbSort = new StringBuilder();

            SqlCommand loDBCmd = new SqlCommand();

            // Build Display Columns
            foreach (string vCol in queryReq.DisplayColumns)
            {
                lsColName = vCol.Trim().ToUpper();  // Use whatever standard is used in project
                if (tableDef.ContainsKey(lsColName))
                {
                    lsbDisplay.Append(lsColName + ", ");
                }
                else
                {
                    log.LogWarning("Unknown column ({0}) asked for table NEWREG display.", lsColName);
                }
            }

            // Build Filter Columns
            System.Data.Common.DbParameter loParm;
            foreach (IFilterColumnDM vFC in queryReq.FilterColumns)
            {
                lsColName = vFC.Name.Trim().ToUpper();  // Assuming all column names in DB are upper case
                if (tableDef.ContainsKey(lsColName))
                {
                    dbType = tableDef[lsColName];
                    switch (dbType)
                    {
                        // Text
                        case SqlDbType.Char:
                        case SqlDbType.NChar:
                        case SqlDbType.NText:
                        case SqlDbType.NVarChar:
                        case SqlDbType.Text:
                        case SqlDbType.VarChar:
                            lsTypeQualifier = "'";
                            break;

                        // Numeric
                        case SqlDbType.BigInt:
                        case SqlDbType.Bit:
                        case SqlDbType.Decimal:
                        case SqlDbType.Float:
                        case SqlDbType.Int:
                        case SqlDbType.Money:
                        case SqlDbType.SmallInt:
                        case SqlDbType.SmallMoney:
                        case SqlDbType.TinyInt:
                            lsTypeQualifier = "";
                            break;

                        // Log Error - Not implemented
                        case SqlDbType.DateTime:
                        case SqlDbType.SmallDateTime:
                        case SqlDbType.Timestamp:
                        case SqlDbType.Date:
                        case SqlDbType.Time:
                        case SqlDbType.DateTime2:
                        case SqlDbType.DateTimeOffset:
                            log.LogError("Not yet implemented");
                            break;

                        // Log Error
                        case SqlDbType.Image:
                        case SqlDbType.Real:
                        case SqlDbType.UniqueIdentifier:
                        case SqlDbType.VarBinary:
                        case SqlDbType.Variant:
                        case SqlDbType.Xml:
                        case SqlDbType.Udt:
                        case SqlDbType.Structured:
                            log.LogError("Not Supported.");
                            break;

                        default:
                            break;
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

                    switch (vFC.MatchType)
                    {
                        case SQL_MATCH_TYPE.EQUALS:
                            lsbFilter.Append(lsCondition + lsColName + " = " + lsTypeQualifier + "@" + lsColName + lsTypeQualifier);
                            break;
                        case SQL_MATCH_TYPE.LIKE:
                            lsbFilter.Append(lsCondition + lsColName + " LIKE " + lsTypeQualifier + "@" + lsColName + lsTypeQualifier);
                            break;
                        case SQL_MATCH_TYPE.IN:
                            lsbFilter.Append(lsCondition + lsColName + " IN ( @" + lsColName + ")");
                            break;
                        default:
                            break;
                    }
                    // Synchronize parameters
                    loDBCmd.Parameters.Add(new SqlParameter("@" + lsColName, dbType) { Value = SqlHelpers.ScrubValueSQL(vFC.Value) });
                    loParm = new SqlParameter("@" + lsColName, dbType) { Value = SqlHelpers.ScrubValueSQL(vFC.Value) };
                }
                else
                {
                    log.LogWarning("Unknown column ({0}) asked for table NEWREG filter.", lsColName);
                }

            }

            // Build Sort Columns
            foreach (ISortColumnDM vSC in queryReq.SortColumns)
            {
                lsColName = vSC.Name.Trim().ToUpper();  // Assuming all column names in DB are upper case
                if (tableDef.ContainsKey(lsColName))
                {
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
                else
                {
                    log.LogWarning("Unknown column ({0}) asked for table NEWREG sort.", lsColName);
                }
            }

            // Calculate Paging
            liFetchCount = queryReq.PageSize;
            if (liFetchCount == 0) liFetchCount = 1;
            liOffset = queryReq.PageSize * liFetchCount;

            // Test post conditions
            string lsDebug = "";
            log.LogInformation(tableName.ToUpper());
            lsDebug = lsbDisplay.ToString()[..^2];
            log.LogInformation(lsDebug);
            lsDebug = lsbFilter.ToString()[5..];
            log.LogInformation(lsDebug);
            lsDebug = lsbSort.ToString()[..^2];
            log.LogInformation(lsDebug);


            // Build Command
            string lsSQL = String.Format("SELECT {0} FROM {1} WHERE {2} ORDER BY {3} OFFSET {4} ROWS FETCH NEXT {5} ROWS ONLY;",
                lsbDisplay.ToString()[..^2], // 0
                tableName.ToUpper(),         // 1
                lsbFilter.ToString()[5..],   // 2
                lsbSort.ToString()[..^2],    // 3
                liOffset,                    // 4
                liFetchCount                 // 5
                );
            log.LogInformation(lsSQL);
            loDBCmd.CommandText = lsSQL;
            return loDBCmd;
        }
        */

        /// <summary>
        /// Prepares the ingredients needed to build a SQL Command object, excluding the DB Connection.
        /// </summary>
        /// <param name="queryReq"></param>
        /// <param name="tableName"></param>
        /// <param name="tableDef"></param>
        /// <param name="_logger"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public IDynamicSqlIngredientsDM PrepareDynamicCommand(IFlexiQueryRequestDM queryReq, string tableName, IDictionary<string, SqlDbType> tableDef, ILogger _logger)
        {
            if (Log is null && _logger is null) throw new NullReferenceException("No usable logger availaible");
            ILogger log = (_logger is null) ? Log : _logger;

            IDynamicSqlIngredientsDM DSI = new DynamicSqlIngredientsDM();

            // TODO: Basic validation
            // queryReq  is not null
            // tableName is not null
            // tableDef  is not null

            // Working variables
            StringBuilder lsbSQL = new StringBuilder();
            StringBuilder lsbDisplay = new StringBuilder();
            StringBuilder lsbFilter = new StringBuilder();
            StringBuilder lsbSort = new StringBuilder();
            string lsColName = "";
            string lsCondition = "";
            string lsTypeQualifier = "";
            SqlDbType dbType = SqlDbType.Text;
            int liOffset = 0;
            int liFetchCount = 0;

            // Build Display Columns
            foreach (string vCol in queryReq.DisplayColumns)
            {
                lsColName = vCol.Trim().ToLower();  // Assuming all column names in DB are upper case
                if (tableDef.ContainsKey(lsColName))
                {
                    lsbDisplay.Append(lsColName + ", ");
                }
                else
                {
                    log.LogWarning("Unknown column ({0}) asked for table NEWREG display.", lsColName);
                }
            }
            if (string.IsNullOrWhiteSpace(lsbDisplay.ToString())) lsbDisplay.Append("*  ");
            if (lsbDisplay.ToString().Length > 2)
            {
                lsbSQL
                    .Append("SELECT ")
                    .Append(lsbDisplay.ToString()[..^2])
                    .Append(" FROM ")
                    .Append(tableName.ToLowerInvariant())
                    ;
            }

            // Build Filter Columns
            foreach (IFilterColumnDM vFC in queryReq.FilterColumns)
            {
                lsColName = vFC.Name.Trim().ToLower();  // Assuming all column names in DB are upper case
                if (tableDef.ContainsKey(lsColName))
                {
                    dbType = tableDef[lsColName];
                    switch (dbType)
                    {
                        // Text
                        case SqlDbType.Char:
                        case SqlDbType.NChar:
                        case SqlDbType.NText:
                        case SqlDbType.NVarChar:
                        case SqlDbType.Text:
                        case SqlDbType.VarChar:
                        case SqlDbType.DateTime:
                        case SqlDbType.SmallDateTime:
                        case SqlDbType.Timestamp:
                        case SqlDbType.Date:
                        case SqlDbType.Time:
                        case SqlDbType.DateTime2:
                        case SqlDbType.DateTimeOffset:
                            lsTypeQualifier = "'";
                            break;

                        // Numeric
                        case SqlDbType.BigInt:
                        case SqlDbType.Bit:
                        case SqlDbType.Decimal:
                        case SqlDbType.Float:
                        case SqlDbType.Int:
                        case SqlDbType.Money:
                        case SqlDbType.SmallInt:
                        case SqlDbType.SmallMoney:
                        case SqlDbType.TinyInt:
                            lsTypeQualifier = "";
                            break;

                        // Log Error
                        case SqlDbType.Image:
                        case SqlDbType.Real:
                        case SqlDbType.UniqueIdentifier:
                        case SqlDbType.VarBinary:
                        case SqlDbType.Variant:
                        case SqlDbType.Xml:
                        case SqlDbType.Udt:
                        case SqlDbType.Structured:
                        default:
                            log.LogError("Not Supported. {0}", dbType.ToString());
                            break;
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

                    switch (vFC.MatchType)
                    {
                        case SQL_MATCH_TYPE.EQUALS:
                            lsbFilter.Append(lsCondition + lsColName + " = " + lsTypeQualifier + "@" + lsColName + lsTypeQualifier);
                            break;
                        case SQL_MATCH_TYPE.LIKE:
                            lsbFilter.Append(lsCondition + lsColName + " LIKE " + lsTypeQualifier + "@" + lsColName + lsTypeQualifier);
                            break;
                        case SQL_MATCH_TYPE.IN:
                            lsbFilter.Append(lsCondition + lsColName + " IN ( @" + lsColName + ")");
                            break;
                        default:
                            break;
                    }
                    // Synchronize parameters
                    DSI.Parameters.Add(SqlHelpers.NewParameter("@" + lsColName, vFC.Value, dbType));
                }
                else
                {
                    log.LogWarning("Unknown column ({0}) asked for table NEWREG filter.", lsColName);
                }

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
                lsColName = vSC.Name.Trim().ToLower();  // Assuming all column names in DB are upper case
                if (tableDef.ContainsKey(lsColName))
                {
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
                else
                {
                    log.LogWarning("Unknown column ({0}) asked for table NEWREG sort.", lsColName);
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
            log.LogInformation("TABLE:   {0}", tableName.ToLower());
            lsDebug = lsbDisplay.ToString();
            log.LogInformation("DISPLAY: {0}", lsDebug);
            lsDebug = lsbFilter.ToString();
            log.LogInformation("FILTER:  {0}", lsDebug);
            lsDebug = lsbSort.ToString();
            log.LogInformation("FILTER:  {0}", lsDebug);
            // SELECT {0} FROM {1} WHERE {2} ORDER BY {3} OFFSET {4} ROWS FETCH NEXT {5} ROWS ONLY;

            // Build Command
            DSI.CommandText = lsbSQL.ToString();
            log.LogInformation(DSI.CommandText);

            return DSI;
        }

        public void Dispose() { }

    }
}
