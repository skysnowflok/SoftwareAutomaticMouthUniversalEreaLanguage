using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;




namespace Phonemes
{
    public class GenerationManager
    {
        public static int sampleRate {get; set;} = 44100;
        public static double duration {get; set;} = 1.0; // segundos
        public static int samples {get; set;} = (int)(sampleRate*duration);
        private static string folderpath = Path.Combine(Directory.GetCurrentDirectory(), "Phonemes", "vowels");

        public static int[] Frequencies = new int[3];
        private static double pi = Math.PI;
        private static double baseFrequency = 100;
        private static float[] sineWaves = new float[samples];
        private static float[] BaseSineWave = new float[samples];
        private static byte[] audioData = new byte[samples];

            public static Dictionary<string, int[,]> dicionario = new Dictionary<string, int[,]>
        {
            {"vowels", Vowels ?? new int[0, 0]},
            {"nasals", Nasals ?? new int[0, 0]},
            {"approximants", Approximants ?? new int[0, 0]},
            {"fricatives", Fricatives ?? new int[0, 0]},
            {"affricates", Affricates ?? new int[0, 0]}

        };






        


        public static readonly int[,] Vowels = 
        {
            {240, 2400, 2160}, // i vogal i
            {235, 2100, 1865}, // y 
            {390, 2300, 1910}, // e 
            {370, 1900, 1530}, // ø 
            {610, 1900, 1290}, // ɛ vogal e
            {585, 1710, 1125}, // œ
            {850, 1610, 760}, // a vogal a
            {820, 1530, 710}, // ɶ
            {750, 940, 190}, // ɑ 
            {700, 760, 60}, // ɒ
            {600, 1170, 570}, // ʌ
            {500, 700, 200}, // ɔ vogal o
            {460, 1310, 850}, // ɤ
            {360, 640, 280}, // o
            {300, 1390, 1090}, // ɯ
            {250, 595, 345} // u vogal u
        };

        public static readonly int[,] Nasals =
        {
            /*
            Nasals (/m/, /n/, /ŋ/)

            Nasals have a low first formant (F1) (~250-300 Hz) due to the closed oral cavity.
            /m/: F1 ≈ 250-300 Hz, F2 ≈ 1000 Hz, F3 ≈ 2000 Hz
            /n/: F1 ≈ 250-300 Hz, F2 ≈ 1500-2000 Hz, F3 ≈ 2500 Hz
            /ŋ/: F1 ≈ 250-300 Hz, F2 ≈ 1500-2000 Hz (varies), F3 ≈ 2500 Hz
            */

            {250, 100, 2000},
            {250, 1500, 2500},
            {250, 1500, 2500}
        };
        public static readonly int[,] Approximants =
        {
            /*
            Approximants (/w/, /j/, /r/, /l/)

            /w/ (similar to /u/): F1 ≈ 300-400 Hz, F2 ≈ 600-800 Hz, F3 ≈ 2500 Hz
            /j/ (similar to /i/): F1 ≈ 250-300 Hz, F2 ≈ 2000-2500 Hz, F3 ≈ 3000 Hz
            /r/: F1 ≈ 300-400 Hz, F2 ≈ 1000-1600 Hz, F3 ≈ low (~1500 Hz or lower)
            /l/: F1 ≈ 300-400 Hz, F2 ≈ 1000-1500 Hz, F3 ≈ 2500 Hz
            */
            {300, 600, 2500},
            {250, 2000, 3000},
            {300, 1000, 1500},
            {300, 1000, 2500}
        };

        public static readonly int[,] Stops =
        {
            /*
            Stops (/p/, /b/, /t/, /d/, /k/, /ɡ/)

            Stops are better characterized by formant transitions rather than steady-state frequencies.
            Bilabials (/p/, /b/): F2 transition starts low (~600-800 Hz)
            Alveolars (/t/, /d/): F2 transition around 1600-1800 Hz
            Velars (/k/, /ɡ/): F2 transition varies (~1200-2500 Hz), F3 often converges with F2 (velar pinch)
            */
            {0, 600, 0},
            {0, 1600, 0},
            {0, 1200, 0}
        };

        public static readonly int[,] Fricatives =
        {
            /*
            Fricatives (/f/, /v/, /θ/, /ð/, /s/, /z/, /ʃ/, /ʒ/, /h/)

            Fricatives are characterized by broad spectral noise rather than formant structure.
            /s/ and /z/: Strong energy around 4000-8000 Hz
            /ʃ/ and /ʒ/: Strong energy around 2000-5000 Hz
            /f/ and /v/: Broad energy, weak formants (~1000-4000 Hz)
            /h/: Can adopt the formant structure of adjacent vowels
            */
            {0, 4000, 0},
            {0, 20000, 0},
            {0, 10000, 0},
            {0, 0, 0}
        };

        public static readonly int[,] Affricates =
        {
            /*
            Affricates (/ʧ/, /ʤ/)

            /ʧ/ (as in "chop"): Similar to /ʃ/ (~2000-5000 Hz) with a stop closure
            /ʤ/ (as in "judge"): Similar to /ʒ/ (~2000-5000 Hz) with a stop closure
            */
            {0, 2000, 0},
            {0, 20000, 0}
        };

        public static async Task GenerateFiles(params string[] comandos)
        {

            foreach (string comando in comandos)
            {
                if (dicionario.TryGetValue(comando, out int[,] selecionado)) 
                {
                    Console.WriteLine($"Generating files for {folderpath}");
                    GenerateFile(samples, selecionado, folderpath);

                }
                else
                {
                    Console.WriteLine($"Error, bad argument: {comando}");
                    break;
                }
            }
        }

        



        public static void GenerateFile(int samples, int[,] formantsFrequency, string folder)
        {
            float[,] sineWavesBuffer = new float[formantsFrequency.GetLength(0), samples];
            sineWaves = new float[samples]; // Ensure sineWaves is properly initialized

            for (int i = 0; i < samples; i++)
            {
                float x = (float)i / samples;
                BaseSineWave[i] = (float)Math.Sin(2 * pi * baseFrequency * x);
            }

            for (int j = 0; j < samples; j++)
            {
                Parallel.For(0, formantsFrequency.GetLength(0), i =>
                {
                    float x = (float)j / samples;
                    sineWavesBuffer[i, j] = (float)Math.Sin(2 * pi * formantsFrequency[i, 0] * x);
                    sineWaves[j] += sineWavesBuffer[i, j] + BaseSineWave[j];
                });


            }
            audioData = ConvertFloatToInt16(sineWaves);
            for (int j = 0; j < formantsFrequency.GetLength(0); j++)
            {
                using (var writer = new BinaryWriter(File.Open(Path.Combine(folder, $"phoneme{j}.bin"), FileMode.Create)))
                {
                    writer.Write(audioData.Length); // Salva o tamanho do array
                    foreach (byte valor in audioData)
                    {
                        writer.Write(valor); // Salva cada valor
                    }
                }
            }
            Console.WriteLine($"Generated {samples} samples for folder: {folder}");
            Console.WriteLine($"sineWaves length: {sineWaves.Length}");
        }

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






    }
}

/*
    Approximate Formant Frequencies of Consonants (in Hz)
Nasals (/m/, /n/, /ŋ/)

    Nasals have a low first formant (F1) (~250-300 Hz) due to the closed oral cavity.
    /m/: F1 ≈ 250-300 Hz, F2 ≈ 1000 Hz, F3 ≈ 2000 Hz
    /n/: F1 ≈ 250-300 Hz, F2 ≈ 1500-2000 Hz, F3 ≈ 2500 Hz
    /ŋ/: F1 ≈ 250-300 Hz, F2 ≈ 1500-2000 Hz (varies), F3 ≈ 2500 Hz

Approximants (/w/, /j/, /r/, /l/)

    /w/ (similar to /u/): F1 ≈ 300-400 Hz, F2 ≈ 600-800 Hz, F3 ≈ 2500 Hz
    /j/ (similar to /i/): F1 ≈ 250-300 Hz, F2 ≈ 2000-2500 Hz, F3 ≈ 3000 Hz
    /r/: F1 ≈ 300-400 Hz, F2 ≈ 1000-1600 Hz, F3 ≈ low (~1500 Hz or lower)
    /l/: F1 ≈ 300-400 Hz, F2 ≈ 1000-1500 Hz, F3 ≈ 2500 Hz

Stops (/p/, /b/, /t/, /d/, /k/, /ɡ/)

    Stops are better characterized by formant transitions rather than steady-state frequencies.
    Bilabials (/p/, /b/): F2 transition starts low (~600-800 Hz)
    Alveolars (/t/, /d/): F2 transition around 1600-1800 Hz
    Velars (/k/, /ɡ/): F2 transition varies (~1200-2500 Hz), F3 often converges with F2 (velar pinch)

Fricatives (/f/, /v/, /θ/, /ð/, /s/, /z/, /ʃ/, /ʒ/, /h/)

    Fricatives are characterized by broad spectral noise rather than formant structure.
    /s/ and /z/: Strong energy around 4000-8000 Hz
    /ʃ/ and /ʒ/: Strong energy around 2000-5000 Hz
    /f/ and /v/: Broad energy, weak formants (~1000-4000 Hz)
    /h/: Can adopt the formant structure of adjacent vowels

Affricates (/ʧ/, /ʤ/)

    /ʧ/ (as in "chop"): Similar to /ʃ/ (~2000-5000 Hz) with a stop closure
    /ʤ/ (as in "judge"): Similar to /ʒ/ (~2000-5000 Hz) with a stop closure
*/