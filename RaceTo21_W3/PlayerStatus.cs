using System;
namespace RaceTo21
{
	public enum PlayerStatus
	{
		active,//Player can continue draw cards
		stay,//Player doesn't draw card, until active
		bust,//Player loses game, can not draw card
		win,//Player win the game
		Gambler,//Player wanna bet
	}
}

