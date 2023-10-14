using UnityEngine;

namespace Better.Interactor.Runtime.Models
{
    public class ReturnState
    {
        private static ReturnState ValidState { get; } = new ReturnState(true);
        private static ReturnState NonValidState { get; } = new ReturnState(false);
        
        public bool Value { get; }

        private ReturnState(bool value)
        {
            Value = value;
        }
    }
}