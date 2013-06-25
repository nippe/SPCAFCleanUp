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
    static void Main(string[] args)
    {
      if(args.Length != 1) {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Ange path till lösningens root foldeer");
        return;
      }

      string rootPath = args[0];

      DirectoryInfo rootFolder = new DirectoryInfo(rootPath);

      TraverseFoler(rootFolder);


    }

    private static void TraverseFoler(DirectoryInfo currentFolder) {
      foreach (FileInfo fileInfo in currentFolder.GetFiles("*.xml",SearchOption.AllDirectories)) {
        AddDisplayNameToElements(fileInfo.FullName);
      }
    }

    private static void AddDisplayNameToElements(string fileName) {
      if(File.GetAttributes(fileName).HasFlag(FileAttributes.ReadOnly)) {
        Console.WriteLine("{0} is ReadOnly");
        if(Console.ReadLine() == "y") {
          File.SetAttributes(fileName, File.GetAttributes((fileName) & ~FileAttributes.ReadOnly));
        }
      }
      XDocument doc = XDocument.Load(fileName);
      IEnumerable<XElement> fieldRefs = doc.Document.Descendants("FieldRef");
      foreach (XElement fieldRef in fieldRefs) {
        fieldRef.Add(new XAttribute("DisplayName", fieldRef.Attribute("Name").Value));
      }
      doc.Save(fileName, SaveOptions.None);
    }
  }
}
