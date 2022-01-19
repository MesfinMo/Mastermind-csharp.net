using System;

namespace mastermind
{
    public interface IMasterMind 
    {
        MasterMindResult GetSettings();
        MasterMindResult SetNumberOfAttempts(int attemptLimit);
        MasterMindResult SetMinCodeItemValue(int codeItemMinValue);
        MasterMindResult SetMaxCodeItemValue(int codeItemMaxValue);
        MasterMindResult StartToPlay();
        MasterMindResult GuessToSolve(string guessValue);
    }
}