﻿using SQLiteShared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Utils.Extensions;
using XAMLMarkup;
using Tickets.CommonUI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Tickets
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignsMarksPage : Page
    {
        SelectionChangedEventHandler handler = null;

        public SignsMarksPage()
        {
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.NavigationMode != NavigationMode.Back)
            {
                IEnumerable<IGrouping<String, CommonObject>> commonObjects = null;

                var type = e.Parameter;
                if ("signs".Equals(type))
                {
                    commonObjects = AppLogic.Static.PreloadedContent.Signs.Data.GroupBy(sign => sign.num.Substring(0, sign.num.IndexOf('.')), sign => new CommonObject(sign)).ToArray();
                }
                else if ("marks".Equals(type))
                {
                    commonObjects = AppLogic.Static.PreloadedContent.Marks.Data.GroupBy(mark => mark.num.Substring(0, mark.num.IndexOf('.')), mark => new CommonObject(mark)).ToArray();
                }
                else
                {
                    throw new NotImplementedException(String.Format("Unexpected type: {0}", e.Parameter));
                }

                handler = (s, ev) =>
                {
                    if (ev.AddedItems.Count == 0)
                    {
                        return;
                    }
                    if (String.Empty.Equals(ev.AddedItems.Single()))
                    {
                        cmbSections.SelectedItem = ev.RemovedItems.Single();
                    }
                    else
                    {
                        var commonObjectsArray = commonObjects.Single(commonObject => commonObject.Key.Equals(ev.AddedItems.Single().ToString())).ToArray();
                        cnvsSignsMarks.DataSource = null;
                        cnvsSignsMarks.DataSource = new VirtualLinkedList<CommonObject[]>(new CommonObject[][] { commonObjectsArray },
                        (dataSource, current) =>
                        {
                            return dataSource.FirstOrDefault();
                        },
                        (dataSource, current) =>
                        {
                            if (this.Frame.CanGoBack)
                            {
                                this.Frame.GoBack();
                            }
                            return null;
                        });
                    }
                };
                var itemsSource = commonObjects.Select(commonObject => commonObject.Key).ToArray();
                if (itemsSource.Length < 6)
                {
                    itemsSource = itemsSource.Concat(Enumerable.Range(0, 6 - itemsSource.Length).Select(i => String.Empty)).ToArray();
                }
                this.cmbSections.ItemsSource = itemsSource;
                this.cmbSections.SelectionChanged += handler;
                this.cmbSections.SelectedItem = itemsSource.First();
            }
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                this.cmbSections.SelectionChanged -= handler;
            }
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var image = sender as Image;
            this.Frame.Navigate(typeof(SignMarkPage), image.Tag);
        }
    }
}
