using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace PageObjectGeneratorLibrary {
    [SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.1")]
    [SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.2")]
    public class PageObjectGenerator {
        private readonly string _controllerName;
        private readonly string _viewName;
        private string _generatedText;

        public PageObjectGenerator(string controllerName, string viewName) {
            _controllerName = controllerName;
            _viewName = viewName;
        }

        public PageObjectGenerator GenerateClassBegin() {
            var result = "using OpenQA.Selenium;\n" +
                         "using OpenQA.Selenium.Support.PageObjects;\n" +
                         "using OpenQA.Selenium.Support.UI;\n" +
                         "using System;\n" +
                         "using System.Collections.Generic;\n" +
                         "\n" +
                         "namespace " + ConvertToUppercase(_controllerName) + " {\n" +
                         "public class " + _viewName + "PageObject {\n" +
                         "private IWebDriver driver;\n" +
                         "private int timeout = 15;\n";

            result += "private readonly string pageUrl = \"";

            result += (_controllerName.Equals("Home")
                          ? string.Empty
                          : "/" + _controllerName)
                      +
                      (_viewName.Equals("Index")
                          ? string.Empty
                          : "/" + _viewName);

            result += "\";\n";

            result += "\n" +
                      "public " + _viewName + "PageObject(IWebDriver driver, int timeout) {\n" +
                      "this.driver = driver;\n" +
                      "this.timeout = timeout;\n" +
                      "}\n" +
                      "\n";
            _generatedText += result;
            return this;
        }

        public PageObjectGenerator GenerateClassEnd() {
            string result = "public " + _viewName + "PageObject VerifyPageUrl() \n" +
                            "{\n" +
                            "new WebDriverWait(driver, TimeSpan.FromSeconds(timeout)).Until<bool>((d) =>\n" +
                            "{\n" +
                            "return d.Url.Contains(pageUrl);\n" +
                            "});\n" +
                            "return this;\n" +
                            "}\n" +
                            "\n" +
                            "}\n" +
                            "\n" +
                            "}\n";
            _generatedText += result;
            return this;
        }

        public string Build() {
            return _generatedText;
        }

        public void GeneratefromIdTagTypeAttributes(string idTag, string tag, string typeTag) {
            string result = string.Empty;
            if (tag == "input") {
                switch (typeTag) {
                    case "button":
                        result = "private IWebElement " + idTag + " => driver.FindElement(By.Id(\"" + idTag +
                                 "\"));\n" +
                                 "\n" +
                                 "public " + _viewName + "PageObject ClickButtonOn" + ConvertToUppercase(idTag) +
                                 "() {\n" +
                                 idTag + ".Click();\n" +
                                 "return this;\n" +
                                 "}\n";
                        break;
                    case "email":
                        result = "private IWebElement " + idTag + " => driver.FindElement(By.Id(\"" + idTag +
                                 "\"));\n" +
                                 "\n" +
                                 "public " + _viewName + "PageObject EnterTextTo" + ConvertToUppercase(idTag) +
                                 "(string text) {\n" +
                                 idTag + ".SendKeys(text);\n" +
                                 "return this;\n" +
                                 "}\n";
                        break;
                    case "number":
                        result = "private IWebElement " + idTag + " => driver.FindElement(By.Id(\"" + idTag +
                                 "\"));\n" +
                                 "\n" +
                                 "public " + _viewName + "PageObject EnterTextTo" + ConvertToUppercase(idTag) +
                                 "(string text) {\n" +
                                 idTag + ".SendKeys(text);\n" +
                                 "return this;\n" +
                                 "}\n";
                        break;
                    case "password":
                        result = "private IWebElement " + idTag + " => driver.FindElement(By.Id(\"" + idTag +
                                 "\"));\n" +
                                 "\n" +
                                 "public " + _viewName + "PageObject EnterTextTo" + ConvertToUppercase(idTag) +
                                 "(string text) {\n" +
                                 idTag + ".SendKeys(text);\n" +
                                 "return this;\n" +
                                 "}\n";
                        break;
                    case "reset":
                        result = "private IWebElement " + idTag + " => driver.FindElement(By.Id(\"" + idTag +
                                 "\"));\n" +
                                 "\n" +
                                 "public " + _viewName + "PageObject ClickResetOn" + ConvertToUppercase(idTag) +
                                 "() {\n" +
                                 idTag + ".Click();\n" +
                                 "return this;\n" +
                                 "}\n";
                        break;
                    case "submit":
                        result = "private IWebElement " + idTag + " => driver.FindElement(By.Id(\"" + idTag +
                                 "\"));\n" +
                                 "\n" +
                                 "public " + _viewName + "PageObject SubmitFormOn" + ConvertToUppercase(idTag) +
                                 "() {\n" +
                                 idTag + ".Submit();\n" +
                                 "return this;\n" +
                                 "}\n";
                        break;
                    case "text":
                        result = "private IWebElement " + idTag + " => driver.FindElement(By.Id(\"" + idTag +
                                 "\"));\n" +
                                 "\n" +
                                 "public " + _viewName + "PageObject EnterTextTo" + ConvertToUppercase(idTag) +
                                 "(string text) {\n" +
                                 idTag + ".SendKeys(text);\n" +
                                 "return this;\n" +
                                 "}\n";
                        break;
                }
            }

            if (!_generatedText.Contains(result))
                _generatedText += result;
        }

        public void GenerateFromRazorTag(string input) {
            if (input.Contains("@Html.ActionLink")) {
                GenerateActionButton(input);
            }
            else if (input.Contains("@Html.EditorFor")) {
                GenerateTextField(input);
            }
        }

        public void GenerateFromHtmlTag(string input, List<string> allViewNames, List<string> allEdges) {
            if (input.Contains("@Url.Action"))
                GenerateRazorButton(input, allViewNames, allEdges);
            else if (!input.Contains("var ")) {
                if (input.Contains("text") || input.Contains("number"))
                    GenerateHtmlTextInput(input);
                else if (input.Contains("asp-for")) {
                    GenerateAspForInput(input);
                }
            }
        }

        private void GenerateAspForInput(string input) {
            int indexAspForTag = input.IndexOf("asp-for=\"") + 9;
            int lengthAspForTag = input.IndexOf('\"', indexAspForTag) - indexAspForTag;
            string aspForTag = input.Substring(indexAspForTag, lengthAspForTag);
            var result = "private IWebElement " + aspForTag + "=> driver.FindElement(By.Id(\"" + aspForTag + "\"));\n" +
                         "\n" +
                         "public " + _viewName + "PageObject EnterTextTo" + ConvertToUppercase(aspForTag) +
                         "(string text) {\n" +
                         aspForTag + ".SendKeys(text);\n" +
                         "return this;\n" +
                         "}\n";
            if (_generatedText.Contains(result)) return;

            _generatedText += result;
        }

        private void GenerateActionButton(string input) {
            string text = input.Trim();
            string result = string.Empty;

            int lengthOfName = text.IndexOf('\"', text.IndexOf('\"') + 1) - text.IndexOf('\"') - 1;

            string methodName = ConvertToCamelCase(text.Substring(text.IndexOf('\"') + 1, lengthOfName));

            int indexOfTarget = text.IndexOf(", \"") + 3;
            int lengthOfTarget = text.IndexOf('\"', indexOfTarget) - indexOfTarget;
            string targetName = text.Substring(indexOfTarget, lengthOfTarget);

            int a = text.IndexOf(',', indexOfTarget + lengthOfTarget);

            if (text.IndexOf(',', indexOfTarget + lengthOfTarget) != -1) {
                return;
            }


            result += "private IWebElement " + methodName + " => driver.FindElement(By.CssSelector(\"a[href='/";


            if (targetName.ToLower().Equals("index")) {
                result += _controllerName;
            }
            else {
                result += _controllerName + "/" + targetName;
            }

            result += "']\"));\n" +
                      "\n" +
                      "public " + _controllerName + "." + targetName + "PageObject Click" +
                      ConvertToUppercase(methodName) + "Link() {\n" +
                      methodName + ".Click();\n" +
                      "return new " + _controllerName + "." + targetName + "PageObject(driver,timeout);\n" +
                      "}";

            _generatedText += result;
        }

        private void GenerateTextField(string input) {
            string text = input.Trim();
            string result = string.Empty;

            string id = text.Substring(text.IndexOf('(') + 1);
            string modelName = id.Substring(0, id.IndexOf(' '));

            int startIndexOfID = id.IndexOf(modelName + ".") + modelName.Length + 1;
            int lengthOfId = id.IndexOf(',', startIndexOfID) - startIndexOfID;


            id = lengthOfId < 1
                ? id.Substring(startIndexOfID).Replace('.', '_').Replace(")", string.Empty)
                : id.Substring(startIndexOfID, lengthOfId);


            result += "private IWebElement " + id + " => driver.FindElement(By.Id(\"" + id + "\"));\n" +
                      "\n" +
                      "public " + _viewName + "PageObject EnterTextTo" + ConvertToUppercase(id) + "(string text) {\n" +
                      id + ".SendKeys(text);\n" +
                      "return this;\n" +
                      "}\n";
            _generatedText += result;
        }

        private void GenerateRazorButton(string input, List<string> allViewNames, List<string> allEdges) {
            string result = string.Empty;

            int indexOfValue = input.IndexOf("value=\"") + 7;
            string valueAttribute =
                input
                    .Substring(indexOfValue, input.IndexOf('\"', indexOfValue) - indexOfValue)
                    .ToLower();
            string text = input.Trim();


            string targetControllerName;
            int indexOfTargetView = text.IndexOf("@Url.Action(\"") + 13;
            int lengthOfTargetView = text.IndexOf("\",", indexOfTargetView) - indexOfTargetView;

            if (lengthOfTargetView < 0) {
                lengthOfTargetView = text.IndexOf("\"", indexOfTargetView) - indexOfTargetView;
                targetControllerName = _controllerName;
            }
            else {
                int indexOfTargetController = text.IndexOf("\"", indexOfTargetView + lengthOfTargetView + 1) + 1;
                int lengthOfTargetController = text.IndexOf("\"", indexOfTargetController) - indexOfTargetController;
                targetControllerName = text.Substring(indexOfTargetController, lengthOfTargetController);
            }

            var targetViewName = text.Substring(indexOfTargetView, lengthOfTargetView);

            if (targetViewName.ToLower().Equals("index")) {
                result += "private IWebElement " + valueAttribute + "=> driver.FindElement(By.CssSelector(\"a[href='/" +
                          targetControllerName + "']\"));\n" +
                          "\n" +
                          "public " + targetControllerName + "IndexPageObject Click" +
                          ConvertToUppercase(valueAttribute) +
                          "() {\n" +
                          valueAttribute + ".Click();\n" +
                          "return new " + targetControllerName + ".IndexPageObject(driver, timeout);\n" +
                          "}\n";
            }
            else {
                if (!allViewNames.Contains(targetControllerName + "/" + targetViewName)) {
                    foreach (string edge in allEdges) {
                        if (edge.Substring(0, edge.IndexOf("->)") - 1)
                            .Equals(targetControllerName + "/" + targetViewName)) {
                            result += "private IWebElement " + valueAttribute +
                                      "=> driver.FindElement(By.CssSelector(\"a[href='/" + targetControllerName + "/" +
                                      targetViewName + "']\"));\n" +
                                      "\n" +
                                      "public " + edge.Substring(edge.IndexOf("->)") + 2).Replace('/', '.') +
                                      "PageObject ClickButtonOn" + ConvertToUppercase(valueAttribute) + "() {\n" +
                                      valueAttribute + ".Click();\n" +
                                      "return new " + edge.Substring(edge.IndexOf("->)") + 2).Replace('/', '.') +
                                      "PageObject(driver, timeout);\n" +
                                      "}\n";
                            break;
                        }
                    }
                }


                else {
                    result += "private IWebElement " + valueAttribute +
                              "=> driver.FindElement(By.CssSelector(\"a[href='/" +
                              targetControllerName + "/" + targetViewName + "']\"));\n" +
                              "\n" +
                              "public " + targetControllerName + "." + targetViewName + "PageObject ClickButtonOn" +
                              ConvertToUppercase(valueAttribute) +
                              "() {\n" +
                              valueAttribute + ".Click();\n" +
                              "return new " + targetControllerName + "." + targetViewName +
                              "PageObject(driver, timeout);\n" +
                              "}\n";
                }
            }

            _generatedText += result;
        }


        private void GenerateHtmlTextInput(string input) {
            string result = string.Empty;

            string tag = input.Substring(1, input.IndexOf(' ') - 1);

            int indexOfValue = input.IndexOf("value=\"") + 7;
            string valueAttribute = input.Substring(indexOfValue, input.IndexOf('\"', indexOfValue) - indexOfValue)
//                .ToLower();
                ;

            int indexOfClass = input.IndexOf("class=\"") + 7;
            string classAttribute = input
                .Substring(indexOfClass, input.IndexOf('\"', indexOfClass) - indexOfClass)
                .ToLower()
                .Replace(' ', '.');
            int indexNameTag = input.IndexOf("name=\"") + 6;
            int lengthNameTag = input.IndexOf('\"', indexNameTag) - indexNameTag;
            string nameTag = input.Substring(indexNameTag, lengthNameTag);

            int indexTypeTag = input.IndexOf("type=\"") + 6;
            int lengthTypeTag = input.IndexOf('\"', indexTypeTag) - indexTypeTag;
            string typeTag = input.Substring(indexTypeTag, lengthTypeTag);
            int indexIdTag = input.IndexOf("id=\"") + 4;
            int lengthIdTag = input.IndexOf('\"', indexIdTag) - indexIdTag;
            string idTag = input.Substring(indexIdTag, lengthIdTag);
            if ((input.Contains(" id=") && (typeTag.Equals("number") || typeTag.Equals("email") ||
                                            typeTag.Equals("text") ||
                                            typeTag.Equals("password"))) ||
                (indexNameTag == 5 && input.Contains(" id="))) {
                result = "private IWebElement " + idTag + "=> driver.FindElement(By.Id(\"" + idTag + "\"));\n" +
                         "\n" +
                         "public " + _viewName + "PageObject EnterTextTo" + ConvertToUppercase(idTag) +
                         "(string text) {\n" +
                         idTag + ".SendKeys(text);\n" +
                         "return this;\n" +
                         "}\n";
            }
            else if (indexOfClass == 6 && (valueAttribute.Contains("@") || indexOfValue == 6)) {
//                if (typeTag.Equals("number") || typeTag.Equals("email") || typeTag.Equals("text") ||
//                    typeTag.Equals("password")) {
//                    result = "private IWebElement " + idTag + "=> driver.FindElement(By.Id(\"" + idTag + "\"));\n" +
//                             "\n" +
//                             "public " + viewName + "PageObject EnterTextTo" + ConvertToUppercase(idTag) +
//                             "(string text) {\n" +
//                             idTag + ".SendKeys(text);\n" +
//                             "return this;\n" +
//                             "}\n";
//                }
//                else {
                if (nameTag.Contains('@')) {
                    nameTag = nameTag.Substring(0, nameTag.IndexOf('@') - 1);
                    result = "public " + _viewName + "PageObject ClickButtonOn" + ConvertToUppercase(nameTag) +
                             "(int index) {\n" +
                             "driver.FindElement(By.Name(String.Format(\"" + nameTag +
                             "[%s]\", index))).Click();\n" +
                             "return this;\n" +
                             "}\n";
                }
                else
                    result = "public " + _viewName + "PageObject ClickButtonOn" + ConvertToUppercase(nameTag) +
                             "(int index) {\n" +
                             "driver.FindElements(By.Name(\"" + nameTag + "\"))[index].Click();\n" +
                             "return this;\n" +
                             "}\n";

//                }
            }
            else if (indexOfClass == 6) {
                int indexValueTag = input.IndexOf("value=\"") + 7;
                int lengthValueTag = input.IndexOf('\"', indexValueTag) - indexValueTag;
                string valueTag = input.Substring(indexValueTag, lengthValueTag);


                result = "private IWebElement " + valueAttribute + " => driver.FindElement(By.XPath(\"//input[@type='" +
                         typeTag + "' and @value='" + valueTag + "']\"));\n" +
                         "\n" +
                         "public " + _viewName + "PageObject ClickButtonOn" + ConvertToUppercase(valueAttribute) +
                         "() {\n" +
                         valueAttribute + ".Click();\n" +
                         "return this;\n" +
                         "}\n";
            }

            else if (valueAttribute.Contains('@') && !nameTag.Contains('@')) {
                result = "public " + _viewName + "PageObject ClickButtonOn" + ConvertToUppercase(nameTag) +
                         "(int index) {\n" +
                         "driver.FindElements(By.Name(\"" + nameTag + "\"))[index].Click();\n" +
                         "return this;\n" +
                         "}\n";
            }

            else {
                valueAttribute = valueAttribute.Replace("@", "").Replace(".", "");

                result = "private IWebElement " + valueAttribute + "=> driver.FindElement(By.CssSelector(\"" + tag +
                         "." + classAttribute + "\"));\n" +
                         "\n" +
                         "public " + _viewName + "PageObject ClickButtonOn" + ConvertToUppercase(valueAttribute) +
                         "() {\n" +
                         valueAttribute + ".Click();\n" +
                         "return this;\n" +
                         "}\n";
            }

            if (_generatedText.Contains(result)) return;


            _generatedText += result;
        }

        public void GenerateFormText(string input) {
            int indexOfNameTag = input.IndexOf("name=\"") + 6;
            int lengthNameTag = input.IndexOf('\"', indexOfNameTag) - indexOfNameTag;
            string nameTag = input.Substring(indexOfNameTag, lengthNameTag);
            string result = string.Empty;
            result += "[FindsBy(How = How.Name, Using = \"" + nameTag + "\")] [CacheLookup]\n" +
                      "private IWebElement " + nameTag + ";\n" +
                      "\n" +
                      "public " + _viewName + "PageObject EnterTextTo" + ConvertToUppercase(nameTag) +
                      "(string text) {\n" +
                      nameTag + ".SendKeys(text);\n" +
                      "return this;\n" +
                      "}\n" +
                      "\n" +
                      "public " + _viewName + "PageObject SubmitFormOn" + ConvertToUppercase(nameTag) +
                      "TextField() {\n" +
                      nameTag + ".Submit();\n" +
                      "return this;\n" +
                      "}\n";
            if (!_generatedText.Contains(result))
                _generatedText += result;
        }

        public void GenerateA(string input) {
            int indexOfAspActionTag = input.IndexOf("asp-action=\"") + 12;
            int lengthOfAspActionTag = input.IndexOf('\"', indexOfAspActionTag) - indexOfAspActionTag;
            string aspActionTag = input.Substring(indexOfAspActionTag, lengthOfAspActionTag);
            string result = string.Empty;

            if (input.Contains("asp-controller=")) {
                int indexOfAspControllerTag = input.IndexOf("asp-action=\"") + 12;
                int lengthOfAspControllerTag = input.IndexOf('\"', indexOfAspControllerTag) - indexOfAspControllerTag;
                string aspControllerTag = input.Substring(indexOfAspControllerTag, lengthOfAspControllerTag);
                // No idea how to deal with controllers
            }
            else
                result += "[FindsBy(How = How.Name, Using = \"" + aspActionTag + "\")] [CacheLookup]\n" +
                          "private IWebElement " + aspActionTag + "ActionLink;\n" +
                          "\n" +
                          "public " + _viewName + "PageObject Click" + ConvertToUppercase(aspActionTag) +
                          "ActionLink() {\n" +
                          aspActionTag + "ActionLink.Click();\n" +
                          "return this;\n" +
                          "}\n";

            if (!_generatedText.Contains(result))
                _generatedText += result;
        }


        private string ConvertToCamelCase(string phrase) {
            string[] splittedPhrase = phrase.Split(' ', '-', '.');
            var sb = new StringBuilder();
            sb.Append(splittedPhrase[0].ToLower());
            splittedPhrase[0] = string.Empty;
            foreach (String s in splittedPhrase) {
                char[] splittedPhraseChars = s.ToCharArray();
                if (splittedPhraseChars.Length > 0)
                    splittedPhraseChars[0] = new String(splittedPhraseChars[0], 1).ToUpper().ToCharArray()[0];

                sb.Append(new String(splittedPhraseChars));
            }

            return sb.ToString();
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