using System.Collections.Generic;

public class OutcomingEntryMessageDto
{
    public string t { get; set; }
    public List<MkDto> mk { get; set; }
    public List<MentionDto> mentions { get; set; }
}

public class MkDto
{
    public string type { get; set; } 
    public int s { get; set; } 
    public int e { get; set; } 
}

public class MentionDto
{
    public string username { get; set; } 
    public int s { get; set; } 
    public int e { get; set; }
}
