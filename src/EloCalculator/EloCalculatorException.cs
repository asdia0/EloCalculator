namespace EloCalculator
{
    using System;

    /// <summary>
    /// Defines an exception thrown in this project.
    /// </summary>
    public class EloCalculatorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EloCalculatorException"/> class.
        /// </summary>
        public EloCalculatorException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EloCalculatorException"/> class.
        /// </summary>
        /// <param name="message">A message about the exception.</param>
        public EloCalculatorException(string message)
            : base(message)
        { }
    }
}
