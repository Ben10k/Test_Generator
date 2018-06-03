using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace MVC_UI_TEST_GENERATOR_GUI {
    public class TestRunner {
        private const string Arguments = "test -v n --no-restore --no-build";
        private readonly IObserver _observer;
        private const string FileName = "dotnet";

        public TestRunner(IObserver observer) {
            _observer = observer;
        }

        public void RunTests(string testProjectDirectory) {
            new Thread(() => {
                Application.Current.Dispatcher.Invoke(delegate { _observer.UpdateTest(); });
                Thread.CurrentThread.IsBackground = true;
                BuildTestProject(testProjectDirectory);

                var testProcess1 = RunDotnetTests(testProjectDirectory);

//                Thread.Sleep(20000);
//                var testProcess2 = RunDotnetTests(testProjectDirectory);

//                while (!testProcess1.HasExited || !testProcess2.HasExited) {
                while (!testProcess1.HasExited ) {
                    Application.Current.Dispatcher.Invoke(delegate { _observer.UpdateTest(); });
                    Thread.Sleep(5000);
                }
                Application.Current.Dispatcher.Invoke(delegate { _observer.UpdateTest(); });
                Application.Current.Dispatcher.Invoke(delegate { _observer.UpdateTest(); });
                Application.Current.Dispatcher.Invoke(delegate { _observer.UpdateTest(); });

                Application.Current.Dispatcher.Invoke(delegate { _observer.TestsHaveFinished(); });
            }).Start();
        }

        private void BuildTestProject(string testProjectDirectory) {
            var processStartInfo =
                new ProcessStartInfo(FileName, "build") {
                    WorkingDirectory = testProjectDirectory,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
            var process = Process.Start(processStartInfo);
            process.WaitForExit();
        }

        private Process RunDotnetTests(string testProjectDirectory) {
            var processStartInfo =
                new ProcessStartInfo(FileName, Arguments) {
                    WorkingDirectory = testProjectDirectory,
                };
            var process = Process.Start(processStartInfo);
            return process;
        }
    }
}