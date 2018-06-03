using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Forms;

namespace MVC_UI_TEST_GENERATOR_GUI.treeViewItem {
    public class Item {
        public string Title { get; set; }

        public string Path { get; set; }

        public string Color { get; set; }
        public ObservableCollection<Item> Items { get; set; }

        public Item()
        {
            Color = "Black";
            Title = "";
            Path = "";
            Items = new ObservableCollection<Item>();
        }

        public void AddItem(Item item) {
            item.Path += Path + item.Title + "/";
            Items.Add(item);
        }
    }
}