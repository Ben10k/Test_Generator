using System.Collections.Generic;
using System.IO;

namespace FileWriterLibrary {
    public class FileWriter {
        private readonly string _testProjectDirectory;
        private readonly string _testProjectName;

        public FileWriter(string testProjectDirectory, string testProjectName) {
            _testProjectDirectory = testProjectDirectory;
            _testProjectName = testProjectName;
        }

        public void CreateCsprojFile() {
            Directory.CreateDirectory(_testProjectDirectory);

            File.WriteAllText(
                _testProjectDirectory + _testProjectName + "Tests.csproj",
                "<Project Sdk=\"Microsoft.NET.Sdk\">\n" +
                "  <PropertyGroup>\n" +
                "    <TargetFramework>netcoreapp2.0</TargetFramework>\n" +
                "  </PropertyGroup>\n" +
                "  <ItemGroup>\n" +
                "    <PackageReference Include=\"Magick.NET-Q16-x64\" Version=\"7.4.5\" />\n" +
                "    <PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"15.6.1\" />\n" +
                "    <PackageReference Include=\"NUnit\" Version=\"3.9.0\" />\n" +
                "    <PackageReference Include=\"NUnit3TestAdapter\" Version=\"3.10.0\" />\n" +
                "    <PackageReference Include=\"Selenium.Firefox.WebDriver\" Version=\"0.19.1\" />\n" +
                "    <PackageReference Include=\"Selenium.Support\" Version=\"3.10.0\" />\n" +
                "    <PackageReference Include=\"Selenium.WebDriver\" Version=\"3.10.0\" />\n" +
                "  </ItemGroup>\n" +
                "</Project>");
        }

        public void CreateScreenshotDiretoryStructure(List<string> allViews) {
            foreach (string view in allViews) {
                Directory.CreateDirectory(_testProjectDirectory + "Screenshots\\" + view);
            }

            File.Create(_testProjectDirectory + "Screenshots\\FailedTests.txt");
        }

        public void WritePageObjectToFile(string controllerName, string viewName, string text) {
            Directory.CreateDirectory(
                _testProjectDirectory + "PageObjects\\" + controllerName);
            File.WriteAllText(
                _testProjectDirectory + "PageObjects\\" + controllerName +
                "\\" + viewName + "PageObject.cs", text);
        }

        public void WriteTestToFile(string directory, string testName, string testText) {
            Directory.CreateDirectory(
                _testProjectDirectory + "Tests\\" + directory);
            File.WriteAllText(
                _testProjectDirectory + "Tests\\" + directory + "\\" +
                testName + ".cs",
                testText);
        }
    }
}