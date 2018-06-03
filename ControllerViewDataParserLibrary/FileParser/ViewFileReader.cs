using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DataStorageLibrary;
using DataStorageLibrary.ViewsContainer.Element;
using PageObjectGeneratorLibrary;
using QuickGraph;

namespace ControllerViewDataParserLibrary.FileParser {
    public class ViewFileReader {
        private readonly string _projectRootDir;
        private readonly string _controller;
        private readonly string _view;
        private readonly PageObjectGenerator _pageObjectGenerator;
        private readonly ApplicationData _applicationData = ApplicationData.Instance;

        public ViewFileReader(string projectRootDir, string controller, string view) {
            _projectRootDir = projectRootDir;
            _controller = controller;
            _view = view;
            _pageObjectGenerator = new PageObjectGenerator(controller, view);
        }

        public string ParseFile() {
            _pageObjectGenerator.GenerateClassBegin();
            foreach (string line in ReadFile()) {
                ReadLine(line);
            }

            return _pageObjectGenerator.GenerateClassEnd().Build();
        }

        private void ReadLine(string line) {
            if (ParseWithId(line)) return;

            ParseRazor(line);

            ParseTagHelpers(line);

            ParseHtml(line);
        }

        private void ParseHtml(string line) {
            if (line.Contains("<form-edit") && line.Contains("default=\"true\"")) {
                int indexOfActionValue = line.IndexOf("action=\"") + 8;
                int lengthOfActionValue = line.IndexOf('\"', indexOfActionValue) - indexOfActionValue;
                string actionValue = line.Substring(indexOfActionValue, lengthOfActionValue);
                AddEdge(_controller + '/' + _view, actionValue.Substring(1));
            }

            if (line.Contains("<form-text") && line.Contains(" name=\""))
                _pageObjectGenerator.GenerateFormText(line.Trim());


            if (line.Contains("<a "))
                _pageObjectGenerator.GenerateA(line.Trim());
        }

        private void ParseTagHelpers(string line) {
            if (line.Contains("asp-action")) {
                int indexOfAspActionValue = line.IndexOf("asp-action=\"") + 12;
                string aspActionValue =
                    line.Substring(indexOfAspActionValue,
                        line.IndexOf('\"', indexOfAspActionValue) - indexOfAspActionValue);
                if (!line.Contains("asp-controller")) {
                    if (aspActionValue.StartsWith("/")) {
                        AddEdge(_controller + "/" + _view, aspActionValue.Substring(1));
                    }
                    else {
                        AddEdge(_controller + "/" + _view, _controller + '/' + aspActionValue);
                    }
                }
                else {
                    int indexOfAspControllerValue = line.IndexOf("asp-controller=\"") + 16;
                    string aspControllerValue =
                        line.Substring(indexOfAspControllerValue,
                            line.IndexOf('\"', indexOfAspControllerValue) - indexOfAspControllerValue);
                    AddEdge(_controller + '/' + _view, aspControllerValue + '/' + aspActionValue);
                }
            }
        }

        private void ParseRazor(string line) {
            if (line.Contains("@Html."))
                _pageObjectGenerator.GenerateFromRazorTag(line.Trim());

            if (line.Contains("Html.BeginForm()"))
                AddEdge(_controller + '/' + _view, _controller + '/' + _view);
        }

        private bool ParseWithId(string line) {
            if (line.Contains(" id=")) {
                string tag = line.Trim().Substring(1, line.Trim().IndexOf(' ') - 1);
                int indexIdTag = line.IndexOf(" id=\"") + 5;
                int lengthIdTag = line.IndexOf('\"', indexIdTag) - indexIdTag;
                string idTag = line.Substring(indexIdTag, lengthIdTag);

                if (!idTag.Contains("@")) {
                    if (line.Contains("type=\"")) {
                        int indexTypeTag = line.IndexOf("type=\"") + 6;
                        int lengthTypeTag = line.IndexOf('\"', indexTypeTag) - indexTypeTag;
                        string typeTag = line.Substring(indexTypeTag, lengthTypeTag);
                        _applicationData
                            .GetControllerByName(_controller)
                            .GetView(_view)
                            .AddElement(new Element(idTag, tag, typeTag));


                        _pageObjectGenerator
                            .GeneratefromIdTagTypeAttributes(idTag, tag, typeTag);
                    }
                    else
                        _applicationData
                            .GetControllerByName(_controller)
                            .GetView(_view)
                            .AddElement(new Element(idTag, tag));
                }

                if (line.Contains("@Url.Action")) {
                    var matches = GetUrlActionMatches(line);
                    if (matches.Count == 1)
                        AddEdge(_controller + '/' + _view, _controller + '/' + matches[0]);
                    else
                        AddEdge(_controller + '/' + _view, matches[2].ToString() + '/' + matches[0]);
                }

                return true;
            }

            return false;
        }

        private MatchCollection GetUrlActionMatches(string line) {
            return Regex.Matches(
                Regex.Match(line, @"(?<=@Url\.Action\(\s?).+(?=\))").ToString(),
                @"(?<=\s?,?" + "\\\"" + @").+?(?=" + "\\\"" + @"\s?,?)");
        }

        private void AddEdge(string from, string to) {
            if (!_applicationData.GetViewDataGraph().ContainsVertex(to))
                _applicationData.GetViewDataGraph().AddVertex(to);

            _applicationData.GetViewDataGraph().AddEdge(from, to);
        }

        private string[] ReadFile() {
            return File.ReadAllLines(_projectRootDir + "Views\\" + _controller + '\\' + _view + ".cshtml");
        }

        private string ConvertToUppercase(string input) {
            switch (input) {
                case null:
                    throw new ArgumentNullException(nameof(input));
                case "":
                    throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default:
                    return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}