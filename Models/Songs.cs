using SQLite;

namespace MusicPlayerApp.Models
{
    public class Song
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; }
        public string FilePath { get; set; }
        public string Artist { get; set; }
        public double Duration { get; set; }
        public string DurationFormatted
        {
            get
            {
                var ts = TimeSpan.FromSeconds(Duration);
                return ts.ToString(@"mm\:ss");
            }
        }
    }
}
