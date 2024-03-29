﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Timetable.Common;
using Timetable.Models;
using Timetable.Providers;
using Timetable.Utils;

// Документацию по шаблону проекта "Универсальное приложение с Hub" см. по адресу http://go.microsoft.com/fwlink/?LinkID=391955

namespace Timetable
{
    /// <summary>
    /// Страница, на которой отображается сгруппированная коллекция элементов.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        /// <summary>
        /// Получает NavigationHelper, используемый для облегчения навигации и управления жизненным циклом процессов.
        /// </summary>
        public NavigationHelper NavigationHelper { get; }

        /// <summary>
        /// Получает DefaultViewModel. Эту модель можно изменить на модель строго типизированных представлений.
        /// </summary>
        public ObservableDictionary DefaultViewModel { get; } = new ObservableDictionary();

        public HubPage()
        {
            InitializeComponent();
            NavigationHelper = new NavigationHelper(this);
            NavigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

        /// <summary>
        /// Заполняет страницу содержимым, передаваемым в процессе навигации.  Также предоставляется любое сохраненное состояние
        /// при повторном создании страницы из предыдущего сеанса.
        /// </summary>
        /// <param name="sender">
        /// Источник события; как правило, <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Данные события, предоставляющие параметр навигации, который передается
        /// <see cref="Frame.Navigate(Type, object)"/> при первоначальном запросе этой страницы, и
        /// словарь состояний, сохраненных этой страницей в ходе предыдущего
        /// сеанса.  Это состояние будет равно NULL при первом посещении страницы.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var days = await CacheProvider.LoadTimetable();
            ColorsHelper.SetRandomColors(days);
            DefaultViewModel["Days"] = days;

            LoadDateRange();
            await LoadGroups();
        }

        #region Регистрация NavigationHelper

        /// <summary>
        /// Методы, предоставленные в этом разделе, используются исключительно для того, чтобы
        /// NavigationHelper для отклика на методы навигации страницы.
        /// Логика страницы должна быть размещена в обработчиках событий для 
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// и <see cref="Common.NavigationHelper.SaveState"/>.
        /// Параметр навигации доступен в методе LoadState 
        /// в дополнение к состоянию страницы, сохраненному в ходе предыдущего сеанса.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            await LoadTimetable();
        }

        private async void GroupList_OnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var selectedGroup = (Group) selectionChangedEventArgs.AddedItems[0];
            SettingsProvider.Group = selectedGroup;
            await LoadTimetable();
        }

        private async Task LoadTimetable()
        {
            if (GroupList.SelectedItem == null) return;

            ProgressBar.IsIndeterminate = true;

            try
            {
                var group = ((Group)GroupList.SelectedItem).Id;
                var dateRange = ((ComboBoxItem)DateRangeList.SelectedItem).Tag.ToString();

                var dr = DateUtils.GetDateRange(dateRange);

                var days = await DataProvider.GetTimetableByGroup(group, dr);
                ColorsHelper.SetRandomColors(days);
                DefaultViewModel["Days"] = days;

                await CacheProvider.SaveTimetable(days);
            }
            catch (WebException)
            {
                await new MessageDialog(
                    "Невозможно загрузить данные. Пожалуйста, проверьте Ваше подключение к Интернет и повторите попытку.",
                    "Проблемы с соединением").ShowAsync();
            }

            ProgressBar.IsIndeterminate = false;
        }

        private async Task LoadGroups()
        {
            var selectedGroup = SettingsProvider.Group;
            try
            {
                DefaultViewModel["Groups"] = await DataProvider.GetGroups();
                if (selectedGroup != null)
                {
                    GroupList.SelectedItem = selectedGroup;
                }
                else
                {
                    GroupList.SelectedIndex = 0;
                }
            }
            catch (WebException)
            {
                if (selectedGroup != null)
                {
                    DefaultViewModel["Groups"] = new ObservableCollection<Group> {selectedGroup};
                    GroupList.SelectedIndex = 0;
                }
                GroupList.IsEnabled = false;
            }
        }

        private void LoadDateRange()
        {
            var dateRangeType = SettingsProvider.DateRangeType;
            DateRangeList.SelectedItem = DateRangeList.Items.Single(o => ((ComboBoxItem)o).Tag.Equals(dateRangeType));
            var selectedItem = (ComboBoxItem)DateRangeList.SelectedItem;
            if (selectedItem != null && selectedItem.Tag.ToString() == "custom")
            {
                selectedItem.Content = SettingsProvider.CustomDateRange.ToString();
            }
        }

        private async void DateRangeList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0 || e.RemovedItems.Count == 0 || e.AddedItems[0] == null || e.RemovedItems[0] == null) return;

            var selectedItem = (ComboBoxItem) e.AddedItems[0];
            var dateRangeType = selectedItem.Tag.ToString();

            if (dateRangeType == "custom")
            {
                var dlg = new DateRangePickerDialog(SettingsProvider.CustomDateRange);
                var dateRange = await dlg.ShowAsync();
                if (dateRange != null)
                {
                    SettingsProvider.CustomDateRange = dateRange;
                    DateRangeList.SelectedIndex = -1; // it's a hack, used to refresh custom date range item content
                    selectedItem.Content = dateRange.ToString();
                    DateRangeList.SelectedItem = selectedItem;
                }
                else
                {
                    DateRangeList.SelectedItem = e.RemovedItems[0];
                    return;
                }
            }
            else
            {
                var deselectedItem = (ComboBoxItem) e.RemovedItems[0];
                if (deselectedItem != null && deselectedItem.Tag.ToString() == "custom")
                {
                    deselectedItem.Content = "Свой вариант";
                }
            }

            SettingsProvider.DateRangeType = dateRangeType;
            await LoadTimetable();
        }
    }
}
