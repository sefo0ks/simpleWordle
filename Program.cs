internal class Program
{
    const ConsoleColor ForegroundColor = ConsoleColor.Black;
    const ConsoleColor BackgroundColor = ConsoleColor.White;
    const ConsoleColor inWordColor = ConsoleColor.Yellow;
    const ConsoleColor inPositionColor = ConsoleColor.Green;
    const int MaxGuesses = 6;

    private static void Main(string[] args)
    {
        List<string> words = new();
        string wordToGuess;

        List<string> previousTries = new();
        int guessesRemain;

        string? guess;

        Setup(out wordToGuess, out guessesRemain, out words);

        bool run = true;
        while (run)
        {
            ClearConsoleAndShowWords(guessesRemain, previousTries, wordToGuess);
            
            guess = GetInput(previousTries, words);
            previousTries.Add(guess);
            guessesRemain--;

            HandleEndGame(guess, wordToGuess, previousTries, guessesRemain);
        }
    }

    private static List<string> GetWordsFromFile()
    {
        List<string> lines = new();
        if (!File.Exists(Environment.CurrentDirectory + @"\Words.txt"))
            return new List<string>();

        using(StreamReader sr = File.OpenText(Environment.CurrentDirectory + @"\Words.txt"))
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                lines.Add(line.ToUpper());
            }
        }

        return lines;
    }

    private static void Setup(out string _wordToGuess, out int _guessesRemain, out List<string> _words)
    {
        Random r = new();
        Console.ForegroundColor = ForegroundColor;
        Console.BackgroundColor = BackgroundColor;

        _words = GetWordsFromFile();
        
        if (_words.Count < 1)
            _words.Add("CLOWN");

        _wordToGuess = _words[r.Next(0, _words.Count)];
        _guessesRemain = MaxGuesses;
    }

    private static void ClearConsoleAndShowWords(int _guessesRemain, List<string> _previousTries, string _wordToGuess)
    {
        Console.Clear();

        Console.WriteLine("Guesses remain: " + _guessesRemain);
        for (int i = 0; i < MaxGuesses; i++)
            {
                if (i <  6 - _guessesRemain)
                {
                    Console.Write($"{i + 1}: ");
                    for (int j = 0; j < _previousTries[i].Length; j++)
                    {
                        char currentChar = _previousTries[i][j];

                        if (_wordToGuess[j] == _previousTries[i][j])
                            Console.BackgroundColor = inPositionColor;
                        else if (_wordToGuess.Contains(currentChar))
                            Console.BackgroundColor = inWordColor;
                        else
                            Console.BackgroundColor = BackgroundColor;

                        Console.Write(currentChar);

                        if (j != _previousTries[i].Length - 1)
                            Console.Write(" ");
                    }
                    Console.BackgroundColor = BackgroundColor;
                    Console.WriteLine();
                }
                else
                    Console.WriteLine($"{i + 1}: _ _ _ _ _");
            }
    }

    private static string GetInput(List<string> _previousTries, List<string> _allWords)
    {
        Console.Write("Your guess: ");
        string input = Console.ReadLine();
        input = input.ToUpper();

        foreach (char ch in input)
        {
            if (Char.IsLetter(ch) == false)
            {
                Console.WriteLine("Wrong character(-s) passed!");
                input = GetInput(_previousTries, _allWords);
            }
        }

        if (input.Length != 5)
        {
            Console.WriteLine("Write 5 characters long words only!");
            input = GetInput(_previousTries, _allWords);
        }
        
        if (_previousTries.Any(prevTry => prevTry.ToUpper() == input))
        {
            Console.WriteLine("You tried this word already.");
            input = GetInput(_previousTries, _allWords);
        }

        if (_allWords.Any(word => word.ToUpper() == input))
            return input;
        else
        {
            Console.WriteLine($"Word {input} is not valid");
            input = GetInput(_previousTries, _allWords);
        }

        return input;
    }

    private static void HandleEndGame(string _guess, string _wordToGuess, List<string> _previousTries, int _guessesRemain)
    {
        bool gameEnded = false;

        if (_guess.ToUpper() == _wordToGuess.ToUpper())
        {
            ClearConsoleAndShowWords(_guessesRemain, _previousTries, _wordToGuess);
            Console.WriteLine("YOU WON!!!");
            gameEnded = true;
        }
        else if (_guessesRemain < 1)
        {
            ClearConsoleAndShowWords(_guessesRemain, _previousTries, _wordToGuess);
            Console.WriteLine($"Guessed word was - {_wordToGuess}");
            Console.WriteLine("YOU LOST!!!");
            gameEnded = true;
        }

        if (gameEnded)
        {
            Console.WriteLine("Want to play more? (Y/N)");
            while (true)
            {
                ConsoleKeyInfo answer = Console.ReadKey();

                switch (answer.Key)
                {
                    case ConsoleKey.Y:
                        Main(new string[0]);
                        break;
                    case ConsoleKey.N:
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}