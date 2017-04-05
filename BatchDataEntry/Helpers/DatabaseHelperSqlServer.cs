using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Helpers
{
    public class DatabaseHelperSqlServer
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected SqlConnection cnn;

        public DatabaseHelperSqlServer(string user, string password, string serverUrl, string databaseName, int timeout = 30)
        {
            cnn = new SqlConnection(string.Format("user id={0};password={1};server={2};Trusted_Connection=yes;database={3};connection timeout={4}", user, password, serverUrl, databaseName, timeout));
        }
    }
}
