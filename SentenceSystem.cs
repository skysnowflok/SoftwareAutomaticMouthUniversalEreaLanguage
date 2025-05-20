using System.Runtime.InteropServices.Marshalling;
using System.Text;
using NAudio.CoreAudioApi;

public static class SentenceSystem
{
    public static string[] exampleSentences = new string[]
    {
        "Olá, como você está?",
        "Estou bem, obrigado.",
        "Qual é o seu nome?",
        "Meu nome é João.",
        "Quantos anos você tem?",
        "Eu tenho 25 anos.",
        "Você gosta de programar?",
        "Sim, eu gosto muito.",
        "Qual é a sua linguagem de programação favorita?",
        "Minha linguagem favorita é C#.",
        "Você gosta de jogos?",
        "Sim, eu gosto de jogos.",
        "Qual é o seu jogo favorito?",
    };


    public static readonly char[] vowels =
    {
        'a',
        'e',
        'i',
        'o',
        'u',
    };

    private static readonly char[] consonants =
    {
        'b',
        'c',
        'd',
        'f',
        'g',
        'h',
        'j',
        'k',
        'l',
        'm',
        'n',
        'p',
        'q',
        'r',
        's',
        't',
        'v',
        'x',
        'y',
        'z',
    };

    private static readonly char[] ponctuations =
    {
        '!',
        '?',
        '.',
        ',',
        ';',
        ':',
        '"',
        '(',
        ')',
    };

    public static void Debug()
    {
        Random random = new Random();

        string randomPhrase = exampleSentences[random.Next(exampleSentences.Length)];
        Console.WriteLine("Enunciado selecionado: " + randomPhrase + "\n");
        List<string> syllables = DiscernSyllables(randomPhrase);
        PrintSyllables(syllables);
    }

    private static List<string> DiscernSyllables(string sentence)
    {
        // Split the sentence into words
        string[] words = sentence.ToLower().Split(' ');
        List<string> syllables = new List<string>();
        char c;

        var currentSyllable = new StringBuilder();

        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i];

            for (int j = 0; j < word.Length; j++)
            {
                c = word[j];

                if (consonants.Contains(c))
                {
                    currentSyllable.Append(c);
                }
                if (vowels.Contains(c))
                {
                    currentSyllable.Append(c);
                    syllables.Add(currentSyllable.ToString());

                    currentSyllable.Clear();
                }
            }
        }


        return syllables;
    }

    static void PrintSyllables(List<string> syllables)
    {
        foreach(string s in syllables) { Console.WriteLine(s); };
    }



}