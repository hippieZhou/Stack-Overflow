using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace UWPClassLibrary.Extensions
{
    public abstract class IncrementalItemSourceBase<TItem>
    {
        public event EventHandler<bool> HasMoreItemsChanged;

        public void RaiseHasMoreItemsChanged(bool value)
        {
            this.HasMoreItemsChanged?.Invoke(this, value);
        }

        protected internal abstract Task LoadMoreItemsAsync(ICollection<TItem> collection, uint suggestLoadCount);

        protected internal virtual void OnRefresh(ICollection<TItem> collection)
        {
            // clearup the collection.
            collection.Clear();

            // reset has more items.
            this.RaiseHasMoreItemsChanged(true);
        }
    }

    public class IncrementalLoadingCollection<TItem, TItemSource> : ObservableCollection<TItem>, ISupportIncrementalLoading where TItemSource : IncrementalItemSourceBase<TItem>
    {
        private readonly TItemSource _itemSource;

        // must be true when init.
        private bool _hasMoreItems = true;

        private bool _isLoading;

        private DateTime _lastLoadedTime;

        public IncrementalLoadingCollection(TItemSource itemSource)
        {
            if (itemSource == null)
            {
                throw new ArgumentNullException(nameof(itemSource));
            }

            itemSource.HasMoreItemsChanged += (object sender, bool hasMoreItems) =>
            {
                this.HasMoreItems = hasMoreItems;
            };
            this._itemSource = itemSource;
        }

        public bool HasMoreItems
        {
            get
            {
                return this._hasMoreItems;
            }
            protected set
            {
                this._hasMoreItems = value;

                // tell view this property has changed.
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasMoreItems)));
            }
        }

        /// <summary>
        /// Is this collection is loading datas.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return this._isLoading;
            }
            protected set
            {
                this._isLoading = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsLoading)));
            }
        }

        public DateTime LastLoadedTime
        {
            get
            {
                return this._lastLoadedTime;
            }
            set
            {
                this._lastLoadedTime = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(LastLoadedTime)));
            }
        }

        public event EventHandler<uint> LoadMoreStarted;

        public event EventHandler<uint> LoadMoreCompleted;

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            // Only one loading operation can running.
            if (this.IsLoading)
            {
                return Task.FromResult<LoadMoreItemsResult>(new LoadMoreItemsResult()
                {
                    Count = 0
                }).AsAsyncOperation();
            }

            this.IsLoading = true;
            this.LoadMoreStarted?.Invoke(this, count);
            return AsyncInfo.Run(async c =>
            {
                uint resultCount = 0;
                try
                {
                    int beforeLoadCount = this.Count;
                    await this._itemSource.LoadMoreItemsAsync(this, count);
                    int afterLoadCount = this.Count;

                    if (afterLoadCount > beforeLoadCount)
                    {
                        resultCount = (uint)(afterLoadCount - beforeLoadCount);
                    }

                    // load success(I means no exception), set the loaded date.
                    this.LastLoadedTime = DateTime.Now;
                    return new LoadMoreItemsResult()
                    {
                        Count = resultCount
                    };
                }
                catch
                {
                    // Item Source will throw any exception, like network exception,
                    // don't let it break down the application.
                    return new LoadMoreItemsResult()
                    {
                        Count = resultCount
                    };
                }
                finally
                {
                    // load finished.
                    this.IsLoading = false;
                    this.LoadMoreCompleted?.Invoke(this, count);
                }
            });
        }

        /// <summary>
        /// Cleanup all the datas and reset <see cref="HasMoreItems"/>
        /// </summary>
        public async void Refresh()
        {
            // tell item source refresh this.
            this._itemSource.OnRefresh(this);

            // after refresh, at least try to load one item to fire UI control.
            await this.LoadMoreItemsAsync(1);
        }
    }
}
