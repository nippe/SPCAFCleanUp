using System;
using System.IO;

namespace SPDisplayNameAdder
{
  public interface ILog
  {
    void Log(string Message);
  }

  public class FileLog : ILog
  {
    private ILog _decoratedLog;

    public FileLog() {}

    public FileLog(ILog decoratedLog) {
      _decoratedLog = decoratedLog;
    }

    public void Log(string Message) {
      using (var sw = new StreamWriter(".\\SPDisplayName.log", true)) {
        sw.Write("{0}\t", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        sw.WriteLine(Message);
      }
    }

  }


  public class ConsoleLog : ILog
  {
    private readonly ILog _decoratedLog;

    public ConsoleLog() {}

    public ConsoleLog(ILog decoratedLog) {
      _decoratedLog = decoratedLog;
    }

    public void Log(string Message) {
      if (_decoratedLog != null) {
        _decoratedLog.Log(Message);
      }

      Console.WriteLine(Message);
    }
  }
}