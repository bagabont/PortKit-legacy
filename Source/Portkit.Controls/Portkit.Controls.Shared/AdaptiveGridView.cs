using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Portkit.Controls
{
    /// <summary>
    /// An adaptive <see cref="GridView"/> that restricts item's aspect ratio to <see cref="AspectWidth"/> and <see cref="AspectHeight"/> values.
    /// </summary>
    public class AdaptiveGridView : GridView
    {
        public static readonly DependencyProperty NumberOfCellsProperty = DependencyProperty.Register(
            nameof(NumberOfCells), typeof(int), typeof(AdaptiveGridView), new PropertyMetadata(1, InvalidateControl));

        /// <summary>
        /// Gets or sets the number of rows or columns depending on orientation
        /// </summary>
        public int NumberOfCells
        {
            get { return (int)GetValue(NumberOfCellsProperty); }
            set { SetValue(NumberOfCellsProperty, value); }
        }

        /// <summary>
        /// Minimum height for item
        /// </summary>
        public double AspectHeight
        {
            get { return (double)GetValue(AspectHeightProperty); }
            set { SetValue(AspectHeightProperty, value); }
        }

        public static readonly DependencyProperty AspectHeightProperty = DependencyProperty.Register(
                nameof(AspectHeight), typeof(double), typeof(AdaptiveGridView),
                new PropertyMetadata(1D, InvalidateControl));

        /// <summary>
        /// Minimum width for item (must be greater than zero)
        /// </summary>
        public double AspectWidth
        {
            get { return (double)GetValue(MinimumItemWidthProperty); }
            set { SetValue(MinimumItemWidthProperty, value); }
        }

        public static readonly DependencyProperty MinimumItemWidthProperty = DependencyProperty.Register(
                nameof(AspectWidth), typeof(double), typeof(AdaptiveGridView),
                new PropertyMetadata(1D, InvalidateControl));

        private static void InvalidateControl(DependencyObject s, DependencyPropertyChangedEventArgs a)
        {
            ((AdaptiveGridView)s).InvalidateMeasure();
        }

        public AdaptiveGridView()
        {
            if (ItemContainerStyle == null)
            {
                ItemContainerStyle = new Style(typeof(GridViewItem));
            }
            ItemContainerStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
            Loaded += AdaptiveGridView_Loaded;
        }

        private void AdaptiveGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (ItemsPanelRoot != null)
            {
                InvalidateMeasure();
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var panel = ItemsPanelRoot as ItemsWrapGrid;
            if (panel == null)
            {
                return base.ArrangeOverride(finalSize);
            }

            if (AspectWidth <= 0 || AspectHeight <= 0)
            {
                throw new ArgumentException($"You need to set {nameof(AspectHeight)} and {nameof(AspectWidth)} to a value greater than 0");
            }

            double itemWidth;
            double itemHeight;

            if (panel.Orientation == Orientation.Horizontal)
            {
                var availableWidth = finalSize.Width - (Padding.Right + Padding.Left);
                itemWidth = availableWidth / NumberOfCells;
                var aspectRatio = AspectHeight / AspectWidth;
                itemHeight = itemWidth * aspectRatio;
            }
            else
            {
                var availableHeight = finalSize.Height - (Padding.Top + Padding.Bottom);
                itemHeight = availableHeight / NumberOfCells;
                var aspectRatio = AspectHeight / AspectHeight;
                itemWidth = itemHeight * aspectRatio;
            }

            panel.ItemWidth = itemWidth;
            panel.ItemHeight = itemHeight;

            return base.ArrangeOverride(finalSize);
        }
    }
}