using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace BatchDataEntry.Views.UserControls
{
    /// <summary>
    /// Interaction logic for ZoomAndPanControlView.xaml
    /// </summary>
    public partial class ZoomAndPanScrollViewerView : UserControl, INotifyPropertyChanged
    {
        public static DependencyProperty SourceProperty =
             DependencyProperty.Register("Source",
                  typeof(BitmapSource), typeof(ZoomAndPanScrollViewerView), new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            SourcePropertyChanged));

        public BitmapSource Source
        {
            get { return base.GetValue(SourceProperty) as BitmapSource; }
            set {
                base.SetValue(SourceProperty, value);
                OnPropertyChanged("Source");
            }
        }

        private static void SourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
            ZoomAndPanScrollViewerView thiscontrol = sender as ZoomAndPanScrollViewerView;

            if (thiscontrol != null)
            {
                BitmapSource bitmap = e.NewValue as BitmapSource;
                thiscontrol.Source = bitmap;
            }

        }

        public ZoomAndPanScrollViewerView()
        {
            this.DataContext = this;
            InitializeComponent();
            SetCanvasDimension(this.Source.Width, this.Source.Height);

            //TODO: Quando cambia l'immagine bisogna reimpostare l'altezza e larghezza del canvas

            //TODO: Impostare Fit immagine come default
        }

        // Reimposta le dimensioni del canvas in base alle dimensioni dell'immagine
        private void SetCanvasDimension(double w, double h)
        {
            if (this.Source != null)
            {
                actualContent.Width = w;
                actualContent.Height = h;
            }
        }

        /// <summary>
        ///     Specifies the current state of the mouse handling logic.
        /// </summary>
        private bool _mouseDragging;

        /// <summary>
        ///     The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point _origContentMouseDownPoint;

        /// <summary>
        ///     Event raised when a mouse button is clicked down over a Rectangle.
        /// </summary>
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            actualContent.Focus();
            Keyboard.Focus(actualContent);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                //
                // When the shift key is held down special zooming logic is executed in content_MouseDown,
                // so don't handle mouse input here.
                //
                return;
            }

            if (_mouseDragging) return;
            _mouseDragging = true;
            _origContentMouseDownPoint = e.GetPosition(actualContent);
            ((Rectangle)sender).CaptureMouse();
            e.Handled = true;
        }

        /// <summary>
        ///     Event raised when a mouse button is released over a Rectangle.
        /// </summary>
        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_mouseDragging) return;
            _mouseDragging = false;
            ((Rectangle)sender).ReleaseMouseCapture();
            e.Handled = true;
        }

        /// <summary>
        ///     Event raised when the mouse cursor is moved when over a Rectangle.
        /// </summary>
        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_mouseDragging) return;

            var curContentPoint = e.GetPosition(actualContent);
            var rectangleDragVector = curContentPoint - _origContentMouseDownPoint;

            //
            // When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.
            //

            _origContentMouseDownPoint = curContentPoint;

            var rectangle = (Rectangle)sender;
            Canvas.SetLeft(rectangle, Canvas.GetLeft(rectangle) + rectangleDragVector.X);
            Canvas.SetTop(rectangle, Canvas.GetTop(rectangle) + rectangleDragVector.Y);

            e.Handled = true;
        }

        void OnPropertyChanged(String prop)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
