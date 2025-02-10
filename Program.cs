// See https://aka.ms/new-console-template for more information

using System;
using System.Reflection;
using NAudio.Wave;
using System.Numerics;
using NAudio.MediaFoundation;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Net.NetworkInformation;
using System.Threading.Tasks;


namespace Terminal 
{    
    class Program
    {
        public static string[,] commands =
        {
            {"generate:", " generates the .bin file for all available phoneme"},
            {"help:", " displays this menu"}
        };
        static async Task Main(string[] args) 
        {

            if (args.Length == 0)
            {
                Console.WriteLine("Insert a command, in case you do not know any, run 'dotnet run help'");
            }
            else
            {
                switch(args[1])
                {
                    case "generate": 
                    string[] comandos = new string[args.Length - 1];
                    Array.Copy(args, comandos, 1);
                    await Phonemes.GenerationManager.GenerateFiles(comandos);
                    break;

                    case "help":
                    Console.WriteLine("Command list: \n");
                    for (int i = 0; i < commands.GetLength(0); i++)
                    {
                        Console.WriteLine(commands[i, 0] + commands[i, 1]);
                    }
                    break;
                }
            }



        }
    }
}

    



    
    /*
    static double BandPassFilter(double input, double centerFreq, int sampleRate, double bandwidth)
    {
        double omega = 2 * Math.PI * centerFreq / sampleRate;
        double alpha = Math.Sin(omega) * Math.Sinh(Math.Log(2) / 2 * bandwidth * omega / Math.Sin(omega));
        double a0 = 1 + alpha;
        double b0 = alpha / a0;
        return input * b0; // Simplified bandpass effect
    }

    static void SaveWave(float[] buffer, int sampleRate)
    {
        for (int i = 1; i < 5; i++) //itera pra criar 5 arquivos
        {

            Console.WriteLine("teste");
        }
    }
}

*/
