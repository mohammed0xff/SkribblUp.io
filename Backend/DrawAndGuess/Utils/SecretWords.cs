using System;
using System.Collections.Generic;

namespace DrawAndGuess.Utils
{
    public static class SecretWords
    {
        private static Random _random = new Random();
        public static readonly string[] Words =
        {
            "tbaby",
            "ears",
            "scissors",
            "cough",
            "cold",
            "laugh",
            "blink",
            "hairbrush",
            "sneeze",
            "spin",
            "hammer",
            "book",
            "phone",
            "waterfall",
            "starship",
            "toothbrush",
            "jump",
            "clap",
            "slapbirthday",
            "president",
            "butterfly",
            "apartment",
            "cradle",
            "coffee",
            "trampoline",
            "waterfall",
            "window",
            "proud",
            "stuck-up",
            "flashlight",
            "marry",
            "overwhelm",
            "flamingo",
            "judge",
            "shadow",
            "halo",
            "measure",
            "clown",
            "chomp",
            "slither",
            "house",
            "winter",

        };
        public static string[] GetTreeRandomWords()
        {
            var randomWords = new List<string>();
            do
            {
                int index = _random.Next(Words.Length);
                if (!randomWords.Contains(Words[index]))
                {
                    randomWords.Add(Words[index]);
                }
            } while (randomWords.Count < 3);

            return randomWords.ToArray();
        }
    }
}
