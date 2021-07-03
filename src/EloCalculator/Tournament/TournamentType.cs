namespace EloCalculator
{
    /// <summary>
    /// Types of tournaments.
    /// </summary>
    public enum TournamentType
    {
        /// <summary>
        /// Arena tournament.
        /// <para>
        /// Pairings: By rating
        /// </para>
        /// <para>
        /// Tiebreaks:
        ///     <list type="number">
        ///     <item>
        ///     <see cref="TournamentPlayer.PerformanceRating"/>.
        ///     </item>
        ///     </list>
        /// </para>
        /// </summary>
        Arena,

        /// <summary>
        /// Swiss tournament: Danish variation.
        /// <para>
        /// Pairings: Identical to the <see cref="TournamentType.Monrad"/> system; players are paired according to their rankings. However, players are allowed to play the same opponent multiple times. This results in #1 always playing #2, #3 always playing #4, etc.
        /// </para>
        /// <para>
        /// Tiebreaks:
        ///     <list type="number">
        ///     <item>
        ///     <see cref="TournamentPlayer.SonnebornBerger"/>.
        ///     </item>
        ///     <item>
        ///     <see cref="TournamentPlayer.MedianBuchholz"/>.
        ///     </item>
        ///     <item>
        ///     <see cref="TournamentPlayer.Culmulative"/>.
        ///     </item>
        ///     <item>
        ///     <see cref="TournamentPlayer.Buchholz"/>.
        ///     </item>
        ///     <item>
        ///     <see cref="TournamentPlayer.Baumbach"/>.
        ///     </item>
        ///     </list>
        /// </para>
        /// </summary>
        Danish,

        /// <summary>
        /// Swiss tournament: Dutch system.
        /// <para>
        /// Pairings: Players are split into two groups based on their score. Players in the first group are then paired with players in the second group.
        /// </para>
        /// <para>
        /// Tiebreaks:
        ///     <list type="number">
        ///     <item>
        ///     <see cref="TournamentPlayer.SonnebornBerger"/>.
        ///     </item>
        ///     <item>
        ///     <see cref="TournamentPlayer.MedianBuchholz"/>.
        ///     </item>
        ///     <item>
        ///     <see cref="TournamentPlayer.Culmulative"/>.
        ///     </item>
        ///     <item>
        ///     <see cref="TournamentPlayer.Buchholz"/>.
        ///     </item>
        ///     <item>
        ///     <see cref="TournamentPlayer.Baumbach"/>.
        ///     </item>
        ///     </list>
        /// </para>
        /// </summary>
        Dutch,

        /// <summary>
        /// Swiss tournament: Monrad system.
        /// <para>
        /// Pairings: Players are paired according to their rankings. For example, #1 will play #2, #3 will play #4, etc. However, if the players have played before, the next suitable player will be chosen.
        /// </para>
        /// <para>
        /// Tiebreaks:
        ///     <list type="number">
        ///     <item>
        ///     <see cref="TournamentPlayer.SonnebornBerger"/>.
        ///     </item>
        ///     <item>
        ///     <see cref="TournamentPlayer.MedianBuchholz"/>.
        ///     </item>
        ///     <item>
        ///     <see cref="TournamentPlayer.Culmulative"/>.
        ///     </item>
        ///     <item>
        ///     <see cref="TournamentPlayer.Buchholz"/>.
        ///     </item>
        ///     <item>
        ///     <see cref="TournamentPlayer.Baumbach"/>.
        ///     </item>
        ///     </list>
        /// </para>
        /// </summary>
        Monrad,

        /// <summary>
        /// Round robin tournament.
        /// <para>
        /// Pairings: Each player will play every other player once.
        /// </para>
        /// <para>
        /// Tiebreaks:
        ///     <list type="number">
        ///     <item>
        ///     <see cref="TournamentPlayer.SonnebornBerger"/>.
        ///     </item>
        ///     </list>
        /// </para>
        /// </summary>
        RoundRobin,
    }
}
