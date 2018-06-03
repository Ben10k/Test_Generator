using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using ControllerViewDataParserLibrary.ControllerViewParser;
using ControllerViewDataParserLibrary.FileParser;
using DataStorageLibrary;
using DataStorageLibrary.ScenariosContainer;
using DataStorageLibrary.ViewsContainer.Element;
using DataStorageLibrary.ViewsContainer.Element.Input;
using FileWriterLibrary;
using MethodScriptGeneratorLibrary;
using MVC_UI_TEST_GENERATOR_GUI.scenarioItemView;
using MVC_UI_TEST_GENERATOR_GUI.SetUpConfigs;
using MVC_UI_TEST_GENERATOR_GUI.treeViewItem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using ScenarioGeneratorLibrary;
using ScenarioGeneratorLibrary.ConcreteScenarioGenerators;
using ScenarioGeneratorLibrary.ScenariosFilter;
using TestScriptBuilderLibrary;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace MVC_UI_TEST_GENERATOR_GUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IObserver {
        private static IElement _selectedDataElement;
        private static readonly SetupConfiguration SetupConfiguration = new SetupConfiguration();
        private static readonly Dictionary<string, string> ScenarioDescriptions = new Dictionary<string, string>();
        private static readonly PreDefinedDataContainer PreDefinedDataContainer = new PreDefinedDataContainer();

        private static IScenarioGenerator _scenarioGenerator;

        private Item _selectedResultElement;
        private FileWriter _fileWriter;
        private readonly ScenarioScriptGenerator _scenarioScriptGenerator = new ScenarioScriptGenerator();

        public MainWindow() {
            InitializeComponent();
            InitializeWindowItems();
            if (File.Exists(graphvizLocation.Text))
                SetupConfiguration.GraphVizLocation = graphvizLocation.Text;
            else
                graphvizLocation.Text = string.Empty;

            DeserializeSetupConfiguration();
        }

        private void SerializeSetupConfiguration() {
            var serializer = new JsonSerializer {
                NullValueHandling = NullValueHandling.Ignore
            };
            using (var streamWriter = new StreamWriter(@"setupConfiguration.json"))
            using (var jsonWriter = new JsonTextWriter(streamWriter)) {
                serializer.Serialize(jsonWriter, SetupConfiguration);
            }
        }

        private void DeserializeSetupConfiguration() {
            try {
                var setupConfigurationJObject = JObject.Parse(File.ReadAllText("setupConfiguration.json"));
                SetupConfiguration.InputProjectFileLocation =
                    setupConfigurationJObject.GetValue("InputProjectFileLocation").ToString();
                SetupConfiguration.InputProjectDirLocation =
                    setupConfigurationJObject.GetValue("InputProjectDirLocation").ToString();
                SetupConfiguration.InputProjectName =
                    setupConfigurationJObject.GetValue("InputProjectName").ToString();
                SetupConfiguration.GraphVizLocation =
                    setupConfigurationJObject.GetValue("GraphVizLocation").ToString();
                SetupConfiguration.ProjectHostUrl =
                    setupConfigurationJObject.GetValue("ProjectHostUrl").ToString();
                SetupConfiguration.OutputProjectLocation =
                    setupConfigurationJObject.GetValue("OutputProjectLocation").ToString();
                graphvizLocation.Text = SetupConfiguration.GraphVizLocation;
                testProjectLocation.Text = SetupConfiguration.OutputProjectLocation;
                projectUnderTestHostName.Text = SetupConfiguration.ProjectHostUrl;
                projectUnderTestFileLocation.Text = SetupConfiguration.InputProjectFileLocation;
                Run_button.IsEnabled = true;
            }
            catch (Exception e) {
                // ignored
            }
        }

        private void InitializeWindowItems() {
            ValidInputTextBox.IsEnabled = false;
            DefaultDataGenerationCheckBox.IsChecked = true;

            TestResultTabItem.IsEnabled = false;
            ProjectDataTabItem.IsEnabled = false;
            GraphViewTabItem.IsEnabled = false;
            TestGeneration_tab.IsEnabled = false;
            ElementEditTabControl.IsEnabled = false;
            ValidInputSaveButton.IsEnabled = false;
            InitScenarioSelection();
            InitElementEditDropDown();

            ValidInputSaveButton.IsEnabled = false;

            TestScenarioDescriptionLabel.Text = "No scenario is selected.";
            GenerateTestProjectButton.IsEnabled = false;

            projectUnderTestHostName.Text = "http://10.0.0.97:5050/";
            SetupConfiguration.ProjectHostUrl = projectUnderTestHostName.Text;

            Run_button.IsEnabled = false;
            NoFilter.IsChecked = true;
            BeforeScreenButton.IsEnabled = false;
            Demo_RadioBox.IsEnabled = false;
            SetupTabItem.Focus();
        }

        private void InitScenarioSelection() {
            TestScenarioSlecectionDropBox.Items.Add("All posible scenarios");
            TestScenarioSlecectionDropBox.Items.Add("Realistic scenarios");

            ScenarioDescriptions.Add("All posible scenarios", "Program will generate all possible scenario cases.");
            ScenarioDescriptions.Add("Realistic scenarios",
                "Program will generate realistic scenario cases. In these scenarios, reset " +
                "elements actions, that are able reset forms, will be executed firstly. After that all input fields, that are type of any input, will be executed. " +
                "The last executed elements, will be buttons, that are resulting in submiting forms or other resulting actions.");
        }

        private void InitElementEditDropDown() {
            foreach (string item in Enum.GetNames(typeof(InputTypes))) {
                EditElementTypeDropDown.Items.Add(item);
            }
        }

        private void UploadItemsToProjectDataTree(ObservableCollection<Item> items) {
            foreach (var item in items)
                ProjectDataTree.Items.Add(item);
        }

        private void UploadItemsToResultTree(ObservableCollection<Item> items) {
            ResultScenarioTree.Items.Clear();
            foreach (var item in items)
                ResultScenarioTree.Items.Add(item);
        }

        private void UploadItemsToResultListViewTree(ObservableCollection<Item> items) {
            ResultsListView_TreeView.Items.Clear();
            foreach (var item in items)
                ResultsListView_TreeView.Items.Add(item);
        }

        private void RunButtonValidator() {
            if (SetupConfiguration.GraphVizLocation != null
                && SetupConfiguration.InputProjectFileLocation != null
                && SetupConfiguration.OutputProjectLocation != null
                && SetupConfiguration.ProjectHostUrl != null) {
                Run_button.IsEnabled = true;
            }
        }

        private void SelectProjectFile_Click(object sender, RoutedEventArgs e) {
            var openFileDialog = new OpenFileDialog {
                Filter = "Visual Studio .NET C# Project (*.csproj)|*.csproj"
            };
            //TODO ataskaita

            if (openFileDialog.ShowDialog() == true) {
                SetupConfiguration.InputProjectFileLocation = openFileDialog.FileName;
                projectUnderTestFileLocation.Text = openFileDialog.FileName;
                RunButtonValidator();
            }

            var projectDirLocation = GetProjectDir();
            SetupConfiguration.InputProjectDirLocation = projectDirLocation;
            var projectName = GetProjectName();
            SetupConfiguration.InputProjectName = projectName;
        }

        private void GraphvizLocationButton_Click(object sender, RoutedEventArgs e) {
            var openFileDialog = new OpenFileDialog {
                Filter = "GraphViz Directed Graph Drower executable.(dot.exe)|dot.exe"
            };

            if (openFileDialog.ShowDialog() == true) {
                SetupConfiguration.GraphVizLocation = openFileDialog.FileName;
                graphvizLocation.Text = openFileDialog.FileName;
                SetupConfiguration.GraphVizLocation = graphvizLocation.Text;
                RunButtonValidator();
            }
        }

        private void OutputProjectLocationButton_Click(object sender, RoutedEventArgs e) {
            using (var fbd = new FolderBrowserDialog()) {
                var resultDialog = fbd.ShowDialog();

                if (resultDialog == System.Windows.Forms.DialogResult.OK
                    && !string.IsNullOrWhiteSpace(fbd.SelectedPath)) {
                    testProjectLocation.Text = fbd.SelectedPath;
                    SetupConfiguration.OutputProjectLocation = fbd.SelectedPath;
                    RunButtonValidator();
                }
            }
        }

        private void UpdateProjectDataElementFields(IElement element) {
            var data = ApplicationData.Instance.GetControllerData();

            foreach (var controllerData in data) {
                foreach (var viewData in controllerData.GetViews()) {
                    foreach (var viewElement in viewData.GetElements()) {
                        if (viewElement == element) {
                            ControllerNameField.Content = controllerData.GetControllerName();
                            ViewNameField.Content = viewData.GetViewName();
                            IdNameField.Content = viewElement.GetId();
                            TagNameField.Content = viewElement.GetTagName();
                            TypeNameField.Content = viewElement.GetTypeName();
                            return;
                        }
                    }
                }
            }

            EditElementTypeDropDown.SelectedItem = "";
        }


        private bool UpdateProjectDataElementFields(string path) {
            var data = ApplicationData.Instance.GetControllerData();

            var variables = path.Split('/').ToList();
            CleanStringList(variables);
            if (variables.Count == 3) {
                var controllerName = variables[0];
                var viewName = variables[1];
                var elementValidIdentifier = variables[2];
                foreach (var controllerData in data) {
                    if (controllerData.GetControllerName().Equals(controllerName)) {
                        ControllerNameField.Content = controllerName;
                        ViewNameField.Content = viewName;
                        var updatedElement = controllerData.GetView(viewName)
                            .GetElementByValidName(elementValidIdentifier);
                        IdNameField.Content = updatedElement.GetId();
                        TagNameField.Content = updatedElement.GetTagName();

                        TypeNameField.Content = updatedElement.GetTypeName();
                        _selectedDataElement = updatedElement;

                        EditElementTypeDropDown.SelectedIndex =
                            EditElementTypeDropDown.Items.IndexOf(_selectedDataElement.GetTypeName());

                        var predefinedData = PreDefinedDataContainer.GetPreDefinedData(_selectedDataElement);

                        if (predefinedData != null)
                            UpdateValidInputFields(predefinedData.GetData());
                        else
                            UpdateDefaultValidInputFields();
                        return true;
                    }
                }
            }

            EditElementTypeDropDown.SelectedItem = "";
            return false;
        }

        private void UpdateValidInputFields(string inputData) {
            ValidInputTextBox.Text = inputData;
            ValidInputCheckBox.IsChecked = true;
            DefaultDataGenerationCheckBox.IsChecked = false;
            ValidInputSaveButton.IsEnabled = false;
        }

        private void UpdateDefaultValidInputFields() {
            DefaultDataGenerationCheckBox.IsChecked = true;
            ValidInputCheckBox.IsChecked = false;
            ValidInputTextBox.IsEnabled = false;
            ValidInputTextBox.Text = "";
            ValidInputSaveButton.IsEnabled = false;
        }

        private void UpdateResultScenarioList(string path) {
            var data = ApplicationData.Instance.GetScenariosContainer();
            var variables = path.Split('/').ToList();
            CleanStringList(variables);
            if (variables.Count == 3) {
                var controllerName = variables[0];
                var viewName = variables[1];
                var scenarioId = variables[2];
                if (scenarioId.Contains("✔") || scenarioId.Contains("❌")) {
                    scenarioId = scenarioId.Substring(1);
                    BeforeScreenButton.IsEnabled = true;
                }
                else {
                    scenarioId = scenarioId.Substring(2);
                    BeforeScreenButton.IsEnabled = false;
                }

                foreach (var scenario in data.GetScenarios()) {
                    if (scenario.GetControllerName().Equals(controllerName) &&
                        scenario.GetViewName().Equals(viewName) && scenario.GetScenarioId().Equals(scenarioId)) {
                        UpdateScenarioList(scenario.GetScenarioNodes());
                        ResultTestNameField.Content = scenario.GetScenarioId();
                        ResultControllerNameField.Content = scenario.GetControllerName();
                        ResultViewNameField.Content = scenario.GetViewName();
                        return;
                    }
                }
            }
        }

        private void UpdateScenarioList(List<Node> nodeList) {
            ScenarioListView.Items.Clear();
            int count = 0;
            foreach (var node in nodeList) {
                if (!node.IsEmpty()) {
                    var scenarioView = new ScenarioView {
                        ElementId = node.GetElement().GetValidIdentifier(),
                        OrderNumber = (++count).ToString()
                    };
                    ScenarioListView.Items.Add(scenarioView);
                }
            }
        }

        private void CleanStringList(List<string> list) {
            for (int i = 0; i < list.Count; i++) {
                if (string.IsNullOrWhiteSpace(list[i])) {
                    list.RemoveAt(i);
                }
            }
        }

        private void ProjectDataTree_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            var item = (Item) ProjectDataTree.SelectedItem;
            if (item != null) {
                if (UpdateProjectDataElementFields(item.Path) &&
                    !_selectedDataElement.GetTypeName().Equals("submit") &&
                    !_selectedDataElement.GetTypeName().Equals("reset") &&
                    !_selectedDataElement.GetTypeName().Equals("button")) {
                    ElementEditTabControl.IsEnabled = true;
                }
                else {
                    ElementEditTabControl.IsEnabled = false;
                }
            }
        }

        private void ElementTypeDropBox_OnSelect(object sender, EventArgs e) {
            TypeChangeSaveButton.IsEnabled = true;
        }

        private void ResultScenarioTree_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            var item = (Item) ResultScenarioTree.SelectedItem;

            if (item != null) {
                _selectedResultElement = item;
                UpdateResultScenarioList(_selectedResultElement.Path);
            }
        }

        private void ResultScenarioListView_Tree_OnSelectedItemChanged(object sender,
            RoutedPropertyChangedEventArgs<object> e) {
            var item = (Item) ResultsListView_TreeView.SelectedItem;

            if (item != null) {
                item.Color = "Black";

                _selectedResultElement = item;
                UpdateResultScenarioList(_selectedResultElement.Path);
            }
        }


        private void Run_button_Click(object sender, RoutedEventArgs e) {
            GraphViewTabItem.IsEnabled = true;
            ProjectDataTabItem.IsEnabled = true;
            ProjectDataTabItem.Focus();
            string projectRootDirectory = SetupConfiguration.InputProjectDirLocation;

            ApplicationData.Instance.SetControllers(
                new ControllerViewReader().GetControllerViews(projectRootDirectory));


            _fileWriter = new FileWriter(SetupConfiguration.TestProjectRootDirectory(),
                SetupConfiguration.InputProjectName);
            _fileWriter.CreateCsprojFile();

            foreach (var controller in ApplicationData.Instance.GetControllerData()) {
                if (controller.GetControllerName().Equals("Base"))
                    continue;

                var controllerFileReader =
                    new ControllerFileReader(projectRootDirectory, controller.GetControllerName());
                controllerFileReader.AddEdges();

                foreach (var view in controller.GetViews())
                    ApplicationData.Instance.GetAllViewNames()
                        .Add(controller.GetControllerName() + '/' + view.GetViewName());
            }


            foreach (var controller in ApplicationData.Instance.GetControllerData()) {
                foreach (var view in controller.GetViews()) {
                    var viewFileReader =
                        new ViewFileReader(projectRootDirectory, controller.GetControllerName(), view.GetViewName());

                    var pageObject = viewFileReader.ParseFile();
                    _fileWriter.WritePageObjectToFile(controller.GetControllerName(), view.GetViewName(), pageObject);
                }
            }

            _fileWriter.CreateScreenshotDiretoryStructure(ApplicationData.Instance.GetAllViewNames());

            WriteGraphToFile(SetupConfiguration.TestProjectRootDirectory() + "/map");
            UploadItemsToProjectDataTree(ItemsMapper.MapControllerItems(ApplicationData.Instance.GetControllerData()));
            ScreenshotImage.Source = new BitmapImage(new Uri(SetupConfiguration.TestProjectRootDirectory() + "map.png",
                UriKind.Absolute));

            SerializeSetupConfiguration();
            ProjectDataTabItem.IsEnabled = true;
            SetupTabItem.IsEnabled = false;
        }

        private void BeforeScreenButton_Click(object sender, RoutedEventArgs e) {
            var variables = _selectedResultElement.Path.Split('/').ToList();
            CleanStringList(variables);
            if (variables.Count == 3) {
                var controllerName = variables[0];
                var viewName = variables[1];
                var testName = variables[2].Substring(1);

                var screenShotView =
                    new ScreenShotView(
                        SetupConfiguration.OutputProjectLocation + "\\" + SetupConfiguration.InputProjectName + "Tests",
                        controllerName,
                        viewName,
                        testName);
                screenShotView.InitializeComponent();
                screenShotView.Show();
            }
        }

        private void DefaultDataGenerationCheckBox_Checked(object sender, RoutedEventArgs e) {
            ValidInputCheckBox.IsChecked = false;
            ValidInputTextBox.IsEnabled = false;
            ValidInputSaveButton.IsEnabled = true;
        }

        private void ValidInputCheckBox_Checked(object sender, RoutedEventArgs e) {
            DefaultDataGenerationCheckBox.IsChecked = false;
            ValidInputTextBox.IsEnabled = true;
            ValidInputSaveButton.IsEnabled = true;
        }

        private void ValidInputSaveButton_Click(object sender, RoutedEventArgs e) {
            if (DefaultDataGenerationCheckBox.IsChecked == true) {
                PreDefinedDataContainer.RemovePredefinedDataOfElement(_selectedDataElement);
                ValidInputTextBox.Text = "";
            }

            if (ValidInputCheckBox.IsChecked == true) {
                var newPredefinedData = new PreDefinedData(_selectedDataElement, ValidInputTextBox.Text);
                PreDefinedDataContainer.AddPredefinedData(newPredefinedData);
            }

            ValidInputSaveButton.IsEnabled = false;
        }

        private void GenerateTestProjectButton_Click(object sender, RoutedEventArgs e) {
            TestResultTabItem.IsEnabled = true;
            GenerateTestProjectButton.IsEnabled = false;

            IScenariosFilter scenariosFilter;

            if (FilterAllFollowingButton.IsChecked == true) {
                scenariosFilter = ScenariosFilterFactory.GetFilter("FilterAllFollowingButton");
            }
            else if (FilterButtonsFollowingButton.IsChecked == true) {
                scenariosFilter = ScenariosFilterFactory.GetFilter("FilterButtonsFollowingButton");
            }
            
            else {
                scenariosFilter = ScenariosFilterFactory.GetFilter("NoFilter");
            }


            ApplicationData.Instance.SetScenarioContainer(GenerateScenarios(scenariosFilter));
             if (Demo_RadioBox.IsChecked == true) {
                scenariosFilter = ScenariosFilterFactory.GetFilter("NoFilter");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Index", "Test6");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Index", "Test7");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Login", "Test9");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Login", "Test10");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Login", "Test11");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test13");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test14");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test15");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test16");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test17");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test18");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test19");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test20");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test21");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test22");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test24");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test25");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test26");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test27");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test28");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test29");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test30");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test31");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test32");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test33");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test34");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test35");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test36");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test37");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test38");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test39");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test40");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test41");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test42");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test43");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test44");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test45");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test46");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test47");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test48");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test49");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test50");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test51");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test52");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test53");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test54");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test55");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test56");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test57");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test58");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test59");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test60");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test61");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test62");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test63");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test64");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test65");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test66");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test67");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test68");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test69");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test70");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test71");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test72");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test73");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test74");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test75");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test76");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test77");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test78");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test79");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test80");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test81");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test82");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test83");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test84");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test85");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test86");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test87");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test88");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test89");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test90");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test91");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test92");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test93");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test94");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test95");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test96");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test97");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test98");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test99");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test100");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test101");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test102");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test103");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test104");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test105");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test106");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test107");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test108");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test109");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test110");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test111");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test112");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test113");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test114");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test115");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test116");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test117");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test118");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test119");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test120");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test121");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test122");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test123");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test124");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test125");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test126");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test127");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test128");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test129");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test130");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test131");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test132");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test133");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test134");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test135");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test136");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test137");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test138");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test139");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test140");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test141");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test142");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test143");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test144");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test145");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test146");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test147");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test148");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test149");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test150");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test151");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test152");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test153");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test154");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test155");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test156");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test157");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test158");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test159");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test160");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test161");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test162");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test163");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test164");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test165");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test166");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test167");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test168");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test169");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test170");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test171");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test172");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test173");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test174");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test175");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test176");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test177");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test178");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test179");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test180");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test181");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test182");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test183");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test184");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test185");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test186");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test187");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test188");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test189");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test190");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test191");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test192");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test193");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test194");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test195");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test196");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test197");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test198");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test199");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test200");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test201");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test202");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Menu", "Test203");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test205");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test206");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test207");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test208");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test209");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test210");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test211");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test212");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test213");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test214");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test215");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test216");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test217");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test218");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test219");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test220");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test221");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test222");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test223");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test224");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test225");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test226");
                ApplicationData.Instance.GetScenariosContainer().RemoveFailedScenario("Home", "Register", "Test227");
            }
            GenerateTests();

            UploadItemsToResultTree(ItemsMapper.MapScenarioItems(ApplicationData.Instance.GetScenariosContainer()));
            UploadItemsToResultListViewTree(
                ItemsMapper.MapItemsTreeToList(
                    ItemsMapper.MapScenarioItems(ApplicationData.Instance.GetScenariosContainer())));
            TestGeneration_tab.IsEnabled = false;

            TestResultTabItem.Focus();
            ListView_Tab.IsSelected = true;
        }

        private ScenariosConainer GenerateScenarios(IScenariosFilter scenariosFilter) {
            var generatedScenarioContainer = new ScenariosConainer();
            var testIndex = 0;
            foreach (var controller in ApplicationData.Instance.GetControllerData()) {
                foreach (var view in controller.GetViews()) {
                    string controllerName = controller.GetControllerName();
                    string viewName = view.GetViewName();

                    var generatedScenraciosList = scenariosFilter.Filter(
                        _scenarioGenerator.GenerateScenraciosList(
                            ApplicationData.Instance.GetControllerByName(controllerName).GetView(viewName)));

                    foreach (var scenario in generatedScenraciosList) {
                        generatedScenarioContainer.AddSenario(new Scenario("Test" + (++testIndex), controllerName,
                            viewName,
                            scenario));
                    }
                }
            }

            return generatedScenarioContainer;
        }

        private void TypeChangeSaveButton_Click(object sender, RoutedEventArgs e) {
            _selectedDataElement.SetTypeName(EditElementTypeDropDown.SelectedItem.ToString());
            UpdateProjectDataElementFields(_selectedDataElement);
            TypeChangeSaveButton.IsEnabled = false;
        }

        private void TestScenarioSlecectionDropBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (TestScenarioSlecectionDropBox.SelectedItem == null)
                return;

            var selectedScenarioType = TestScenarioSlecectionDropBox.SelectedItem.ToString();
            TestScenarioDescriptionLabel.Text = ScenarioDescriptions[selectedScenarioType];

            if (selectedScenarioType.Equals("All posible scenarios")) {
                _scenarioGenerator = new AllPossibleScenariosGenerator();
                if (Demo_RadioBox.IsChecked == true) {
                    Demo_RadioBox.IsChecked = false;
                    NoFilter.IsChecked = true;
                }

                Demo_RadioBox.IsEnabled = false;
            }

            else if (selectedScenarioType.Equals("Realistic scenarios")) {
                _scenarioGenerator = new RealisticScenariosGenerator();
                Demo_RadioBox.IsEnabled = true;
            }


            GenerateTestProjectButton.IsEnabled = true;
        }

        private void SaveUrlButton_Click(object sender, RoutedEventArgs e) {
            var match = new Regex(@"(http|https)://.+/$").Match(projectUnderTestHostName.Text);
            if (!match.Success) {
                MessageBox.Show(@"Host url:""" + projectUnderTestHostName.Text + @""" - is not valid.");
            }
            else {
                SetupConfiguration.ProjectHostUrl = projectUnderTestHostName.Text;
                RunButtonValidator();
            }
        }

        private void WriteGraphToFile(string fileLocation) {
            var graphvizGraph =
                new GraphvizAlgorithm<string, Edge<string>>(ApplicationData.Instance.GetViewDataGraph().GetGraph());

            graphvizGraph.FormatVertex += (sender, args) => args.VertexFormatter.Label = args.Vertex.ToString();
            graphvizGraph.GraphFormat.RankDirection = GraphvizRankDirection.LR;
            graphvizGraph.Generate(new FileDotEngine(SetupConfiguration.GraphVizLocation), fileLocation);
        }

        private void LoadTestResultsButton_Click(object sender, RoutedEventArgs e) {
            UpdateTest();
        }

        private string FindInList(string content, IReadOnlyCollection<string> items) {
            return items.FirstOrDefault(item => item.Contains(content));
        }

        private List<string> GetFinishedTestNames(List<string> testResultsDirs) {
            var testResultsNames = new List<string>();
            foreach (var resResultDir in testResultsDirs) {
                var regex = new Regex(@"(?<=\w\\)\w+(?=_(diff_0|diff_1|initial|latest)\.png)");
                var match = regex.Match(resResultDir);
                var matchString = match.ToString();
                if (!string.IsNullOrEmpty(matchString) && !string.IsNullOrWhiteSpace(matchString)) {
                    testResultsNames.Add(match.ToString());
                }
            }

            testResultsNames = testResultsNames.Distinct().ToList();
            return testResultsNames;
        }

        private string GetProjectName() {
            return new Regex(@"(?<=\\)\w+(?=\.csproj)")
                .Match(SetupConfiguration.InputProjectFileLocation)
                .ToString();
        }

        private string GetProjectDir() {
            return SetupConfiguration.InputProjectFileLocation.Replace(GetProjectName() + ".csproj", "");
        }

        private List<string> GetResultFilesFromDirectory(string directory) {
            return Directory
                .GetFiles(directory, "*.png")
                .ToList();
        }

        private void RegenerateTestsButton_Click(object sender, RoutedEventArgs e) {
            RemoveFailedTests();
            GenerateTests();
            UploadItemsToResultTree(ItemsMapper.MapScenarioItems(ApplicationData.Instance.GetScenariosContainer()));
            UploadItemsToResultListViewTree(
                ItemsMapper.MapItemsTreeToList(
                    ItemsMapper.MapScenarioItems(ApplicationData.Instance.GetScenariosContainer())));
        }

        private void GenerateTests() {
            foreach (var controller in ApplicationData.Instance.GetControllerData()) {
                foreach (var view in controller.GetViews()) {
                    string controllerName = controller.GetControllerName();
                    string viewName = view.GetViewName();

                    var testClass = TestBuilder
                        .GenerateClassBegin(SetupConfiguration.InputProjectName + "Test", controllerName, viewName,
                            SetupConfiguration.TestProjectRootDirectory())
//                        .WithOpenPageTest(controllerName, viewName, SetupConfiguration.ProjectHostUrl)
                        ;

                    foreach (var scenario in ApplicationData.Instance.GetScenariosContainer()
                        .GetScenarios(controllerName, viewName)) {
                        string generatedScenarioScript = _scenarioScriptGenerator.GenerateScenarioScript(
                            scenario.GetScenarioNodes(), PreDefinedDataContainer);

                        testClass = testClass.WithTest(
                            scenario.GetScenarioId(),
                            generatedScenarioScript,
                            controllerName, viewName, SetupConfiguration.ProjectHostUrl);
                    }

                    _fileWriter.WriteTestToFile(controllerName, controllerName + viewName + "Test", testClass.Build());
                }
            }
        }

        private void RemoveFailedTests() {
            var allFailedTests =
                File.ReadAllLines(SetupConfiguration.TestProjectRootDirectory() + "Screenshots/FailedTests.txt");

            foreach (string failedTest in allFailedTests) {
                var strings = failedTest.Split(new[] {"-->"}, StringSplitOptions.None);
                var controllerName = strings[0];
                var viewName = strings[1];
                var scenarioId = strings[2];
                ApplicationData.Instance.GetScenariosContainer()
                    .RemoveFailedScenario(controllerName, viewName, scenarioId);
            }
        }

        private void ProjectDataBack_Button_Click(object sender, RoutedEventArgs e) {
            ProjectDataTree.Items.Clear();
            ApplicationData.Instance.ClearControllersData();
            ApplicationData.Instance.ClearScenariosData();
            ControllerNameField.Content = "";
            ViewNameField.Content = "";
            IdNameField.Content = "";
            TagNameField.Content = "";
            TypeNameField.Content = "";
            ElementEditTabControl.IsEnabled = false;
            EditElementTypeDropDown.SelectedValue = "";
            ValidInputTextBox.Text = "";
            ValidInputCheckBox.IsChecked = false;
            DefaultDataGenerationCheckBox.IsChecked = true;
            ValidInputSaveButton.IsEnabled = false;
            TypeChangeSaveButton.IsEnabled = false;
            ProjectDataTabItem.IsEnabled = false;
            PreDefinedDataContainer.ClearData();
            SetupTabItem.IsEnabled = true;
            SetupTabItem.Focus();
        }

        private void TestsGenerationBack_Button_Click(object sender, RoutedEventArgs e) {
            ProjectDataTabItem.IsEnabled = true;
            TestGeneration_tab.IsEnabled = false;
            ProjectDataTabItem.Focus();
        }

        private void TestsResultsBack_button_Click(object sender, RoutedEventArgs e) {
            ResultsListView_TreeView.Items.Clear();
            ResultScenarioTree.Items.Clear();
            ScenarioListView.Items.Clear();

            ResultControllerNameField.Content = "";
            ResultTestNameField.Content = "";
            ResultViewNameField.Content = "";
            ApplicationData.Instance.ClearScenariosData();
            TestGeneration_tab.IsEnabled = true;
            TestResultTabItem.IsEnabled = false;
            TestScenarioSlecectionDropBox.SelectedIndex = -1;
            TestGeneration_tab.Focus();
        }

        private void ProjectDataNextButton_Click(object sender, RoutedEventArgs e) {
            TestGeneration_tab.IsEnabled = true;
            ProjectDataTabItem.IsEnabled = false;
            TestGeneration_tab.Focus();
        }

        private void RunTests_button_Click(object sender, RoutedEventArgs e) {
            RunTests_button.Content = "Running...";
            RunTests_button.IsEnabled = false;
            RegenerateTestsButton.IsEnabled = false;
            TestsResultsBack_button.IsEnabled = false;
            new TestRunner(this).RunTests(SetupConfiguration.TestProjectRootDirectory());
        }

        public void UpdateTest() {
            var screenshotsDir = $"{SetupConfiguration.OutputProjectLocation}\\{GetProjectName()}Tests\\Screenshots";
            foreach (var controller in ApplicationData.Instance.GetControllerData()) {
                foreach (var view in controller.GetViews()) {
                    var picsDirectory = screenshotsDir + "\\" + controller.GetControllerName() + "\\" +
                                        view.GetViewName() + "\\";
                    var resultFiles = GetResultFilesFromDirectory(picsDirectory);
                    var finishedTestScenariosNames = GetFinishedTestNames(resultFiles);

                    foreach (var finishedTestScenarioName in finishedTestScenariosNames) {
                        var initial = FindInList(finishedTestScenarioName + "_initial.png", resultFiles);
                        var latest = FindInList(finishedTestScenarioName + "_latest.png", resultFiles);
                        var diff0 = FindInList(finishedTestScenarioName + "_diff_0", resultFiles);
                        var diff1 = FindInList(finishedTestScenarioName + "_diff_1", resultFiles);

                        var scenario = ApplicationData.Instance.GetScenariosContainer()
                            .GetScenario(controller.GetControllerName(), view.GetViewName(),
                                finishedTestScenarioName);

                        if ((initial != null && latest != null) && (diff0 != null || diff1 != null)) {
                            var scenarioResult = diff0 != null
                                ? "pass"
                                : "fail";
                            scenario?.SetScenarioResult(scenarioResult);
                        }
                    }
                }
            }


            var allFailedTests =
                File.ReadAllLines(SetupConfiguration.TestProjectRootDirectory() + "Screenshots/FailedTests.txt");

            foreach (string failedTest in allFailedTests) {
                var strings = failedTest.Split(new[] {"-->"}, StringSplitOptions.None);
                var controllerName = strings[0];
                var viewName = strings[1];
                var scenarioId = strings[2];
                var scenario = ApplicationData.Instance.GetScenariosContainer()
                    .GetScenario(controllerName, viewName, scenarioId);

                scenario?.SetScenarioResult("crashed");
            }

            var treeViewResults = ItemsMapper.MapScenarioItems(ApplicationData.Instance.GetScenariosContainer());
            var listViewResults = ItemsMapper.MapItemsTreeToList(treeViewResults);
            UploadItemsToResultTree(treeViewResults);
            UploadItemsToResultListViewTree(listViewResults);
        }

        public void TestsHaveFinished() {
            RunTests_button.Content = "Run Tests";
            RunTests_button.IsEnabled = true;
            RegenerateTestsButton.IsEnabled = true;
            TestsResultsBack_button.IsEnabled = true;
        }
    }
}