using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class ColorConsole
{

    public static void Default(string _messege)
    {
        DebugManager.instance.EnqueMessege(_messege);
        //Console.WriteLine(_messege);
    }

    public static void SystemDebug(string _messege)
    {
        DebugManager.instance.EnqueSystemMsg(_messege);
    }

    public static void ConsoleColor(string _messege)
    {
        Console.ForegroundColor = System.ConsoleColor.DarkBlue;
        Console.WriteLine(_messege);
        Console.ResetColor();
    }


}

