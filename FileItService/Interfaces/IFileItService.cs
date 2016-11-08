using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using FileItDataLayer.Models;
using FileItService.DTOs;

namespace FileItService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFileItService" in both code and config file together.
    [ServiceContract]
    public interface IFileItService
    {
        [OperationContract]
        bool InitService();

        [OperationContract]
        FileItResponse Authenticate(string user, string pass);

        [OperationContract]
        FileItResponse SetCabinetAccess(string user, string pass, string accesstype, string targetuser, string cabinetid, bool allowaccess);

        [OperationContract]
        FileItResponse AddUpdateUser(string user, string pass, FileItUser userobj);

        [OperationContract]
        FileItResponse SetUserActive(string user, string pass, string targetuser, bool active);

        [OperationContract]
        FileItResponse SetUserPassword(string user, string pass, string targetuser, string newpass);

        [OperationContract]
        Dictionary<string, string> GetCabinets(string user, string pass, string targetuser, bool allavailable);

        [OperationContract]
        FileItCabinet GetCabinet(string user, string pass, string cabinetId);

        [OperationContract]
        FileItResponse CreateCabinet(string user, string pass, FileItTemplate template, string cabinetName);

        [OperationContract]
        FileItResponse DeleteDocument(string user, string pass, string cabinetid, string documentId);

        [OperationContract]
        FileItResponse GetDocuments(string user, string pass, string cabinetid, List<DTOs.FileItDocumentLookup> lookups, bool includeThumbs);

        [OperationContract]
        FileItResponse GetDocumentsById(string user, string pass, List<FileItDocumentIdLookup> documentIds, bool includeThumbs, bool includeDeleted);

        [OperationContract]
        FileItResponse UploadDocuments(string user, string pass, string cabinetId, DTOs.FileItDocument[] documents);
    }
    /*
    [DataContract]
    public class FileItUser
    {
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string Pass { get; set; }
        [DataMember]
        public string Account { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Profile { get; set; }
        [DataMember]
        public bool ActiveUser { get; set; }
        [DataMember]
        public int PrintWidth { get; set; }

        public bool MergeWithDB(FileItDataLayer.Models.USER user)
        {
            //merge the dto with the database
            user.USERNAME = this.UserName;
            user.FULLNAME = this.FullName;
            user.PASS = this.Pass;
            user.ACCOUNT = this.Account;
            user.EMAIL = this.Email;
            user.DESCRIPTION = this.Description;
            user.PROFILE = this.Profile;
            user.ActiveUserBool = this.ActiveUser;
            user.PRINTWIDTH = this.PrintWidth;
            return true;
        }
    }

    [DataContract]
    public class FileItResponse
    {
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public FileItCabinet Cabinet { get; set; }
        [DataMember]
        public List<FileItDocument> Documents { get; set; }

    }

    [DataContract]
    public class FileItCabinet
    {
        [DataMember]
        public string CabinetId { get; set; }
        [DataMember]
        public string CabinetName { get; set; }
        [DataMember]
        public FileItTemplate Template { get; set; }
    }

    [DataContract]
    public class FileItTemplate
    {
        [DataMember]
        public string TemplateName { get; set; }
        [DataMember]
        public List<FileItTemplateDefinition> TemplateDefinitions { get; set; }

        public void ConvertFromTEMPLATE(TEMPLATE t)
        {
            this.TemplateName = t.TEMPLATENAME;
            this.TemplateDefinitions = new List<FileItTemplateDefinition>();
            t.TemplateDefinitions.ForEach(td => this.TemplateDefinitions.Add(ConvertFromTEMPLATEDEFINITION(td)));
        }

        public FileItTemplateDefinition ConvertFromTEMPLATEDEFINITION(TEMPLATE_DEFINITION td)
        {
            return new FileItTemplateDefinition()
           {
               INDEXNUM = td.INDEXNUM,
               INDEXNAME = td.INDEXNAME
           };
        }
    }

    [DataContract]
    public class FileItTemplateDefinition
    {
        [DataMember]
        public string TEMPLATENAME { get; set; }
        [DataMember]
        public string INDEXNAME { get; set; }
        [DataMember]
        public short INDEXNUM { get; set; }
        [DataMember]
        public Nullable<short> MAXLENGTH { get; set; }
        [DataMember]
        public string DATATYPE { get; set; }
        [DataMember]
        public string MUSTFILL { get; set; }
        [DataMember]
        public string MUSTENTER { get; set; }
        [DataMember]
        public string VIEWINDEX { get; set; }
        [DataMember]
        public string SEARCHINDEX { get; set; }
        [DataMember]
        public string OCRACTIVE { get; set; }
        [DataMember]
        public Nullable<decimal> XCOORDINATE { get; set; }
        [DataMember]
        public Nullable<decimal> YCOORDINATE { get; set; }
        [DataMember]
        public Nullable<decimal> ZONEHEIGHT { get; set; }
        [DataMember]
        public Nullable<decimal> ZONEWIDTH { get; set; }
        [DataMember]
        public Nullable<short> LINKEDFIELDID { get; set; }
        [DataMember]
        public string BARCODETYPE { get; set; }
        [DataMember]
        public string LOOKUP { get; set; }
        [DataMember]
        public string HIDECOLUMN { get; set; }

        //public static implicit operator FileItTemplateDefinition(TEMPLATE_DEFINITION td)
        //{
        //    return new FileItTemplateDefinition()
        //    {
        //        INDEXNUM = td.INDEXNUM,
        //        INDEXNAME = td.INDEXNAME
        //    };
        //}
    }
    [DataContract]
    public class FileItDocument
    {
        [DataMember]
        public string CabinetID { get; set; }
        [DataMember]
        public string ImageBase64 { get; set; }
        [DataMember]
        public string WebImageBase64 { get; set; }
        [DataMember]
        public string WebImageBase64Src { get; set; }
        [DataMember]
        public string WebImageThumbBase64 { get; set; }
        [DataMember]
        public string WebImageThumbBase64Src { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public Dictionary<int, string> IndexInformation { get; set; }
    }
    */
   /* [DataContract]
    public class FileItDocumentLookup
    {
        [DataMember]
        public int IndexNumber { get; set; }
        [DataMember]
        public string LookupValue { get; set; }
        [DataMember]
        public string Operator { get; set; }
    }*/
}
