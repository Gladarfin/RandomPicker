using System.Collections.Generic;

namespace RandomPicker.App.Models;

public class CompletedVideos
{
    public List<int> CompletedList { get; set; }
    
    public CompletedVideos()
    {
        
    }
    public CompletedVideos(List<int> completedVideos)
    {
        CompletedList = completedVideos;
    }
}