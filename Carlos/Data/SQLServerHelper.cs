using System;
using System.Data;
using Carlos.Exceptions;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Carlos.Data
{
    /// <summary>
    /// SQL Server数据库操作接口封装应用类，用于实现最常用的SQL Server数据库操作。
    /// </summary>
    public class SQLServerHelper : IDbHelper, IDisposable
    {
        /// <summary>
        /// 构造函数，通过一个指定的数据库连接字符串地址来初始化当前实例。
        /// </summary>
        /// <param name="dbConnectionString">指定的数据库连接字符串。</param>
        public SQLServerHelper(string dbConnectionString)
        {
            ConnectionString = dbConnectionString;
            DbConnector = new SqlConnection(ConnectionString);
            Command = new SqlCommand(string.Empty, DbConnector);
        }
        /// <summary>
        /// 获取或设置当前实例的SqlConnection实例。
        /// </summary>
        public SqlConnection DbConnector { get; set; }
        /// <summary>
        /// 获取当前数据库的连接状态。
        /// </summary>
        public ConnectionState ConnectionStatus => DbConnector.State;
        /// <summary>
        /// 获取或设置当前数据库的连接字符串。
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 获取当前实例的连接实例是否处于“已连接”的状态。
        /// </summary>
        /// <exception cref="NullReferenceException">当数据库连接为NULL时，则抛出这个异常。</exception>
        public bool IsConnected
        {
            get
            {
                if (Connection == null) throw new NullReferenceException("The connecction must be effective.");
                if (ConnectionState != ConnectionState.Closed) return true;
                else return false;
            }
        }
        /// <summary>
        /// 获取或设置当前实例的数据库连接实例。
        /// </summary>
        private SqlConnection Connection { get; set; }
        /// <summary>
        /// 获取或设置当前实例的数据库查询实例。
        /// </summary>
        private SqlCommand Command { get; set; }
        /// <summary>
        /// 获取当前实例的数据库连接状态。
        /// </summary>
        public ConnectionState ConnectionState => Connection.State;
        /// <summary>
        /// 连接当前实例指定的数据库。
        /// </summary>
        public bool Connect()
        {
            Connection?.Open();
            return IsConnected;
        }
        /// <summary>
        /// 断开当前数据库的连接。
        /// </summary>
        public bool Disconnect()
        {
            Connection?.Close();
            return IsConnected;
        }
        /// <summary>
        /// 执行指定的SQL语句。
        /// </summary>
        /// <param name="sqlSentence">需要被执行的SQL语句。</param>
        /// <returns>如果该方法未产生任何异常，则会返回数据表里面受影响的数据行。</returns>
        /// <exception cref="EmptySqlCommandLineException">当出现空的SQL语句时，则会抛出这个异常。</exception>
        /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
        /// <exception cref="SqlGrammarErrorException">当SQL语法出现错误时，则会抛出这个异常。</exception>
        public int ExecuteSql(string sqlSentence)
        {
            if (sqlSentence.Length == 0 || sqlSentence == null) throw new EmptySqlCommandLineException();
            if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
            int impact = -1;
            Command.CommandText = sqlSentence;
            try
            {
                impact = Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    throw new SqlGrammarErrorException();
                }
            }
            return impact;
        }
        /// <summary>     
        /// 执行指定的SQL语句。
        /// </summary>     
        /// <param name="sqlSentence">要执行的增删改的SQL语句。</param>     
        /// <param name="parameters">执行增删改语句所需要的参数，参数必须以它们在SQL语句中的顺序为准。</param>     
        /// <returns>对SQLite数据库执行增删改操作，返回受影响的行数。</returns>
        /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
        public int ExecuteSql(string sqlSentence, IList<SqlParameter> parameters)
        {
            int affectedRows = 0;
            if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
            DbTransaction transaction = DbConnector.BeginTransaction();
            SqlCommand command = new SqlCommand(sqlSentence, DbConnector);
            if (!(parameters == null || parameters.Count == 0))
            {
                foreach (SqlParameter parameter in parameters) command.Parameters.Add(parameter);
            }
            affectedRows = command.ExecuteNonQuery();
            transaction.Commit();
            return affectedRows;
        }
        /// <summary>
        /// 执行指定的SQL语句，返回一个包含查询结果的DataTable。
        /// </summary>
        /// <param name="sqlSentence">需要被执行的SQL语句。</param>
        /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准。</param>
        /// <returns>如果执行未产生任何异常，则会返回这个操作之后的查询结果。</returns>
        /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
        public DataTable ExecuteSqlToDataTable(string sqlSentence, IList<SqlParameter> parameters)
        {
            SqlCommand command = new SqlCommand(sqlSentence, DbConnector);
            if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
            if (!(parameters == null || parameters.Count == 0))
            {
                foreach (SqlParameter parameter in parameters) command.Parameters.Add(parameter);
            }
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable data = new DataTable();
            adapter.Fill(data);
            return data;
        }
        /// <summary>
        /// 执行一个查询语句，返回一个关联的SqlDataReader实例。
        /// </summary>
        /// <param name="sqlSentence">需要被执行的SQL语句。</param>
        /// <returns>如果执行未产生任何异常，则会返回这个操作之后的查询结果关联的SqlDataReader实例。</returns>
        /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
        public SqlDataReader ExecuteSqlToReader(string sqlSentence)
        {
            if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
            Command = new SqlCommand(sqlSentence, DbConnector);
            return Command.ExecuteReader();
        }
        /// <summary>
        /// 执行一个查询语句，返回一个关联的SqlDataReader实例。
        /// </summary>
        /// <param name="sqlSentence">需要被执行的SQL语句。</param>
        /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准。</param>
        /// <returns>如果执行未产生任何异常，则会返回这个操作之后的查询结果关联的SqlDataReader实例。</returns>
        /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
        public SqlDataReader ExecuteSqlToReader(string sqlSentence, IList<SqlParameter> parameters)
        {
            Command = new SqlCommand(sqlSentence, DbConnector);
            if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
            if (!(parameters == null || parameters.Count == 0))
            {
                foreach (SqlParameter parameter in parameters) Command.Parameters.Add(parameter);
            }
            Connect();
            return Command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        /// <summary>
        /// 检查当前数据库中指定的数据表格是否存在。
        /// </summary>
        /// <param name="tableName">需要被检查是否存在的表格。</param>
        /// <returns>如果这个表格存在，则返回true，否则将返回false。</returns>
        /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
        public bool DataTableExists(string tableName)
        {
            bool exists = false;
            List<string> tables = GetAllDataTableName();
            Parallel.For(0, tables.Count, (index, interrupt) =>
            {
                if (tables[index] == tableName)
                {
                    exists = true;
                    interrupt.Stop();
                }
            });
            return exists;
        }
        /// <summary>
        /// 根据指定条件判断指定数据表中的某一条记录是否存在。
        /// </summary>
        /// <param name="tableName">需要被判定记录存在性的数据表。</param>
        /// <param name="condition">用于判定的条件，这个条件会直接作为SQL语句的条件。</param>
        /// <returns>如果这条记录存在，则返回true，否则将返回false。</returns>
        /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
        public bool RecordExists(string tableName, string condition)
        {
            if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
            string sql = "select * from " + tableName + " where " + condition;
            return ExecuteSqlToReader(sql).HasRows;
        }
        /// <summary>
        /// 获取当前数据库中所有数据表的表名称。
        /// </summary>
        /// <returns>如果操作无异常，则会以列表实例的形式返回当前数据库中所有数据表的表名称。</returns>
        public List<string> GetAllDataTableName()
        {
            List<string> dtnames = new List<string>();
            if (ConnectionStatus == ConnectionState.Closed) Connect();
            SqlDataReader reader = ExecuteSqlToReader(@"SELECT Name FROM SysObjects Where XType='U' ORDER BY Name");
            while (reader.Read()) dtnames.Add(reader.GetString(0));
            return dtnames;
        }
        /// <summary>     
        /// 查询数据库中的所有数据类型信息。
        /// </summary>     
        /// <returns>操作成功之后会返回所查询数据库中的所有数据类型的相关信息。</returns>     
        /// <exception cref="ConnectionNotExistsException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
        public DataTable GetSchema()
        {
            if (ConnectionStatus == ConnectionState.Closed) throw new ConnectionNotExistsException();
            DataTable data = DbConnector.GetSchema("Tables");
            return data;
        }
        /// <summary>
        /// 初始化当前的数据库连接。
        /// </summary>
        /// <returns>如果初始化成功则会返回true，若被该方法捕获了一些无法处理的异常则会视为初始化失败，并返回false。</returns>
        public bool InitializeConnection()
        {
            bool isconned = true;
            try
            {
                DbConnector = new SqlConnection(ConnectionString);
                Command = new SqlCommand(string.Empty, DbConnector);
            }
            catch (Exception ex)
            {
                if (ex != null) isconned = false;
            }
            return isconned;
        }
        /// <summary>
        /// 获取当前SQLServerDBOIEncapsulation实例的完整类名的字符串表达形式。
        /// </summary>
        /// <returns>该操作会返回一个当前实例的完整类名的字符串表达形式。</returns>
        public override string ToString()
        {
            return "Cabinink.DataTreatment.Database.SQLServerDBOIEncapsulation";
        }
        /// <summary>
        /// 手动释放该对象引用的所有内存资源。
        /// </summary>
        public void Dispose()
        {
            DbConnector.Dispose();
            Command.Dispose();
        }
    }
}
