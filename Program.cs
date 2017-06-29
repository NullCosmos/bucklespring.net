using System.Windows.Forms;

namespace bucklespring.net
{
    class Program
    {
        private static OpenALKeySoundPlayer openALKeySoundPlayer = null;

        static void Main(string[] args)
        {
            using (openALKeySoundPlayer = new OpenALKeySoundPlayer())
            using (WindowsHook winHook = new WindowsHook())
            {
                // Subscribe to keyboard events
                winHook.OnKeyboardEvent += WinHook_OnKeyboardEvent;

                // Let app run until told to close. Allowing events to be processed until close event is received.
                Application.Run();

                // Unsubscribe to keyboard events, cleaning up winHook properly
                winHook.OnKeyboardEvent -= WinHook_OnKeyboardEvent;
            }            
        }

        private static void WinHook_OnKeyboardEvent(int vkCode, bool IsDown)
        {
            // Tell the sound player to play the key sound
            openALKeySoundPlayer.PlayKey(vkCode, IsDown);
        }
                
    }
}
