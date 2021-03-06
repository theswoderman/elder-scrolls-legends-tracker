﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ESLTracker.ViewModels.Decks;

namespace ESLTracker.Controls.Decks
{
    /// <summary>
    /// Interaction logic for DeckTypeSelector.xaml
    /// </summary>
    public partial class DeckTypeSelector : UserControl
    {
        new public DeckTypeSelectorViewModel DataContext
        {
            get
            {
                return (DeckTypeSelectorViewModel)base.DataContext;
            }
            set
            {
                base.DataContext = value;
            }
        }

        public DeckTypeSelector()
        {
            InitializeComponent();
        }
    }
}
