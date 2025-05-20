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
        private static bool isGenerating = false;
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
                (selecionado, key) = GetPhonemeFrequencies(comando);
                Console.WriteLine($"Generating files for {comando} \n");

                for (int i = 0; i < selecionado.GetLength(0); i++)
                {
                    int threadIndex = i;
                    ThreadList.Add(new Thread(() =>
                    {
                        Thread.CurrentThread.Name = threadIndex.ToString();
                        System.Console.WriteLine($"Initiating thread number {Thread.CurrentThread.Name}");
                        var result = GenerateFormants(Frequencies.samples, selecionado, folderpath, threadIndex);
                        lock (result)
                        {
                        GenerateFile(result, threadIndex);
                        }
                    }));
                    
                    foreach(Thread thread in ThreadList)
                    {
                        thread.Start();
                        thread.Join();
                    }

                    lock (ThreadList)
                    {
                        ThreadList.Clear();
                    }

                    lock (fileLock)
                    {
                        System.Console.WriteLine("Finished generating files for phoneme " + i);
                    }
                }

                GenerateDebugWavs(selecionado.GetLength(0));
            }

            
        }

        public static List<byte> GenerateFormants(int samples, int[,] formantsFrequency, string folder, int line)
        {
            float[] sineWaves = new float[samples]; // Ensure sineWaves is properly initialized
            float[] compositeSineWAves = new float[samples];
            List<Thread> FormantsThreadsList = new List<Thread>();
            List<float> allSineWaves = new List<float>();
            List<byte> audioData = new List<byte>();


            for (int column = 0; column < formantsFrequency.GetLength(1); column++)
            {
                System.Console.WriteLine($"Generating formant {column} for phoneme {line}");
                for(int j = 0; j < samples; j++)
                {
                    float x = (float)j / samples;
                    sineWaves[j] = (float)Math.Sin(2 * Frequencies.pi * formantsFrequency[line, column] * x);
                    compositeSineWAves[j] += sineWaves[j];
                }
            }

            ConvertFloatToInt16(compositeSineWAves).ToList().ForEach(audioData.Add);

            lock (audioData)
            {
                return audioData;  
            }
        }

        static byte[] ConvertFloatToInt16(float[] samples)
        {
            var bytesArray = new byte[samples.Length * 2];
            for (int i = 0; i< samples.Length; i++)
            {
                var sample = samples[i];
                // Limitar entre -1 e 1
                sample = Math.Max(-1, Math.Min(1, sample));

                // Converter para short (16-bit)
                var shortSample = (short)(sample * 32767);

                // Converter para bytes
                bytesArray[i * 2] = (byte)(shortSample & 0xFF);
                bytesArray[i * 2 + 1] = (byte)(shortSample >> 8);
            };
            return bytesArray;
        }

        static void GenerateFile(List<byte> audioData, int i)
        {
            System.Console.WriteLine("Generating file for phoneme " + i);


            lock(fileLock)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "Phonemes", "vowels");
                string wavPath = Path.Combine(folder, $"phoneme{i}.bin");

                int sampleRate = 44100; // Adjust if needed
                int numChannels = 1;    // Mono audio
                int bitsPerSample = 16; // 16-bit audio
                int byteRate = sampleRate * numChannels * (bitsPerSample / 8);
                int blockAlign = numChannels * (bitsPerSample / 8);
                int subchunk2Size = audioData.Count;
                int chunkSize = 36 + subchunk2Size;

                using (FileStream fs = new FileStream(wavPath, FileMode.Create))
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    // RIFF Header
                    writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                    writer.Write(chunkSize);
                    writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

                    // fmt Chunk
                    writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
                    writer.Write(16);  // PCM format chunk size
                    writer.Write((short)1);  // Audio format (1 = PCM)
                    writer.Write((short)numChannels);
                    writer.Write(sampleRate);
                    writer.Write(byteRate);
                    writer.Write((short)blockAlign);
                    writer.Write((short)bitsPerSample);

                    // data Chunk
                    writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
                    writer.Write(subchunk2Size);
                    writer.Write(audioData.ToArray()); // Write the actual PCM data
                }
            }
        }


        static void GenerateDebugWavs(int quantidade)
        {
            for (int i = 0; i < quantidade; i++)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Phonemes", "vowels", $"phoneme{i}.bin");
                byte[] audioData = File.ReadAllBytes(path);
                File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "Phonemes", "vowels", "debug", $"phoneme{i}.wav"), audioData);
            }
        }
    }



}
