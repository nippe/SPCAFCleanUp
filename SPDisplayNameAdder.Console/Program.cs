using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SPDisplayNameAdder
{
  class Program
  {

    static bool yesToAll = false;
    private static ILog logger;
    private static List<string> files = new List<string>();


    static void Main(string[] args)
    {
      if(args.Length != 1) {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Ange path till lösningens root foldeer");
        return;
      }

      logger = new ConsoleLog(new FileLog());

      string rootPath = args[0];
      DirectoryInfo rootFolder = new DirectoryInfo(rootPath);
      TraverseFoler(rootFolder);
      Console.WriteLine("\n\n{0} files changed", files.Count.ToString());
    }

    private static void TraverseFoler(DirectoryInfo currentFolder) {
      foreach (FileInfo fileInfo in currentFolder.GetFiles("Elements.xml",SearchOption.AllDirectories)) {
        AddDisplayNameToElements(fileInfo.FullName);
      }
    }

    private static void AddDisplayNameToElements(string fileName) {
      AskAndUnlockFileIfReadOnlyFlag(fileName);
      bool change = false;

      logger.Log("Loading " + fileName);
      XDocument doc = XDocument.Load(fileName);
      XNamespace ns = "http://schemas.microsoft.com/sharepoint/";
      IEnumerable<XElement> fieldRefs = doc.Descendants(ns + "FieldRef");
      
      foreach (XElement fieldRef in fieldRefs) {
        if(fieldRef.Attribute("DisplayName") == null) {
          logger.Log("Adding DiplayName for " + fieldRef.Attribute("Name").Value);
          fieldRef.Add(new XAttribute("DisplayName", fieldRef.Attribute("Name").Value));
          change = true;
        }

        if(fieldRef.Attribute("ID") != null && !fieldRef.Attribute("ID").Value.StartsWith("{")) {
          logger.Log("Added cuerly braces for " + fieldRef.Attribute("ID").Value);
          fieldRef.Attribute("ID").Value = String.Format("{{{0}}}", fieldRef.Attribute("ID").Value);
          change = true;
        }
      }

      if (change) {
        if(!files.Contains(fileName)) {
          files.Add(fileName);
        }
        logger.Log("Saving file");
        doc.Save(fileName, SaveOptions.None);
      }
    }



    private static void AskAndUnlockFileIfReadOnlyFlag(string fileName) {
      if (File.GetAttributes(fileName).HasFlag(FileAttributes.ReadOnly)) {
        if (!yesToAll) {
          Console.WriteLine("{0} is ReadOnly. Remove ReadOnly flag? [A]ll, [Y]es to this file or [N]o:", fileName);
          ConsoleKeyInfo answer = Console.ReadKey();
          if (answer.KeyChar == 'y' || answer.KeyChar == 'a') {
            if (answer.KeyChar == 'a') {
              yesToAll = true;
            }
            File.SetAttributes(fileName, File.GetAttributes(fileName) & ~FileAttributes.ReadOnly);
          }
        }
        else {
          File.SetAttributes(fileName, File.GetAttributes(fileName) & ~FileAttributes.ReadOnly);          
        }
      }
    }
  }
}
