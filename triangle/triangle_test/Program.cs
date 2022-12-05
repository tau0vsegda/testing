using System;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;

public class TriangleTests
{
    private const string PROGRAM_PATH = "D:\\study\\тестирование\\triangle\\triangle\\bin\\Debug\\net6.0\\triangle.exe";
    private const string TESTCACES_PATH = "D:\\study\\тестирование\\triangle\\triangle_test\\testCases.txt";
    private const string RESULT_PATH = "D:\\study\\тестирование\\triangle\\triangle_test\\Result.txt";

    private static string outputLine = "";

    private static void ProcOutHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (!String.IsNullOrEmpty(outLine.Data))
        {
            outputLine = outLine.Data;
        }
    }

    private static void StartProcessAndGetOutputData(String args)
    {
        var proc = new Process();
        proc.StartInfo.FileName = PROGRAM_PATH;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.OutputDataReceived += ProcOutHandler;
        proc.StartInfo.Arguments = args;
        proc.Start();
        proc.BeginOutputReadLine();
        proc.WaitForExit();
        proc.Close();
    }

    private static bool CheckOutput(String expectedOutput)
    {
        return outputLine == expectedOutput;
    }

    public static void Main()
    {
        try
        {
            using StreamReader reader = new StreamReader(TESTCACES_PATH);
            using StreamWriter writer = new StreamWriter(RESULT_PATH);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                List<string> testCase = new(line.Split(';'));
                if (testCase.Count() == 2)
                {
                    StartProcessAndGetOutputData(testCase[0]);
                    if (CheckOutput(testCase[1].Trim()))
                    {
                        writer.WriteLine("Sucсess");
                    }
                    else
                    {
                        writer.WriteLine("Error");
                    }
                }


            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

}