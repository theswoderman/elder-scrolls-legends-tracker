﻿using System;
using System.Collections.Generic;
using System.Linq;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.DataModel
{
    public class Deck
    {
        public Guid DeckId { get; set; }
        public DeckType Type { get; set; }
        public string Name { get; set; }
        public DeckAttributes Attributes { get; set; } = new DeckAttributes();
        public DeckClass Class { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }


        public Deck()
        {
            DeckId = Guid.NewGuid(); //if deserialise, will be overriten!, if new generate!
            CreatedDate = DateTime.Now;
        }

        public int Victories {
            get {
                return GetDeckGames().Where(g => g.Outcome == GameOutcome.Victory).Count();
            }
        }

        public int Defeats
        {
            get
            {
                return GetDeckGames().Where(g => g.Outcome == GameOutcome.Defeat).Count();
            }
        }

        public string WinRatio
        {
            get
            {
                int gamesTotal = GetDeckGames().Count();
                if (gamesTotal != 0)
                {
                    return Math.Round((double)Victories / (double)GetDeckGames().Count() * 100, 0).ToString();
                }
                else
                {
                    return "-";
                }
            }
        }

        public IEnumerable<Game> GetDeckGames()
        {
            return Tracker.Instance.Games.Where(g => g.Deck.DeckId == this.DeckId);
        }

        
    }
}