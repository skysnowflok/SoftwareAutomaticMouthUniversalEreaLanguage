using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;

class FrequenciasBase 
{
    static int frequenciaSelecionada = 0;
    static double pi = Math.PI;
    static int sampleRate = Formants.sampleRate;
    static float T;
    static int[] Frequencias = {100, 185};
    static void TrocarFrequencia(string[] args)
    {
        if (args.Length > 0 && args[0] == "trocarfrequencia") 
        {
            frequenciaSelecionada = (frequenciaSelecionada == 0) ? 1 : 0;

            Console.WriteLine(Frequencias[1]);
            Console.WriteLine($"A frequencia selecionada é: {Frequencias[frequenciaSelecionada]}Hz.");
            SaveToFile();
        }
        else
        {
            Console.WriteLine("Erro, argumento indevido");
        }
    }

    public static float[] Calcular(int seletor)
    {
    float[] BaseSineWave = new float[sampleRate];
    for (int i = 0; i < sampleRate; i++)
    {
        T = (float)i / sampleRate;
        BaseSineWave[i] = (float)Math.Sin(2 * pi * T * Frequencias[seletor]);
    }
    return BaseSineWave;
    }
    public static void SaveToFile()
    {
        float[] BaseSineWave = Calcular(frequenciaSelecionada);
        using (var writer = new BinaryWriter(File.Open("dados.bin", FileMode.Create)))
        {
            writer.Write(BaseSineWave.Length); // Salva o tamanho do array
            foreach (float valor in BaseSineWave)
            {
                writer.Write(valor); // Salva cada valor
            }
    }
    }
    
} 
