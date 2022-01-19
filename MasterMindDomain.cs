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
        private readonly string _status;
        //private List<string> _errorMessages;
        private string _errorMessage;
        public MasterMindDomain() 
        {
            // initialize with defualt settings
            
            this._codeLength = 4;
            this._codeItemMinValue = 1;
            this._codeItemMaxValue = 6;
            this._attemptLimit = 10;
            _code = new Dictionary<int, List<int>>();
            this._numberOfAttemptLeft = this._attemptLimit;
            this._status = "Ready to Start to Play";
        }
        public MasterMindResult GetSettings()
        {
            var result = new MasterMindSetting(
                this._codeLength
                , _codeItemMinValue
                , _codeItemMaxValue
                , _attemptLimit
                , _status
            );
            return result;

        }
        public MasterMindResult SetNumberOfAttempts(int attemptLimit)
        {
            this._attemptLimit = attemptLimit;
            return GetSettings();
        }

        public MasterMindResult SetMinCodeItemValue(int codeItemMinValue)
        {
            this._codeItemMinValue = codeItemMinValue;
            return GetSettings();
        }
        public MasterMindResult SetMaxCodeItemValue(int codeItemMaxValue)
        {
            this._codeItemMaxValue = codeItemMaxValue;
            return GetSettings();
        }
        public MasterMindResult StartToPlay()
        {
            GenerateCode();
            var result = new MasterMindStartPlayResult(_codeValue, this._numberOfAttemptLeft, "Code Generated Successfully");
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
                    //result.Message = $"{errorMessage} \nYou have {this._numberOfAttemptLeft} attempts left.";
                    //HandleErrorException(errorMessage);
                }
            }
            else
            {
                // attempt limit exceeds
                this._isWin = false;
                this._isError = true;
                this._errorMessage = "error - attempt limit exceeds";
                //result.Message = $"{errorMessage} \nYou have {this._numberOfAttemptLeft} attempts left.";
                //HandleErrorException(errorMessage);
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
                    result.Message = $"{this._errorMessage} \nYou have {this._numberOfAttemptLeft} attempts left.";
                else if(this._numberOfAttemptLeft == 0)
                    result.Message = $"You have lost.";
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
            using(StringReader reader = new StringReader(guessValue))
            {
                reader.Read(guessChars);
            }

            var attemptResultValue = new StringBuilder(_codeLength);

            for(var i=0; i<_codeLength; i++)
            {
                int guessNumValue;
                if (int.TryParse(guessChars[i].ToString(), out guessNumValue))
                {
                    if(IsValidGuessValueItem(guessNumValue))
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
                        //var errorMessage = "error - codeItem number value is out of the set rannge";
                        //HandleErrorException(errorMessage);
                    }
                }
                else
                {
                    // codeItem character is not an integer value
                    this._isError = true;
                    this._errorMessage = "error - guessValue is not an integer value";
                    //HandleErrorException(errorMessage);

                }
            }
            return  attemptResultValue.ToString();
        }
        private bool IsValidGuessValueItem(int guessValueItem)
        {
            return guessValueItem >= _codeItemMinValue && guessValueItem <= _codeItemMaxValue;
        }
        // private void HandleErrorException(string errorMessage)
        // {
        //     _errorMessages = _errorMessages ?? new List<string>(); // initializes if null
        //     _errorMessages.Add(errorMessage);
        // }
    }
        
}