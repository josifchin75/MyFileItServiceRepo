using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Management;

namespace MyFileItService.Helpers
{
    public class SqlDataHelper : IDisposable
    {
        private readonly DbContext _context;

        public SqlDataHelper(DbContext context)
        {
            _context = context;
        }

        public DataTable GetDataTable(string sqlQuery)
        {
            DataTable tb = new DataTable();
            try
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(_context.Database.Connection);

                using (var cmd = factory.CreateCommand())
                {
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = _context.Database.Connection;
                    using (var adapter = factory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;

                        tb = new DataTable();
                        adapter.Fill(tb);
                        //return tb;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.LogError(string.Format("Error occurred during SQL query execution {0}"));
                ExceptionHelper.LogError(ex);
                //throw new SqlExecutionException(string.Format("Error occurred during SQL query execution {0}", sqlQuery), ex);
            }

            return tb;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}