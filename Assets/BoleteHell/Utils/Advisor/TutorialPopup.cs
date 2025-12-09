using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace BoleteHell.Utils.Advisor
{
    public class TutorialPopup : MonoBehaviour
    {
        [SerializeField]
        private UIDocument _ui;
        
        [SerializeField]
        private float _typewriteDelayMult = 1.0f;

        [SerializeField]
        private Align _alignment = Align.TopLeft;
        
        private VisualElement _root;
        private VisualElement _main;
        private VisualElement _avatar;
        private ScrollView _scrollView;
        private bool _isActive = false;
        private bool _isProcessingQueue = false;
        
        public record ShowOptions(
            Speaker Speaker,
            string Message,
            float Delay = 1.0f,
            bool PreventDuplicates = false
        );
        
        private readonly struct QueuedMessage
        {
            public readonly Speaker Speaker;
            public readonly string Message;
            public readonly float Delay;

            public QueuedMessage(Speaker speaker, string message, float delay)
            {
                Speaker = speaker;
                Message = message;
                Delay = delay;
            }
        }
        
        private readonly Queue<QueuedMessage> _messageQueue = new();

        public enum Align
        {
            TopLeft,
            TopRight,
            BottomLeft
        }

        public void Show(ShowOptions options)
        {
            if (options.PreventDuplicates && _messageQueue.Any(m => m.Message == options.Message && m.Speaker == options.Speaker))
                return;
            
            _messageQueue.Enqueue(new QueuedMessage(options.Speaker, options.Message, options.Delay));
            if (!_isProcessingQueue)
                StartCoroutine(ProcessQueue());
        }

        public IEnumerator ShowAsync(ShowOptions options)
        {
            if (!options.PreventDuplicates || _messageQueue.All(m => m.Message != options.Message || m.Speaker != options.Speaker))
            {
                _messageQueue.Enqueue(new QueuedMessage(options.Speaker, options.Message, options.Delay));
                if (!_isProcessingQueue)
                    StartCoroutine(ProcessQueue());
            }
            
            while (_messageQueue.Count > 0 || _isActive)
                yield return null;
        }

        private IEnumerator ProcessQueue()
        {
            _isProcessingQueue = true;
            
            while (_messageQueue.Count > 0)
            {
                var msg = _messageQueue.Dequeue();
                yield return ShowInternal(msg.Speaker, msg.Message);
                yield return new WaitForSeconds(msg.Delay);
            }
            
            Hide();
            _isProcessingQueue = false;
        }

        private IEnumerator ShowInternal(Speaker speaker, string message)
        {
            Init();

            var speakerLabel = _root.Q<Label>("speaker");
            var messageLabel = _root.Q<Label>("message");

            bool wasActive = _isActive;
            _isActive = true;

            _avatar.style.backgroundImage = Background.FromTexture2D(speaker.Avatar);
            speakerLabel.text = speaker.Name;
            messageLabel.text = "";

            _main.RemoveFromClassList("bottom");
            _main.RemoveFromClassList("right");

            switch (_alignment)
            {
                case Align.BottomLeft:
                    _main.AddToClassList("bottom");
                    break;
                case Align.TopRight:
                    _main.AddToClassList("right");
                    break;
            }

            if (!wasActive)
            {
                yield return null;
                _main.AddToClassList("show");
                yield return new WaitForSeconds(0.5f);
            }

            yield return Typewrite(message, messageLabel);
        }

        public void Hide()
        {
            _main.RemoveFromClassList("show");
            _isActive = false;
        }

        private IEnumerator Typewrite(string message, Label messageLabel)
        {
            for (int i = 0; i < message.Length; i++)
            {
                string visible = message.Substring(0, i + 1);
                string hidden = message.Substring(i + 1);
                messageLabel.text = $"{visible}<color=#00000000>{hidden}</color>";
                _scrollView.scrollOffset = new Vector2(0, 9999);

                float charDelay = message[i] switch
                {
                    '.' or '!' or '?' => 0.16f,
                    ';' or ':' => 0.08f,
                    ',' => 0.04f,
                    _ => 0.02f
                };

                yield return new WaitForSeconds(charDelay * _typewriteDelayMult);
            }
            
            messageLabel.text = message;
        }

        private void Init()
        {
            Debug.Assert(_ui);
            
            if (_root != null)
                return;

            _root = _ui.rootVisualElement;
            _main = _root.Q<VisualElement>("main");
            Debug.Assert(_main != null);
            
            _avatar = _root.Q<VisualElement>("avatar");
            Debug.Assert(_avatar != null);
            
            _scrollView = _root.Q<ScrollView>("messageView");
            Debug.Assert(_scrollView != null);
        }

        private void Start()
        {
            Init();
        }
    }
}
