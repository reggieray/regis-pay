namespace Regis.Pay.Mocks
{
    public class InMemoryMockingService : IInMemoryMockingService
    {
        private bool _returnError = false;

        public bool ShouldError()
        {
            return _returnError;
        }

        public void TurnErrorOff()
        {
            _returnError = false;
        }

        public void TurnErrorOn()
        {
            _returnError = true;
        }
    }

    public interface IInMemoryMockingService
    {
        void TurnErrorOn();

        void TurnErrorOff();

        bool ShouldError();
    }
}
