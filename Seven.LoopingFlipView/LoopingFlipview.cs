using System;
using System.Linq;
using Seven.Controls.Panels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Seven.Controls
{
    [TemplatePart(Name = "LoopingPanel", Type = typeof(LoopingItemsPanel))]
    [TemplatePart(Name = "NextButton", Type = typeof(Button))]
    [TemplatePart(Name = "PreviousButton", Type = typeof(Button))]
    [TemplateVisualState(GroupName = "DisplayStates", Name = "Show")]
    [TemplateVisualState(GroupName = "DisplayStates", Name = "Hide")]
    public class LoopingFlipview : GridView
    {
        private readonly DispatcherTimer _updateTimer;
        private bool _templateApplied;
        private Button _nextButton;
        private Button _previousButton;        

        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register("Interval", typeof(int), typeof(LoopingFlipview), new PropertyMetadata(0, IntervalSet));
        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        public static readonly DependencyProperty ContextIndicatorVisiblityProperty = DependencyProperty.Register("ContextIndicatorVisiblity", typeof(Visibility), typeof(LoopingFlipview), new PropertyMetadata(Visibility.Visible));
        public Visibility ContextIndicatorVisiblity
        {
            get { return (Visibility)GetValue(ContextIndicatorVisiblityProperty); }
            set { SetValue(ContextIndicatorVisiblityProperty, value); }
        }

        private static void IntervalSet(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                var control = d as LoopingFlipview;
                control?.SetTimerState();
            }
        }

        public LoopingFlipview()
        {
            _updateTimer = new DispatcherTimer();
            _updateTimer.Tick += (s, e) =>
            {
                SetNextItem();
            };

            DefaultStyleKey = typeof(LoopingFlipview);
            Loaded += (s, e) =>
            {
                InitializePanel();
            };
            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateInertia;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _nextButton = GetTemplateChild("NextButton") as Button;
            _previousButton = GetTemplateChild("PreviousButton") as Button;

            _nextButton.Click += NextButton_Click;
            _previousButton.Click += PreviousButton_Click;
            _templateApplied = true;

            InitializePanel();
        }

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            if (e.IsInertial)
            {
                if (e.Cumulative.Translation.X > 100)
                    SetPreviousItem();
                if (e.Cumulative.Translation.X < -100)
                    SetNextItem();
            }
            base.OnManipulationCompleted(e);
        }

        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
            InitializePanel();
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (_templateApplied)
            {
                VisualStateManager.GoToState(this, "Show", true);
            }
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (_templateApplied)
            {
                VisualStateManager.GoToState(this, "Hide", true);
            }
            base.OnPointerExited(e);
        }

        private void InitializePanel()
        {
            if (_templateApplied && Items?.Any() == true)
            {
                var index = Math.Max(Items.IndexOf(SelectedItem), 0);

                SetSelectedItem(index);
            }
        }

        private void SetTimerState()
        {
            _updateTimer.Interval = TimeSpan.FromSeconds(Interval);
            if (Interval > 0 && !_updateTimer.IsEnabled)
            {
                _updateTimer.Start();
            }
            else if (Interval == 0 && _updateTimer.IsEnabled)
            {
                _updateTimer.Stop();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            SetNextItem();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            SetPreviousItem();
        }

        private void SetNextItem()
        {
            var index = SelectedIndex;
            if (index > -1)
            {
                index++;
                if (index >= Items.Count)
                {
                    index = 0;
                }

                SetSelectedItem(index);
            }
        }

        private void SetPreviousItem()
        {
            var index = SelectedIndex;
            if (index > -1)
            {
                index--;
                if (index < 0)
                {
                    index = Items.Count - 1;
                }

                SetSelectedItem(index);
            }
        }

        private void SetSelectedItem(int index)
        {
            SelectedIndex = index;
            SelectedItem = Items[index];

            var loopingItemsPanel = ItemsPanelRoot as LoopingItemsPanel;
            if (loopingItemsPanel != null)
            {
                loopingItemsPanel.ScrollIntoView(index);
            }
        }
    }
}
