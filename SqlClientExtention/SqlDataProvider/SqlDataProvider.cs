using SqlClientExtention.Extentions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SqlClientExtention.SqlDataProvider
{
    public class SqlDataProvider : IDisposable
    {
        private SqlCommand Command;
        public string ConnectionString { get; set; }
        private object Parameters;
        private SqlDataReader Reader;
        private bool isFirstFetch = true;
        private bool isFinished = false;
        public bool HasResult {
            get { return (Reader != null) && Reader.HasRows; }
        } 
        
        public SqlDataProvider(string connectionString) =>
            ConnectionString = connectionString;

        private void AddParameter(object Parameters)
        {
            this.Parameters = Parameters;
            if (Parameters.GetType().IsAnonymousType())
                Command.Parameters.AddParams(Parameters);
            else
                Command.Parameters.AddParams(Parameters, Parameters.GetType());
        }
        private void ReturnOutputParameters()
        {
            foreach (SqlParameter p in Command.Parameters)
            {
                if (p.Direction.Equals(ParameterDirection.Output))
                {
                    string paramName = p.ParameterName.Substring(1);
                    this.Parameters.SetPropertyValue(paramName, p.Value);
                }
            }
        }

        public void Finish()
        {
            if (!isFinished)
            {
                Command.Cancel();
                Command.Connection.Close();

                if (Reader != null)
                    Reader.Close();

                isFinished = true;
                this.ReturnOutputParameters();
            }
        }

        public List<TResult> FetchRowSet<TResult>()
            where TResult : class, new()
        {
            if (!Reader.IsClosed)
            {
                if (isFirstFetch == false)
                    Reader.NextResult();
                else
                    isFirstFetch = false;
                
                return Reader.ToList<TResult>();
            }
            
            this.Finish();
            return new List<TResult>();
        }

        public void ExecuteReader(string spName, object Parameters)
        {
            Command = new SqlCommand(spName, new SqlConnection(ConnectionString));
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandTimeout = 120;
            this.AddParameter(Parameters);
            
            Command.Connection.Open();
            Reader = Command.ExecuteReader();
        }

        public int ExecuteNonQuery(string spName, object Parameters)
        {
            Command = new SqlCommand(spName, new SqlConnection(ConnectionString));
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandTimeout = 120;
            this.AddParameter(Parameters);

            Command.Connection.Open();
            return Command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            this.Finish();
        }
    }
}
