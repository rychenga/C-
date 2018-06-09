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
        //public string Txt_path;  // file path
        private List<string> P_str_Name = new List<string>(); // table list 

        //取得Access 中含有那些Table list 
        public List<string> GetTableList(string _sConnectString)//對下拉列表進行資料繫結
        {
            //連接Access資料庫 & 打開資料庫連接
            //OleDbConnection olecon = OleDbOpenConn(Txt_path);
            OdbcConnection olecon = new OdbcConnection(_sConnectString);
            if (olecon.State == ConnectionState.Open) olecon.Close();
            olecon.Open();

            //連接Access資料庫
            DataTable DTable = olecon.GetSchema("Tables");//實例化表對像

            DataTableReader DTReader = new DataTableReader(DTable);//實例化表讀取對像
            while (DTReader.Read())//循環讀取
            {
                if (DTReader["Table_Type"].ToString()=="TABLE")//判斷TABLE_TYPE="TABLE"才GET
                P_str_Name.Add(DTReader["Table_Name"].ToString().Replace('$', ' ').Trim());//記錄工作表名稱
            }

            DTable = null;//清空表對像
            DTReader = null;//清空表讀取對像
            //olecon.Close();//關閉資料庫連接
            if (olecon.State == ConnectionState.Open) olecon.Close();//關閉資料庫連接
            return P_str_Name;//回傳List
        }


        //取得 MDB table raw data to DataTable
        public DataTable GetOdbcDataTable(string _sConnectString, string _sSqlcmd)
        {
            //建立一個DataTable
            DataTable myDataTable = new DataTable();

            //建立ODBC Adapter
            System.Data.Odbc.OdbcDataAdapter adapter = new System.Data.Odbc.OdbcDataAdapter(_sSqlcmd, _sConnectString);

            adapter.Fill(myDataTable);

            adapter.Dispose();//close adapter

            return myDataTable;//回傳DataTable
        }

        //將DataGridView Data sync to Access by ODBC
        public void ODBCSyncData(string _sConnectString, string tabName, DataTable Rows)
        {
            DataTable test = new DataTable();

            string query = string.Format("SELECT * FROM {0}", tabName);

            //連接Access資料庫 & 打開資料庫連接
            OdbcConnection olecon = new OdbcConnection(_sConnectString);
            if (olecon.State == ConnectionState.Open) olecon.Close();//關閉資料庫連接
            olecon.Open();

            //建立ODBC Adapter
            System.Data.Odbc.OdbcDataAdapter adapter = new System.Data.Odbc.OdbcDataAdapter(query, olecon);
            //adapter.SelectCommand = new OleDbCommand(query, icn);
            adapter.SelectCommand = new OdbcCommand(query, olecon);

            //OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter);
            //System.Data.Odbc.OdbcCommandBuilder builder = new OdbcCommandBuilder(adapter);
            OdbcCommandBuilder builder = new OdbcCommandBuilder(adapter);

            //// Without the OleDbCommandBuilder, this line would fail.
            //adapter.UpdateCommand = builder.GetUpdateCommand();
            //adapter.InsertCommand = builder.GetInsertCommand();
            //adapter.DeleteCommand = builder.GetDeleteCommand();

            adapter.Fill(test);

            adapter.Update(Rows); //Sync(Insert or Update or Delete ) to Access MDB file

            if (olecon.State == ConnectionState.Open) olecon.Close();//關閉資料庫連接
            adapter.Dispose();//close adapter
            Rows.Dispose();//釋放Rows(DataTable)
            test.Dispose();//釋放test(DataTable)
        }

    }
}
