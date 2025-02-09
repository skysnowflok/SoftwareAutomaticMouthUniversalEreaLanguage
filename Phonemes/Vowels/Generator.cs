using NAudio.CoreAudioApi;

namespace Phonemes.Vowels
{
    public class Generator 
    {
        private static string folderpath = Path.Combine(Directory.GetCurrentDirectory(), "Phonemes", "Vowels");

        public static int sampleRate {get; set;} = 44100;
        public static double duration {get; set;} = 1.0; // segundos
        public static int samples {get; set;} = (int)(sampleRate*duration);
        public static int[] Frequencies = new int[3];
        private static double pi = Math.PI;
        private static double baseFrequency = 100;
        private static float[] sineWaves = new float[samples];
        private static float[] BaseSineWave = new float[samples];
        private static byte[] audioData = new byte[samples];




        private static float[,] sineWavesBuffer = new float[Phonemes.Frequencies.Vowels.GetLength(1), samples];

        public static void GenerateFiles(int samples, int[,] formantsFrequency)
        {
            for (int i = 0; i < samples; i++)
            {
                float x = (float)i/samples;
                BaseSineWave[i] = (float)Math.Sin(2*pi*baseFrequency*x);
            }
            for (int j = 0; j < formantsFrequency.GetLength(0); j++)
            {
                Parallel.For(0, 3, i =>{
                float x = (float)j/samples;

                sineWavesBuffer[i, j] = (float)Math.Sin(2*pi*formantsFrequency[j, i]*x);

                sineWaves[j] = sineWaves[j] + sineWavesBuffer[i, j] + BaseSineWave[j];
                });

                audioData = ConvertFloatToInt16(sineWaves);

                using (var writer = new BinaryWriter(File.Open(Path.Combine(folderpath, $"phoneme{j}.bin"), FileMode.Create)))
                {
                    writer.Write(audioData.Length); // Salva o tamanho do array
                    foreach (float valor in audioData)
                    {
                        writer.Write(valor); // Salva cada valor
                    }
                }       

            }
            Console.WriteLine(sineWaves.GetLength(0));
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