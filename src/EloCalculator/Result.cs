namespace EloCalculator
{
    /// <summary>
    /// Types of <see cref="Game.Result"/>s.
    /// </summary>
    public enum Result
    {
        /// <summary>
        /// Represents a <see cref="Game.White"/> victory (1-0).
        /// </summary>
        White,

        /// <summary>
        /// Represents a <see cref="Game.Black"/> victory (0-1).
        /// </summary>
        Black,

        /// <summary>
        /// Represents a draw. (1/2-1/2).
        /// </summary>
        Draw,
    }
}
