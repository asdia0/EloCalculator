# EloCalculator

Calculates the Elo rating of players and creates a leaderboard.

## Usage

Create a .xlsx file in the same directory as the executable with game details in the following format:

Column 1: Player 1's name  
Column 2: Player 2's name  
Column 3: Result (0-1, 1-0, 1/2-1/2)  
Column 4: Date  

## Customisation

The k and benchmark coefficients used in the formula that updates a player's elo can be changed in [Game.cs](https://github.com/asdia0/EloCalculator/blob/master/EloCalculator/Game.cs).
