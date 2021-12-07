﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

namespace mltoolcli
{
    internal static class Program
    {
        private static double[] ModelContent;
        private static string ModelName;
        private static string ModelPath;
        
        private static string DatasetName;
        private static string DatasetPath;
        private static int[] DatasetContent;
        
        private static void Main(string[] args)
        {
            if (args.Length is 0)
            {
                PrintHelp("help");
                return;
            }

            switch (args[0])
            {
                case "help":
                        PrintHelp("help");
                    break;
                
                case "new":
                    if (args.Length is 3)
                    {
                        switch (args[1])
                        {
                            case @"model":
                                string modelPath =  $"{AppContext.BaseDirectory}{(args[2].EndsWith(".model") ? args[2] : $"{args[2]}.model")}";
                                File.Create(modelPath).Close();
                                CallScript(@"newmodel", modelPath);
                                break;
                            
                            case @"dataset":
                                string datasetPath = $"{AppContext.BaseDirectory}{(args[2].EndsWith(".dataset") ? args[2] : $"{args[2]}.dataset")}";
                                File.Create(datasetPath).Close();
                                CallScript(@"newdataset", datasetPath);
                                break;
                            
                            default:
                                PrintHelp("new");
                                break;
                        }
                    }
                    else
                        PrintHelp("new");
                    break;

                case "eval":
                    if (args.Length is 2 && double.TryParse(args[1], out double numberInput))
                        Calculate(numberInput);
                    else
                        PrintHelp("eval");
                    break;
                
                case "train":
                    break;
                
                case "load":
                    break;

                default:
                    Console.WriteLine(@"mltool: '{0}' geçerli bir komut değil", args[0]);
                    break;
            }
        }

        /// <summary>
        /// Calls python script from "pyscripts" folder in base directory of running program
        /// </summary>
        /// <param name="scriptName">Name of needed script</param>
        /// <param name="additionalArgs">Any additional arguments, these will be concatenated to script path</param>
        /// <exception cref="FileNotFoundException">May throw this exception if script file is not found</exception>
        /// <exception cref="NotImplementedException"></exception>
        private static void CallScript(string scriptName, string additionalArgs)
        {
            if (!File.Exists($"{AppContext.BaseDirectory}pyscripts/{scriptName}.py") && !File.Exists(AppContext.BaseDirectory + $"pyscripts\\{scriptName}.py"))
                throw new FileNotFoundException(TurkishStrings.ExcpMsg_ScriptNotFound, AppContext.BaseDirectory + scriptName);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                ProcessStartInfo info = new() { FileName = "python3", Arguments = $"{AppContext.BaseDirectory}pyscripts/{scriptName}.py {additionalArgs}" };
                Process prc = new() { StartInfo = info };
                prc.Start();   
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotImplementedException();
            }
            else
            {
                Console.WriteLine(@"mltool: Ewww");
            }
        }

        private static void Calculate(double number)
        {
            if (ModelContent is null || ModelName is null)
                Console.WriteLine("mltool: [HATA] Model ismi veya model içeriği bulunamadı. Modeli 'load' komutu ile yüklediğinize emin misiniz?");
            else
            {
                double result = 0;
                for (int i = 0; i < 6; i++)
                    result += ModelContent[i] * Math.Pow(number, 5 - i);
                Console.WriteLine($@"{ModelName}({number}) = {result}");
            }
        }
        
        private static void PrintHelp(string command)
        {
            switch (command)
            {
                case "help":
                    Console.WriteLine(TurkishStrings.Version);
                    Console.WriteLine(TurkishStrings.HelpMsg_Usage);
                    Console.WriteLine('\n' + TurkishStrings.HelpMsg_ListName);
                    Console.WriteLine(TurkishStrings.HelpMsg_Exit);
                    Console.WriteLine(TurkishStrings.HelpMsg_Help);
                    Console.WriteLine(TurkishStrings.HelpMsg_New);
                    Console.WriteLine(TurkishStrings.HelpMsg_Test);
                    Console.WriteLine(TurkishStrings.HelpMsg_Train);
                    Console.WriteLine(TurkishStrings.HelpMsg_Load);
                    Console.WriteLine('\n' + TurkishStrings.HelpMsg_Info);
                    break;
                
                case "new":
                    Console.WriteLine(@"mltool new:");
                    Console.WriteLine(TurkishStrings.Syntax_New0);
                    Console.WriteLine(TurkishStrings.Syntax_New1);
                    Console.WriteLine(TurkishStrings.Syntax_New2);
                    break;
                
                case "eval":
                    Console.WriteLine(@"mltool eval:");
                    Console.WriteLine(TurkishStrings.Syntax_Eval0);
                    Console.WriteLine(TurkishStrings.Syntax_Eval1);
                    break;
                
                case "train":
                    Console.WriteLine(@"mltool train:");
                    Console.WriteLine(TurkishStrings.Syntax_Train0);
                    Console.WriteLine(TurkishStrings.Syntax_Train1);
                    break;
                
                case "load":
                    Console.WriteLine(@"mltool load:");
                    Console.WriteLine(TurkishStrings.Syntax_Load0);
                    Console.WriteLine(TurkishStrings.Syntax_Load1);
                    Console.WriteLine(TurkishStrings.Syntax_Load2);
                    break;
            }
        }
    }
}