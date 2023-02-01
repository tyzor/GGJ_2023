namespace GGJ.Inputs
{
    public static class Input
    {
        private static bool _setup;

        public static GameInputs GameInputs
        {
            get
            {
                if (_setup)
                    return _sInput;
                _sInput = new GameInputs();
                _setup = true;

                return _sInput;
            }
        }

        private static GameInputs _sInput;
    }
}
