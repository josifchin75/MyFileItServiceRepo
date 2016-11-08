using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileItDataLayer.Helpers;
using System.Data;
using System.IO;

namespace FileItDataLayer.Models
{
    public partial class CABINET
    {
        private string FBSERVER = System.Configuration.ConfigurationManager.AppSettings["FireBirdServer"].ToString();
        private static string FileItDataPath = System.Configuration.ConfigurationManager.AppSettings["FileItDataPath"].ToString();
        private static string EmptyCabinetFilePath = System.Configuration.ConfigurationManager.AppSettings["FileItEmptyCabinetFilePath"].ToString();

        public TEMPLATE Template
        {
            get
            {
                TEMPLATE result = null;
                using (var db = new SystemFileitEntities())
                {
                    result = db.TEMPLATES.Single(t => t.TEMPLATENAME.Equals(this.TEMPLATENAME, StringComparison.CurrentCultureIgnoreCase));
                }

                return result;
            }
        }

        public string CabinetConnectionString
        {
            get
            {
                var result = "Database=" + this.CABINETFULLPATH + ";User=SYSDBA;Password=masterkey;Dialect=3;Server=" + FBSERVER;
                return result;
            }
        }

        public bool DeleteDocument(string documentId) {
            var result = false;
            var sql = "UPDATE " + CABINETID + "_IMAGES SET DELETED='Y' WHERE FILENAME='" + documentId + "'";
            result = FireBirdHelper.GenericCommand(sql, this.CabinetConnectionString);
            return result;
        }

        public List<FileItDocument> GetDocuments(List<FileItDocumentLookup> lookups, bool IncludeThumbs)
        {
            var result = new List<FileItDocument>();

            var sql = "SELECT * FROM " + CABINETID + "_IMAGES";
            var whereClause = "";
            if (lookups.Any())
            {
                lookups.ForEach(l =>
                {
                    whereClause += whereClause.Length == 0 ? " WHERE " : "";
                    whereClause += l.ToWhereClause();
                });
            }
            sql = sql + whereClause;

            var docs = FireBirdHelper.GenericSelect(sql, CabinetConnectionString);

            foreach (DataRow dr in docs.Rows)
            {
                result.Add(new FileItDocument(dr, OSTOREPATH, IncludeThumbs));
            }

            return result;
        }

        public List<FileItDocument> GetDocumentsByIds(List<string> ids, bool IncludeThumbs, bool includeDeleted)
        {
            var result = new List<FileItDocument>();

            var sql = "SELECT * FROM " + CABINETID + "_IMAGES";

            var whereClause = "";
            if (ids.Any())
            {
                ids.ForEach(i =>
                {
                    whereClause += whereClause.Length == 0 ? " WHERE " : " AND ";
                    whereClause += "FILENAME='" + i + "'";
                });
            }
            if (!includeDeleted) {
                whereClause += whereClause.Length == 0 ? " WHERE " : " AND ";
                whereClause += "DELETED='N'";
            }
            sql = sql + whereClause;

            var docs = FireBirdHelper.GenericSelect(sql, CabinetConnectionString);

            foreach (DataRow dr in docs.Rows)
            {
                result.Add(new FileItDocument(dr, OSTOREPATH, IncludeThumbs));
            }

            return result;
        }

        private static string GenerateCabinetId(string cabinetName, ref string CABINETNAMEVALUE)
        {
            var result = "";
            var bFound = false;
            int i = 0;
            var cabinetNameExists = false;
            //handle long names
            if (cabinetName.Length > 18) {
                cabinetName = cabinetName.Substring(0, 18);
            }
            
            using (var db = new SystemFileitEntities())
            {
                cabinetNameExists = db.CABINETS.Any(c => c.CABINETNAME.Equals(cabinetName, StringComparison.CurrentCultureIgnoreCase));

                while (!bFound && i < 99999)
                {
                    var ticks = DateTime.Now.Ticks.ToString();
                    result = cabinetName.Replace(" ", "") + ticks.Substring(ticks.Length - 5, 5);
                    
                    bFound = !db.CABINETS.Any(c => c.CABINETID.Equals(result, StringComparison.CurrentCultureIgnoreCase));
                    if (cabinetNameExists && bFound)
                    {
                        CABINETNAMEVALUE += ticks.Substring(ticks.Length - 5, 5);
                    }
                    i++;
                }
            }
            return result;
        }

        public static bool Create(string cabinetName, ref string cabinetId, string username, TEMPLATE template, string dataPath, string cabinetTemplatePath)
        {
            var result = false;
            var cabinetNameValidated = cabinetName;
            //get a valid cabinetId
            //validate cabinet name

            cabinetId = GenerateCabinetId(cabinetName, ref cabinetNameValidated);

            //create all folder paths
            var paths = new List<string>() {
                Path.Combine(FileItDataPath, cabinetId),
                Path.Combine(FileItDataPath, cabinetId, "oStore"),
                Path.Combine(FileItDataPath, cabinetId, "tStore"),
                Path.Combine(FileItDataPath, cabinetId, "wStore")   
            };

            paths.ToList().ForEach(p =>
            {
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }
            });

            //copy the template to the correct location (rename in the process)
            var newCabinetFilePath = Path.Combine(FileItDataPath, cabinetId, cabinetId + ".FILEIT");
            File.Copy(EmptyCabinetFilePath, newCabinetFilePath);

            using (var db = new SystemFileitEntities())
            {
                if (!db.TEMPLATES.Any(t => t.TEMPLATENAME.Equals(template.TEMPLATENAME, StringComparison.CurrentCultureIgnoreCase)))
                {
                    template.DESCRIPTION = "Auto Template From SVC";
                    template.ACCOUNT = "[Default System Account]";
                    db.TEMPLATES.Add(template);
                    template.TemplateDefinitions.ToList().ForEach(td =>
                    {
                        td.TEMPLATENAME = template.TEMPLATENAME;
                        db.TEMPLATE_DEFINITION.Add(td);
                    });
                    db.SaveChanges();
                }
            }

            //add tables to the empty FILEIT db
            var newCabinet = new CABINET()
            {
                CABINETID = cabinetId,
                CABINETNAME = cabinetNameValidated,
                CABINETFULLPATH = newCabinetFilePath,
                ROOTPATH = Path.Combine(FileItDataPath, cabinetId),
                OSTOREPATH = Path.Combine(FileItDataPath, cabinetId, "oStore"),
                TSTOREPATH = Path.Combine(FileItDataPath, cabinetId, "tStore"),
                WSTOREPATH = Path.Combine(FileItDataPath, cabinetId, "wStore"),
                ACCOUNT = "Default System Account",
                ADMINISTRATOR = username,
                DESCRIPTION = "FileIt Service Created Cabinet",
                TEMPLATENAME = template.TEMPLATENAME
            };

            newCabinet.CreateTables();

            //create CABINET record
            using (var db = new SystemFileitEntities())
            {
                db.CABINETS.Add(newCabinet);
                newCabinet.SetAccess("user", username, true);
                db.SaveChanges();
            }
            result = true;

            return result;
        }



        private void CreateTables()
        {
            var cabSQL = "CREATE TABLE " + CABINETID + "_IMAGES" + " (" +
              " FILENAME VARCHAR(8) NOT NULL," +
              " EXTENSION VARCHAR(4) NOT NULL," +
              " ARCHIVED VARCHAR(3)," +
              " INDEXEDON TIMESTAMP NOT NULL," +
              " PAGENUMBER SMALLINT NOT NULL," +
              " FILESIZE INTEGER," +
              " PixelType VARCHAR(10)," +
              " PixelDepth VARCHAR(10)," +
              " Resolution VARCHAR(10)," +
              " PixelSizeX VARCHAR(25)," +
              " PixelSizeY VARCHAR(25)," +
              " Orientation VARCHAR(10)," +
              " Storageid VARCHAR(1) DEFAULT 'A', " +
              " Side VARCHAR(1) DEFAULT 'A', " +
              " PUBLIC VARCHAR(1) DEFAULT 'N', " +
              " VERSION_NO INTEGER DEFAULT 0, " +
              " CHECKEDOUT VARCHAR(1) DEFAULT 'N', " +
              " VERSIONID VARCHAR(20) DEFAULT '', " +
              " ALTSTORAGE VARCHAR(1) DEFAULT '', " +
              " DELETED VARCHAR(1) DEFAULT 'N', " +
              " USERID  VARCHAR(20) DEFAULT '', " +
              " STATIONID	VARCHAR(20) DEFAULT '', " +
              " BATCHNO	VARCHAR(20) DEFAULT '' ";

            //get template definition
            var idx = 1;
            Template.TemplateDefinitions.ToList().ForEach(td =>
            {
                switch (td.DATATYPE.ToLower())
                {
                    case "date":
                        cabSQL += ", INDEX" + idx + " DATE";
                        break;
                    case "number":
                        cabSQL += ", INDEX" + idx + " INTEGER";
                        break;
                    default:
                        cabSQL += ", INDEX" + idx + " VARCHAR(" + td.MAXLENGTH + ") NOT NULL";
                        break;
                }
                idx++;
            });

            //now add primary key
            cabSQL += ",     Primary Key(FILENAME)     );";

            FireBirdHelper.GenericCommand(cabSQL, CabinetConnectionString);
        }

        public bool Add(FileItDocument doc)
        {
            ExceptionHelper.LogDebug("In Add Cabinet Document...");
            var result = false;
            StringBuilder sql = new StringBuilder();

            int fileSize = 501;

            //add the document to the database
            sql.Append("INSERT INTO " + CABINETID + "_IMAGES ");
            sql.Append("(");
            sql.Append("FILENAME, DELETED, PAGENUMBER, INDEXEDON, EXTENSION, ARCHIVED, FILESIZE, PUBLIC, VERSION_NO, VERSIONID, USERID, STATIONID, BATCHNO");
            doc.Indexes.ToList().ForEach(i =>
            {
                sql.Append(", INDEX" + i.Key.ToString());
            });
            sql.Append(") VALUES (");
            sql.Append("'" + doc.FILENAME.Replace("'", "''") + "',");
            sql.Append("'" + (doc.DELETED ? "Y" : "N") + "',");
            sql.Append("'" + "1" + "',");
            sql.Append("'" + doc.INDEXEDON.ToString("MM/dd/yyyy") + "',");
            sql.Append("'" + doc.EXTENSION.ToString().Replace("'", "''") + "',");
            sql.Append("'" + doc.ARCHIVED.ToString().Replace("'", "''") + "',");
            sql.Append("'" + fileSize.ToString() + "',");
            sql.Append("'" + (doc.PUBLIC ? "Y" : "N") + "',");
            sql.Append("'" + doc.VERSION_NO.ToString().Replace("'", "''") + "',");
            sql.Append("'" + doc.VERSIONID.ToString().Replace("'", "''") + "',");
            sql.Append("'" + doc.USERID.ToString().Replace("'", "''") + "',");
            sql.Append("'" + doc.STATIONID.ToString().Replace("'", "''") + "',");
            sql.Append("'" + doc.BATCHNO.ToString().Replace("'", "''") + "'");
            ExceptionHelper.LogDebug(sql.ToString());
            doc.Indexes.ToList().ForEach(i =>
            {
                //i.Value.ToString().Replace("'", "''")
                sql.Append(", " + FireBirdHelper.FormatValue(i.Value));
            });
            sql.Append(")");

            //var t = sql.ToString();
            ExceptionHelper.LogDebug("SQL Insert:" + sql.ToString());

            if (FireBirdHelper.GenericCommand(sql.ToString(), CabinetConnectionString))
            {
                //save the file
                var folderPath = doc.AddImageFileFolderPath(OSTOREPATH);
                //make sure there are no extra . in the name
                var filePath = System.IO.Path.Combine(folderPath, doc.FILENAME + (doc.EXTENSION.IndexOf('.') > -1 ? "": ".") + doc.EXTENSION);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                FileHelper.Base64ToFile(filePath, doc.ImageBase64);
                result = true;
            }
            return result;
        }

        public bool SetAccess(string accesstype, string username, bool allowaccess)
        {
            bool result = false;
            using (var db = new FileItDataLayer.Models.SystemFileitEntities())
            {
                if (allowaccess)
                {
                    if (!db.CABINETS_ACCESS.Any(ca => ca.CABINETNAME == CABINETNAME
                              && ca.ACCESSTYPE.Equals(accesstype, StringComparison.CurrentCultureIgnoreCase)
                              && ca.ACCESSNAME.Equals(username, StringComparison.CurrentCultureIgnoreCase)
                        ))
                    {
                        var newaccess = new CABINETS_ACCESS()
                        {
                            CABINETNAME = CABINETNAME,
                            ACCESSTYPE = accesstype,
                            ACCESSNAME = username
                        };
                        db.CABINETS_ACCESS.Add(newaccess);
                    }
                }
                else
                {
                    var cabinetaccess = db.CABINETS_ACCESS.FirstOrDefault(ca => ca.CABINETNAME == CABINETNAME);
                    db.CABINETS_ACCESS.Remove(cabinetaccess);
                }
                db.SaveChanges();
                result = true;
            }

            return result;
        }
    }
}
