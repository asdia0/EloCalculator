# EloCalculator

EloCalculator is a program designed to automate chess rating and tournament calculations.

An example program can be found [here](src/EloCalculator.Example/Program.cs).


## Features

- Elo calculator
- Tournament scores calculator
  - [Conventional score](https://en.wikipedia.org/wiki/Chess_tournament#Scoring)
  - [Sonneborn-Berger (Neustadl)](https://en.wikipedia.org/wiki/Sonneborn%E2%80%93Berger_score)
  - [Buchholz](https://en.wikipedia.org/wiki/Buchholz_system)
  - [Median Buchholz](https://en.wikipedia.org/wiki/Tie-breaking_in_Swiss-system_tournaments#Median_/_Buchholz_/_Solkoff)
  - Buchholz Cut 1 (Buchholz but the lowest score is not considered)
  - [Culmulative](https://en.wikipedia.org/wiki/Tie-breaking_in_Swiss-system_tournaments#Cumulative)
  - [Baumbach](https://en.wikipedia.org/wiki/Tie-breaking_in_Swiss-system_tournaments#Most_wins_(Baumbach))
- Tournament types
  - [Dutch](https://en.wikipedia.org/wiki/Swiss-system_tournament#Dutch_system)
  - [Monrad](https://en.wikipedia.org/wiki/Swiss-system_tournament#Monrad_system)
  - [Danish](https://en.wikipedia.org/wiki/Swiss-system_tournament#Danish_system)
  - [Round-Robin](https://en.wikipedia.org/wiki/Chess_tournament#Round-robin)
  - [Arena](https://support.chess.com/article/335-what-are-arena-tournaments)
  - [Match](https://www.chess.com/terms/chess-match)
- Tournament pairings
- Leaderboards (global and tournament)
