using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class CardTable
    {
        public CardTable()
        {
            Console.WriteLine("Setting Up Table...");
        }

        /* Shows the name of each player and introduces them by table position.
         * Is called by Game object.
         * Game object provides list of players.
         * Calls Introduce method on each player object.
         */
        public void ShowPlayers(List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].Introduce(i + 1); // List is 0-indexed but user-friendly player positions would start with 1...
            }
        }

        /* Gets the user input for number of players.
         * Is called by Game object.
         * Returns number of players to Game object.
         */
        public int GetNumberOfPlayers()
        {
            Console.Write("How many players? ");
            string response = Console.ReadLine();
            int numberOfPlayers;
            while (int.TryParse(response, out numberOfPlayers) == false
                || numberOfPlayers < 2 || numberOfPlayers > 8)
            {
                Console.WriteLine("Invalid number of players.");
                Console.Write("How many players?");
                response = Console.ReadLine();
            }
            return numberOfPlayers;
        }

        /* Gets the name of a player
         * Is called by Game object
         * Game object provides player number
         * Returns name of a player to Game object
         */
        public string GetPlayerName(int playerNum)
        {
            
            Console.Write("What is the name of player# " + playerNum + "? ");
            string response = Console.ReadLine();
            while (response.Length < 1)
            {
                Console.WriteLine("Invalid name.");
                Console.Write("What is the name of player# " + playerNum + "? ");
                response = Console.ReadLine();
            }
            return response;
           
        }

        //Level 2 Gambler Game
        //Call by Game object

        
        public int PlaceBet(List<Player> playerList)
        {
            Console.WriteLine("===============Bet Phase=================");

            int TotalChips = 0;

            foreach (Player p in playerList)
            {
                Console.Write(p.name + ",how many chips you wanna bet?");
                string response = Console.ReadLine();//Get bets
                if (int.TryParse(response, out int Chipamount)) 
                {
                    int PotChips = p.bet(Chipamount);
                    if(PotChips >= 0)
                    {
                        TotalChips += Chipamount;
                        break;//Use break to stop asking
                    }
                    else
                    {
                        Console.WriteLine("You don't have enough chips");
                    }
                }
                else
                {
                    Console.WriteLine("Enter a vaild number.");
                }
            }
            Console.WriteLine($"As a result, the pot has ${TotalChips}.");
            return TotalChips;
        }

        /**To do List
         * * A player can choose to draw up to 3 cards each turn, but they get all cards at once; they don’t get to 
        decide after each card (more risk, but can get to 21 faster!)
         * **/




        public int OfferHowManyCards(Player player)
        {
            while (true)
            {
                Console.WriteLine("********Draw Phase********");  
                Console.Write(player.name + ",How many cards you wanna draw(0/1/2/3)?");
                string response = Console.ReadLine();
                if (int.TryParse(response, out int howManyCards))
                {
                    if (howManyCards <= 3 && howManyCards >= 0)
                    {
                        return howManyCards;
                    }
                    else
                    {
                        Console.WriteLine("Please Type a valid namber");//Wrong number
                    }
                }
                else
                {
                    Console.WriteLine("Please Type a valid number");//Invaild number
                }
            }
        }

        /*To do List:Players who went “bust” lose points equal to their hand total minus 21. 
            o Players who “stay” earn no points for the round.
            o Game ends when one player reaches an agreed-upon score (for example, 100 points) or after an
            agreed-upon number of rounds.
                • Allow players to customize this number (rounds or score, whichever you choose) at start
                of game.
                    • At start of game, let players choose whether game will last a specific number of
                    rounds or until an agreed-upon score is reached.*/




        public void ShowHand(Player player)
        {   
            if (player.cards.Count > 0) 
            {
                Console.Write(player.name + " has ");

                string allCards = ""; 

                foreach (Card card in player.cards)
                {
                    
                    allCards = allCards + card.fullName ; 
                }

                Console.WriteLine(allCards.Remove(allCards.Length - 2) + " = " + player.score + "/21 "); 

                Console.WriteLine(player.name + "points: " + player.points);

                if (player.status != PlayerStatus.active) 
                {
                    Console.Write("(" + player.status.ToString().ToUpper() + ")");
                }
                Console.WriteLine();
            }
        }

    
        public void ShowHands(List<Player> players)
        {
            foreach (Player player in players)
            {
                ShowHand(player);
            }
        }

        
        public void AnnounceWinner(Player player)
        {
            
            Console.WriteLine(player.name + " wins!");

            Console.WriteLine(player.name + "'s points: " + player.points +  " , this player wins!");

        }

       
        public void resultForNoDrawnCard()
        {
            Console.WriteLine("No player draws card!");
        }
    }
}