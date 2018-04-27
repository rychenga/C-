//http://einboch.pixnet.net/blog/post/245703881-c%23%E5%B0%8Daccess%E6%AA%94%E6%A1%88%E9%80%B2%E8%A1%8C%E5%A2%9E%E3%80%81%E5%88%AA%E3%80%81%E6%9F%A5%E3%80%81%E6%94%B9%E5%8A%9F%E8%83%BD
//http://j796160836.pixnet.net/blog/post/26514348-%5Bc%23%E8%A6%96%E7%AA%97%5Dado.net%E9%80%A3%E6%8E%A5access%E8%B3%87%E6%96%99%E5%BA%AB%E7%9A%84%E5%9B%9B%E7%A8%AE%E6%96%B9%E5%BC%8F
//[C#視窗]Ado.net連接Access資料庫的四種方式

//***Access(ODBC Version)
//[OdbcConnection->OdbcDataAdapter->DataSet->dataGridView1]
string strSQL ="Select * from table";
try
{
IDbConnection dbConn = new System.Data.Odbc.OdbcConnection(@"Driver={Microsoft Access Driver (*.mdb)};DBQ=|DataDirectory|\database.mdb");
dbConn.Open();
IDbDataAdapter dbAdapter = new System.Data.Odbc.OdbcDataAdapter(strSQL, (System.Data.Odbc.OdbcConnection)dbConn);
System.Data.DataSet dbSet = new System.Data.DataSet();
dbAdapter.Fill(dbSet);
dataGridView1.DataSource = dbSet.Tables[0];
}
catch (System.Data.Odbc.OdbcException ex)
{
MessageBox.Show(ex.ToString());
}
//***Access(ODBC Version)
//[OdbcConnection->OdbcCommand->DataTable->dataGridView1]
int i;
System.Data.DataTable dt = new DataTable();
System.Data.DataRow dr;
string strSQL ="Select * from table";
try
{
IDbConnection dbConn = new System.Data.Odbc.OdbcConnection(@"Driver={Microsoft Access Driver (*.mdb)};DBQ=|DataDirectory|\database.mdb");
dbConn.Open();
IDbCommand dbCommand = new System.Data.Odbc.OdbcCommand(strSQL, (System.Data.Odbc.OdbcConnection)dbConn);
IDataReader dbReader = dbCommand.ExecuteReader();
//
for (i = 0; i < dbReader.FieldCount; i++)
dt.Columns.Add(newDataColumn(dbReader.GetName(i)));
//
while (dbReader.Read())
{
dr = dt.NewRow();
for (i = 0; i < dbReader.FieldCount; i++)
dr[i] = dbReader[i];
dt.Rows.Add(dr);
}
}
catch (System.Data.Odbc.OdbcException ex)
{
MessageBox.Show(ex.ToString());
}
dataGridView1.DataSource = dt;
//***Access(OleDB Version)
//[OleDbConnection->OleDbDataAdapter->DataSet->dataGridView1]
string strSQL="Select * from table";
try
{
IDbConnection dbConn = new System.Data.OleDb.OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\database.mdb;Persist Security Info=True");
IDbDataAdapter dbAdapter = new System.Data.OleDb.OleDbDataAdapter(strSQL, (System.Data.OleDb.OleDbConnection)dbConn);
System.Data.DataSet dbSet = new System.Data.DataSet();
dbAdapter.Fill(dbSet);
dataGridView1.AutoGenerateColumns = true;
dataGridView1.DataSource = dbSet.Tables[0];
}
catch (System.Data.OleDb.OleDbException ex)
{
MessageBox.Show(ex.ToString());
}
//***Access(OleDB Version)
//[OleDbConnection->OleDbCommand->DataTable->dataGridView1]
int i;
System.Data.DataTable dt = new DataTable();
System.Data.DataRow dr;
string strSQL ="Select * from table";
try
{
IDbConnection dbConn = new System.Data.OleDb.OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\database.mdb;Persist Security Info=True");
dbConn.Open();
IDbCommand dbCommand = new System.Data.OleDb.OleDbCommand(strSQL, (System.Data.OleDb.OleDbConnection)dbConn);
IDataReader dbReader = dbCommand.ExecuteReader();
//
for (i = 0; i < dbReader.FieldCount; i++)
dt.Columns.Add(newDataColumn(dbReader.GetName(i)));
//
while (dbReader.Read())
{
dr = dt.NewRow();
for (i = 0; i < dbReader.FieldCount; i++)
dr[i] = dbReader[i];
dt.Rows.Add(dr);
}
}
catch (System.Data.Odbc.OdbcException ex)
{
MessageBox.Show(ex.ToString());
}
dataGridView1.DataSource = dt;
