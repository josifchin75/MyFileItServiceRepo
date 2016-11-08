using MyFileItDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileItService.Helpers
{
    public class ReportHelper
    {
        private MyFileItEntities _db;

        public ReportHelper(MyFileItEntities db)
        {
            _db = db;
        }

        public DataTable CreateEventPlayerDocumentStatusReport(int TeamEventId)
        {
            return GetReportData(CreateEventPlayerDocumentStatusReportSQL(TeamEventId));
        }

        private string CreateEventPlayerDocumentStatusReportSQL(int TeamEventId)
        {
            return @"select
                 ust.NAME
                 , auplayer.FIRSTNAME
                 , auplayer.LASTNAME
                 , auplayer.EMAILADDRESS
                 , auplayer.PHONE
                 , tepr.JERSEYNUMBER
                 , tepr.PLAYERPOSITION
                 , td.TEAMEVENTID
                 , t.NAME  as TEAMEVENTNAME
                 , td.DOCUMENTNAME
                 , td.MUSTBEVERIFIED
                 , sd.DATECREATED as DATESENT
                 , sd.VERIFIEDDATE
                 , aureviewer.EMAILADDRESS
                 , sd.VERIFIEDAPPUSERID
                 , auplayer.ID as APPUSERID
                 from TEAMEVENTDOCUMENT td
                 left join TEAMEVENT t on td.TEAMEVENTID = t.ID
                 left join TEAMEVENTPLAYERROSTER tepr on tepr.TEAMEVENTID = t.ID
                 left join APPUSER auplayer on auplayer.ID = tepr.APPUSERID
                 left join USERSTAGETYPE ust on tepr.USERSTAGETYPEID = ust.ID
                 left join SHAREDOCUMENT sd on sd.TEAMEVENTDOCUMENTID = td.ID and sd.APPUSERID = tepr.APPUSERID
                 left join APPUSER aureviewer on aureviewer.ID = sd.VERIFIEDAPPUSERID
                 where td.TEAMEVENTID = " + TeamEventId.ToString() + @" 
                 order by auplayer.LASTNAME, td.DOCUMENTNAME
            ";
        }

        private DataTable GetReportData(string sql)
        {
            DataTable dt = new DataTable();
            using (var sqlHelper = new SqlDataHelper(_db))
            {
                dt = sqlHelper.GetDataTable(sql);
            }

            return dt;
        }
    }
}
