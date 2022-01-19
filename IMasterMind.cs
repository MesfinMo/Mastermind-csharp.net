using System;

namespace mastermind
{
    public interface IMasterMind 
    {
        MasterMindResult GetSettings();
        void SetNumberOfAttempts(int attemptLimit);
        void SetMinCodeItemValue(int codeItemMinValue);
        void SetMaxCodeItemValue(int codeItemMaxValue);
        MasterMindResult StartToPlay();
        MasterMindResult GuessToSolve(string guessValue);
    }
}