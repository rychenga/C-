# C-
learning C Sharp code

http://mogerwu.pixnet.net/blog/post/28856780-c%23%E7%A8%8B%E5%BC%8F%E4%B8%AD%E5%AF%ABlog

  public static class EventLog {
      public static string FilePath { get; set; }
   
      public static void Write(string format, params object[] arg) {
          Write(string.Format(format, arg));
      }
   
      public static void Write(string message) {
          if (string.IsNullOrEmpty(FilePath)) {
              FilePath = Directory.GetCurrentDirectory();
          }
          string filename = FilePath + 
              string.Format("\\{0:yyyy}\\{0:MM}\\{0:yyyy-MM-dd}.txt", DateTime.Now);
          FileInfo finfo = new FileInfo(filename);
          if (finfo.Directory.Exists == false) {
              finfo.Directory.Create();
          }
          string writeString = string.Format("{0:yyyy/MM/dd HH:mm:ss} {1}", DateTime.Now, message) + Environment.NewLine;
          File.AppendAllText(filename, writeString, Encoding.Unicode);
      }
  }
