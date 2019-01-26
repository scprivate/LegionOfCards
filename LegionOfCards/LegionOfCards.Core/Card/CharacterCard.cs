using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegionOfCards.Core.Game;

namespace LegionOfCards.Core.Card
{
    /// <summary>
    /// Repräsentiert eine Charakterkarte mit deren Informationen und Funktionen.
    /// </summary>
    public abstract class CharacterCard : BaseCard
    {
        public int Attack { get; set; }

        public int Defense { get; set; }

        public Position Mode { get; set; }

        public event Action<Player> Call;
        public event Action<Player, bool> Support;
        public event Action<Player, Position> ChangePosition; 

        protected CharacterCard(int id, string name, string description, int attack = 0, int defense = 0, bool hasEffect = true) : base(id, name, description, hasEffect)
        {
            Attack = attack;
            Defense = defense;
        }

        public enum Position
        {
            Attack, DefenseHidden, DefenseShown
        }
    }
}
