namespace MathTek.Exceptions
{
    using System;

    public class MathExceptions : Exception
    {
        public MathExceptions(string message) : base(message) { }
    }

    public class InvalidOrMissingJSONFile : MathExceptions
    {
        public InvalidOrMissingJSONFile() :
            base("Invalid or Missing json file, check Resources folder")
        { }
        public InvalidOrMissingJSONFile(string message) : base(message) { }
    }
}