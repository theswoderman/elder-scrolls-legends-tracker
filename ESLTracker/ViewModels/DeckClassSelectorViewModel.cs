﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels
{
    public class DeckClassSelectorViewModel : ViewModelBase, IDeckClassSelectorViewModel
    {
        /// <summary>
        /// fiter of attributes, 
        /// bool value binded to isenabled property of trigger button
        /// </summary>
        public Dictionary<DeckAttribute, bool> FilterButtonState { get; set; }

        /// <summary>
        /// source for drop down , list of classes that match current filter
        /// </summary>
        public ObservableCollection<DeckClass> FilteredClasses { get; set; }

        private DeckClass? selectedClass;
        public DeckClass? SelectedClass {
            get
            {
                return selectedClass;
            }
            set
            {
                selectedClass = value;
                RaisePropertyChangedEvent("SelectedClass");
            }
        }

        public DeckAttributes SelectedClassAttributes
        {
            get
            {
                if (SelectedClass.HasValue)
                {
                    return ClassAttributesHelper.Classes[SelectedClass.Value];
                }
                else
                {
                    return null;
                }
            }
        }

        //command for filter toggle button pressed
        public ICommand CommandFilterButtonPressed
        {
            get { return new RelayCommand(new Action<object>(FilterClicked)); }
        }

        public DeckClassSelectorViewModel()
        {
            FilterButtonState = new Dictionary<DeckAttribute, bool>();
            foreach (DeckAttribute a in Enum.GetValues(typeof(DeckAttribute)))
            {
                FilterButtonState.Add(a, false);
            }

            FilteredClasses = new ObservableCollection<DeckClass>();
            FilterCombo();
        }

        public void FilterClicked(object param)
        {
            DeckAttribute attrib;
            if (!Enum.TryParse<DeckAttribute>(param.ToString(), out attrib))
            {
                throw new ArgumentException(string.Format("Unknow value for deck attribute={0}", param));
            }

            //toggle filter value
            FilterButtonState[attrib] = ! FilterButtonState[attrib];

            FilterCombo();
        }

        public void FilterCombo()
        {
            var filteredClasses = Utils.ClassAttributesHelper.FindClassByAttribute(FilterButtonState.Where(f => f.Value).Select(f => f.Key));
            FilteredClasses.Clear();
            foreach(DeckClass dc in filteredClasses)
            {
                FilteredClasses.Add(dc);
            }
            if ((FilteredClasses.Count >= 1)
                && (FilterButtonState.Any(f => f.Value)))
            {
                SelectedClass = FilteredClasses[0];
            }
            else
            {
                SelectedClass = null;
            }
        }
                
        public void Reset()
        {
            foreach (DeckAttribute a in Enum.GetValues(typeof(DeckAttribute)))
            {
                FilterButtonState[a] = false;
            }
            FilterCombo();
            RaisePropertyChangedEvent("FilterButtonState");
        }
    }
}
