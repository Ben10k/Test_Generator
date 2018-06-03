using System;
using System.Linq;

namespace TestScriptBuilderLibrary {
    public class TestBuilder {
        private string _generatedText;

        private TestBuilder(string text) {
            _generatedText = text;
        }

        public static TestBuilder GenerateClassBegin(string projectName, string controllerName, string viewName,
            string testProjectDir) {
            string text =
                "using System;\n" +
                "using System.Drawing;\n" +
                "using System.IO;\n" +
                "using System.Reflection;\n" +
                "using System.Threading;\n" +
                "using NUnit.Framework;\n" +
                "using OpenQA.Selenium;\n" +
                "using OpenQA.Selenium.Firefox;\n" +
                "using ImageMagick;\n" +
                $"using {controllerName};\n" +
                "\n" +
                $"namespace {projectName.Replace("-", "")}Tests.Tests.{controllerName} {{\n" +
                "    [TestFixture]\n" +
                $"    public class {controllerName}{viewName}Test {{\n" +
                "        private const string ScreenshotsDirectory =\n" +
                $"            @\"{testProjectDir}Screenshots\\{controllerName}\\{viewName}\";\n" +
                "\n" +
                $"        private const string ControllerName = \"{controllerName}\";\n" +
                $"        private const string ViewName = \"{viewName}\";\n" +
                "\n" +
                "        private void DeleteOldScreenshots(string testName) {\n" +
                "            File.Delete(DiffScreenshotPath(testName));\n" +
                "            File.Delete(NoDiffScreenshotPath(testName));\n" +
                "        }\n" +
                "\n" +
                "        private string NoDiffScreenshotPath(string testName) {\n" +
                "            return ScreenshotsDirectory + '\\\\' + testName + \"_diff_0.png\";\n" +
                "        }\n" +
                "\n" +
                "        private string DiffScreenshotPath(string testName) {\n" +
                "            return ScreenshotsDirectory + '\\\\' + testName + \"_diff_1.png\";\n" +
                "        }\n" +
                "\n" +
                "        private string LatestScreenshotPath(string testName) {\n" +
                "            return ScreenshotsDirectory + '\\\\' + testName + \"_latest.png\";\n" +
                "        }\n" +
                "\n" +
                "        private string InitialScreenshotPath(string testName) {\n" +
                "            return ScreenshotsDirectory + '\\\\' + testName + \"_initial.png\";\n" +
                "        }\n" +
                "\n" +
                "        private void SaveAndCompareScreenshots(string testName, Screenshot currentSscreenshot) {\n" +
                "            currentSscreenshot.SaveAsFile(LatestScreenshotPath(testName), ScreenshotImageFormat.Png);\n" +
                "            IMagickImage initialImage = new MagickImage(InitialScreenshotPath(testName));\n" +
                "            IMagickImage latestImage = new MagickImage(LatestScreenshotPath(testName));\n" +
                "            MagickImage diffImage = new MagickImage();\n" +
                "            initialImage.Compare(latestImage, ErrorMetric.Absolute, diffImage);\n" +
                "            diffImage.Write(Math.Abs(initialImage.Compare(latestImage).MeanErrorPerPixel) <= 0.1\n" +
                "                ? NoDiffScreenshotPath(testName)\n" +
                "                : DiffScreenshotPath(testName));\n" +
                "        }\n" +
                "\n";
            return new TestBuilder(text);
        }

        public string Build() {
            _generatedText +=
                "    }\n" +
                "}";
            return _generatedText;
        }

        public TestBuilder WithOpenPageTest(string controllerName, string viewName, string hostUrl) {
            string destination = (controllerName.Equals("Home")
                                     ? String.Empty
                                     : controllerName + "/")
                                 +
                                 (viewName.Equals("Index")
                                     ? String.Empty
                                     : viewName + "/");
            string text =
                "        [Test]\n" +
                "        public void OpenPageTest(){\n" +
                "            using (var driver = new FirefoxDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))) {\n" +
                "                driver.Manage().Window.Maximize();\n" +
                "                driver.Navigate().GoToUrl(\"" + hostUrl + destination + "\");\n" +
                "                var " + controllerName.ToLower() + viewName + "PageObject = " +
                "new " + viewName + "PageObject(driver, 60).VerifyPageUrl();\n" +
                "            }\n" +
                "        }\n" +
                "\n";

            return new TestBuilder(_generatedText + text);
        }

        public TestBuilder WithTest(string testName, string testContents, string controllerName, string viewName,
            string hostUrl) {
//            string destination = (controllerName.Equals("Home")
//                                     ? Empty
//                                     : controllerName + "/")
//                                 +
//                                 (viewName.Equals("Index")
//                                     ? Empty
//                                     : viewName + "/");
            string destination = controllerName + "/" +
                                 (viewName.Equals("Index")
                                     ? String.Empty
                                     : viewName + "/");

            string text = "        [Test]\n" +
                          $"        public void {testName}() {{\n" +
                          "            using (var driver = new FirefoxDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))) {\n" +
                          $"                string testName = \"{testName}\";\n" +
                          "                DeleteOldScreenshots(testName);\n" +
                          "                driver.Manage().Window.Size = new Size(1440,900);\n" +
                          $"                driver.Navigate().GoToUrl(\"{hostUrl}{destination}\");\n" +
                          $"                var {firstCharToLower(viewName)}PageObject = new {viewName}PageObject(driver, 60);\n" +
                          "                try {\n" +
                          $"{convertBareMethodsIntoPageObjectMethods(firstCharToLower(viewName), testContents)}" +
                          "                }\n" +
                          "                catch(Exception e){\n" +
                          "                    using (StreamWriter sw = File.AppendText(\"../../../Screenshots/FailedTests.txt\")) {\n" +
                          "                        sw.WriteLine($\"{ControllerName}-->{ViewName}-->{testName}-->{e.Message}\");\n" +
                          "                        throw;\n" +
                          "                    }\n" +
                          "                }\n" +
                          "                Thread.Sleep(2000);\n" +
                          "\n" +
                          "                Screenshot currentSscreenshot = driver.GetScreenshot();\n" +
                          "                if (File.Exists(InitialScreenshotPath(testName)))\n" +
                          "                    SaveAndCompareScreenshots(testName, currentSscreenshot);\n" +
                          "                else\n" +
                          "                    currentSscreenshot.SaveAsFile(InitialScreenshotPath(testName), ScreenshotImageFormat.Png);\n" +
                          "            }\n" +
                          "        }\n" +
                          "\n";
            return new TestBuilder(_generatedText + text);
        }

        private string convertBareMethodsIntoPageObjectMethods(string pageObjectVarName, string text) {
            return text
                .Split('\n')
                .Where(line => line != String.Empty)
                .Aggregate(String.Empty,
                    (current, line) =>
                        current + String.Format("                    {0}PageObject.{1}\n", pageObjectVarName, line));
        }

        private string firstCharToLower(string text) {
            return text.Substring(0, 1).ToLower() + text.Substring(1);
        }
    }
}