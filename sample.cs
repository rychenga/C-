[C#視窗]Ado.net連接Access資料庫的四種方式

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
