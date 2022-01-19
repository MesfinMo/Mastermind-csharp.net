using System;

namespace mastermind
{
    class Program
    {
        static void Main(string[] args)
        {

            var mastermind = new MasterMindDomain();

            //mastermind.SetNumberOfAttempts(5);
            var settingsResult = (MasterMindSetting)mastermind.GetSettings();
            
            var startToPlayResult = (MasterMindStartPlayResult)mastermind.StartToPlay();

            Console.WriteLine(startToPlayResult.Message);

            var attemptsLeft = startToPlayResult.AttemptsLeft;
            var isWin = startToPlayResult.IsWin;
            var messageResult = startToPlayResult.Message;
            while(attemptsLeft > 0 && !isWin)
            {
                Console.WriteLine("Enter your guess...");
                var guessInput = Console.ReadLine();

                var guessAttemptResult = (MasterMindAttemptResult)mastermind.GuessToSolve(guessInput);
                attemptsLeft = guessAttemptResult.AttemptsLeft;
                isWin = guessAttemptResult.IsWin;
                messageResult = guessAttemptResult.Message;
                Console.WriteLine(guessAttemptResult.AttemptResult);
                Console.WriteLine(messageResult);
            }
        }
    }
}
