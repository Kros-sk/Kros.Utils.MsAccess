using Kros.Data.BulkActions;

namespace Kros.Utils.UnitTests.Data.BulkActions
{
    /// <summary>
    /// Wraps an <see cref="IBulkActionDataReader"/> and cancels the given <see cref="CancellationTokenSource"/>
    /// when <see cref="Read"/> is called for the <c>cancelOnReadNumber</c>-th time (1-based).
    /// Used to prove that a token passed to a bulk action is observed while data is being copied.
    /// </summary>
    internal sealed class CancellingBulkActionDataReader : IBulkActionDataReader
    {
        private readonly IBulkActionDataReader _inner;
        private readonly CancellationTokenSource _cts;
        private readonly int _cancelOnReadNumber;
        private int _readCount;

        public CancellingBulkActionDataReader(IBulkActionDataReader inner, CancellationTokenSource cts, int cancelOnReadNumber)
        {
            _inner = inner;
            _cts = cts;
            _cancelOnReadNumber = cancelOnReadNumber;
        }

        public int FieldCount => _inner.FieldCount;
        public string GetName(int i) => _inner.GetName(i);
        public int GetOrdinal(string name) => _inner.GetOrdinal(name);
        public object GetValue(int i) => _inner.GetValue(i);
        public string GetString(int i) => _inner.GetString(i);
        public bool IsDBNull(int i) => _inner.IsDBNull(i);

        public bool Read()
        {
            _readCount++;
            if (_readCount == _cancelOnReadNumber)
            {
                _cts.Cancel();
            }
            return _inner.Read();
        }

        public void Dispose() => _inner.Dispose();
    }
}
