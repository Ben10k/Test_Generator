namespace MVC_UI_TEST_GENERATOR_GUI.SetUpConfigs {
    public class SetupConfiguration {
        public string InputProjectFileLocation { get; set; }
        public string InputProjectDirLocation { get; set; }
        public string InputProjectName { get; set; }
        public string GraphVizLocation { get; set; }
        public string ProjectHostUrl { get; set; }
        public string OutputProjectLocation { get; set; }

        public SetupConfiguration() {
            InputProjectName = null;
            InputProjectFileLocation = null;
            InputProjectDirLocation = null;
            GraphVizLocation = null;
            ProjectHostUrl = null;
            OutputProjectLocation = null;
        }

        public string TestProjectRootDirectory() {
            return OutputProjectLocation + "\\" + InputProjectName + "Tests\\";
        }
    }
}