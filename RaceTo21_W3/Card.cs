using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceTo21
{
    //Cards that game need and their attributes.
    public class Card
    {
        public string id;

        public string fullName;

        public Card (string shortName,string longName)
        {
            id = shortName;
            fullName = longName;
        }

    }
}
