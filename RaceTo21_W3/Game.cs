﻿using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class Game
    {
        int numberOfPlayers; // number of players in current game
        List<Player> players = new List<Player>(); // list of objects containing player data
        CardTable cardTable; // object in charge of displaying game information
        Deck deck = new Deck(); // deck of cards
        int currentPlayer = 0; // current player on list
        public Tasks nextTask; // keeps track of game state
        private bool cheating = true; // lets you cheat for testing purposes if true

        public Game(CardTable c)
        {
            cardTable = c;
            deck.buildDeck();
            deck.Shuffle();
            deck.ShowAllCards();
            nextTask = Tasks.GetNumberOfPlayers;
        }

        /* Adds a player to the current game
         * Called by DoNextTask() method
         */
        public void AddPlayer(string n)
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
                    /* Implementation:
                     * A player can choose to draw up to 3 cards each turn, but they get
                     * all cards at once; they don’t get to decide after each card
                     */

                    bool drawOneCard = cardTable.OfferACard(player); // Check if the player want to draw a card
                    bool drawThreeCard = false; // Keep the result of OfferThreeCards method

                    // When the player want to draw a card, the game will ask the player if he/she wants to draw three cards
                    if (drawOneCard)
                    {
                        drawThreeCard = cardTable.OfferThreeCards(player);
                    }

                    if (drawOneCard && !drawThreeCard)
                    {
                        // The player want to draw one card
                        Card card = deck.DealTopCard();
                        player.cards.Add(card);
                        player.score = ScoreHand(player);
                        if (player.score > 21)
                        {
                            player.status = PlayerStatus.bust;
                        }
                        else if (player.score == 21)
                        {
                            player.status = PlayerStatus.win;

                        }
                    }
                    else if (drawOneCard && drawThreeCard)
                    {
                        // The player want to draw three cards
                        for (int i = 1; i <= 3; i++)
                        {
                            Card card = deck.DealTopCard();
                            player.cards.Add(card);
                        }
                        player.score = ScoreHand(player);
                        if (player.score > 21)
                        {
                            player.status = PlayerStatus.bust;
                        }
                        else if (player.score == 21)
                        {
                            player.status = PlayerStatus.win;

                        }

                        // If the player still not bust or win, the player will stay
                        if (player.status == PlayerStatus.active)
                        {
                            player.status = PlayerStatus.stay; // If the player want to draw three cards, the player will stay
                        }

                    }
                    else
                    {
                        player.status = PlayerStatus.stay;
                    }
                }
                cardTable.ShowHand(player);
                nextTask = Tasks.CheckForEnd;
            }
            else if (nextTask == Tasks.CheckForEnd)
            {
                if (!CheckActivePlayers())
                {
                    Player winner = DoFinalScoring();

                    // Adjust: Put the winner detection out of the AnnounceWinner method. If no player draws card, the game will not stop.
                    if (winner != null)
                    {
                        cardTable.AnnounceWinner(winner);

                        /* Console.Write("Press <Enter> to exit... ");
                         while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                         nextTask = Tasks.GameOver;*/

                        IsContinue();
                    }
                    else
                    {
                        cardTable.resulfForNoDrawnCard(winner); // Add a new method to told all players that nobody draws card.
                        // Reset all players status
                        foreach (var player in players)
                        {
                            player.status = PlayerStatus.active;
                        }

                        nextTask = Tasks.PlayerTurn;
                    }

                }
                else
                {
                    currentPlayer++;
                    if (currentPlayer > players.Count - 1)
                    {
                        currentPlayer = 0; // back to the first player...
                    }
                    nextTask = Tasks.PlayerTurn;
                }
            }
            else // we shouldn't get here...
            {
                Console.WriteLine("I'm sorry, I don't know what to do now!");
                nextTask = Tasks.GameOver;
            }
        }

        public int ScoreHand(Player player)
        {
            int score = 0;
            if (cheating == true && player.status == PlayerStatus.active)
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

        public bool CheckActivePlayers()
        {
            // Adjust: When check the first winner, end the game
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.win)
                {
                    return false; // check the first winner
                }
            }

            // Adjust: Returns the end signal if all but one player bust
            int bustPlayerNumber = 0; // Save bust player number

            foreach (var player in players)
            {
                if (player.status == PlayerStatus.bust)
                {
                    bustPlayerNumber++; // If one player bust, plus the bust player number
                }
            }
            // Check if all but one players are bust
            if (bustPlayerNumber == players.Count - 1)
            {
                return false;
            }


            foreach (var player in players)
            {
                if (player.status == PlayerStatus.active)
                {
                    return true; // at least one player is still going!
                }
            }

            return false; // everyone has stayed!
        }

        public Player DoFinalScoring()
        {
            int highScore = 1; // Adjust: change the highScore from 0 to 1, because when a player draw a card the highScore must be 1 or more, so there is no problem.
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
            if (highScore > 0) // someone scored, anyway!
            {
                // find the FIRST player in list who meets win condition
                return players.Find(player => player.score == highScore); // Adjust: Because the default highScore is 1, so if there is nobody draw card, it will return null
            }

            // return null; // everyone must have busted because nobody won!
            return players.Find(player => player.status != PlayerStatus.bust);  // Adjust: if all but one player “busts”, remaining player should immediately win
        }


        /* Implementation:
         * At end of round, each player is asked if they want to keep playing. If a player says no, they are
         * removed from the player list. If only 1 player remains, that player is the winner (equivalent to
         * everyone else “folding” in a card game).  If players choose to keep going, a new deck is built
         * and shuffled. In addition, player list is shuffled, to ensure the same person doesn’t always win 
         * a tiebreaker. 
         */
        public void IsContinue()
        {
            int lastTotalPlayers = players.Count; // Store the last total players

            for (int i = 0; i < players.Count; i++)
            {
                Console.Write(players[i].name + ", do you want to play another turn? (Y/N)");
                string response = Console.ReadLine();
                // If the player agree, all data of this player will be reset
                if (response.ToUpper().StartsWith("Y"))
                {
                    players[i].cards = new List<Card>();
                    players[i].score = 0;
                    players[i].status = PlayerStatus.active;
                }
                // If the player disagree, the player will be removed
                else if (response.ToUpper().StartsWith("N"))
                {
                    players.RemoveAt(i);
                    i--; // When data in the list is removed, the Index of all the data after it is subtracted by one, so here i is also subtracted by one to avoid skipping data. 
                }
                else
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }

            // When the number of players decreases, but is greater than one, shuffle player and reset currentPlayer to avoid error.
            // If players.Count < lastTotalPlayers, it is definitly someone give up
            if (players.Count <= lastTotalPlayers && players.Count > 1)
            {
                shufflePlayer();
                currentPlayer = 0;
                nextTask = Tasks.IntroducePlayers;
            }
            // If just one player wants to continue, that player win the game, and game over
            else if (players.Count == 1)
            {
                Console.WriteLine(players[0].name + " wins!");
                Console.Write("Press <Enter> to exit... ");
                while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                nextTask = Tasks.GameOver;
            }
            // If there is no player wants to continue, the game will end
            else
            {
                Console.WriteLine("Nobody wants to continue!");
                Console.Write("Press <Enter> to exit... ");
                while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                nextTask = Tasks.GameOver;
            }
        }


        /* This method is use to shuffle the player list
         */
        public void shufflePlayer()
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
