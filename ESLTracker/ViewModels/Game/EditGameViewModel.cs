﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels.Game
{
    public class EditGameViewModel : ViewModelBase
    {
        public DataModel.Game game = new DataModel.Game();
        public DataModel.Game Game
        {
            get { return game; }
            set
            {
                game = value;
                Game.PropertyChanged += Game_PropertyChanged;
                RaisePropertyChangedEvent("Game");
            }
        }

        public bool DisplayPlayerRank
        {
            get
            {
                return this.Game.Type == DataModel.Enums.GameType.PlayRanked;
            }
        }

        public bool DisplayBonusRound
        {   
            get
            {
                return this.Game.Type == DataModel.Enums.GameType.PlayRanked;
            }
        }

        public IEnumerable<GameType> AllowedGameTypes
        {
            get
            {
                return GetAllowedGameTypes();

            }
        }

        string opponentClassWins = "Select opponent class";
        public string OpponentClassWins
        {
            get
            {
                return opponentClassWins;
            }
            set
            {
                opponentClassWins = value;
                RaisePropertyChangedEvent("OpponentClassWins");
            }
        }
       
        public string SummaryText
        {
            get
            {
                if (! String.IsNullOrWhiteSpace(game.OpponentName)
                    && game.OpponentClass.HasValue)
                {
                    return string.Format("Game vs {0} ({1})", game.OpponentName, game.OpponentClass);
                }
                else
                {
                    return "Waitng for game details...";
                }
            }
        }

        public RelayCommandWithSettings CommandButtonCreate
        {
            get
            {
                return new RelayCommandWithSettings(
                    new Action<object, Properties.ISettings>(CommandButtonCreateExecute),
                    new Func<object, Properties.ISettings, bool>(CommandButtonCreateCanExecute),
                    Properties.Settings.Default
                    );
            }
        }

        public EditGameViewModel()
        {
            //this.PropertyChanged += EditGameViewModel_PropertyChanged;
            Game.PropertyChanged += Game_PropertyChanged;
            Tracker.Instance.PropertyChanged += Instance_PropertyChanged;
            
        }

        //private void EditGameViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName != "SummaryText")
        //    {
        //        RaisePropertyChangedEvent("SummaryText");
        //    }
        //}

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ActiveDeck")
            {
                RaisePropertyChangedEvent("AllowedGameTypes");
            }
            //throw new NotImplementedException();
        }

        private void Game_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Type")
            {
                RaisePropertyChangedEvent("DisplayPlayerRank");
                RaisePropertyChangedEvent("DisplayBonusRound");

                if (this.Game.Type == DataModel.Enums.GameType.PlayRanked)
                {
                    this.Game.BonusRound = false;
                }
                else
                {
                    this.Game.BonusRound = null;
                }
            }
            else if ((e.PropertyName == "OpponentName")
                || (e.PropertyName == "OpponentClass"))
            {
                  RaisePropertyChangedEvent("SummaryText");
            }
        }

        public void UpdateBindings()
        {
            RaisePropertyChangedEvent("Game");
        }

        public void CommandButtonCreateExecute(object parameter, Properties.ISettings settings)
        {
            object[] args = parameter as object[];
            GameOutcome? outcome = EnumManager.ParseEnumString<GameOutcome>(args[0] as string);
            EditGameViewModel model = args[1] as EditGameViewModel;
            DeckClassSelectorViewModel opponentClass = args[2] as DeckClassSelectorViewModel;
            Controls.PlayerRank opponentRank = args[3] as Controls.PlayerRank;
            Controls.PlayerRank playerRank = args[4] as Controls.PlayerRank;
            if ((model != null)
                && (outcome.HasValue)
                && (opponentClass != null)
                && opponentClass.SelectedClass.HasValue)
            {
                model.Game.Deck = Tracker.Instance.ActiveDeck;
                model.Game.OpponentClass = opponentClass.SelectedClass.Value;
                model.Game.OpponentAttributes.AddRange(opponentClass.SelectedClassAttributes);
                model.Game.Outcome = outcome.Value;

                if (model.Game.Type == GameType.PlayRanked)
                {
                    model.Game.OpponentRank = opponentRank.SelectedItem;
                    model.Game.OpponentLegendRank = opponentRank.LegendRank;

                    if (playerRank.SelectedItem.HasValue)
                    {
                        model.Game.PlayerRank = playerRank.SelectedItem;
                        model.Game.PlayerLegendRank = playerRank.LegendRank;

                        settings.PlayerRank = playerRank.SelectedItem.Value;
                    }
                }

                DataModel.Game addedGame = model.Game;
                Tracker.Instance.Games.Add(model.Game);

                Utils.Messenger.Default.Send(
                    new Utils.Messages.EditDeck() { Deck = game.Deck },
                    Utils.Messages.EditDeck.Context.StatsUpdated);

                FileManager.SaveDatabase();

                model.Game = new DataModel.Game();

                //restore values that are likely the same,  like game type, player rank etc
                model.Game.Type = addedGame.Type;
                model.Game.PlayerRank = addedGame.PlayerRank;
                model.Game.PlayerLegendRank = addedGame.PlayerLegendRank;
                model.UpdateBindings();

                //clear opp class
                opponentClass.Reset();

                //clear opp rank
                opponentRank.SelectedItem = null;
                opponentRank.LegendRank = null; 

            }

        }

        public bool CommandButtonCreateCanExecute(object parameter, Properties.ISettings settings)
        {
            return true;
        }

        private IEnumerable<GameType> GetAllowedGameTypes()
        {
            if (Tracker.Instance.ActiveDeck != null)
            {
                switch (Tracker.Instance.ActiveDeck.Type)
                {
                    case DeckType.Constructed:
                        return new List<GameType>() { GameType.PlayCasual, GameType.PlayRanked };
                        break;
                    case DeckType.VersusArena:
                        return new List<GameType>() { GameType.VersusArena };
                        this.Game.Type = GameType.VersusArena;
                        break;
                    case DeckType.SoloArena:
                        return new List<GameType>() { GameType.SoloArena };
                        this.Game.Type = GameType.SoloArena;
                        break;
                    default:
                        break;
                }
            }
            return Enum.GetValues(typeof(GameType)).Cast<GameType>();
        }

        public void ShowWinsVsClass(DeckClass? deckClass)
        {
            this.Game.OpponentClass = deckClass; //ugly hack until class slectro can be bound in xaml
            if ((deckClass != null) && (Tracker.Instance.ActiveDeck != null))
            {
                var res = Tracker.Instance.ActiveDeck.GetDeckVsClass(deckClass);
                dynamic data = (res as System.Collections.IEnumerable).Cast<object>().FirstOrDefault();
                if (data == null)
                {
                    data = new { Class = deckClass, Victory = 0, Defeat = 0, WinPercent = "-" };
                }
                this.OpponentClassWins = string.Format("vs {0}: {1}-{2} ({3}%)",
                    data.Class,
                    data.Victory,
                    data.Defeat,
                    data.WinPercent);
            }
            else
            {
                this.OpponentClassWins = "Select opponent class";
            }
        }


    }
}
