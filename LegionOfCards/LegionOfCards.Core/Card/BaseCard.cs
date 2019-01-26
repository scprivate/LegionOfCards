using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegionOfCards.Core.Game;

namespace LegionOfCards.Core.Card
{
    /// <summary>
    /// Repräsentiert alle Karten als Basis, liefert Basisevents, Funktionen und speichert Informationen, die alle Karten benötigen.
    /// </summary>
    public abstract class BaseCard
    {
        #region Properties
        public int Id { get; }

        public string Name { get; }

        public string Description { get; }

        public bool HasEffect { get; set; }
        #endregion

        #region Events
        public event Action<Player> Activate; 
        #endregion

        public BaseCard(int id, string name, string description, bool hasEffect = true)
        {
            HasEffect = hasEffect;
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
