using Newtonsoft.Json;

public class Customer
{
    [JsonProperty(PropertyName = "id")]
    public string id { get; set; }
    public string Title { get; set; }
    public string NO { get; set; }
    public string Address { get; set; }
    public string TEL { get; set; }
}