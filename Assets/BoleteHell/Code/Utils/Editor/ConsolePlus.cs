using System.Collections.Generic;
using BoleteHell.Code.Utils.Editor.Logging.Logger;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ILogger = BoleteHell.Code.Utils.Editor.Logging.Logger.ILogger;

namespace BoleteHell.Code.Utils.Editor.Logging
{
    public class ConsolePlus : EditorWindow, ILogger
    {
        [SerializeField]
        private VisualTreeAsset visualTreeAsset;

        private ListView _logsView;
        private readonly List<LogEntry> _logEntries = new(100);

        [MenuItem("Window/Bolete Hell/Console+")]
        public static void ShowExample()
        {
            ConsolePlus wnd = GetWindow<ConsolePlus>();
            wnd.titleContent = new GUIContent("Console+");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualElement labelFromUXML = visualTreeAsset.Instantiate();
            root.Add(labelFromUXML);

            SetupLogsView(root);
        }

        private void SetupLogsView(VisualElement root)
        {
            _logsView = root.Q<ListView>("logs");
            _logsView.itemsSource = _logEntries;
            _logsView.makeItem = () => new Label();
            _logsView.bindItem = (element, index) => 
            {
                var label = (Label)element;
                var entry = _logEntries[index];
                label.text = $"({entry.timestamp:HH:mm:ss}) [{entry.category}] {entry.formattedMessage}";
            };
            
            Logging.Log.AddLogger(this);
        }

        // TODO: Use buffered listview for very long lists?
        // idk if default listview does that
        public void Log(LogEntry entry)
        {
            _logEntries.Add(entry);
            _logsView.RefreshItems();
        }
    }
}
