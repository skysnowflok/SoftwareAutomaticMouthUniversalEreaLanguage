namespace SoftwareAutomaticMouthUniversalEreaLanguage.Phonemes
{
    public static class Frequencies
    {    
        public static int sampleRate {get; set;} = 44100;
        public static double duration {get; set;} = 1.0; // segundos
        public static int samples {get; set;} = (int)(sampleRate*duration);
 

        public static int[] frequencies = new int[3];
        public static double pi = Math.PI;
        public static double baseFrequency = 100;
        public static float[] sineWaves = new float[samples];
        public static float[] BaseSineWave = new float[samples];
        public static byte[] audioData = new byte[samples];

        public static Dictionary<string, int[,]> dicionario = new Dictionary<string, int[,]>
        {
            { "vowels", new int[,]
                {
                    {240, 2400, 2160}, // i vogal i 0
                    {235, 2100, 1865}, // y 1
                    {390, 2300, 1910}, // e 2
                    {370, 1900, 1530}, // ø 3
                    {610, 1900, 1290}, // ɛ vogal e 4
                    {585, 1710, 1125}, // œ 5
                    {850, 1610, 760}, // a vogal a 6
                    {820, 1530, 710}, // ɶ 7
                    {750, 940, 190}, // ɑ 8
                    {700, 760, 60}, // ɒ 9
                    {600, 1170, 570}, // ʌ 10
                    {500, 700, 200}, // ɔ vogal o 11
                    {460, 1310, 850}, // ɤ 12
                    {360, 640, 280}, // o 13
                    {300, 1390, 1090}, // ɯ 14
                    {250, 595, 345} // u vogal u 15
                }
            },
            { "nasals", new int[,]
                {
                /*
                Nasals (/m/, /n/, /ŋ/)

                Nasals have a low first formant (F1) (~250-300 Hz) due to the closed oral cavity.
                /m/: F1 ≈ 250-300 Hz, F2 ≈ 1000 Hz, F3 ≈ 2000 Hz
                /n/: F1 ≈ 250-300 Hz, F2 ≈ 1500-2000 Hz, F3 ≈ 2500 Hz
                /ŋ/: F1 ≈ 250-300 Hz, F2 ≈ 1500-2000 Hz (varies), F3 ≈ 2500 Hz
                */
                    {250, 100, 2000}, // m
                    {250, 1500, 2500}, // n
                    {250, 1500, 2500} // ŋ
                }
            },
            { "approximants", new int[,]
                {
                /*
                Approximants (/w/, /j/, /r/, /l/)

                /w/ (similar to /u/): F1 ≈ 300-400 Hz, F2 ≈ 600-800 Hz, F3 ≈ 2500 Hz
                /j/ (similar to /i/): F1 ≈ 250-300 Hz, F2 ≈ 2000-2500 Hz, F3 ≈ 3000 Hz
                /r/: F1 ≈ 300-400 Hz, F2 ≈ 1000-1600 Hz, F3 ≈ low (~1500 Hz or lower)
                /l/: F1 ≈ 300-400 Hz, F2 ≈ 1000-1500 Hz, F3 ≈ 2500 Hz
                */
                    {300, 600, 2500}, // w
                    {250, 2000, 3000}, // j
                    {300, 1000, 1500}, // r
                    {300, 1000, 2500} // l
                }
            },
            { "stops", new int[,]
                {
                /*
                Stops (/p/, /b/, /t/, /d/, /k/, /ɡ/)

                Stops are better characterized by formant transitions rather than steady-state frequencies.
                Bilabials (/p/, /b/): F2 transition starts low (~600-800 Hz)
                Alveolars (/t/, /d/): F2 transition around 1600-1800 Hz
                Velars (/k/, /ɡ/): F2 transition varies (~1200-2500 Hz), F3 often converges with F2 (velar pinch)
                */
                    {0, 600, 0}, // p
                    {0, 1600, 0}, // t
                    {0, 1200, 0} // k
                }
            },
            { "fricatives", new int[,]
                {
                /*
                Fricatives (/f/, /v/, /θ/, /ð/, /s/, /z/, /ʃ/, /ʒ/, /h/)

                Fricatives are characterized by broad spectral noise rather than formant structure.
                /s/ and /z/: Strong energy around 4000-8000 Hz
                /ʃ/ and /ʒ/: Strong energy around 2000-5000 Hz
                /f/ and /v/: Broad energy, weak formants (~1000-4000 Hz)
                /h/: Can adopt the formant structure of adjacent vowels
                */
                    {0, 4000, 0}, // s
                    {0, 20000, 0}, // ʃ
                    {0, 10000, 0}, // f
                    {0, 0, 0} // h
                }
            },
            { "affricates", new int[,]
                {
                /*
                Affricates (/ʧ/, /ʤ/)

                /ʧ/ (as in "chop"): Similar to /ʃ/ (~2000-5000 Hz) with a stop closure
                /ʤ/ (as in "judge"): Similar to /ʒ/ (~2000-5000 Hz) with a stop closure
                */
                    {0, 2000, 0}, // ʧ
                    {0, 20000, 0} // ʤ
                }
            }
        };


    }
}