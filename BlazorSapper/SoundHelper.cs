using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BlazorSapper
{
    public class SoundHelper
    {
        private readonly IJSRuntime _jsRuntime;

        public SoundHelper(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ValueTask Flag()
        {
            return Play(nameof(Flag));
        }

        public ValueTask UnFlag()
        {
            return Play(nameof(UnFlag));
        }

        public ValueTask Click()
        {
            return Play(nameof(Click));
        }

        public ValueTask Wrong()
        {
            return Play(nameof(Wrong));
        }

        public ValueTask GameOver()
        {
            return Play(nameof(GameOver));
        }

        public ValueTask GameWin()
        {
            return Play(nameof(GameWin));
        }

        private ValueTask Play(string name)
        {
            return _jsRuntime.InvokeVoidAsync("BlazorSounds.play", name);
        }
    }
}
