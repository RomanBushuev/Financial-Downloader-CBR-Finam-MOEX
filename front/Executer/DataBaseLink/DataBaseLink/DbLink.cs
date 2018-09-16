using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;

namespace DataBaseLink
{
    public class DbLink
    {
        private IDbConnection _connection;

        public DbLink(IDbConnection connection)
        {
            _connection = connection;
        }

        public IDbConnection GetConnection()
        {
            if (_connection != null && _connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            return _connection;
        }

        public void Close()
        {
            if(_connection.State == ConnectionState.Open)
                _connection.Close();
            _connection.Dispose();
        }
    }

    public class Fabricate
    {
        public static IDbConnection CreateConnection(string connection, ConnectionType type)
        {
            if (type == ConnectionType.Npgsql)
            {
                Npgsql.NpgsqlConnection npgConnection = new NpgsqlConnection(connection);
                return npgConnection;
            }

            return null;
        }
    }

    [Flags]
    public enum ConnectionType
    {
        Deafult = 1 << 0,
        Npgsql = 1 << 1,
    }


}
