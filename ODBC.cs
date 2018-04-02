using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jeff.DB
{
    public class ODBC
    {
        public string Ssqlcmd;
        public string SsPath;
        public List<string> columns = new List<string>();
        public List<List<string>> rows = new List<List <string>>();

        private List<string> _tempRows = new List<string>();
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
            System.Data.Odbc.OdbcDataAdapter adapter = new System.Data.Odbc.OdbcDataAdapter(Ssqlcmd, _sConnectString);
            //建立一個DataTable
            System.Data.DataTable GetDataTable = new System.Data.DataTable();
            //adapter fill data into GetDataTable(DataTable)
            adapter.Fill(GetDataTable);

            //get columns name
            foreach (System.Data.DataColumn col in GetDataTable.Columns)
            {
                columns.Add(col.ToString());
            }

            //get rowdata by 2 Array
            foreach (System.Data.DataRow pri in GetDataTable.Rows)
            {    
                _tempRows = new List<string>();

                foreach (System.Data.DataColumn col in GetDataTable.Columns)
                {
                    //Console.WriteLine("{0}:{1}", col, pri[col]);
                    _tempRows.Add(pri[col].ToString());
                }
                rows.Add(_tempRows);
            }

        }

    }
}
