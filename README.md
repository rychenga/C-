# C-
出處：https://dotblogs.com.tw/mis2000lab/2008/08/15/4919

ADO.NET有兩大重點，分別是 DataReader與DataSet（資料集）這兩種。跟舊版的ASP不同，當年的ADO都是使用 RecordSet（資料錄集）。 

Connection、Command、DataReader和 DataAdapter物件表示 .NET Framework 資料提供者（DataProvider）模型的核心項目。下列表格說明這些物件。 


物件                  說明
Connection            建立連接（連線）至特定資料來源（例如：資料庫）。
Command               對資料來源執行命令，尤其是SQL指令。
DataReader            從資料來源讀取「順向」且「唯讀」的資料流。
DataAdapte           （DataSet資料集）將資料來源整個填入（.Fill()方法）到 DataSet。 或是把 DataSet更新（.Update()方法）之後的資料，回傳至資料來源。


ADO.NET有兩大重點，分別是 DataReader與DataSet（資料集）這兩種。跟舊版的ASP不同，當年的ADO都是使用 RecordSet（資料錄集）。

關於ADO.NET的DataReader與DataSet，我們簡單說明如下：

DataReader---- 很類似以前ADO的RecordSet，但資料指標只能循序向前（Forward）無法後退，所以無法撰寫「分頁」的功能。優點是效率高、速度快，很適合用來作為單純的工作，例如：展示大量資料。
 
DataSet   ---- 一種離線運作的資料庫快取，存放在主機的記憶體裡面。DataSet是透過DataAdapter（資料配接器）來執行SQL指令，功能非常強大，幾乎就是資料庫的翻版（如：DataView、DataTable、Relationship等等都作得到）。
可以把資料庫複製到主機的記憶體裡面，如此一來面對關係複雜的多重資料表，就能快速地處理。缺點是：使用DataSet比較消耗資源。

如果要修改資料的話，也是先修改DataSet（主機記憶體）裡面的資料，然後再回寫到資料庫裡面。
在ADO.NET裡頭，傳統的分頁功能都是用DataSet來完成。資料來源控制項預設值都採用DataSet（如SqlDataSource預設的DataSourceMode = DataSet）。

把上述的觀念，我們用表格來比較如下： 


（資料來源，以MS SQL Server為例）                                                            ADO.NET兩大物件的比較
                                           DataSet                                                                                                 DataReader
連接資料庫（Connection）                  （不需要，因為SqlDataAdapter會自動開啟連結，使用後自動關閉）                                             SqlConnection.Open()
                                           請看：[ADO.NET]DataSet與DataAdapter，為何不需要自己寫程式去開啟資料庫連線與關閉之？
                                           
執行SQL指令                                SqlDataAdapter                                                                                          SqlCommand
1. Select                                  1. .Fill()方法                                                                                          1. .ExecuteReader()方法  
2. Delete/Update/Insert                    2. .Update()方法                                                                                        2. .ExecuteNonQuery()方法

資料指標的 移動
                                           DataSet類似資料庫行為的資料快取。這些資料將存放在記憶體裡面，所以可以自由靈活地操作內部資料。           讀取資料時，只能「唯讀、順向（Forward）」的動作。

如何處理資料庫與 資料表？                  可以處理複雜的資料庫關聯與多個DataTable、DataView。                                                     透過使用者自訂的SQL指令來存取。適合處理單一的資料表。

消耗資源                                   較大                                                                                                    小，而且快速


分頁功能（Paging）                         有                                                                                                      無

