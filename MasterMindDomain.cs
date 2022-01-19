using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace mastermind
{
    public class MasterMindDomain : IMasterMind
    {
        private readonly int _codeLength;
        private  int _codeItemMinValue;
        private  int _codeItemMaxValue;
        private int _attemptLimit;
        private int _numberOfAttemptLeft;

        // container to hold randomly generated number codes and their -
        // respective positions
        private readonly Dictionary<int, List<int>> _code;
        // private readonly bool _allowDuplicateCodeValue = true;
        private string _codeValue;
        private string _guessResultValue;
        private bool _isWin = false;
        private bool _isError = false;
        private string _errorMessage;
        public MasterMindDomain() 
        {
            // initialize with defualt settings
            
            this._codeLength = 4;
            SetMinCodeItemValue(1);
            SetMaxCodeItemValue(6);
            SetNumberOfAttempts(10);
            _code = new Dictionary<int, List<int>>();
        }
        public MasterMindResult GetSettings()
        {
            var result = new MasterMindSetting(
                this._codeLength
                , _codeItemMinValue
                , _codeItemMaxValue
                , _attemptLimit
            );
            return result;

        }
        public void SetNumberOfAttempts(int attemptLimit)
        {
            this._attemptLimit = attemptLimit;
            this._numberOfAttemptLeft = this._attemptLimit;
        }

        public void SetMinCodeItemValue(int codeItemMinValue)
        {
            this._codeItemMinValue = codeItemMinValue;
        }
        public void SetMaxCodeItemValue(int codeItemMaxValue)
        {
            this._codeItemMaxValue = codeItemMaxValue;
        }
        public MasterMindResult StartToPlay()
        {
            GenerateCode();
            var startMessag = $@"
            Welcome to Mastermind!
            Enter a {this._codeLength} digit number between {this._codeItemMinValue} and {this._codeItemMaxValue}
            Duplicate digits are allowed.
            You have {this._attemptLimit} attempts to solve.";

            var result = new MasterMindStartPlayResult(this._numberOfAttemptLeft, startMessag);
            return result;
        }

        public MasterMindResult GuessToSolve(string guessValue)
        {
            if(this._numberOfAttemptLeft > 0) // checks for attempt limit
            {
                this._numberOfAttemptLeft--; // decrease _numberOfAttemptLeft by 1
                
                // reset local values
                this._isError = false;
                this._errorMessage = string.Empty;
                this._guessResultValue = string.Empty;

                // validate guessValue for integer values
                if(IsValidGuessValue(guessValue))
                {
                    
                    var evaluateResult = EvaluateGuess(guessValue);
                    this._guessResultValue = evaluateResult;
                    
                    if(this._guessResultValue == "++++")
                        this._isWin = true;
                    else
                        this._isWin = false;
                }
                else
                {
                    // invalid guess value
                    this._isError = true;
                    this._isWin = false;
                    this._errorMessage = "error - invalid guess value";
                }
            }
            else
            {
                // attempt limit exceeds
                this._isWin = false;
                this._isError = true;
                this._errorMessage = "error - attempt limit exceeds";
            }

            return ConstructResultModel();
        }
        private MasterMindAttemptResult ConstructResultModel()
        {
            var result = new MasterMindAttemptResult();
            result.AttemptsLeft = this._numberOfAttemptLeft;
            result.AttemptResult = this._guessResultValue;
            result.IsWin = _isWin;
            if(_isWin)
            {
                // You won.
                result.Message = "You won.";
            }
            else
            {
                // construct messages.
                if(_isError)
                    result.Message = $"{this._errorMessage} \n Valid value is a {this._codeLength} digit number between {this._codeItemMinValue} and {this._codeItemMaxValue} \nYou have {this._numberOfAttemptLeft} attempts left.";
                else if(this._numberOfAttemptLeft == 0)
                    result.Message = $"You have lost. \n Answer was {this._codeValue}";
                else
                    result.Message = $"You have {this._numberOfAttemptLeft} attempts left.";
            }
            return result;
        }
        private void GenerateCode()
        {
            var codeValue = new StringBuilder(_codeLength);

            for(var i=0; i < _codeLength; i++)
            {
                // generate a random integer between the min and max code value
                Random rnd = new Random();
                var codeItem  = rnd.Next(_codeItemMinValue, _codeItemMaxValue);

                // append codeItem as string value
                codeValue.Append(codeItem);

                // look into the code container to see if codeItem already -
                // exsists, if so add a position, if not add new codeItem and position
                List<int> codeItemPostions;
                if(_code.TryGetValue(codeItem, out codeItemPostions))
                {
                    codeItemPostions = codeItemPostions ?? new List<int>();

                    codeItemPostions.Add(i); // adds the postion of codeItem
                }
                else
                {
                    codeItemPostions = new List<int>();
                    codeItemPostions.Add(i);
                    _code.Add(codeItem, codeItemPostions);
                }
            }
            this._codeValue = codeValue.ToString();

        }

        private bool IsValidGuessValue(string guessValue)
        {
            var isValid = false;
            isValid = !string.IsNullOrEmpty(guessValue); // checks null or empty guessValue
                
            isValid = guessValue.Length == _codeLength; // checks number of characters in guessValue equals to the expected set code length
            
            return isValid;
        }

        private string EvaluateGuess(string guessValue)
        {
            var guessChars = new char[_codeLength];
            // separate guess value into individual characters and store in array
            using(StringReader reader = new StringReader(guessValue))
            {
                reader.Read(guessChars);
            }

            // initialize to store attempt result values (++--, --+-, ++++)
            var attemptResultValue = new StringBuilder(_codeLength);

            for(var i=0; i<_codeLength; i++)
            {
                int guessNumValue;
                // try to read out each character as integer value
                if (int.TryParse(guessChars[i].ToString(), out guessNumValue))
                {
                    if(IsValidGuessValueItem(guessNumValue)) // checks valid number range
                    {
                        List<int> codeItemPostions;
                        if(_code.TryGetValue(guessNumValue, out codeItemPostions))
                        {
                            // guess value item found
                            // check its position correctness
                            if(codeItemPostions.Contains(i)) // index as current position in the guess
                            {
                                // correct position
                                // append guess result character as string value
                                attemptResultValue.Append('+'); // '+' for correct digit
                            }
                            else
                            {
                                // wrong position
                                attemptResultValue.Append('-'); // '-' for correct but wrong position digit
                            }
                        }
                        else
                        {
                            // wrong guess
                            attemptResultValue.Append(' '); // ' ' for wrong digit
                        }
                    }
                    else
                    {
                        // codeItem number value is out of the set rannge
                        attemptResultValue.Append(' ');
                    }
                }
                else
                {
                    // codeItem character is not an integer value
                    this._isError = true;
                    this._errorMessage = "error - guessValue is not an integer value";

                }
            }
            return  attemptResultValue.ToString();
        }
        private bool IsValidGuessValueItem(int guessValueItem)
        {
            return guessValueItem >= _codeItemMinValue && guessValueItem <= _codeItemMaxValue;
        }
    }
        
}