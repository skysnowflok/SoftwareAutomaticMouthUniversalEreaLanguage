using System.Dynamic;
using System.Reflection;



public class Formants
{
    public static int sampleRate {get; set;} = 44100;
    public static double duration {get; set;} = 1.0; // segundos
    public static int samples {get; set;} = (int)(sampleRate*duration);
    public static float[,] formantesT {get; set;} = new float[lTabelaDeFormantes, cTabelaDeFormantes];
    public static double baseFrequency {get; set;}
    public static int[,] formantsFrequency {get; set;} = 
    {
        { 730, 2440, 1090 }, // 'a'  2440
        { 610, 1900, 1290 },  // 'e' 
        { 240, 2400, 2160 },  // 'i' 
        { 500, 700, 200 },  // 'o'
        { 300, 870, 2240 }  // 'u' 
    };

    public static int lTabelaDeFormantes = formantsFrequency.GetLength(0);
    public static int cTabelaDeFormantes = formantsFrequency.GetLength(1);

    float[,] formantsSin = new float[lTabelaDeFormantes, cTabelaDeFormantes];
/*
    public float[,] Calcular()
    { 
        sampleRate = 44100;
        duration = 1;

        formantesT = FormantsT(formantsFrequency);
        baseFrequency = 100;
        return formantesT;
    }
*/
    public static float[,] FormantsT(float[,] tabelaDeFormantes)
    {
        
        float[,] formantsT = new float[lTabelaDeFormantes, cTabelaDeFormantes];

        for (int i = 0; i < lTabelaDeFormantes; i++)
        {
            for (int j = 0; j < cTabelaDeFormantes; j++)
            {
                        formantsT[i, j] = tabelaDeFormantes[i, j]/samples;
            }
        }
        return formantsT;
        }
    }

