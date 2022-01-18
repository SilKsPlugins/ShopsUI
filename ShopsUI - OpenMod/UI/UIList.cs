using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly int _displayDelay;
        private readonly int _displayAmount;
        
        private CancellationTokenSource? _loadingCts;
        private int _loadingMax;

        public T this[int index] => _currentContents[index];

        public UIList(int max, Display displayAction, DisplayNone displayNoneAction, float displayDelay, int displayAmount)
        {
            if (displayDelay < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(displayDelay));
            }

            if (displayAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(displayAmount));
            }

            Max = max;

            _currentContents = new();

            _displayAction = displayAction;
            _displayNoneAction = displayNoneAction;
            _displayDelay = (int)(displayDelay * 1000); // convert to milliseconds
            _displayAmount = displayAmount;
        }

        public UniTask UpdateContents(IEnumerable<T> newContents, CancellationToken cancellationToken = default)
        {
            UniTask.RunOnThreadPool(async () =>
            {
                try
                {
                    await UniTask.SwitchToMainThread();

                    _loadingCts?.Cancel();

                    var cts = new CancellationTokenSource();
                    var token = cts.Token;

                    _loadingCts = cts;

                    cancellationToken.Register(() => cts.Cancel());

                    var oldContents = _currentContents;
                    _currentContents = newContents.Take(Max).ToList();

                    var totalCount = Math.Max(Math.Max(_currentContents.Count, oldContents.Count), _loadingMax);
                    var totalSets = (totalCount - 1) / _displayAmount + 1;

                    _loadingMax = totalCount;

                    for (var set = 0; set < totalSets; set++)
                    {
                        if (set > 0 && _displayDelay > 0)
                        {
                            await UniTask.Delay(_displayDelay, cancellationToken: token);
                        }

                        await UniTask.SwitchToMainThread(token);

                        for (var i = set * _displayAmount; i < totalCount && i < (set + 1) * _displayAmount; i++)
                        {
                            DisplayElement(i, _currentContents);
                        }
                    }

                    _loadingMax = 0;
                }
                catch (TaskCanceledException)
                {
                    // Task was cancelled. This is expected and is not an issue.
                }
            }, cancellationToken: cancellationToken).Forget();

            return UniTask.CompletedTask;
        }

        private void DisplayElement(int index, IList<T> elements)
        {
            var element = elements.ElementAtOrDefault(index);

            if (element == null)
            {
                _displayNoneAction(index);
            }
            else
            {
                _displayAction(element, index);
            }
        }
    }
}
