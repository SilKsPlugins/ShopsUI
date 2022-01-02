using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ShopsUI.UI
{
    public class UIList<T>
    {
        public delegate void Display(T element, int index);
        public delegate void DisplayNone(int index);

        public int Max { get; }

        private List<T> _currentContents;

        private readonly Display _displayAction;
        private readonly DisplayNone _displayNoneAction;

        public T this[int index] => _currentContents[index];

        public UIList(int max, Display displayAction, DisplayNone displayNoneAction)
        {
            Max = max;

            _currentContents = new();

            _displayAction = displayAction;
            _displayNoneAction = displayNoneAction;
        }

        public async UniTask UpdateContents(IEnumerable<T> newContents)
        {
            await UniTask.SwitchToMainThread();

            var contents = newContents.Take(Max).ToList();

            var i = 0;

            for (; i < contents.Count; i++)
            {
                _displayAction(contents[i], i);
            }

            for (; i < _currentContents.Count; i++)
            {
                _displayNoneAction(i);
            }

            _currentContents = contents;
        }
    }
}
