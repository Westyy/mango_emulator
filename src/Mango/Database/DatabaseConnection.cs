using log4net;
using Mango.Collections;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Mango.Database
{
    /// <summary>
    /// Provides access to an open database connection.
    /// (Pooling is handled by the MySQL library itself and not here)
    /// </summary>
    class DatabaseConnection : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Database.DatabaseConnection");

        private readonly ObjectPool<DatabaseConnection> _poolReturn;

        private MySqlConnection _con;
        private MySqlCommand _cmd;

        private MySqlTransaction _trans;
        private List<MySqlParameter> _params;

        DateTime Start;

        public DatabaseConnection(string ConnectionStr, ObjectPool<DatabaseConnection> Pool)
        {
            this._poolReturn = Pool;
            this._con = new MySqlConnection(ConnectionStr);
            this._cmd = this._con.CreateCommand();
        }

        public void Open()
        {
            if (this._con.State == ConnectionState.Open) { throw new InvalidOperationException("Connection already open."); }
            this._con.Open();

            this.Start = DateTime.Now;
        }

        public bool IsOpen()
        {
            return this._con.State == ConnectionState.Open;
        }

        public void AddParameter(string param, object value)
        {
            if (this._params == null) { this._params = new List<MySqlParameter>(); }
            this._params.Add(new MySqlParameter(param, value));
        }

        public void SetQuery(string Query)
        {
            this._cmd.CommandText = Query;
        }

        public int ExecuteNonQuery()
        {
            if (this._params != null && this._params.Count > 0)
            {
                this._cmd.Parameters.AddRange(this._params.ToArray());
            }

            try
            {
                return this._cmd.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                log.Error("MySql Error: ", e);
                throw e;
            }
            finally
            {
                this._cmd.CommandText = string.Empty;
                this._cmd.Parameters.Clear();

                if (this._params != null && this._params.Count > 0) { this._params.Clear(); }
            }
        }

        public int SelectLastId()
        {
            //this._cmd.CommandText = "SELECT LAST_INSERT_ID();";

            try
            {
                return (int)this._cmd.LastInsertedId;
            }
            catch (MySqlException e)
            {
                log.Error("MySql Error: ", e);
                throw e;
            }
            finally
            {
                this._cmd.CommandText = string.Empty;
            }
        }

        public int ExecuteSingleInt()
        {
            if (this._params != null && this._params.Count > 0)
            {
                this._cmd.Parameters.AddRange(this._params.ToArray());
            }

            try
            {
                return int.Parse(this._cmd.ExecuteScalar().ToString());
            }
            catch (MySqlException e)
            {
                log.Error("MySql Error: ", e);
                throw e;
            }
            finally
            {
                this._cmd.CommandText = string.Empty;
                this._cmd.Parameters.Clear();

                if (this._params != null && this._params.Count > 0) { this._params.Clear(); }
            }
        }

        public bool TryExecuteSingleInt(out int Value)
        {
            if (this._params != null && this._params.Count > 0)
            {
                this._cmd.Parameters.AddRange(this._params.ToArray());
            }

            try
            {
                object s = this._cmd.ExecuteScalar();

                if (s == null)
                {
                    Value = 0;
                    return false;
                }

                Value = int.Parse(s.ToString());
                return true;
            }
            catch (MySqlException e)
            {
                log.Error("MySql Error: ", e);
                throw e;
            }
            finally
            {
                this._cmd.CommandText = string.Empty;
                this._cmd.Parameters.Clear();

                if (this._params != null && this._params.Count > 0) { this._params.Clear(); }
            }
        }

        public MySqlDataReader ExecuteReader()
        {
            if (this._params != null && this._params.Count > 0)
            {
                this._cmd.Parameters.AddRange(this._params.ToArray());
            }

            try
            {
                return this._cmd.ExecuteReader();
            }
            catch (MySqlException e)
            {
                log.Error("MySql Error: ", e);
                log.Error(e.Source);
                throw e;
            }
            finally
            {
                this._cmd.CommandText = string.Empty;
                this._cmd.Parameters.Clear();

                if (this._params != null && this._params.Count > 0) { this._params.Clear(); }
            }
        }

        public DataSet ExecuteDataSet()
        {
            if (this._params != null && this._params.Count > 0)
            {
                this._cmd.Parameters.AddRange(this._params.ToArray());
            }

            DataSet Set = new DataSet();

            try
            {
                using (MySqlDataAdapter Adapter = new MySqlDataAdapter(this._cmd))
                {
                    Adapter.Fill(Set);
                }

                return Set;
            }
            catch (MySqlException e)
            {
                log.Error("MySql Error: ", e);
                throw e;
            }
            finally
            {
                this._cmd.CommandText = string.Empty;
                this._cmd.Parameters.Clear();

                if (this._params != null && this._params.Count > 0) { this._params.Clear(); }
            }
        }

        public DataTable ExecuteTable()
        {
            DataSet DataSet = this.ExecuteDataSet();
            return DataSet.Tables.Count > 0 ? DataSet.Tables[0] : null;
        }

        public DataRow ExecuteRow()
        {
            DataTable DataTable = this.ExecuteTable();
            return DataTable.Rows.Count > 0 ? DataTable.Rows[0] : null;
        }

        public void BeginTransaction()
        {
            this._trans = this._con.BeginTransaction();
        }

        public void Commit()
        {
            if (this._trans == null) { throw new InvalidOperationException("Transaction not started."); }
            this._trans.Commit();
        }

        public void Rollback()
        {
            if (this._trans == null) { throw new InvalidOperationException("Transaction not started."); }
            this._trans.Rollback();
        }

        public void Dispose()
        {
            if (this._con.State == ConnectionState.Open)
            {
                this._con.Close();
                this._con = null;
            }

            if (this._params != null)
            {
                this._params.Clear();
                this._params = null;
            }

            if (this._trans != null)
            {
                this._trans.Dispose();
                this._trans = null;
            }

            if (this._cmd != null)
            {
                this._cmd.Dispose();
                this._cmd = null;
            }

            int Finish = (DateTime.Now - Start).Milliseconds;

            if (DatabaseManager.SHOW_QUERY_TIME)
            {
                log.Debug("Query completed in " + Finish + "ms");
            }

            if (Finish >= 5000)
            {
                log.Warn("Query took 5 seconds or longer");
            }

            //this._poolReturn.PutObject(this);
        }
    }
}
