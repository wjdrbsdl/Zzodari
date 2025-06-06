using System;


public class ColorConsole
{

    public static void Default(string _messege)
    {
        DebugManager.instance.EnqueMessege(_messege);
        //Console.WriteLine(_messege);
    }

    public static void RuleWarning(string _msg)
    {
        DebugManager.instance.EnqueRuleMessege(_msg);
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

