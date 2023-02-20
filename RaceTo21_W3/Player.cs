using System;
using System.Collections.Generic;

namespace RaceTo21
{
	public class Player
	{
		public string name;//Save private name
		public List<Card> cards = new List<Card>(); // To store the player's hands
		public PlayerStatus status = PlayerStatus.active; // Keep the player's status
		public int score; // Keep the score of the player's hands
		public int points = 0; // To store the Points the player won
		public int chips = 100;//Amount of Chips every player has


        //public string Name
		//{
	    //get { return Name; }
		//}



        public Player(string n)
		{
			name = n;
        }

		/* Introduces player by name
		 * Called by CardTable object
		 */
		public void Introduce(int playerNum)
		{
			Console.WriteLine("Hello, I'm " + name + ", player " + playerNum);
		}

		public int bet(int Chipamount) 
		{
		if(Chipamount > chips)
			{
				return -1;
			}

			chips -= Chipamount;
			return Chipamount;

		}

		
	}
}

