using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using SoftwareAutomaticMouthUniversalEreaLanguage.Phonemes;
using System.Threading;

namespace Phonemes
{
    public class GenerationManager
    {
        private static readonly object fileLock = new object();
        public static string folderpath = Path.Combine(Directory.GetCurrentDirectory(), "Phonemes", "vowels");
        public static List<Thread> ThreadList = new List<Thread>();
        public static (int[,], string keyFinal) GetPhonemeFrequencies(string key)
        {
            if (Frequencies.dicionario.TryGetValue(key, out int[,] frequencies) && frequencies != null)
            {
                return (frequencies, key);
            }
            else
            {
                Console.WriteLine("erro: " + frequencies);
                throw new ArgumentException($"Invalid key: {key}");
            }
        }

        public static void GenerateFiles(string[] comandos)
        {
            ThreadList.Clear();
            int[,] selecionado;
            string key;
            foreach (string comando in comandos)
            {
                Console.WriteLine($"Generating files for {comando} \n");
                (selecionado, key) = GetPhonemeFrequencies(comando);
                Console.WriteLine($"Generating files for {key} \n");

                for (int i = 0; i < selecionado.GetLength(0); i++)
                {
                    System.Console.WriteLine(i);
                    ThreadList.Add(new Thread(() =>
                    {
                        Thread.CurrentThread.Name = i.ToString();
                        System.Console.WriteLine(Thread.CurrentThread.Name);
                        var result = GenerateFormants(Frequencies.samples, selecionado, folderpath);
                        lock (result)
                        {
                        GenerateFile(result, i);
                        }
                    }));
                }
            }
            foreach (Thread thread in ThreadList)
            {
                thread.Start();
            }
            
        }

        public static List<byte> GenerateFormants(int samples, int[,] formantsFrequency, string folder)
        {
            float[] sineWaves = new float[samples]; // Ensure sineWaves is properly initialized
            List<Thread> FormantsThreadsList = new List<Thread>();
            List<float> allSineWaves = new List<float>();
            List<byte> audioData = new List<byte>();

            for (int i = 0; i < formantsFrequency.GetLength(1); i++)
            {
                FormantsThreadsList.Add(new Thread(() =>
                {
                Thread.CurrentThread.Name = i.ToString();
                Console.WriteLine(Thread.CurrentThread.Name); 
                var result = GenerateFormant(samples, Thread.CurrentThread.Name ?? "0", formantsFrequency);
                lock (allSineWaves)
                {
                    allSineWaves.AddRange(result);
                    audioData[i] = ConvertFloatToInt16(allSineWaves.ToArray())[i];
                }
                }));
            };
            foreach (Thread thread in FormantsThreadsList)
            {
                thread.Start();
            }
            return audioData;
        }

        static List<float> GenerateFormant(int samples, string ThreadName, int[,] formantsfrequency) 
        {
            float[] sineWave = new float[samples];
            int ThreadNumber = int.Parse(ThreadName);
            List<float> SineWaves = new List<float>();

            if (!int.TryParse(Thread.CurrentThread.Name, out int ThreadColumn))
            {
                ThreadColumn = 0; // Evita erro se o nome for inválido
            }

            int maxRows = formantsfrequency.GetLength(0);
            int maxColumns = formantsfrequency.GetLength(1);

            if (ThreadNumber < 0 || ThreadNumber >= maxRows ||
                ThreadColumn < 0 || ThreadColumn >= maxColumns)
            {
                Console.WriteLine($"Erro: Índices fora dos limites! ThreadNumber={ThreadNumber}, ThreadColumn={ThreadColumn}");
                return new List<float>(); // Evita erro retornando lista vazia
            }
            for (int i = 0; i < samples; i++)
            {
                float x = (float)i / samples;
                sineWave[i] = (float)Math.Sin(2 * Frequencies.pi * formantsfrequency[ThreadNumber, ThreadColumn] * x);
                SineWaves.Add(sineWave[i]);
            }
            return SineWaves;

        }
        static byte[] ConvertFloatToInt16(float[] samples)
        {
            var bytesArray = new byte[samples.Length * 2];
            Parallel.For(0, samples.Length, i =>
            {
                var sample = samples[i];
                // Limitar entre -1 e 1
                sample = Math.Max(-1, Math.Min(1, sample));

                // Converter para short (16-bit)
                var shortSample = (short)(sample * 32767);

                // Converter para bytes
                bytesArray[i * 2] = (byte)(shortSample & 0xFF);
                bytesArray[i * 2 + 1] = (byte)(shortSample >> 8);
            });
            return bytesArray;
        }

        static void GenerateFile(List<byte> audioData, int i)
        {
            lock(fileLock)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Phonemes", "vowels", $"phoneme{i}.bin");
                File.WriteAllBytes(path, audioData.ToArray());
            }
        }
    }
}
