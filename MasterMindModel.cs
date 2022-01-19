using System;
using System.Collections.Generic;

namespace mastermind
{
    public class MasterMindResult 
    {
        public bool IsWin { get; set; }
        public int AttemptsLeft { get; set; }
        public string Message { get; set; }
    }

    public class MasterMindSetting : MasterMindResult
    {
        public int CodeLength { get; set; }
        public int CodeMinValue { get; set; }
        public int CodeMaxValue { get; set; }
        public int AttemptLimit { get; set; }
        
        public MasterMindSetting()
        {
        }
        public MasterMindSetting(int codeLength
            , int codeMinValue
            , int codeMaxValue
            , int attemptLimit
            , string message
        )
        {
            this.CodeLength = codeLength;
            this.CodeMinValue = codeMinValue;
            this.CodeMaxValue = codeMaxValue;
            this.AttemptLimit = attemptLimit;
            this.Message = message;
        }
    }
    public class MasterMindStartPlayResult : MasterMindResult
    {
        public string CodeValue { get; set; }
        public MasterMindStartPlayResult(string codeValue
            , int attemptsLeft
            , string message
        )
        {
            this.CodeValue = codeValue;
            this.AttemptsLeft = attemptsLeft;
            this.Message = message;
        }

    }
    public class MasterMindAttemptResult : MasterMindResult
    {
        public bool IsError { get; set; }
        public string AttemptResult { get; set; }
        public List<string> Errors { get; set; }
        public MasterMindAttemptResult()
        {
        }
        // public MasterMindAttemptResult(bool isWin, int attemptsLeft
        //     , string message)
        // {
        //     this.IsWin = isWin;
        //     this.AttemptsLeft = attemptsLeft;
        //     this.Message = message;
        // }
        // public MasterMindAttemptResult(bool isError, int attemptsLeft
        //     , List<string> errors)
        // {
        //     this.IsError = isError;
        //     this.AttemptsLeft = attemptsLeft;
        //     this.Errors = errors;
        // }

    }

}