// See https://aka.ms/new-console-template for more information

using System;
using System.Reflection;
using NAudio.Wave;
using System.Numerics;
using NAudio.MediaFoundation;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Net.NetworkInformation;


namespace Principal 
{    
    class Program
    {
        static void Main() 
        {
            Formants formantes = new Formants();



            static List<float> doOneSecond(int samples, double baseFrequency, int phenome, float[,] formants) 
            {
                var glottalWave = new List<float>(samples*3); 
                double pi = Math.PI;
                float[,] sineWave = new float[2, samples];
                int[] Freq = new int[2];

                for (int i = 0; i < 2; i++)
                {
                    Freq[i] =  Formants.formantsFrequency[phenome, i];
                    Console.WriteLine(Freq[i]);
                }

                using (var reader = new BinaryReader(File.Open("dados.bin", FileMode.Open)))
                {
                    int tamanho = reader.ReadInt32();
                    float[] frequenciaBase = new float[tamanho];
                    
                    for (int i = 0; i < tamanho; i++)
                    {
                        frequenciaBase[i] = reader.ReadSingle();
                    }
                }

                Parallel.For(0, 2, i =>
                {

                    for (int j = 0; j < samples; j++)
                    {
                        float T = (float)j / samples;
                        sineWave[i, j]= (float)Math.Sin(2 * pi * T * Freq[i]);


                    }
                    lock(glottalWave)
                    {
                        for (int j = 0; j < samples; j++)
                        {
                            glottalWave.AddRange(sineWave[i, j]); 
                        }
                    }
                    Console.WriteLine(glottalWave[i]);
                });
                return glottalWave;
            }


            /*
            for (int i = 0; i < Formants.lTabelaDeFormantes; i++)
            {
                for (int j = 0; j < Formants.cTabelaDeFormantes; j++)
                {
                    Console.WriteLine(Formants.formantesT[i, j]);
                };
            };
            */

            static byte[] ConvertFloatToInt16(float[] samples)
            {
                    var bytesArray = new byte[samples.Length * 2];
                    for (int i = 0; i < samples.Length; i++)
                    {
                    var sample = samples[i];
                    // Limitar entre -1 e 1
                    sample = Math.Max(-1, Math.Min(1, sample));
                    
                    // Converter para short (16-bit)
                    var shortSample = (short)(sample * 32767);
                    
                    // Converter para bytes
                    bytesArray[i * 2] = (byte)(shortSample & 0xFF);
                    bytesArray[i * 2 + 1] = (byte)(shortSample >> 8);
                }
                return bytesArray;
            }


            static void ConvertToFile() 
            {
                using (var waveFile = new WaveFileWriter("formant.wav", new WaveFormat(Formants.sampleRate, 16, 1)))
                {
                    List<byte> audioDataSixteenCompiled = new List<byte>();
                    for (int i = 0; i < 4; i++)
                    {
                        var audioData = doOneSecond(Formants.samples, Formants.baseFrequency, i, Formants.formantesT);
                        byte[] audioDataSixteen = ConvertFloatToInt16(audioData.ToArray());
                        audioDataSixteenCompiled.AddRange(audioDataSixteen); 
                    }
                    waveFile.Write(audioDataSixteenCompiled.ToArray(), 0, audioDataSixteenCompiled.Count);
                }
           }
        ConvertToFile();
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
