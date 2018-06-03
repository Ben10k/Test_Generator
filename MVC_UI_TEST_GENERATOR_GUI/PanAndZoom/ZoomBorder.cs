// https://stackoverflow.com/a/6782715

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MVC_UI_TEST_GENERATOR_GUI.PanAndZoom {
    public class ZoomBorder : Border {
        private UIElement _child;
        private Point _origin;
        private Point _start;

        private TranslateTransform GetTranslateTransform(UIElement element) {
            return (TranslateTransform) ((TransformGroup) element.RenderTransform)
                .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(UIElement element) {
            return (ScaleTransform) ((TransformGroup) element.RenderTransform)
                .Children.First(tr => tr is ScaleTransform);
        }

        public override UIElement Child {
            get { return base.Child; }
            set {
                if (value != null && value != Child)
                    Initialize(value);
                base.Child = value;
            }
        }

        private void Initialize(UIElement element)
        {
            this.Background = Brushes.White;
            _child = element;
            if (_child != null) {
                TransformGroup group = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(st);
                group.Children.Add(tt);
                _child.RenderTransform = group;
                _child.RenderTransformOrigin = new Point(0.0, 0.0);
                MouseWheel += child_MouseWheel;
                MouseLeftButtonDown += child_MouseLeftButtonDown;
                MouseLeftButtonUp += child_MouseLeftButtonUp;
                MouseMove += child_MouseMove;
                PreviewMouseRightButtonDown += child_PreviewMouseRightButtonDown;
            }
        }

        private void Reset() {
            if (_child != null) {
                // reset zoom
                var st = GetScaleTransform(_child);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                // reset pan
                var tt = GetTranslateTransform(_child);
                tt.X = 0.0;
                tt.Y = 0.0;
            }
        }

        #region Child Events

        private void child_MouseWheel(object sender, MouseWheelEventArgs e) {
            if (_child != null) {
                var st = GetScaleTransform(_child);
                var tt = GetTranslateTransform(_child);

                double zoom = e.Delta > 0 ? .2 : -.2;
                if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                    return;

                var relative = e.GetPosition(_child);

                var abosuluteX = relative.X * st.ScaleX + tt.X;
                var abosuluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX += zoom;
                st.ScaleY += zoom;

                tt.X = abosuluteX - relative.X * st.ScaleX;
                tt.Y = abosuluteY - relative.Y * st.ScaleY;
            }
        }

        private void child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (_child != null) {
                var tt = GetTranslateTransform(_child);
                _start = e.GetPosition(this);
                _origin = new Point(tt.X, tt.Y);
                Cursor = Cursors.Hand;
                _child.CaptureMouse();
            }
        }

        private void child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (_child != null) {
                _child.ReleaseMouseCapture();
                Cursor = Cursors.Arrow;
            }
        }

        private void child_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            Reset();
        }

        private void child_MouseMove(object sender, MouseEventArgs e) {
            if (_child != null && _child.IsMouseCaptured) {
                var tt = GetTranslateTransform(_child);
                Vector v = _start - e.GetPosition(this);
                tt.X = _origin.X - v.X;
                tt.Y = _origin.Y - v.Y;
            }
        }

        #endregion
    }
}