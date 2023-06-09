using System;
using System.Data;
using Carlos.Exceptions;
using System.Data.SQLite;
using System.Data.Common;
using System.Collections.Generic;
namespace Carlos.Data
{
    /// <summary>
    /// SQLite数据库辅助类，简化了一部分数据库操作。
    /// </summary>
    public class SQLiteHelper : IDbHelper, IDisposable
    {
        private bool mDisposedValue;
        /// <summary>
        /// 构造函数，通过指定的数据库连接字符串创建一个SQLiteHelper实例。
        /// </summary>
        /// <param name="connectionString">数据库连接字符串。</param>
        public SQLiteHelper(string connectionString)
        {
            if (ConnectionString != null) ConnectionString = connectionString;
            if (Connection != null) Connection = new SQLiteConnection(ConnectionString);
        }
        /// <summary>
        /// 构造函数，通过指定的SQLite数据库连接实例（而非数据库连接字符串）创建一个SQLiteHelper实例。
        /// </summary>
        /// <param name="connection">适用于SQLite数据库的连接实例。</param>
        public SQLiteHelper(SQLiteConnection connection)
        {
            if (Connection != null) Connection = connection;
            if (ConnectionString != null) ConnectionString = Connection.ConnectionString;
        }
        /// <summary>
        /// 构造函数，通过指定的文件URL创建一个SQLiteHelper实例。
        /// </summary>
        /// <param name="databaseFileUri">SQLite数据库的文件URL。</param>
        public SQLiteHelper(Uri databaseFileUri)
        {
            if (ConnectionString != null) ConnectionString = $"Data Source={databaseFileUri.LocalPath}";
            if (Connection != null) Connection = new SQLiteConnection(ConnectionString);
        }
        /// <summary>
        /// 获取或设置当前实例的数据库连接字符串。
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 获取或设置当前实例的数据库连接实例。
        /// </summary>
        private SQLiteConnection Connection { get; set; }
        /// <summary>
        /// 获取或设置当前实例的数据库查询实例。
        /// </summary>
        private SQLiteCommand Command { get; set; }
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
        /// 获取当前实例的数据库连接状态。
        /// </summary>
        public ConnectionState ConnectionState => Connection.State;
        /// <summary>
        /// 获取当前数据库连接等实例是否处于就绪状态，与此同时并获取当前实例的详细的状态信息。
        /// </summary>
        /// <param name="info">详细的状态信息。</param>
        /// <returns>该操作会返回一个Boolean数据，如果当前数据库已经做好连接或者相关的就绪准备，则返回true，否则返回false。</returns>
        public virtual bool IsReadyConnect(out string info)
        {
            bool connStrIsNull = string.IsNullOrWhiteSpace(ConnectionString);
            bool connInstanceIsNull = Connection == null;
            info = connStrIsNull ? "[Warnning] The connection string is null, empty or white space.\n" : "[OK] The connection string is normal.\n";
            if (info != null) info += connInstanceIsNull ? "[Warnning] The connection is not initialization.\n" : "[OK] The connection is normal.\n";
            return !(connInstanceIsNull || connStrIsNull);
        }
        /// <summary>
        /// 开始连接到数据库。
        /// </summary>
        /// <returns>如果连接成功，则返回true，否则返回false。</returns>
        public virtual bool Connect()
        {
            Connection?.Open();
            if (Command != null) Command = new SQLiteCommand(Connection);
            return ConnectionState != ConnectionState.Closed;
        }
        /// <summary>
        /// 断开与数据库的连接。
        /// </summary>
        /// <returns>如果断开连接成功，则返回true，否则返回false。</returns>
        public virtual bool Disconnect()
        {
            Connection?.Close();
            return !IsConnected;
        }
        /// <summary>
        /// 执行指定的SQL语句。
        /// </summary>
        /// <param name="sql">需要被执行的SQL语句。</param>
        /// <returns>对SQLite数据库执行查询操作，返回受影响的行数。</returns>
        /// <exception cref="ArgumentException">当SQL语句为Empty或者为NULL时，则将会抛出这个异常。</exception>
        /// <exception cref="NullReferenceException">当数据库连接实例为NULL或者数据库未连接时，则将会抛出这个异常。</exception>
        public virtual int ExecuteSql(string sql)
        {
            if (sql.Length == 0 || string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("The SQL string is not null, empty or white space.", "sql");
            if (!IsConnected) throw new NullReferenceException("Your DB connection is null or it's disconnected.");
            int impact = -1;
            if (Command != null) Command.CommandText = sql;
            try
            {
                impact = Command.ExecuteNonQuery();
            }
            catch (Exception throwedException)
            {
                if (throwedException != null)
                {
                    string msg = "Incorrect SQL syntax or format.";
                    throw new SqlGrammarOrFormatException(msg, "sql");
                }
            }
            return impact;
        }
        /// <summary>     
        /// 执行指定的SQL语句。
        /// </summary>     
        /// <param name="sql">要执行的增删改的SQL语句。</param>     
        /// <param name="parameters">执行增删改语句所需要的参数，参数必须以它们在SQL语句中的顺序为准。</param>     
        /// <returns>对SQLite数据库执行增删改操作，返回受影响的行数。</returns>
        /// <exception cref="NullReferenceException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
        public virtual int ExecuteSql(string sql, IList<SQLiteParameter> parameters)
        {
            if (!IsConnected) throw new NullReferenceException("Your DB connection is null or it's disconnected.");
            DbTransaction transaction = Connection.BeginTransaction();
            if (Command != null) Command.CommandText = sql;
            if (!(parameters == null || parameters.Count == 0))
            {
                foreach (SQLiteParameter parameter in parameters)
                    Command.Parameters.Add(parameter);
            }
            int affectedRows = Command.ExecuteNonQuery();
            transaction?.Commit();
            return affectedRows;
        }
        /// <summary>
        /// 执行指定的SQL语句，返回一个包含查询结果的DataTable。
        /// </summary>
        /// <param name="sql">需要被执行的SQL语句。</param>
        /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准。</param>
        /// <returns>如果执行未产生任何异常，则会返回这个操作之后的查询结果。</returns>
        /// <exception cref="NullReferenceException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
        public virtual DataTable ExecuteSqlToDataTable(string sql, IList<SQLiteParameter> parameters)
        {
            if (!IsConnected) throw new NullReferenceException("Your DB connection is null or it's disconnected.");
            if (Command != null) Command.CommandText = sql;
            if (!(parameters == null || parameters.Count == 0))
            {
                foreach (SQLiteParameter parameter in parameters)
                    Command.Parameters.Add(parameter);
            }
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(Command);
            DataTable data = new DataTable();
            adapter?.Fill(data);
            return data;
        }
        /// <summary>
        /// 执行一个查询语句，返回一个关联的SQLiteDataReader实例。
        /// </summary>
        /// <param name="sql">需要被执行的SQL语句。</param>
        /// <returns>如果执行未产生任何异常，则会返回这个操作之后的查询结果关联的SQLiteDataReader实例。</returns>
        /// <exception cref="NullReferenceException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
        public virtual SQLiteDataReader ExecuteSqlToReader(string sql)
        {
            if (!IsConnected) throw new NullReferenceException("Your DB connection is null or it's disconnected.");
            if (Command != null) Command.CommandText = sql;
            return Command.ExecuteReader();
        }
        /// <summary>
        /// 执行一个查询语句，返回一个关联的SQLiteDataReader实例。
        /// </summary>
        /// <param name="sql">需要被执行的SQL语句。</param>
        /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准。</param>
        /// <returns>如果执行未产生任何异常，则会返回这个操作之后的查询结果关联的SQLiteDataReader实例。</returns>
        /// <exception cref="NullReferenceException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
        public virtual SQLiteDataReader ExecuteSqlToReader(string sql, IList<SQLiteParameter> parameters)
        {
            if (!IsConnected) throw new NullReferenceException("Your DB connection is null or it's disconnected.");
            if (Command != null) Command.CommandText = sql;
            if (!(parameters == null || parameters.Count == 0))
            {
                foreach (SQLiteParameter parameter in parameters)
                    Command.Parameters.Add(parameter);
            }
            return Command.ExecuteReader();
        }
        /// <summary>
        /// 检查指定的数据表在数据库中是否存在。
        /// </summary>
        /// <param name="tableName">指定的数据表名称。</param>
        /// <returns>该操作会返回一个Boolean数据，如果指定的数据表存在于数据库中，则返回true，否则返回false。</returns>
        /// <exception cref="NullReferenceException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
        public virtual bool DataTableExists(string tableName)
        {
            if (!IsConnected) throw new NullReferenceException("Your DB connection is null or it's disconnected.");
            if (Command != null) Command.CommandText = $"select count(*) from sqlite_master where type='table' and name='{tableName}';";
            return Convert.ToInt32(Command.ExecuteScalar()) != 0;
        }
        /// <summary>
        /// 获取当前数据库中所有数据表的表名称。
        /// </summary>
        /// <returns>如果操作无异常，则会以列表实例的形式返回当前数据库中所有数据表的表名称。</returns>
        public virtual List<string> GetAllDataTableName()
        {
            List<string> dtNames = new List<string>();
            if (!IsConnected) Connect();
            SQLiteDataReader reader = ExecuteSqlToReader(@"select name from sqlite_master where type='table'");
            while (reader.Read()) dtNames.Add(reader.GetString(0));
            return dtNames;
        }
        /// <summary>     
        /// 查询数据库中的所有数据类型信息。
        /// </summary>     
        /// <returns>操作成功之后会返回所查询数据库中的所有数据类型的相关信息。</returns>     
        /// <exception cref="NullReferenceException">当数据库未连接或者连接已断开时，则会抛出这个异常。</exception>
        public virtual DataTable GetSchema()
        {
            if (!IsConnected) throw new NullReferenceException("Your DB connection is null or it's disconnected.");
            return Connection.GetSchema("TABLES");
        }
        /// <summary>
        /// 根据指定的路径创建一个SQLite数据库。
        /// </summary>
        /// <param name="databaseFileUrl">需要被创建的数据库的路径。</param>
        public static void CreateDatabase(string databaseFileUrl)
        {
            SQLiteHelper sqlite = new SQLiteHelper(new Uri(databaseFileUrl));
            sqlite.Connect();
            if (sqlite.IsConnected)
            {
                sqlite.ExecuteSql("create table demo(id integer not null primary key autoincrement unique)");
                sqlite.ExecuteSql("drop table demo");
            }
            sqlite.Disconnect();
        }
        /// <summary>
        /// 获取当前实例的字符串表达形式。
        /// </summary>
        /// <returns>该操作会返回一个字符串，这个字符串包含了最基本的实例信息。</returns>
        public override string ToString() => $"SQLiteHelper=[ConnectionString:{ConnectionString},ConnectionState:{ConnectionState}]";
        /// <summary>
        /// 如果环境条件允许，则将尝试释放和回收当前实例。
        /// </summary>
        /// <param name="disposing">决定是否尝试释放和回收当前实例包含的由CLR托管的资源。</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposedValue)
            {
                if (disposing)
                {
                    Connection?.Dispose();
                    Command?.Dispose();
                    if (ConnectionString != null) ConnectionString = null;
                }
                mDisposedValue = true;
            }
        }
        /// <summary>
        /// 尝试释放和回收当前实例，并请求CLR不要调用指定对象的终结器。
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
