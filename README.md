# C-
learning C Sharp code for 
Directory of windows forms application (.exe)

[C#]
using System.IO;
using System.Windows.Forms;

string appPath = Path.GetDirectoryName(Application.ExecutablePath);


Directory of any loaded assembly (.exe or .dll)

[C#]
using System.IO;
using System.Reflection;

string path = Path.GetDirectoryName( Assembly.GetAssembly(typeof(MyClass)).CodeBase);


