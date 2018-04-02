using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace Jeff.DB
{
    public class ODBC
    {
        public string Ssqlcmd;
        public string SsPath;
        public DataTable GetDataTable;
        
        private OdbcDataAdapter _adapter;
        private string _sDB;
        private string _sUser;
        private string _sPwd;

        public void ConnectDB()
        {
            //讀取ini by Source for DB
            _sDB = Actini.ReadIni("Source", "DB", SsPath);
            //讀取ini by Source for User
            _sUser = Actini.ReadIni("Source", "UID", SsPath);
            //讀取ini by Source for Pwd""
            _sPwd = Actini.ReadIni("Source", "pwd", SsPath);

            //宣告connectString 
            //string _sConnectString = "DSN=" + _sDB + ";UID=" + _sUser + ";PWD=" + _sPwd + ";";
            string _sConnectString = string.Format("DSN={0};UID={1};PWD={2};", _sDB, _sUser, _sPwd);
            //建立ODBC Adapter
            //System.Data.Odbc.OdbcDataAdapter adapter = new System.Data.Odbc.OdbcDataAdapter(Ssqlcmd, _sConnectString);
            _adapter = new OdbcDataAdapter(Ssqlcmd,_sConnectString);
            //建立一個DataTable
            //System.Data.DataTable GetDataTable = new System.Data.DataTable();
            GetDataTable = new DataTable();
            //adapter fill data into GetDataTable(DataTable)
            _adapter.Fill(GetDataTable);
        }

    }
}
