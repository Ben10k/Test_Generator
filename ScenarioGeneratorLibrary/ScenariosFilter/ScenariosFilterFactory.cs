using System;

namespace ScenarioGeneratorLibrary.ScenariosFilter {
    public static class ScenariosFilterFactory {
        private static FilterAllFollowingButton _filterAllFollowingButton;
        private static FilterButtonsFollowingButton _filterButtonsFollowingButton;
        private static NoFilter _noFilter;

        public static IScenariosFilter GetFilter(string type) {
            switch (type) {
                case "FilterAllFollowingButton":
                    return _filterAllFollowingButton
                           ?? (_filterAllFollowingButton = new FilterAllFollowingButton());
                case "FilterButtonsFollowingButton":
                    return _filterButtonsFollowingButton
                           ?? (_filterButtonsFollowingButton = new FilterButtonsFollowingButton());
                case "NoFilter":
                    return _noFilter
                           ?? (_noFilter = new NoFilter());
                default:
                    return null;
            }
        }
    }
}