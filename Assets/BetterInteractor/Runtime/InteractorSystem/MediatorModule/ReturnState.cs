namespace Better.Interactor.Runtime.MediatorModule
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