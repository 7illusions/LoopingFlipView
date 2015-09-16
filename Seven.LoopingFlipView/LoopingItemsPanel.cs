using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Seven.Controls.Panels
{
    public class LoopingItemsPanel : Panel
    {
        private readonly TimeSpan _animationDuration = TimeSpan.FromMilliseconds(300);
        private int _selectedIndex = -1;
        private Slider _animationSlider;        
        private double _offset;
        private double _itemWidth = 1d;
        private bool _templateApplied;
        private int ItemsCount => Children?.Count ?? 0;

        public LoopingItemsPanel()
        {
            ManipulationMode = ManipulationModes.TranslateX;
            SizeChanged += OnSizeChanged;
            _animationSlider = new Slider
            {
                SmallChange = 0.0000000001,
                Minimum = double.MinValue,
                Maximum = double.MaxValue,
                StepFrequency = 0.0000000001
            };
            _animationSlider.ValueChanged += OnAnimationOffsetChanged;
        }
        
        protected override Size ArrangeOverride(Size finalSize)
        {
            Clip = new RectangleGeometry { Rect = new Rect(0, 0, finalSize.Width, finalSize.Height) };

            double positionLeft = 0d;

            foreach (var item in Children)
            {
                if (item != null)
                {
                    Size desiredSize = item.DesiredSize;

                    if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                    {
                        var rect = new Rect(positionLeft, 0, desiredSize.Width, desiredSize.Height);
                        item.Arrange(rect);
                        item.RenderTransform = new TranslateTransform();
                        positionLeft += desiredSize.Width;
                    }
                }
            }

            _templateApplied = true;

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var maxItemHeight = 0d;
            var size = base.MeasureOverride(availableSize);
            Clip = new RectangleGeometry { Rect = new Rect(0, 0, size.Width, size.Height) };

            foreach (var container in Children)
            {
                container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                if (container.DesiredSize.Height > maxItemHeight)
                {
                    maxItemHeight = container.DesiredSize.Height;
                }

                if (_itemWidth != container.DesiredSize.Width)
                {
                    _itemWidth = container.DesiredSize.Width;
                }
            }
            size.Height = maxItemHeight;
            return (size);
        }

        public void ScrollIntoView(int index)
        {
            _selectedIndex = index;
            var item = Children[index];
            var rect = item.TransformToVisual(this).TransformBounds(new Rect(0, 0, item.DesiredSize.Width, item.DesiredSize.Height));
            ScrollIntoView(item, rect, true);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_selectedIndex > -1)
            {
                var child = Children[_selectedIndex];
                _itemWidth = child.DesiredSize.Width;
                TranslateTransform compositeTransform = (TranslateTransform)child.RenderTransform;

                if (compositeTransform == null)
                    return;
                var rect = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));

                ScrollIntoView(child, rect, false);
            }
        }

        private void OnAnimationOffsetChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            UpdatePositions(e.NewValue - e.OldValue);
        }

        private void ScrollIntoView(UIElement item, Rect rect, bool animate = true)
        {
            if (!_templateApplied)
                return;

            var transform = item.RenderTransform as TranslateTransform;
            if (transform != null)
            {
                var centerLeftOffset = (ActualWidth / 2d) - (_itemWidth) / 2d;
                var deltaOffset = centerLeftOffset - rect.X;

                if (animate)
                {
                    AnimateToPosition(transform.X, transform.X + deltaOffset);
                }
                else
                {
                    _offset = 0;
                    UpdatePositions(deltaOffset);
                }
            }
        }

        private void AnimateToPosition(double fromOffset, double toOffset)
        {
            var storyboard = new Storyboard();
            var animation = new DoubleAnimation
            {
                EnableDependentAnimation = true,
                From = fromOffset,
                To = toOffset,
                Duration = _animationDuration,
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
            };

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, _animationSlider);
            Storyboard.SetTargetProperty(animation, "Value");

            _animationSlider.ValueChanged -= OnAnimationOffsetChanged;
            _animationSlider.Value = fromOffset;
            _animationSlider.ValueChanged += OnAnimationOffsetChanged;

            storyboard.Begin();
        }

        private void UpdatePositions(double offsetDelta)
        {
            double maxLogicalWidth = ItemsCount * _itemWidth;
            
            _offset = (_offset + offsetDelta) % maxLogicalWidth;
            
            int itemIndex = (int)(Math.Abs(_offset) / _itemWidth);

            int nextItemIndex;
            double offsetAfter;
            double offsetBefore;

            if (_offset > 0)
            {
                nextItemIndex = ItemsCount - itemIndex - 1;
                offsetAfter = _offset;

                if (_offset % maxLogicalWidth == 0)
                {
                    nextItemIndex++;
                }

                offsetBefore = offsetAfter - maxLogicalWidth;
            }
            else
            {
                nextItemIndex = itemIndex;
                offsetBefore = _offset;
                offsetAfter = maxLogicalWidth + offsetBefore;
            }


            // Left of selected item
            UpdatePosition(nextItemIndex, ItemsCount, offsetBefore);

            //Right of selectedItem
            UpdatePosition(0, nextItemIndex, offsetAfter);
        }

        private void UpdatePosition(int startIndex, int endIndex, double offset)
        {
            for (var i = startIndex; i < endIndex; i++)
            {
                var loopListItem = Children[i];
                TranslateTransform compositeTransform = loopListItem.RenderTransform as TranslateTransform;
                if (compositeTransform != null)
                {
                    compositeTransform.X = offset;
                }
            }
        }
    }
}
