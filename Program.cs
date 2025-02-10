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
using NAudio.CoreAudioApi;
using SoftwareAutomaticMouthUniversalEreaLanguage.Phonemes;


namespace Terminal 
{    
    class Program
    {
        public static string[,] commands =
        {
            {"generate:", " generates the .bin file for all available phoneme"},
            {"help:", " displays this menu"}
        };
        static void Main(string[] args) 
        {

            if (args.Length == 0)
            {
                Console.WriteLine("Insert a command, in case you do not know any, run 'dotnet run help'");
            }
            else
            {
                switch(args[0])
                {
                    case "vowels": 
                    Phonemes.GenerationManager.GenerateFiles(args);
                    break;

                    case "help":
                    Console.WriteLine("Command list: \n");
                    for (int i = 0; i < commands.GetLength(0); i++)
                    {
                        Console.WriteLine(commands[i, 0] + commands[i, 1]);
                    }
                    break;
                    case "convert":
                    for (int i = 0; i < 16; i++)
                    {
                        ConvertBinToWave($"Phonemes/vowels/phoneme{i}.bin", $"wavs/phoneme{i}.wav");
                    }
                    break;
                }
            }



        }

        static void ConvertBinToWave(string binPath, string wavePath)
        {
            WaveFormat waveFormat;
            using (WaveFileWriter writer = new WaveFileWriter(wavePath, waveFormat = new WaveFormat(Frequencies.sampleRate, 16, 1)))
            {
                byte[] audioData = File.ReadAllBytes(binPath);
                writer.Write(audioData, 0, audioData.Length);
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
