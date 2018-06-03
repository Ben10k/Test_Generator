using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace MVC_UI_TEST_GENERATOR_GUI {
    /// <summary>
    /// Interaction logic for ScreenShotView.xaml
    /// </summary>
    public partial class ScreenShotView {
        private readonly string _projectLocation;
        private readonly string _controllerName;
        private readonly string _viewName;
        private readonly string _testName;

        public ScreenShotView(string projectLocation, string controllerName, string viewName, string testName) {
            _projectLocation = projectLocation;
            _controllerName = controllerName;
            _viewName = viewName;
            _testName = testName;
            InitializeComponent();
            ScreenShotViewWindow.Title = $"{controllerName}/{viewName}/{testName}";

            LabasRytas.Source =
                new BitmapImage(new Uri(ScreenshotLocationInitial(), UriKind.Absolute));
            LabaDiena.Source =
                new BitmapImage(new Uri(ScreenshotLocationLatest(), UriKind.Absolute));

            LabasVakaras.Source =
                File.Exists(ScreenshotLocationDiff0())
                    ? new BitmapImage(new Uri(ScreenshotLocationDiff0(), UriKind.Absolute))
                    : new BitmapImage(new Uri(ScreenshotLocationDiff1(), UriKind.Absolute));
        }


        private string ScreenshotLocationDiff1() {
            return $"{_projectLocation}\\Screenshots\\{_controllerName}\\{_viewName}\\{_testName}_diff_1.png";
        }

        private string ScreenshotLocationDiff0() {
            return $"{_projectLocation}\\Screenshots\\{_controllerName}\\{_viewName}\\{_testName}_diff_0.png";
        }

        private string ScreenshotLocationLatest() {
            return $"{_projectLocation}\\Screenshots\\{_controllerName}\\{_viewName}\\{_testName}_latest.png";
        }

        private string ScreenshotLocationInitial() {
            return $"{_projectLocation}\\Screenshots\\{_controllerName}\\{_viewName}\\{_testName}_initial.png";
        }
    }
}