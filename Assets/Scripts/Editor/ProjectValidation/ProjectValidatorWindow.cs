using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MetroidvaniaEditor.Validation {
    public class ProjectValidatorWindow : EditorWindow {
        public abstract class ProjectValidatorView {
            protected ProjectValidatorWindow root;
            public string name { get; protected set; }

            public ProjectValidatorView(ProjectValidatorWindow root) {
                this.root = root;
            }

            public abstract void OnViewExit();
            public abstract void OnViewEnter();
            public abstract void OnSearchChange(string search);
        }

        private const string k_WindowTitle = "Project Validator";
        private const string k_VisualTreeAssetPath = "Assets/Editor/UI/Templates/ProjectValidatorWindow.uxml";

        public static List<System.Type> viewTypes { get; private set; }

        private List<ProjectValidatorView> _views = new List<ProjectValidatorView>();
        private int _selectedViewIndex;
        private ProjectValidatorView _selectedView;

        private ToolbarMenu _toolbarViewsMenu;
        private ToolbarSearchField _toolbarSearchField;

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void Initialize() {
            viewTypes = TypeCache.GetTypesDerivedFrom(typeof(ProjectValidatorWindow.ProjectValidatorView))
                .Where(t => !t.IsAbstract).ToList();
        }

        [MenuItem("Window/General/Project Validator")]
        public static void ShowWindow() {
            if (viewTypes.Count == 0) {
                Debug.Log("Could not find any classes derived from ProjectValidatorWindow.ProjectValidatorView in the project.");
                return;
            }

            ProjectValidatorWindow window = GetWindow<ProjectValidatorWindow>();
            window.Show();
        }

        private void OnEnable() {
            minSize = new Vector2(600, 250);
            titleContent = new GUIContent(k_WindowTitle);
        }

        public void CreateGUI() {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_VisualTreeAssetPath);
            visualTree.CloneTree(rootVisualElement);

            _toolbarViewsMenu = rootVisualElement.Q<ToolbarMenu>("toolbar-views-menu");
            _toolbarSearchField = rootVisualElement.Q<ToolbarSearchField>("toolbar-search-field");

            _toolbarSearchField.RegisterValueChangedCallback(cEvent => OnSearchChange(cEvent.newValue));

            object[] viewConstructorParams = new object[] { this };
            for (int i = 0; i < viewTypes.Count; i++) {
                System.Type viewType = viewTypes[i];
                ProjectValidatorView viewInstance = System.Activator.CreateInstance(viewType, viewConstructorParams) as ProjectValidatorView;
                _views.Add(viewInstance);
                int index = i;
                _toolbarViewsMenu.menu.AppendAction(viewInstance.name, x => ToggleView(index),
                    x => _selectedViewIndex == index ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            }

            ToggleView(0);
        }

        public void ToggleView(int viewIndex) {
            if (_selectedView != null && viewIndex == _selectedViewIndex)
                return;

            _selectedView?.OnViewExit();
            _toolbarSearchField.SetValueWithoutNotify(string.Empty);

            _selectedViewIndex = viewIndex;
            _selectedView = _views[viewIndex];
            _selectedView.OnViewEnter();
        }

        private void OnSearchChange(string newSearch) {
            _selectedView?.OnSearchChange(newSearch);
        }
    }
}
