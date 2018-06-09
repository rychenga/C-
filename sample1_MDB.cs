using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace Jeff.DB
{
    public class MDB
    {
        //public string Txt_path;  // file path
        private List<string> P_str_Name = new List<string>(); // table list 

        //連線Access MDB
        private static OleDbConnection OleDbOpenConn(string Database)
        {
            //連接Access資料庫
            string cnstr = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Database);
            OleDbConnection icn = new OleDbConnection();
            icn.ConnectionString = cnstr;
            if (icn.State == ConnectionState.Open) icn.Close();
            icn.Open();//打開資料庫連接
            return icn;//回傳OleDbConnection
        }

        //將DataGridView Data sync to Access
        public void OleDbSyncData(string Database, string tabName, DataTable Rows)
        {
            DataTable test = new DataTable();

            //連接Access資料庫 & 打開資料庫連接
            OleDbConnection icn = OleDbOpenConn(Database);

            string query = string.Format("SELECT * FROM {0}", tabName);
            OleDbDataAdapter adapter = new OleDbDataAdapter(query, icn);
            adapter.SelectCommand = new OleDbCommand(query, icn);
            

            OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter);

            adapter.Fill(test);

            adapter.Update(Rows); //Sync(Insert or Update or Delete ) to Access MDB file

            //test.AcceptChanges();
            //Rows.AcceptChanges(); //AcceptChanges這道會把那Flag都清掉，因此對Adapter來說 那個DatatTable就視為沒變動過.

            adapter.Dispose();//close adapter
            if (icn.State == ConnectionState.Open) icn.Close();//關閉資料庫連接
            Rows.Dispose();//釋放Rows(DataTable)
            test.Dispose();//釋放test(DataTable)
        }

        //對資料表進行新增、修改及刪除等功能
        public static void OleDbInsertUpdateDelete(string Database, string OleDbSelectString)
        {
            //連接Access資料庫 & 打開資料庫連接
            OleDbConnection icn = OleDbOpenConn(Database);
            OleDbCommand cmd = new OleDbCommand(OleDbSelectString, icn);
            cmd.ExecuteNonQuery();
            if (icn.State == ConnectionState.Open) icn.Close();//關閉資料庫連接
        }

        //取得 MDB table raw data to DataTable
        public DataTable GetOleDbDataTable(string Database, string SQL)
        {
            //建立一個DataTable
            DataTable myDataTable = new DataTable();

            //連接Access資料庫 & 打開資料庫連接
            OleDbConnection icn = OleDbOpenConn(Database);

            //建立OLEDB Adapter
            OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, icn);
            adapter = new OleDbDataAdapter(SQL, icn);

            //da adapter fill data into ds(DataTable)
            adapter.Fill(myDataTable);

            adapter.Dispose();//close adapter
            if (icn.State == ConnectionState.Open) icn.Close();//關閉資料庫連接

            return myDataTable;//回傳DataTable
        }

        //取得Access 中含有那些Table list 
        public List<string> GetTableList(string Txt_path)//對下拉列表進行資料繫結
        {
            //連接Access資料庫 & 打開資料庫連接
            OleDbConnection olecon = OleDbOpenConn(Txt_path);

            //連接Access資料庫
            System.Data.DataTable DTable = olecon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });//實例化表對像
            //System.Data.DataTable DTable = olecon.GetSchema("Tables");//實例化表對像
            //System.Data.DataTable DTable = olecon.GetSchema("Tables", new string[] { null, null, null, "TABLE" });//實例化表對像

            DataTableReader DTReader = new DataTableReader(DTable);//實例化表讀取對像
            while (DTReader.Read())//循環讀取
            {
                P_str_Name.Add(DTReader["Table_Name"].ToString().Replace('$', ' ').Trim());//記錄工作表名稱
            }

            DTable = null;//清空表對像
            DTReader = null;//清空表讀取對像
            //olecon.Close();//關閉資料庫連接
            if (olecon.State == ConnectionState.Open) olecon.Close();//關閉資料庫連接
            return P_str_Name;//回傳List
        }

    }
}
