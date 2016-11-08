using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using FileItDataLayer.Helpers;

namespace FileItDataLayer.Helpers
{
    public class FireBirdHelper
    {
        public static string FormatValue(string  val){
            var result = "";
            if (val != null)
            {
                result = "'" + val.Replace("'", "''") + "'";
            }
            else {
                result = "null";
            }
            return result;
        }

        public static DataTable GenericSelect(string sql, string connectionstring)
        {
            FbConnection cn = null;
            FbCommand cmd = null;
            DataSet ds = new DataSet();
            FbDataAdapter da = null;
            DataTable result = null;

            try
            {
                cn = new FbConnection(connectionstring);
                cn.Open();
                cmd = new FbCommand(sql, cn);
                da = new FbDataAdapter(cmd);
                da.Fill(ds);
                result = ds.Tables[0];
            }
            catch (FbException ex)
            {
                ExceptionHelper.Log(ex);
            }
            finally {
                da.Dispose();
                cmd.Dispose();
                cn.Dispose();
            }

            //handle nulls?
            return result;
        }

        public static bool GenericCommand(string sql, string connectionstring)
        {
            FbConnection cn = null;
            FbCommand cmd = null;
            var result = false;

            try
            {
                cn = new FbConnection(connectionstring);
                cn.Open();
                cmd = new FbCommand(sql, cn);
                cmd.ExecuteNonQuery();

                result = true;
            }
            catch (FbException ex)
            {
                ExceptionHelper.Log(ex);
            }
            finally
            {
                cmd.Dispose();
                cn.Dispose();
            }

            //handle nulls?
            return result;
        }
    }
}
