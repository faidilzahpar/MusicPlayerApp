using ManagedBass;

namespace MusicPlayerApp.Services
{
    public class AudioPlayerService
    {
        private int _stream;
        public int StreamHandle => _stream;

        public AudioPlayerService()
        {
            Bass.Init();
        }

        public void Play(string filePath)
        {
            Stop(); // hentikan stream lama

            _stream = Bass.CreateStream(filePath);

            if (_stream == 0)
            {
                // Jika gagal load
                return;
            }

            Bass.ChannelPlay(_stream);
        }

        public void Pause()
        {
            if (_stream != 0)
            {
                Bass.ChannelPause(_stream);
            }
        }

        public void Resume()
        {
            if (_stream != 0)
            {
                Bass.ChannelPlay(_stream); // Resume playback
            }
        }

        public void Stop()
        {
            if (_stream != 0)
            {
                Bass.ChannelStop(_stream);     // stop audio
                Bass.StreamFree(_stream);      // free stream
                _stream = 0;
            }
        }
    }
}
