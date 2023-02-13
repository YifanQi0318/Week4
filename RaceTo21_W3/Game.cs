using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class Game
    {
        private int numberOfPlayers; // number of players in current game
        private List<Player> players = new List<Player>(); // list of objects containing player data
        
        private List<Player> giveUpPlayers = new List<Player>(); // To keep the data of players who give up

        private CardTable cardTable; // object in charge of displaying game information
        private Deck deck = new Deck(); // deck of cards
        private int currentPlayer = 0; // current player on list
        public Tasks nextTask; // keeps track of game state
        private readonly bool cheating = true; // lets you cheat for testing purposes if true
        public bool Cheating { get { return cheating; } } // Use this to keep "cheating" readonly

        private int highPoints = 0; // Implementation: Set a variable to keep track the high points (part of Level2)
        private int pointsToGameOver = 60; // Implementation: Set a variable to determine the score that needs to be reached to end the game

        public Game(CardTable c)
        {
            cardTable = c;
            deck.buildDeck();
            deck.Shuffle();
            // deck.ShowAllCards();
            Console.WriteLine("*****************"); 
            Console.WriteLine("The final winner will be the one who has more than " + pointsToGameOver + " points!");
            Console.WriteLine("If everyone quits early, or if only one player is present, the player who has the highest points wins！");
            

            nextTask = Tasks.GetNumberOfPlayers;
        }

        /* Adds a player to the current game
         * Called by DoNextTask() method
         */
        private void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }

        /* Figures out what task to do next in game
         * as represented by field nextTask
         * Calls methods required to complete task
         * then sets nextTask.
         */



        public void DoNextTask()
        {
            Console.WriteLine("================================"); // this line should be elsewhere right?
            if (nextTask == Tasks.GetNumberOfPlayers)
            {
                numberOfPlayers = cardTable.GetNumberOfPlayers();
                nextTask = Tasks.GetNames;
            }
            else if (nextTask == Tasks.GetNames)
            {
                for (var count = 1; count <= numberOfPlayers; count++)
                {
                    var name = cardTable.GetPlayerName(count);
                    AddPlayer(name); // NOTE: player list will start from 0 index even though we use 1 for our count here to make the player numbering more human-friendly
                }
                nextTask = Tasks.IntroducePlayers;
            }
            else if (nextTask == Tasks.IntroducePlayers)
            {
                cardTable.ShowPlayers(players);
                nextTask = Tasks.PlayerTurn;
            }
            else if (nextTask == Tasks.PlayerTurn)
            {
                cardTable.ShowHands(players);
                Player player = players[currentPlayer];
                if (player.status == PlayerStatus.active)
                {
                   
                    int drawnCardNumber = cardTable.OfferHowManyCards(player);

                    if (drawnCardNumber != 0) // If the player want to draw cards
                    {
                        
                        for (int i = 0; i < drawnCardNumber; i++) // Draw the number of cards indicated by the player
                        {
                            Card card = deck.DealTopCard();
                            player.cards.Add(card);
                        }

                        player.score = ScoreHand(player);
                        

                        if (player.score > 21)
                        {
                                player.status = PlayerStatus.bust;

                                int losePoints = player.score - 21; 
                                player.points -= losePoints;
                                cardTable.ShowHand(player);

                        }
                        else if (player.score == 21) // The player win
                        {
                                player.status = PlayerStatus.win;
                                cardTable.ShowHand(player);

                        }
                        else 
                        {
                            cardTable.ShowHand(player);
                            Console.Write("Do you want to stay? (Y/N)");
                            string response = Console.ReadLine();
                            if (response.ToUpper().StartsWith("Y")) 
                            {
                                player.status = PlayerStatus.stay;
                            }
                            else if (response.ToUpper().StartsWith("N")) 
                            {
                                player.status = PlayerStatus.active; 
                            }
                            else
                            {
                                Console.WriteLine("Please answer Y/N!");
                            }

                        }           
                    }
                    else 
                    {
                        player.status = PlayerStatus.stay;
                    }

                }
                
                nextTask = Tasks.CheckForEnd;
            }
            else if (nextTask == Tasks.CheckForEnd)
            {
                if (!CheckActivePlayers()) 
                {
                    Player winner = DoFinalScoring(); 
  
                    if (winner != null) 
                    {
                        winner.points += winner.score; 
                    }
                   

                   
                    if (winner != null) 
                    {
                        cardTable.AnnounceWinner(winner);
                        IsContinue(); 
                    }
                    else 
                    {
                        cardTable.resultForNoDrawnCard(); // Add a new method to told all players that nobody draws card.
                        // Reset all players status
                        foreach (var player in players)
                        {
                            player.status = PlayerStatus.active;
                        }

                        if (currentPlayer == (players.Count - 1)) // If the current player is the final player, reset the variable currentPlayer
                        {
                            currentPlayer = 0; // back to the first player...
                        }

                        nextTask = Tasks.PlayerTurn;
                    }

                }
                else
                {
                    currentPlayer++;
                    
                    nextTask = Tasks.PlayerTurn;
                }
            }
            else // we shouldn't get here...
            {
                Console.WriteLine("I'm sorry, I don't know what to do now!");
                nextTask = Tasks.GameOver;
            }
        }

      
        private int ScoreHand(Player player)
        {
            int score = 0;

            if (Cheating == true && player.status == PlayerStatus.active)
            {
                string response = null;
                while (int.TryParse(response, out score) == false)
                {
                    Console.Write("OK, what should player " + player.name + "'s score be?");
                    response = Console.ReadLine();
                }
                return score;
            }
            else
            {
                foreach (Card card in player.cards)
                {
                    string faceValue = card.id.Remove(card.id.Length - 1);
                    switch (faceValue)
                    {
                        case "K":
                        case "Q":
                        case "J":
                            score = score + 10;
                            break;
                        case "A":
                            score = score + 1;
                            break;
                        default:
                            score = score + int.Parse(faceValue);
                            break;
                    }
                }
            }
            return score;
        }

        
        private bool CheckActivePlayers()
        {
            
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.win)
                {
                    return false; 
                }
            }

            
            int bustPlayerNumber = 0; 

            foreach (var player in players)
            {
                if (player.status == PlayerStatus.bust)
                {
                    bustPlayerNumber++; 
                }
            }
            
            if (bustPlayerNumber == players.Count - 1)
            {
                return false;
            }


            foreach (var player in players)
            {
                if (player.status == PlayerStatus.active)
                {
                    return true; 
                }
            }

            return false;
        }


       
        private Player DoFinalScoring()
        {
            int highScore = 0; 
            foreach (var player in players)
            {
                cardTable.ShowHand(player);
                if (player.status == PlayerStatus.win) // someone hit 21
                {
                    return player;
                }
                if (player.status == PlayerStatus.stay) // still could win...
                {
                    if (player.score > highScore)
                    {
                        highScore = player.score;
                    }
                }
                // if busted don't bother checking!
            }

            if (highScore > 0) // someone scored, anyway! And no one bust
            {
                // find the FIRST player in list who meets win condition
                return players.Find(player => player.score == highScore); 
            }

            // Fix: Check if there is a player
            if (players.Find(player => player.status == PlayerStatus.bust) != null)
            {
                return players.Find(player => player.status != PlayerStatus.bust);  // Adjust: if all but one player “busts”, remaining player should immediately win
            }

            // No player draws card
            return null;
        }


        /* Implementation:
         * At end of round, each player is asked if they want to keep playing. If a player says no, they are
         * removed from the player list. If only 1 player remains, that player is the winner (equivalent to
         * everyone else “folding” in a card game).  If players choose to keep going, a new deck is built
         * and shuffled. In addition, player list is shuffled, to ensure the same person doesn’t always win 
         * a tiebreaker. 
         */
        
        private void IsContinue()
        {
            
            int lastTotalPlayers = players.Count;

            
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].points > highPoints)
                {
                    highPoints = players[i].points;
                }
            }
            if (highPoints < pointsToGameOver)
            {
                Console.WriteLine("================================");
                for (int i = 0; i < players.Count; i++)
                {
                    Console.Write(players[i].name + ", do you want to play another round? (Y/N)");
                    string response = Console.ReadLine();
                  
                    if (response.ToUpper().StartsWith("Y"))
                    {
                        players[i].cards = new List<Card>();
                        players[i].score = 0;
                        players[i].status = PlayerStatus.active;
                    }
                   
                    else if (response.ToUpper().StartsWith("N"))
                    {
                        giveUpPlayers.Add(players[i]);
                        players.RemoveAt(i);
                        i--;  
                    }
                    else
                    {
                        Console.WriteLine("Please answer Y(es) or N(o)!");
                    }
                }

                
                if (players.Count <= lastTotalPlayers && players.Count > 1) 
                {
                    shufflePlayer();
                    deck.buildDeck();
                    deck.Shuffle();
                    currentPlayer = 0;
                    nextTask = Tasks.IntroducePlayers;
                }
                else 
                {
                    Console.WriteLine("================================");
                    int winnerPoints = 0;
                    foreach(var player in players)
                    {
                        Console.WriteLine(player.name + "'s points: " + player.points);
                        if (winnerPoints < player.points)
                        {
                            winnerPoints = player.points;
                        }
                    }
                    foreach(var player in giveUpPlayers)
                    {
                        Console.WriteLine(player.name + "'s points: " + player.points);
                        if (winnerPoints < player.points)
                        {
                            winnerPoints = player.points;
                        }
                    }

                    if (players.Find(player => player.score == winnerPoints) != null)
                    {
                        Console.WriteLine(players.Find(player => player.score == winnerPoints).name + " is the final winner!");
                    }

                                      

                    Console.Write("Press <Enter> to exit... ");
                    while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                    nextTask = Tasks.GameOver;
                }
            }
            else
            {
                Console.WriteLine("================================");
                foreach (var player in players)
                {
                    Console.WriteLine(player.name + "'s points: " + player.points);
                }
                foreach (var player in giveUpPlayers)
                {
                    Console.WriteLine(player.name + "'s points: " + player.points);
                }
                Console.WriteLine(players.Find(player => player.points == highPoints).name + " is the final winner!");
                Console.Write("Press <Enter> to exit... ");
                while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                nextTask = Tasks.GameOver;
            } 
        }


        
        private void shufflePlayer()
        {
            Random rng = new Random();

            for (int i = 0; i < players.Count; i++)
            {
                Player tmp = players[i];
                int swapindex = rng.Next(players.Count);
                players[i] = players[swapindex];
                players[swapindex] = tmp;
            }
        }
    }
}
