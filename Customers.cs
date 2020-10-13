using Newtonsoft.Json;

public class Customer
{
    [JsonProperty(PropertyName = "id")]
    public string id { get; set; }
    
    [JsonProperty(PropertyName ="公司名稱")]
    public string Title { get; set; }
    [JsonProperty(PropertyName ="﻿統編")]
    public string NO { get; set; }
    [JsonProperty(PropertyName="公司地址")]
    public string Address { get; set; }
    public string TEL { get; set; }
}

public class Wrapper
{
    [JsonProperty(PropertyName = "result")]
    public Result Result { get; set; }
}

public class Result
{
    [JsonProperty(PropertyName = "limit")]
    public int Limit { get; set; }
    [JsonProperty(PropertyName = "offset")]
    public int Offset { get; set; }
    [JsonProperty(PropertyName = "count")]
    public int Count { get; set; }
    [JsonProperty(PropertyName = "sort")]
    public string Sort { get; set; }
    [JsonProperty(PropertyName = "results")]
    public Customer[] Results { get; set; }

}