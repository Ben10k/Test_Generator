using System;
using System.Collections.Generic;
using System.Linq;
using DataStorageLibrary.ViewsContainer.Element;
using DataStorageLibrary.ViewsContainer.Element.Clickable;
using DataStorageLibrary.ViewsContainer.Element.Input;

namespace DataStorageLibrary.ViewsContainer {
    public static class ElementValidator {
        private static List<string> GetClickableTypeNamesList() {
            return Enum.GetNames(typeof(ClickableTypes)).ToList();
        }

        private static List<string> GetInputTypeNamesList() {
            return Enum.GetNames(typeof(InputTypes)).ToList();
        }

        private static List<string> GetInputTagNamesList() {
            return Enum.GetNames(typeof(InputTags)).ToList();
        }

        private static List<string> GetClickableTagNamesList() {
            return Enum.GetNames(typeof(ClickableTags)).ToList();
        }

        public static bool IsInList(string input, List<string> list) {
            foreach (var item in list) {
                if (IsEqualString(item, input)) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsInputField(IElement element) {
            var result = IsInList(element.GetTagName(), GetInputTagNamesList()) &&
                         IsInList(element.GetTypeName(), GetInputTypeNamesList());

            return result;
        }

        public static bool IsClickable(IElement element) {
            var result = IsInList(element.GetTagName(), GetClickableTagNamesList()) &&
                         IsInList(element.GetTypeName(), GetClickableTypeNamesList());

            return result;
        }

        public static List<string> GetAllValidTags() {
            var resultList = new List<string>();
            resultList.AddRange(GetClickableTagNamesList());
            resultList.AddRange(GetInputTagNamesList());

            return resultList;
        }

        public static List<string> GetAllValidTypes() {
            var resultList = new List<string>();
            resultList.AddRange(GetInputTypeNamesList());
            resultList.AddRange(GetClickableTypeNamesList());

            return resultList;
        }

        private static bool IsEqualString(string first, string second) {
            return (first.ToLower().Equals(second.ToLower()));
        }
    }
}