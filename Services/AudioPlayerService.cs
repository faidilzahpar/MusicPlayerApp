using ManagedBass;

namespace MusicPlayerApp.Services
{
    public class AudioPlayerService
    {
        private int _stream = 0;

        public AudioPlayerService()
        {
            Bass.Init();
        }

        public void Play(string filePath)
        {
            // Hentikan stream lama jika ada
            if (_stream != 0)
            {
                Bass.ChannelStop(_stream);
                Bass.StreamFree(_stream);
            }

            // Buat stream baru
            _stream = Bass.CreateStream(filePath);

            if (_stream == 0)
            {
                throw new Exception("Gagal memuat file audio: " + filePath);
            }

            Bass.ChannelPlay(_stream);
        }

        public void Stop()
        {
            if (_stream != 0)
            {
                Bass.ChannelStop(_stream);
                Bass.StreamFree(_stream);
                _stream = 0;
            }
        }
    }
}
